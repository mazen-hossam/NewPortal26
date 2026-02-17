using Microsoft.EntityFrameworkCore;
using TheBoys.API.Entities;

namespace TheBoys.API.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<News> News { get; set; }
    public DbSet<News_univ> News_Univs { get; set; }
    public DbSet<News_univ_trans> News_Univs_trans { get; set; }
    public DbSet<NewsTranslation> NewsTranslations { get; set; }
    public DbSet<Language> Languages { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //  modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
