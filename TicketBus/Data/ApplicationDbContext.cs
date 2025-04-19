using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TicketBus.Models;

namespace TicketBus.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Coach> Coaches { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<DiscountDetails> DiscountDetails { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<DropOff> DropOffs { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Passenger> Passengers { get; set; }
        public DbSet<Pickup> Pickups { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<RegistForm> RegistForms { get; set; }
        public DbSet<BusRoute> BusRoutes { get; set; }
        public DbSet<RouteStop> RouteStops { get; set; }
        public DbSet<ScheduleDetails> ScheduleDetails { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ServiceDetails> ServiceDetails { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TypeNews> TypeNews { get; set; }
        public DbSet<VehicleType> VehicleTypes { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Dữ liệu mẫu cho VehicleType
            modelBuilder.Entity<VehicleType>().HasData(
                new VehicleType
                {
                    IdType = 1,
                    TypeCode = "VT001",
                    NameType = "Xe khách 16 chỗ",
                    SeatCount = 16,
                    State = VehicleTypeState.HoatDong
                },
                new VehicleType
                {
                    IdType = 2,
                    TypeCode = "VT002",
                    NameType = "Xe khách 29 chỗ",
                    SeatCount = 29,
                    State = VehicleTypeState.HoatDong
                },
                new VehicleType
                {
                    IdType = 3,
                    TypeCode = "VT003",
                    NameType = "Xe khách 45 chỗ",
                    SeatCount = 45,
                    State = VehicleTypeState.HoatDong
                },
                new VehicleType
                {
                    IdType = 4,
                    TypeCode = "VT004",
                    NameType = "Xe limousine 9 chỗ",
                    SeatCount = 9,
                    State = VehicleTypeState.HoatDong
                }
            );

            // Cấu hình khóa chính
            modelBuilder.Entity<Bill>().HasKey(b => b.IdBill);
            modelBuilder.Entity<Brand>().HasKey(b => b.IdBrand);
            modelBuilder.Entity<City>().HasKey(c => c.IdCity);
            modelBuilder.Entity<Coach>().HasKey(c => c.IdCoach);
            modelBuilder.Entity<Coupon>().HasKey(c => c.IdCoupon);
            modelBuilder.Entity<District>().HasKey(d => d.IdDistrict);
            modelBuilder.Entity<DropOff>().HasKey(d => d.IdDropOff);
            modelBuilder.Entity<Employee>().HasKey(e => e.IdEmployee);
            modelBuilder.Entity<Feedback>().HasKey(f => f.IdFeedback);
            modelBuilder.Entity<News>().HasKey(n => n.IdNews);
            modelBuilder.Entity<Passenger>().HasKey(p => p.IdPassenger);
            modelBuilder.Entity<Pickup>().HasKey(p => p.IdPickup);
            modelBuilder.Entity<Position>().HasKey(p => p.IdPos);
            modelBuilder.Entity<Price>().HasKey(p => p.IdPrice);
            modelBuilder.Entity<RegistForm>().HasKey(r => r.IdRegist);
            modelBuilder.Entity<BusRoute>().HasKey(r => r.IdRoute);
            modelBuilder.Entity<RouteStop>().HasKey(rs => rs.IdStop);
            modelBuilder.Entity<Seat>().HasKey(s => s.IdSeat);
            modelBuilder.Entity<Service>().HasKey(s => s.IdService);
            modelBuilder.Entity<Ticket>().HasKey(t => t.IdTicket);
            modelBuilder.Entity<TypeNews>().HasKey(tn => tn.IdTypeNews);
            modelBuilder.Entity<VehicleType>().HasKey(vt => vt.IdType);

            // Cấu hình khóa chính tổ hợp
            modelBuilder.Entity<DiscountDetails>()
                .HasKey(dd => new { dd.IdCoupon, dd.IdBill });

            modelBuilder.Entity<ScheduleDetails>()
                .HasKey(sd => new { sd.IdCoach, sd.IdRoute });

            modelBuilder.Entity<ServiceDetails>()
                .HasKey(svd => new { svd.IdType, svd.IdService });

            // Cấu hình mối quan hệ
            // Bill
            modelBuilder.Entity<Bill>()
                .HasOne(b => b.Passenger)
                .WithMany()
                .HasForeignKey(b => b.IdPassenger)
                .OnDelete(DeleteBehavior.Restrict);

            // Coach
            modelBuilder.Entity<Coach>()
                .HasOne(c => c.VehicleType)
                .WithMany()
                .HasForeignKey(c => c.IdType)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Coach>()
                .HasOne(c => c.RegistForm)
                .WithMany()
                .HasForeignKey(c => c.IdRegist)
                .OnDelete(DeleteBehavior.Restrict);

            // DiscountDetails
            modelBuilder.Entity<DiscountDetails>()
                .HasOne(dd => dd.Coupon)
                .WithMany()
                .HasForeignKey(dd => dd.IdCoupon)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DiscountDetails>()
                .HasOne(dd => dd.Bill)
                .WithMany()
                .HasForeignKey(dd => dd.IdBill)
                .OnDelete(DeleteBehavior.Restrict);

            // District
            modelBuilder.Entity<District>()
                .HasOne(d => d.City)
                .WithMany()
                .HasForeignKey(d => d.IdCity)
                .OnDelete(DeleteBehavior.Restrict);

            // DropOff
            modelBuilder.Entity<DropOff>()
                .HasOne(d => d.City)
                .WithMany()
                .HasForeignKey(d => d.IdCity)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DropOff>()
                .HasOne(d => d.RegistForm)
                .WithMany()
                .HasForeignKey(d => d.IdRegist)
                .OnDelete(DeleteBehavior.Restrict);

            // Employee
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Brand)
                .WithMany()
                .HasForeignKey(e => e.IdBrand)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Position)
                .WithMany()
                .HasForeignKey(e => e.IdPos)
                .OnDelete(DeleteBehavior.Restrict);

            // Feedback
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Passenger)
                .WithMany()
                .HasForeignKey(f => f.IdPassenger)
                .OnDelete(DeleteBehavior.Restrict);

            // News
            modelBuilder.Entity<News>()
                .HasOne(n => n.TypeNews)
                .WithMany()
                .HasForeignKey(n => n.IdTypeNews)
                .OnDelete(DeleteBehavior.Restrict);

            // Pickup
            modelBuilder.Entity<Pickup>()
                .HasOne(p => p.City)
                .WithMany()
                .HasForeignKey(p => p.IdCity)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pickup>()
                .HasOne(p => p.RegistForm)
                .WithMany()
                .HasForeignKey(p => p.IdRegist)
                .OnDelete(DeleteBehavior.Restrict);

            // Price
            modelBuilder.Entity<Price>()
                .HasOne(p => p.BusRoute)
                .WithMany()
                .HasForeignKey(p => p.IdRoute)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Price>()
                .HasOne(p => p.RouteStopStart)
                .WithMany()
                .HasForeignKey(p => p.IdStopStart)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Price>()
                .HasOne(p => p.RouteStopEnd)
                .WithMany()
                .HasForeignKey(p => p.IdStopEnd)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Price>()
                .HasOne(p => p.Coach)
                .WithMany()
                .HasForeignKey(p => p.IdCoach)
                .OnDelete(DeleteBehavior.Restrict);

            // RegistForm
            modelBuilder.Entity<RegistForm>()
                .HasOne(r => r.Brand)
                .WithMany()
                .HasForeignKey(r => r.IdBrand)
                .OnDelete(DeleteBehavior.Restrict);

            // Route
            modelBuilder.Entity<BusRoute>()
                .HasOne(r => r.RegistForm)
                .WithMany()
                .HasForeignKey(r => r.IdRegist)
                .OnDelete(DeleteBehavior.Restrict);

            // RouteStop
            modelBuilder.Entity<RouteStop>()
                .HasOne(rs => rs.BusRoute)
                .WithMany()
                .HasForeignKey(rs => rs.IdRoute)
                .OnDelete(DeleteBehavior.Restrict);

            // ScheduleDetails
            modelBuilder.Entity<ScheduleDetails>()
                .HasOne(sd => sd.Coach)
                .WithMany()
                .HasForeignKey(sd => sd.IdCoach)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ScheduleDetails>()
                .HasOne(sd => sd.BusRoute)
                .WithMany()
                .HasForeignKey(sd => sd.IdRoute)
                .OnDelete(DeleteBehavior.Restrict);

            // Seat
            modelBuilder.Entity<Seat>()
                .HasOne(s => s.Coach)
                .WithMany()
                .HasForeignKey(s => s.IdCoach)
                .OnDelete(DeleteBehavior.Restrict);

            // ServiceDetails
            modelBuilder.Entity<ServiceDetails>()
                .HasOne(svd => svd.VehicleType)
                .WithMany()
                .HasForeignKey(svd => svd.IdType)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ServiceDetails>()
                .HasOne(svd => svd.Service)
                .WithMany()
                .HasForeignKey(svd => svd.IdService)
                .OnDelete(DeleteBehavior.Restrict);

            // Ticket
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Seat)
                .WithMany()
                .HasForeignKey(t => t.IdSeat)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Price)
                .WithMany()
                .HasForeignKey(t => t.IdPrice)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Passenger)
                .WithMany()
                .HasForeignKey(t => t.IdPassenger)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Employee)
                .WithMany()
                .HasForeignKey(t => t.IdEmployee)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
