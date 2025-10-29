using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication2.db
{
    public class TaxiContext : IdentityDbContext<ApplicationUser>
    {
        public TaxiContext(DbContextOptions<TaxiContext> options) : base(options) { }

        public DbSet<Service> Services { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>()
                .Property(o => o.Distance)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalPrice)
                .HasPrecision(18, 2);


            modelBuilder.Entity<Service>()
                .Property(s => s.PricePerKm)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Service>()
                .Property(s => s.BasePrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Driver>()
                .Property(d => d.Rating)
                .HasPrecision(3, 2);

            modelBuilder.Entity<Car>()
                .HasMany(c => c.Drivers)
                .WithMany(d => d.Cars)
                .UsingEntity(j => j.ToTable("CarDriver"));

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Order)
                .WithMany()
                .HasForeignKey(r => r.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Driver)
                .WithMany()
                .HasForeignKey(r => r.DriverId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Service>().HasData(
                new Service
                {
                    Id = 1,
                    Name = "Економ",
                    Description = "Стандартний автомобіль для щоденних поїздок",
                    PricePerKm = 8.50m,
                    BasePrice = 40.00m,
                    CarType = "Sedan"
                },
                new Service
                {
                    Id = 2,
                    Name = "Комфорт",
                    Description = "Комфортабельний автомобіль середнього класу",
                    PricePerKm = 12.00m,
                    BasePrice = 60.00m,
                    CarType = "Business Sedan"
                },
                new Service
                {
                    Id = 3,
                    Name = "Бізнес",
                    Description = "Преміум автомобіль для ділових поїздок",
                    PricePerKm = 18.00m,
                    BasePrice = 100.00m,
                    CarType = "Premium"
                },
                new Service
                {
                    Id = 4,
                    Name = "Мінівен",
                    Description = "Просторий автомобіль для великих компаній (до 7 осіб)",
                    PricePerKm = 15.00m,
                    BasePrice = 80.00m,
                    CarType = "Minivan"
                }
            );

            modelBuilder.Entity<Car>().HasData(
                new Car { Id = 1, Brand = "Toyota", Model = "Camry", LicensePlate = "AA1234BB", Color = "Сірий", Year = 2020, BodyType = "Sedan", IsAvailable = true },
                new Car { Id = 2, Brand = "Skoda", Model = "Octavia", LicensePlate = "BB5678CC", Color = "Білий", Year = 2021, BodyType = "Sedan", IsAvailable = true },
                new Car { Id = 3, Brand = "Mercedes-Benz", Model = "E-Class", LicensePlate = "CC9012DD", Color = "Чорний", Year = 2022, BodyType = "Sedan", IsAvailable = true },
                new Car { Id = 4, Brand = "Volkswagen", Model = "Multivan", LicensePlate = "DD3456EE", Color = "Синій", Year = 2019, BodyType = "Minivan", IsAvailable = true }
            );

            modelBuilder.Entity<Driver>().HasData(
                new Driver { Id = 1, Name = "Іван Петренко", Phone = "+380671234567", LicenseNumber = "ВК123456", Experience = 5, Rating = 4.8m },
                new Driver { Id = 2, Name = "Олена Коваленко", Phone = "+380502345678", LicenseNumber = "АС789012", Experience = 3, Rating = 4.9m },
                new Driver { Id = 3, Name = "Андрій Шевченко", Phone = "+380933456789", LicenseNumber = "НМ345678", Experience = 7, Rating = 4.7m }
            );
        }
    }
}