using Login;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Configuração do Entity Framework Core para o MySQL
builder.Services.AddDbContext<ApplicationDbContext>((_, options) =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), new MySqlServerVersion(new Version(8, 0, 27))));

// Configuração da autenticação por cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "MyAppCookie";
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.LoginPath = "/api/login"; // Define a rota para a ação de login
        options.AccessDeniedPath = "/api/accessdenied"; // Define a rota para a ação de acesso negado
    });

// Adicione outros serviços necessários

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseRouting();

app.UseCors(builder => builder
    .WithOrigins("http://localhost:3000") // Adicione a origem do seu cliente React
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials()
);

app.MapControllers();

app.Run();
