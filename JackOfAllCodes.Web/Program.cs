using JackOfAllCodes.Web.DataAccess;
using JackOfAllCodes.Web.Repositories;
using Microsoft.EntityFrameworkCore;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Check if the environment is production
if (builder.Environment.IsProduction())
{
    // Load connection string from AWS Parameter Store
    var ssmClient = new AmazonSimpleSystemsManagementClient();
    var parameterPath = "/production/jackofallcodes.web/ConnectionStrings/BlogPostDbConnectionString";

    var parameterRequest = new GetParameterRequest
    {
        Name = parameterPath,
        WithDecryption = true
    };

    var response = ssmClient.GetParameterAsync(parameterRequest).Result;
    var dbConnectionString = response.Parameter.Value;

    // Set the connection string dynamically from AWS Parameter Store
    builder.Services.AddDbContext<BlogPostDBContext>(options =>
        options.UseNpgsql(dbConnectionString));
}
else
{
    // Use the connection string from appsettings.json for non-production (development) environments
    builder.Services.AddDbContext<BlogPostDBContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("BlogPostDbConnectionString")));
}

// Inject repositories
builder.Services.AddScoped<ITagRepository, TagRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
