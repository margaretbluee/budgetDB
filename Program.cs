using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

    Log.Information(" Application has started successfully!");

    
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
  
builder.Host.UseSerilog();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()  
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ISupermarketRepository, SupermarketRepository>();
builder.Services.AddHttpClient<GoogleSearchService>();
builder.Services.AddHttpClient<GoogleSearchService>();


builder.Services.AddScoped<MasoutisScraper>();
builder.Services.AddScoped<SklavenitisScraper>();

builder.Services.AddScoped<AbScraper>();
builder.Services.AddScoped<DiscountScraper>();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        options.JsonSerializerOptions.WriteIndented = true;
    });
builder.Services.AddScoped<Func<string, IScraper>>(provider => key =>
{
    return key.ToLower() switch
    {
        "masoutis" => provider.GetRequiredService<MasoutisScraper>(),
        "ab"       => provider.GetRequiredService<AbScraper>(),
        "discount" => provider.GetRequiredService<DiscountScraper>(),
        "sklavenitis" => provider.GetRequiredService<SklavenitisScraper>(),

        _ => throw new KeyNotFoundException($"No scraper registered for '{key}'")
    };
});

builder.Services.AddDbContext<BudgetDbContext>(options => 
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection") ,
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    
    ) 
 );


 
builder.Services.AddOpenApi();
builder.Host.UseSerilog();
var app = builder.Build();

 
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
 
app.UseCors("AllowAll");

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapControllers();
});

 

app.MapControllers();

app.Run();

 