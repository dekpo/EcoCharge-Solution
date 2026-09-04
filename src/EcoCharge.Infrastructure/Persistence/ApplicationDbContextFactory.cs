using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EcoCharge.Infrastructure.Persistence;

/// <summary>
/// Design-time factory used by <c>dotnet ef</c> so migrations can be generated
/// without spinning up the full ASP.NET host. Targets SQLite, the default
/// provider for local development and the documented VPS path.
/// </summary>
public sealed class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlite("Data Source=ecocharge.design.db");
        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
