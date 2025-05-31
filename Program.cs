using Microsoft.EntityFrameworkCore;
  
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
  
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

builder.Services.AddDbContext<BudgetDbContext>(options => 
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection") ,
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    
    ) 
 );


 
builder.Services.AddOpenApi();

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

 