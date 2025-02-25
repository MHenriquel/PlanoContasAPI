using Microsoft.EntityFrameworkCore;
using PlanoContasAPI.Models;
using PlanoContasAPI.Repositories;
using PlanoContasAPI.Repositories.Interfaces;
using PlanoContasAPI.Services;
using PlanoContasAPI.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<PlanoContasContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PlanoContasConnection")));


builder.Services.AddControllers();
builder.Services.AddScoped<IPlanoContasRepository, PlanoContasRepository>(); 
builder.Services.AddScoped<IPlanoContasService, PlanoContasService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
