using Microsoft.EntityFrameworkCore;
using LoncotesLibrary.Models;

public class LoncotesLibraryDbContext : DbContext
{
    public DbSet<Patron> Patrons {get; set;}
    public DbSet<MaterialType> MaterialTypes {get; set;}
    public DbSet<Material> Materials {get; set;}
    public DbSet<Genre> Genres {get; set;}
    public DbSet<Checkout> Checkouts {get; set;}

    public LoncotesLibraryDbContext(DbContextOptions<LoncotesLibraryDbContext> context) : base(context)
    {

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MaterialType>().HasData(new MaterialType[]
        {
            new MaterialType {Id = 1, Name = "Book", CheckoutDays = 21},
            new MaterialType {Id = 2, Name="DVD", CheckoutDays=7},
            new MaterialType {Id=3, Name="Magazine", CheckoutDays=14}
        });
        modelBuilder.Entity<Patron>().HasData(new Patron[]
        {
            new Patron { Id = 1, FirstName = "John", LastName = "Doe", Address = "123 Library St", Email = "johndoe@example.com", IsActive = true },
            new Patron { Id = 2, FirstName = "Jane", LastName = "Smith", Address = "456 Book Ave", Email = "janesmith@example.com", IsActive = false }
        });
        modelBuilder.Entity<Genre>().HasData(new Genre[]
        {
            new Genre { Id = 1, Name = "Science Fiction" },
            new Genre { Id = 2, Name = "Mystery" },
            new Genre { Id = 3, Name = "Fantasy" },
            new Genre { Id = 4, Name = "Non-Fiction" },
            new Genre { Id = 5, Name = "Historical Fiction" }
        });
        modelBuilder.Entity<Material>().HasData(new Material[]
        {
            new Material { Id = 1, MaterialName = "Dune", MaterialTypeId = 1, GenreId = 3, OutOfCirculationSince = null },
            new Material { Id = 2, MaterialName = "The Great Gatsby", MaterialTypeId = 1, GenreId = 5, OutOfCirculationSince = null },
            new Material { Id = 3, MaterialName = "Cosmos", MaterialTypeId = 1, GenreId = 4, OutOfCirculationSince = DateTime.Parse("2021-05-10") },
            new Material { Id = 4, MaterialName = "The Matrix", MaterialTypeId = 2, GenreId = 3, OutOfCirculationSince = null },
            new Material { Id = 5, MaterialName = "Sherlock Holmes Collection", MaterialTypeId = 1, GenreId = 2, OutOfCirculationSince = null },
            new Material { Id = 6, MaterialName = "Harry Potter and the Sorcerer's Stone", MaterialTypeId = 1, GenreId = 3, OutOfCirculationSince = null },
            new Material { Id = 7, MaterialName = "Inception", MaterialTypeId = 2, GenreId = 3, OutOfCirculationSince = null },
            new Material { Id = 8, MaterialName = "Sapiens: A Brief History of Humankind", MaterialTypeId = 1, GenreId = 4, OutOfCirculationSince = null },
            new Material { Id = 9, MaterialName = "National Geographic Magazine - July 2023", MaterialTypeId = 3, GenreId = 4, OutOfCirculationSince = DateTime.Parse("2023-08-01") },
            new Material { Id = 10, MaterialName = "A Brief History of Time", MaterialTypeId = 1, GenreId = 4, OutOfCirculationSince = null }
        });
    }
}