using Microsoft.EntityFrameworkCore;

namespace IntroSite;

public enum Status { Planned, Executed, Passed, Failed }

public class  TestPoint
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public Status Status { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
}

public class  AppDb(DbContextOptions<AppDb> options) : DbContext(options)
{
    public DbSet<TestPoint> TestPoints => Set<TestPoint>();    
}