using GymManagementDAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementDAL.Data.Configurations
{
    public class GymUserConfiguration : IEntityTypeConfiguration<GymUser>
    {
        public void Configure(EntityTypeBuilder<GymUser> builder)
        {
            builder.Property(u => u.Name)
                .HasColumnType("varchar")
                .HasMaxLength(50);

            builder.Property(u => u.Email)
                .HasColumnType("varchar")
                .HasMaxLength(100);


            builder.Property(u => u.Phone)
                .HasColumnType("varchar")
                .HasMaxLength(11);

            builder.OwnsOne(u => u.Address, address =>
            {
                address.Property(a => a.BuildingNumber)
                    .HasColumnName("BuildingNumber");


                address.Property(a => a.Street)
                    .HasColumnType("varchar")
                    .HasMaxLength(30)
                    .HasColumnName("Street");


                address.Property(a => a.City)
                    .HasColumnType("varchar")
                    .HasMaxLength(30)
                    .HasColumnName("City");
            });

            builder.HasIndex(u => u.Email)
                .IsUnique();
            builder.HasIndex(u => u.Phone)
                .IsUnique();

            builder.ToTable(x =>
            {
                x.HasCheckConstraint("GymUser_EmailCheck", "Email LIKE '%_@__%.__%'");
                x.HasCheckConstraint("GymUser_PhoneCheck", "Phone Like '01%' and Phone Not Like '%[^0-9]%'");
            });

        }
    }
}
