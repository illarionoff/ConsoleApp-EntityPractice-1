using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp_EntityPractice_1
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public ApplicationContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=helloappdb;Trusted_Connection=True");
        }

        //public ApplicationContext(DbContextOptions<ApplicationContext> options)
        //    : base(options)
        //{
        //    Database.EnsureCreated();
        //}
    }

    internal class Program
    {
        private static void Main(string[] args)
        {

            //var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();

            //var options = optionsBuilder
            //    .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=helloappdb;Trusted_Connection=True;")
            //    .Options;

            //using (ApplicationContext db = new ApplicationContext(options))
            using (ApplicationContext db = new ApplicationContext())
            {
                User user1 = new User {Name = "Tom", Age = 33};
                User user2 = new User {Name = "Alice", Age = 26};

                db.Users.Add(user1);
                db.Users.Add(user2);
                db.SaveChanges();
            }

            using (ApplicationContext db = new ApplicationContext())
            {
                var users = db.Users.ToList();
                foreach (User u in users)
                {
                    Console.WriteLine($"{u.Id}.{u.Name} - {u.Age}");
                }
            }

            using (ApplicationContext db = new ApplicationContext())
            {
                // получаем первый объект
                User user = db.Users.FirstOrDefault();
                if (user != null)
                {
                    user.Name = "Bob";
                    user.Age = 1;
                    //обновляем объект
                    //db.Users.Update(user);
                    db.SaveChanges();
                }

                // выводим данные после обновления
                Console.WriteLine("\nДанные после редактирования:");
                var users = db.Users.ToList();
                foreach (User u in users)
                {
                    Console.WriteLine($"{u.Id}.{u.Name} - {u.Age}");
                }

            }

            using (ApplicationContext db = new ApplicationContext())
            {
                // получаем первый объект
                User user = db.Users.FirstOrDefault();
                if (user != null)
                {
                    //удаляем объект
                    db.Users.Remove(user);
                    db.SaveChanges();
                }

                // выводим данные после обновления
                Console.WriteLine("\nДанные после удаления:");
                var users = db.Users.ToList();
                foreach (User u in users)
                {
                    Console.WriteLine($"{u.Id}.{u.Name} - {u.Age}");
                }
            }
        }
    }
}