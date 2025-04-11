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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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
                .OnDelete(DeleteBehavior.Restrict); // Ngăn xóa Passenger nếu có Bill liên quan

            // Coach
            modelBuilder.Entity<Coach>()
                .HasOne(c => c.VehicleType)
                .WithMany()
                .HasForeignKey(c => c.IdType)
                .OnDelete(DeleteBehavior.Restrict); // Ngăn xóa VehicleType nếu có Coach liên quan

            modelBuilder.Entity<Coach>()
                .HasOne(c => c.RegistForm)
                .WithMany()
                .HasForeignKey(c => c.IdRegist)
                .OnDelete(DeleteBehavior.Restrict); // Ngăn xóa RegistForm nếu có Coach liên quan

            // DiscountDetails
            modelBuilder.Entity<DiscountDetails>()
                .HasOne(dd => dd.Coupon)
                .WithMany()
                .HasForeignKey(dd => dd.IdCoupon)
                .OnDelete(DeleteBehavior.Restrict); // Ngăn xóa Coupon nếu có DiscountDetails liên quan

            modelBuilder.Entity<DiscountDetails>()
                .HasOne(dd => dd.Bill)
                .WithMany()
                .HasForeignKey(dd => dd.IdBill)
                .OnDelete(DeleteBehavior.Restrict); // Ngăn xóa Bill nếu có DiscountDetails liên quan

            // District
            modelBuilder.Entity<District>()
                .HasOne(d => d.City)
                .WithMany()
                .HasForeignKey(d => d.IdCity)
                .OnDelete(DeleteBehavior.Restrict); // Ngăn xóa City nếu có District liên quan

            // DropOff
            modelBuilder.Entity<DropOff>()
                .HasOne(d => d.City)
                .WithMany()
                .HasForeignKey(d => d.IdCity)
                .OnDelete(DeleteBehavior.Restrict); // Ngăn xóa City nếu có DropOff liên quan

            modelBuilder.Entity<DropOff>()
                .HasOne(d => d.RegistForm)
                .WithMany()
                .HasForeignKey(d => d.IdRegist)
                .OnDelete(DeleteBehavior.Restrict); // Ngăn xóa RegistForm nếu có DropOff liên quan

            // Employee
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Brand)
                .WithMany()
                .HasForeignKey(e => e.IdBrand)
                .OnDelete(DeleteBehavior.Restrict); // Ngăn xóa Brand nếu có Employee liên quan

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Position)
                .WithMany()
                .HasForeignKey(e => e.IdPos)
                .OnDelete(DeleteBehavior.Restrict); // Ngăn xóa Position nếu có Employee liên quan

            // Feedback
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Passenger)
                .WithMany()
                .HasForeignKey(f => f.IdPassenger)
                .OnDelete(DeleteBehavior.Restrict); // Ngăn xóa Passenger nếu có Feedback liên quan

            // News
            modelBuilder.Entity<News>()
                .HasOne(n => n.TypeNews)
                .WithMany()
                .HasForeignKey(n => n.IdTypeNews)
                .OnDelete(DeleteBehavior.Restrict); // Ngăn xóa TypeNews nếu có News liên quan

            // Pickup
            modelBuilder.Entity<Pickup>()
                .HasOne(p => p.City)
                .WithMany()
                .HasForeignKey(p => p.IdCity)
                .OnDelete(DeleteBehavior.Restrict); // Ngăn xóa City nếu có Pickup liên quan

            modelBuilder.Entity<Pickup>()
                .HasOne(p => p.RegistForm)
                .WithMany()
                .HasForeignKey(p => p.IdRegist)
                .OnDelete(DeleteBehavior.Restrict); // Ngăn xóa RegistForm nếu có Pickup liên quan

            // Price
            modelBuilder.Entity<Price>()
                .HasOne(p => p.BusRoute)
                .WithMany()
                .HasForeignKey(p => p.IdRoute)
                .OnDelete(DeleteBehavior.Restrict); // Ngăn xóa BusRoute nếu có Price liên quan

            modelBuilder.Entity<Price>()
                .HasOne(p => p.RouteStopStart)
                .WithMany()
                .HasForeignKey(p => p.IdStopStart)
                .OnDelete(DeleteBehavior.Restrict); // Ngăn xóa RouteStop (điểm bắt đầu) nếu có Price liên quan

            modelBuilder.Entity<Price>()
                .HasOne(p => p.RouteStopEnd)
                .WithMany()
                .HasForeignKey(p => p.IdStopEnd)
                .OnDelete(DeleteBehavior.Restrict); // Ngăn xóa RouteStop (điểm kết thúc) nếu có Price liên quan

            modelBuilder.Entity<Price>()
                .HasOne(p => p.Coach)
                .WithMany()
                .HasForeignKey(p => p.IdCoach)
                .OnDelete(DeleteBehavior.Restrict); // Ngăn xóa Coach nếu có Price liên quan

            // RegistForm
            modelBuilder.Entity<RegistForm>()
                .HasOne(r => r.Brand)
                .WithMany()
                .HasForeignKey(r => r.IdBrand)
                .OnDelete(DeleteBehavior.Restrict); // Ngăn xóa Brand nếu có RegistForm liên quan

            // Route
            modelBuilder.Entity<BusRoute>()
                .HasOne(r => r.RegistForm)
                .WithMany()
                .HasForeignKey(r => r.IdRegist)
                .OnDelete(DeleteBehavior.Restrict); // Ngăn xóa RegistForm nếu có BusRoute liên quan

            // RouteStop
            modelBuilder.Entity<RouteStop>()
                .HasOne(rs => rs.BusRoute)
                .WithMany()
                .HasForeignKey(rs => rs.IdRoute)
                .OnDelete(DeleteBehavior.Restrict); // Ngăn xóa BusRoute nếu có RouteStop liên quan

            // ScheduleDetails
            modelBuilder.Entity<ScheduleDetails>()
                .HasOne(sd => sd.Coach)
                .WithMany()
                .HasForeignKey(sd => sd.IdCoach)
                .OnDelete(DeleteBehavior.Restrict); // Ngăn xóa Coach nếu có ScheduleDetails liên quan

            modelBuilder.Entity<ScheduleDetails>()
                .HasOne(sd => sd.BusRoute)
                .WithMany()
                .HasForeignKey(sd => sd.IdRoute)
                .OnDelete(DeleteBehavior.Restrict); // Ngăn xóa BusRoute nếu có ScheduleDetails liên quan

            // Seat
            modelBuilder.Entity<Seat>()
                .HasOne(s => s.Coach)
                .WithMany()
                .HasForeignKey(s => s.IdCoach)
                .OnDelete(DeleteBehavior.Restrict); // Ngăn xóa Coach nếu có Seat liên quan

            // ServiceDetails
            modelBuilder.Entity<ServiceDetails>()
                .HasOne(svd => svd.VehicleType)
                .WithMany()
                .HasForeignKey(svd => svd.IdType)
                .OnDelete(DeleteBehavior.Restrict); // Ngăn xóa VehicleType nếu có ServiceDetails liên quan

            modelBuilder.Entity<ServiceDetails>()
                .HasOne(svd => svd.Service)
                .WithMany()
                .HasForeignKey(svd => svd.IdService)
                .OnDelete(DeleteBehavior.Restrict); // Ngăn xóa Service nếu có ServiceDetails liên quan

            // Ticket
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Seat)
                .WithMany()
                .HasForeignKey(t => t.IdSeat)
                .OnDelete(DeleteBehavior.Restrict); // Ngăn xóa Seat nếu có Ticket liên quan

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Price)
                .WithMany()
                .HasForeignKey(t => t.IdPrice)
                .OnDelete(DeleteBehavior.Restrict); // Ngăn xóa Price nếu có Ticket liên quan

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Passenger)
                .WithMany()
                .HasForeignKey(t => t.IdPassenger)
                .OnDelete(DeleteBehavior.Restrict); // Ngăn xóa Passenger nếu có Ticket liên quan

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Employee)
                .WithMany()
                .HasForeignKey(t => t.IdEmployee)
                .OnDelete(DeleteBehavior.Restrict); // Ngăn xóa Employee nếu có Ticket liên quan

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.BusRoute)
                .WithMany()
                .HasForeignKey(t => t.IdRoute)
                .OnDelete(DeleteBehavior.Restrict); // Ngăn xóa BusRoute nếu có Ticket liên quan

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Coach)
                .WithMany()
                .HasForeignKey(t => t.IdCoach)
                .OnDelete(DeleteBehavior.Restrict); // Ngăn xóa Coach nếu có Ticket liên quan
        }

    }
}
