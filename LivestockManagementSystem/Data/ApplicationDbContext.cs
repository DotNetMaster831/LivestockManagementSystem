using LivestockManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LivestockManagementSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Cow> Cow { get; set; }
        public DbSet<Sheep> Sheep { get; set; }

        private static string DbPath = Path.Combine(Environment.CurrentDirectory, "Resources", "Database", "FarmData.db");

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename={DbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cow>().HasData(
                new Cow { Id = 1, Expense = 46, Weight = 400, Colour = "Red", Milk = 23.5 },
                new Cow { Id = 2, Expense = 40.5, Weight = 450.5, Colour = "Black", Milk = 27.1 },
                new Cow { Id = 3, Expense = 43, Weight = 430, Colour = "Black", Milk = 21.9 },
                new Cow { Id = 4, Expense = 39, Weight = 330.5, Colour = "Red", Milk = 16.4 },
                new Cow { Id = 5, Expense = 30.5, Weight = 310, Colour = "Red", Milk = 15.8 }
            );

            modelBuilder.Entity<Sheep>().HasData(
                new Sheep { Id = 50001, Expense = 6.5, Weight = 22.5, Colour = "White", Wool = 9.5 },
                new Sheep { Id = 50002, Expense = 6.5, Weight = 28.5, Colour = "White", Wool = 9.2 },
                new Sheep { Id = 50003, Expense = 6.5, Weight = 33.2, Colour = "White", Wool = 8.9 },
                new Sheep { Id = 50004, Expense = 5.5, Weight = 23.5, Colour = "White", Wool = 8.2 },
                new Sheep { Id = 50005, Expense = 6.4, Weight = 25.5, Colour = "Black", Wool = 9.3 },
                new Sheep { Id = 50006, Expense = 5.4, Weight = 24.2, Colour = "White", Wool = 8.8 }
            );
        }

    }
}
