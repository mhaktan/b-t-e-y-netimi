using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ButceYonetimi.EntityFrameworkCore
{
    public class ButceYonetimiDbContextFactory : IDesignTimeDbContextFactory<ButceYonetimiDbContext>
    {
        public ButceYonetimiDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../ButceYonetimi.Web.Host"))
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var connStr = configuration.GetConnectionString("Default");
            var builder = new DbContextOptionsBuilder();
            builder.UseNpgsql(connStr);
            return new ButceYonetimiDbContext(builder.Options);
        }
    }
}
