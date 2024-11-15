using LoncotesLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using LoncotesLibrary.Models.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;

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

app.MapGet("/api/materials", (LoncotesLibraryDbContext db, int? materialTypeId, int? genreId) => 
{
    List<Material> materials = db.Materials
    .Where(material => material.OutOfCirculationSince == null)
    .Include(m => m.MaterialType)
    .Include(m => m.Genre).ToList();
    if (materialTypeId != null)
    {
        materials = materials.Where(m => m.MaterialTypeId == materialTypeId).ToList();
    }
    if (genreId != null)
    {
        materials = materials.Where(m => m.GenreId == genreId).ToList();
    }
    return materials
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

app.MapGet("/api/materials/{id}", (LoncotesLibraryDbContext db, int id) => 
{   
    return db.Materials
    .Where(m => m.Id == id)
    .Include(m => m.MaterialType)
    .Include(m => m.Genre)
    .Include(m => m.Checkouts)
    .ThenInclude(c => c.Patron)
    .Select(m => new MaterialDTO 
    {
        Id = m.Id,
        MaterialName = m.MaterialName,
        GenreId = m.GenreId,
        Genre = new GenreDTO { Id = m.Genre.Id, Name = m.Genre.Name},
        MaterialTypeId = m.MaterialTypeId,
        MaterialType = new MaterialTypeDTO { Id = m.MaterialType.Id, Name = m.MaterialType.Name, CheckoutDays = m.MaterialType.CheckoutDays},
        OutOfCirculationSince = m.OutOfCirculationSince,
        Checkouts = m.Checkouts.Select(c => new CheckoutDTO
            {
                Id = c.Id,
                PatronId = c.PatronId,
                Patron = new PatronDTO 
                {
                    Id = c.Patron.Id,
                    FirstName = c.Patron.FirstName,
                    LastName = c.Patron.LastName,
                    Address = c.Patron.Address,
                    Email = c.Patron.Email,
                    IsActive = c.Patron.IsActive

                },
                CheckoutDate = c.CheckoutDate
                

            }).ToList()

    });
});

app.MapPost("/api/materials", (LoncotesLibraryDbContext db , Material material) => 
{
    db.Materials.Add(material);
    db.SaveChanges();
    return Results.Created($"/api/materials/{material.Id}", new MaterialDTO 
        {
            Id = material.Id,
            MaterialName = material.MaterialName,
            MaterialTypeId = material.MaterialTypeId,
            GenreId = material.GenreId,

        });
});

app.MapPut("/api/materials/{id}", (LoncotesLibraryDbContext db, int id) => 
{
    Material material = db.Materials.SingleOrDefault(m => m.Id == id);
    material.OutOfCirculationSince = DateTime.Now;
    db.SaveChanges();
    return Results.Accepted();
});

app.MapGet("/api/materialtypes", (LoncotesLibraryDbContext db) => 
{
    return db.MaterialTypes.Select(mt => new MaterialTypeDTO
    {
        Id = mt.Id,
        Name = mt.Name,
        CheckoutDays = mt.CheckoutDays
    });

});

app.MapGet("/api/genres", (LoncotesLibraryDbContext db) => 
{
    return db.Genres.Select(g => new GenreDTO
    {
        Id = g.Id,
        Name = g.Name
    });

});

app.MapGet("/api/patrons", (LoncotesLibraryDbContext db) => 
{
    return db.Patrons.Select(p => new PatronDTO
    {
        Id = p.Id,
        FirstName = p.FirstName,
        LastName = p.LastName,
        Address = p.Address,
        Email = p.Email,
        IsActive = p.IsActive
    });

});

app.MapGet("/api/patrons/{id}", (LoncotesLibraryDbContext db, int id) => 
{
   
    ;
    return db.Patrons
    .Include(p => p.Checkouts)
    .ThenInclude(c => c.Material)
    .ThenInclude(m => m.MaterialType)
    .Select(p => new PatronDTO 
    {
        Id = p.Id,
        FirstName = p.FirstName,
        LastName = p.LastName,
        Address = p.Address,
        Email = p.Email,
        IsActive = p.IsActive,
        Checkouts = p.Checkouts.Select(c => new CheckoutDTO 
        {
            Id = c.Id,
            CheckoutDate = c.CheckoutDate,
            Material = new MaterialDTO { 
                Id = c.Material.Id, 
                MaterialName = c.Material.MaterialName, 
                MaterialType = new MaterialTypeDTO 
                {
                    Id = c.Material.MaterialType.Id,
                    Name = c.Material.MaterialType.Name,
                    CheckoutDays = c.Material.MaterialType.CheckoutDays

                }
            }
            

        }).ToList()
        
        
    }).SingleOrDefault(p => p.Id == id);
});

app.MapPut("/api/patrons/{id}", (LoncotesLibraryDbContext db, int id) => 
{
    Patron patron = db.Patrons.SingleOrDefault(p => p.Id == id);
    if (patron != null)
    {
        patron.IsActive = false;
        db.SaveChanges();
        return Results.Accepted();
    }
    return Results.BadRequest();
});

app.MapPost("/api/checkouts", (LoncotesLibraryDbContext db, Checkout checkout) => 
{
    if (db.Materials.All(m => m.OutOfCirculationSince != null && m.Id == checkout.MaterialId))
    {
        return Results.BadRequest();
    }
    checkout.CheckoutDate = DateTime.Today;
    db.Add(checkout);
    db.SaveChanges();
    return Results.Created($"/api/checkouts/{checkout.Id}", new CheckoutDTO
    {
        Id = checkout.Id,
        MaterialId = checkout.MaterialId,
        PatronId = checkout.PatronId,
        CheckoutDate = checkout.CheckoutDate
    });
});

app.MapPut("/api/checkouts/{id}/return", (int id, LoncotesLibraryDbContext db) => 
{
    Checkout checkout = db.Checkouts.SingleOrDefault(c => c.Id == id);
    if (checkout != null)
    {
        checkout.ReturnDate = DateTime.Today;
        db.SaveChanges();
        return Results.Accepted();
    }
    return Results.BadRequest();
});

app.MapGet("/api/Materials/available", (LoncotesLibraryDbContext db) => 
{
    return db.Materials
    .Where(m => m.OutOfCirculationSince == null)
    .Where(m => m.Checkouts.All(c => c.ReturnDate != null))
    .Include(m => m.Genre)
    .Include(m => m.MaterialType)
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
            }

    }).ToList();
});




app.Run();

