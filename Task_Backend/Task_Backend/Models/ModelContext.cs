using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Task_Backend.Models;

public partial class ModelContext : IdentityDbContext<User>
{
    public ModelContext(DbContextOptions<ModelContext> options) : base(options) { }

    public DbSet<Document> Documents { get; set; }
    public DbSet<DocumentItem> DocumentItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // schema cfg for identity

        modelBuilder.HasDefaultSchema("dbo");

        modelBuilder.Entity<IdentityUserLogin<string>>()
            .ToTable("UserLogins");

        modelBuilder.Entity<IdentityUserRole<string>>()
            .ToTable("UserRoles");

        modelBuilder.Entity<IdentityUserToken<string>>()
            .ToTable("UserTokens");

        modelBuilder.Entity<IdentityRole>()
            .ToTable("Roles");

        modelBuilder.Entity<IdentityUserClaim<string>>()
            .ToTable("UserClaims");

        modelBuilder.Entity<IdentityRoleClaim<string>>()
            .ToTable("RoleClaims");

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
        });

        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Type).IsRequired();
            entity.Property(e => e.Date).HasColumnName("date").IsRequired();
            entity.Property(e => e.FirstName).IsRequired();
            entity.Property(e => e.LastName).IsRequired();
            entity.Property(e => e.City).IsRequired();

            entity.HasMany(e => e.DocumentItems)
                .WithOne(di => di.Document)
                .HasForeignKey(di => di.DocumentId);
        });


        modelBuilder.Entity<DocumentItem>(entity =>
        {
            entity.HasKey(e => new { e.DocumentId, e.Ordinal });

            entity.Property(e => e.Product).IsRequired();
            entity.Property(e => e.Quantity).IsRequired();
            entity.Property(e => e.Price).IsRequired();
            entity.Property(e => e.TaxRate).IsRequired();
        });

    }
    
}