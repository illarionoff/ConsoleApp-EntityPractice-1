using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp_EntityPractice_1
{

    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int CountryId { get; set; }
        public Country Country { get; set; }
        public List<User> Users { get; set; }
    }
    // должность пользователя
    public class Position
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<User> Users { get; set; }
    }
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int? CompanyId { get; set; }
        public Company Company { get; set; }
        public int? PositionId { get; set; }
        public Position Position { get; set; }
    }
    // страна компании
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CapitalId { get; set; }
        public City Capital { get; set; }  // столица страны
        public List<Company> Companies { get; set; }
    }
    // столица страны
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ApplicationContext : DbContext
    {
        public DbSet<Company> Companies { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Position> Positions { get; set; }
        public ApplicationContext()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=newcompaniesdb;Trusted_Connection=True");
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<User>()
        //        .HasOne(p => p.Company)
        //        .WithMany(t => t.Users)
        //        .OnDelete(DeleteBehavior.Cascade);
        //}

    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            using (var db = new ApplicationContext())
            {
                Position manager = new Position { Name = "Manager" };
                Position developer = new Position { Name = "Developer" };
                db.Positions.AddRange(manager, developer);

                City washington = new City { Name = "Washington" };
                db.Cities.Add(washington);

                Country usa = new Country { Name = "USA", Capital = washington };
                db.Countries.Add(usa);

                Company microsoft = new Company { Name = "Microsoft", Country = usa };
                Company google = new Company { Name = "Google", Country = usa };
                db.Companies.AddRange(microsoft, google);
                db.SaveChanges();
                User tom = new User { Name = "Tom", Company = microsoft, Position = manager };
                User bob = new User { Name = "Bob", Company = google, Position = developer };
                User alice = new User { Name = "Alice", Company = microsoft, Position = developer };
                User kate = new User { Name = "Kate", Company = google, Position = manager };
                db.Users.AddRange(tom, bob, alice, kate);
                db.SaveChanges();


                ////Eager loading
                //// получаем пользователей
                //var users = db.Users
                //    .Include(u => u.Company)  // добавляем данные по компаниям
                //    .ThenInclude(comp => comp.Country)      // к компании добавляем страну 
                //    .ThenInclude(count => count.Capital)    // к стране добавляем столицу
                //    .Include(u => u.Position) // добавляем данные по должностям
                //    .ToList();
                //foreach (var user in users)
                //{
                //    Console.WriteLine($"{user.Name} - {user.Position.Name}");
                //    Console.WriteLine($"{user.Company?.Name} - {user.Company?.Country.Name} - {user.Company?.Country.Capital.Name}");
                //    Console.WriteLine("----------------------");     // для красоты
                //}

                //Explicit loading
                Company company = db.Companies.FirstOrDefault();
                db.Users.Where(p => p.CompanyId == company.Id).Load();

                Console.WriteLine($"Company: {company.Name}");
                foreach (var p in company.Users)
                    Console.WriteLine($"User: {p.Name}");

                db.Entry(company).Collection(t => t.Users).Load();

                Console.WriteLine($"Company: {company.Name}");
                foreach (var p in company.Users)
                    Console.WriteLine($"User: {p.Name}");

                User user = db.Users.FirstOrDefault();
                db.Entry(user).Reference(x => x.Company).Load();
                Console.WriteLine($"{user.Name} - {user?.Company.Name}");
            }
        }
    }
}