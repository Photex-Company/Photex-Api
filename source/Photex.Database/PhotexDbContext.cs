using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Photex.Database.Entities;

namespace Photex.Database
{
    public class PhotexDbContext : IdentityDbContext<ApplicationUser, IdentityRole<long>, long>
    {
        public PhotexDbContext(DbContextOptions<PhotexDbContext> options)
            : base(options) { }

        public DbSet<Image> Images { get; set; }
        public DbSet<Catalogue> Catalogues { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany<Catalogue>()
                .WithOne()
                .HasForeignKey(x => x.UserId);

            modelBuilder.Entity<Catalogue>()
                .HasIndex(x => x.UserId);

            modelBuilder.Entity<Catalogue>()
                .Property(x => x.Name)
                .HasMaxLength(255);

            modelBuilder.Entity<Catalogue>()
                .HasMany(x => x.Images)
                .WithOne(x => x.Catalogue);

            modelBuilder.Entity<Image>()
                .Property(x => x.Description)
                .HasMaxLength(1024);
        }
    }
}
