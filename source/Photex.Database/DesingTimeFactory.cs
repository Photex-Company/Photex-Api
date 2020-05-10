using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Photex.Database
{
    class PhotexDbContextContextDesignTimeFactory : IDesignTimeDbContextFactory<PhotexDbContext>
    {
        public PhotexDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<PhotexDbContext>();
            builder.UseSqlServer(@"Server=tcp:photex-db-server.database.windows.net,1433;Initial Catalog=PhotexDb;Persist Security Info=False;User ID=jbarczyk;Password=Kuba1997;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            return new PhotexDbContext(builder.Options);
        }
    }
}
