using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Photex.Database
{
    class PhotexDbContextContextDesignTimeFactory : IDesignTimeDbContextFactory<PhotexDbContext>
    {
        public PhotexDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<PhotexDbContext>();
            builder.UseSqlServer(@"Server=localhost;Trusted_Connection=True;database=PhotexDb");
            return new PhotexDbContext(builder.Options);
        }
    }
}
