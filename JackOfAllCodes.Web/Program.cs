using JackOfAllCodes.Web.DataAccess;
using JackOfAllCodes.Web.Repositories;
using Microsoft.EntityFrameworkCore;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Serilog;
using Serilog.Sinks.AwsCloudWatch;
using Amazon.CloudWatchLogs;
using JackOfAllCodes.Web.Middleware;
using JackOfAllCodes.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using JackOfAllCodes.Web.Data;
using JackOfAllCodes.Web.Models.Domain;
using Amazon.SimpleEmail;
using JackOfAllCodes.Web.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfiguration) =>
{
    loggerConfiguration
        .Enrich.FromLogContext()
        .WriteTo.Console() // Logs to console for local debugging
        .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day);

    if (context.HostingEnvironment.IsProduction())
    {
        // Create AWS CloudWatch Logs client
        //var cloudWatchClient = new AmazonCloudWatchLogsClient();

        //// Configure CloudWatch logging for production
        //var cloudWatchOptions = new CloudWatchSinkOptions
        //{
        //    LogGroupName = "JackOfAllCodesWeb", // CloudWatch Log Group Name
        //    MinimumLogEventLevel = Serilog.Events.LogEventLevel.Information, 
        //    LogStreamNameProvider = new DefaultLogStreamProvider(), 
        //    CreateLogGroup = true // Automatically creates log group if it doesn't exist
        //};

        //// Add CloudWatch sink
        //loggerConfiguration.WriteTo.AmazonCloudWatch(cloudWatchOptions, cloudWatchClient);
    }
    else
    {
        loggerConfiguration.MinimumLevel.Debug();
    }
});

// Add services to the container.
builder.Services.AddControllersWithViews();

// Check if the environment is production
if (builder.Environment.IsProduction())
{
    Log.Information("Environment is Production.");

    // Load connection string from AWS Parameter Store
    var ssmClient = new AmazonSimpleSystemsManagementClient();

    // Define the paths for the connection strings in AWS Parameter Store
    var blogPostDbParameterPath = "/production/jackofallcodes.web/ConnectionStrings/BlogPostDbConnectionString";
    var authDbParameterPath = "/production/jackofallcodes.web/ConnectionStrings/AuthDbConnectionString";

    var blogPostDbConnectionString = string.Empty;
    var authDbConnectionString = string.Empty;

    try
    {
        // Fetch the BlogPost DB connection string from Parameter Store
        var blogPostResponse = ssmClient.GetParameterAsync(new GetParameterRequest
        {
            Name = blogPostDbParameterPath,
            WithDecryption = true
        }).Result;

        blogPostDbConnectionString = blogPostResponse.Parameter.Value;

        // Fetch the Auth DB connection string from Parameter Store
        var authResponse = ssmClient.GetParameterAsync(new GetParameterRequest
        {
            Name = authDbParameterPath,
            WithDecryption = true
        }).Result;

        authDbConnectionString = authResponse.Parameter.Value;

        // Configure BlogPostDbContext with BlogPost connection string
        builder.Services.AddDbContext<BlogPostDBContext>(options =>
            options.UseNpgsql(blogPostDbConnectionString));

        // Configure ApplicationDbContext with Auth connection string
        builder.Services.AddDbContext<AuthDbContext>(options =>
            options.UseNpgsql(authDbConnectionString));

        // SES Email setup for production
        builder.Services.AddAWSService<IAmazonSimpleEmailService>();

    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "Failed to load DB connection string from AWS Parameter Store.");
        throw;
    }
}
else
{
    Log.Information("Environment is Development.");

    // BlogPost DB
    builder.Services.AddDbContext<BlogPostDBContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("BlogPostDbConnectionString")));

    // AuthDbContext
    builder.Services.AddDbContext<AuthDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("AuthDbConnectionString")));

    // SES Email setup for local development
    var awsOptions = builder.Configuration.GetAWSOptions(); // Uses default profile
    builder.Services.AddSingleton<IAmazonSimpleEmailService>(sp =>
        new AmazonSimpleEmailServiceClient(awsOptions.Credentials, awsOptions.Region));
}

// Register Identity services
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireDigit = true;
    })
.AddEntityFrameworkStores<AuthDbContext>()
.AddDefaultTokenProviders();

// Inject repositories
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IBlogPostRepository, BlogPostRepository>();
builder.Services.AddScoped<ILikePostRepository, LikePostRepository>();
builder.Services.AddScoped<ICommentPostRepository, CommentPostRepository>();
builder.Services.AddScoped<IFileSystemService, FileSystemService>();
builder.Services.AddScoped<IFileUploadService, FileUploadService>();
builder.Services.AddScoped<IAccountService, AccountService>();

// SES Email services
builder.Services.AddTransient<EmailSenderService>();

// Configure Password Reset token lifespan
builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
    options.TokenLifespan = TimeSpan.FromMinutes(30)
);

// Add cookie authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";  // Redirect to Login if not authenticated
        options.LogoutPath = "/Account/Logout";  // Redirect to Logout
        options.AccessDeniedPath = "/Account/AccessDenied";  // Optional
    });

var app = builder.Build();

// Apply migrations automatically on app start
if (app.Environment.IsDevelopment())
{
    // AuthDb
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var authDbContext = services.GetRequiredService<AuthDbContext>();
        authDbContext.Database.Migrate();  // This will apply any pending migrations for Identity DB
    }

    // Apply migrations for BlogPostDbContext
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var blogPostDbContext = services.GetRequiredService<BlogPostDBContext>();
        blogPostDbContext.Database.Migrate();  // This will apply any pending migrations for BlogPost DB
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();