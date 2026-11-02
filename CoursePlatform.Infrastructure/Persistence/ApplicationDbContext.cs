using CoursePlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoursePlatform.Infrastructure.Persistence;

/// <summary>
/// EF Core context for the application. Entity mapping lives in
/// <c>Persistence/Configurations</c> so this file stays small as features are
/// added.
/// </summary>
public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
