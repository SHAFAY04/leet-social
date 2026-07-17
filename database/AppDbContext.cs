

using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{

    public AppDbContext(
        DbContextOptions<AppDbContext> options
    ) : base(options) { }

    public DbSet<UserEntity> Users { get; set; }
    public DbSet<AuthEntity> Auth { get; set; }
    public DbSet<MediaEntity> Media { get; set; }
}