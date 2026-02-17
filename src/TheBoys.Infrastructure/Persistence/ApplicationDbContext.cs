using Microsoft.EntityFrameworkCore;
using TheBoys.Domain.Entities;

namespace TheBoys.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public DbSet<News> News { get; set; }
    public DbSet<NewsUniv> NewsUnivs { get; set; }
    public DbSet<NewsUnivTranslation> NewsUnivsTranslations { get; set; }
    public DbSet<NewsTranslation> NewsTranslations { get; set; }
    public DbSet<Language> Languages { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
