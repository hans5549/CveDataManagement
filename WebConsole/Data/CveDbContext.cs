using Microsoft.EntityFrameworkCore;

namespace WebConsole.Data;

public class CveDbContext : DbContext
{
    private readonly IConfiguration _configuration;

    public CveDbContext(
        DbContextOptions<CveDbContext> options,
        IConfiguration configuration) : base(options)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = _configuration.GetConnectionString("SqlConnection")!;
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    }
}