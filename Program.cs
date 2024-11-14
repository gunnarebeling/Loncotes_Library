using LoncotesLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using LoncotesLibrary.Models.DTOs;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddNpgsql<LoncotesLibraryDbContext>(builder.Configuration["LoncotesLibraryDbConnectionString"]);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/api/materials", (LoncotesLibraryDbContext db) => 
{
    return db.Materials
    .Where(material => material.OutOfCirculationSince == null)
    .Select(material => new MaterialDTO
    {
        Id = material.Id,
        MaterialTypeId = material.MaterialTypeId,
        MaterialName = material.MaterialName,
        MaterialType = new MaterialTypeDTO 
            {
                Id = material.MaterialType.Id, 
                Name = material.MaterialType.Name, 
                CheckoutDays = material.MaterialType.CheckoutDays
            },
        GenreId = material.GenreId,
        Genre = new GenreDTO 
            {
                Id = material.Genre.Id,
                Name = material.Genre.Name
            },
        
    });
});


app.Run();

