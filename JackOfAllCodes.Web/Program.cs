using JackOfAllCodes.Web.DataAccess;
using JackOfAllCodes.Web.Repositories;
using Microsoft.EntityFrameworkCore;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Serilog;
using Serilog.Sinks.AwsCloudWatch;
using Amazon.CloudWatchLogs;
using JackOfAllCodes.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfiguration) =>
{
    loggerConfiguration
        .Enrich.FromLogContext()
        .WriteTo.Console(); // Logs to console for local debugging

    if (context.HostingEnvironment.IsDevelopment())
    {
        // Write to file only in Development
        loggerConfiguration.WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day);
    }

    if (context.HostingEnvironment.IsProduction())
    {
        // Create AWS CloudWatch Logs client
        var cloudWatchClient = new AmazonCloudWatchLogsClient();

        // Configure CloudWatch logging for production
        var cloudWatchOptions = new CloudWatchSinkOptions
        {
            LogGroupName = "JackOfAllCodesWeb", // CloudWatch Log Group Name
            MinimumLogEventLevel = Serilog.Events.LogEventLevel.Information, 
            LogStreamNameProvider = new DefaultLogStreamProvider(), 
            CreateLogGroup = true // Automatically creates log group if it doesn't exist
        };

        // Add CloudWatch sink
        loggerConfiguration.WriteTo.AmazonCloudWatch(cloudWatchOptions, cloudWatchClient);
    }
    else
    {
        loggerConfiguration.MinimumLevel.Debug();
    }
});

try
{
    Log.Information("Starting the application");

    // Add services to the container.
    builder.Services.AddControllersWithViews();

    // Check if the environment is production
    if (builder.Environment.IsProduction())
    {
        Log.Information("Environment is Production. Loading DB connection string from AWS Parameter Store.");

        // Load connection string from AWS Parameter Store
        var ssmClient = new AmazonSimpleSystemsManagementClient();
        var parameterPath = "/production/jackofallcodes.web/ConnectionStrings/BlogPostDbConnectionString";

        var parameterRequest = new GetParameterRequest
        {
            Name = parameterPath,
            WithDecryption = true
        };

        try
        {
            var response = ssmClient.GetParameterAsync(parameterRequest).Result;
            var dbConnectionString = response.Parameter.Value;

            // Set the connection string dynamically from AWS Parameter Store
            builder.Services.AddDbContext<BlogPostDBContext>(options =>
                options.UseNpgsql(dbConnectionString));
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Failed to load DB connection string from AWS Parameter Store.");
            throw;
        }
    }
    else
    {
        Log.Information("Environment is Development. Loading DB connection string from appsettings.json.");

        // Use the connection string from appsettings.json for non-production environments
        builder.Services.AddDbContext<BlogPostDBContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("BlogPostDbConnectionString")));
    }

    // Inject repositories
    builder.Services.AddScoped<ITagRepository, TagRepository>();
    builder.Services.AddScoped<IBlogPostRepository, BlogPostRepository>();

    var app = builder.Build();

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

    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly.");
}
finally
{
    Log.Information("Shutting down application");
    Log.CloseAndFlush();
}
