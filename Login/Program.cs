using Login;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// Configura��o do Entity Framework Core para o MySQL
builder.Services.AddDbContext<ApplicationDbContext>((_, options) =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), new MySqlServerVersion(new Version(8, 0, 27))));

// Configura��o da autentica��o por cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "MyAppCookie";
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.LoginPath = "/api/login"; // Define a rota para a a��o de login
        options.AccessDeniedPath = "/api/accessdenied"; // Define a rota para a a��o de acesso negado
    });


// Configura��o da autoriza��o
builder.Services.AddAuthorization();


// Adicione outros servi�os necess�rios

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors(builder => builder
    .WithOrigins("http://localhost:3000") // Adicione a origem do seu cliente React
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials()
);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
