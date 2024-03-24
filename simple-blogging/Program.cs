using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using simple_blogging.Data;
using System.Reflection;
using Web.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>().AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Blog API",
        Description = "An ASP.NET Core Web API for managing Blog items"
    });
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddControllersWithViews();
//builder.Services.AddControllers();

//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SimpleBlogAPI", Version = "v1" });
//});



// builder.Services.AddSwaggerDocument(configure => configure.Title = "Simple Blog API");
//builder.Services.AddOpenApiDocument(document => document.DocumentName = "a");
//builder.Services.AddSwaggerDocument(document => document.DocumentName = "b");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
//app.UseOpenApi(); // serve documents (same as app.UseSwagger())
//app.UseSwaggerUi3(); // serve Swagger UI
//app.UseReDoc(); // serve ReDoc UI

app.UseSwagger();
app.UseSwaggerUI(c =>
c.SwaggerEndpoint("/swagger/v1/swagger.json",
                  "SimpleBlogAPI v1"));

//app.UseSwaggerUI(options =>
//{
//    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
//    options.RoutePrefix = string.Empty;
//});
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
