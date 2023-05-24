using Microsoft.EntityFrameworkCore;
using SixMinApi.models;

namespace SixMinApi.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt)
        {
            
        }
        public DbSet<Command> Commands => Set<Command>();
    }
}