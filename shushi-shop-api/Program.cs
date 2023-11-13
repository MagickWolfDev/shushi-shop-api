using Microsoft.EntityFrameworkCore;
using shushi_shop_api.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<shushi_shop_apiContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("shushi_shop_apiContext") ?? throw new InvalidOperationException("Connection string 'shushi_shop_apiContext' not found.")));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseRouting();
app.UseCors("AnyMethods");
app.UseAuthorization();


app.MapControllerRoute(
    name: "/api/",
    pattern: "{controller=UsersController}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "/api/",
    pattern: "{controller=DeshesController}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "/api/",
    pattern: "{controller=TypesController}/{action=Index}/{id?}");


app.MapControllers();

app.Run();
