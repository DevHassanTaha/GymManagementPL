using GymManagementDAL.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementDAL.Data.Contexts
{
    public class GymDbContext : IdentityDbContext<ApplicationUser>
    {
        public GymDbContext(DbContextOptions<GymDbContext> options) : base(options)
        {

        }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.Entity<ApplicationUser>(au =>
            {
                au.Property(u => u.FirstName)
                .HasColumnType("varchar")
                .HasMaxLength(50);

                au.Property(u => u.LastName)
                .HasColumnType("varchar")
                .HasMaxLength(50);

            });
        }

        #region DbSet
        public DbSet<Trainer> trainers { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Category> categories { get; set; }
        public DbSet<Session> sessions { get; set; }
        public DbSet<Booking> bookings { get; set; }
        public DbSet<Membership> Memberships { get; set; }
        public DbSet<HealthRecord> HealthRecords { get; set; }
        public DbSet<Plan> Plans { get; set; }
        #endregion
    }
}
