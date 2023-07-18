using Microsoft.EntityFrameworkCore;

namespace TaskManagementApi.Data
{
    public class TaskManagementApiContext : DbContext
    {
        public TaskManagementApiContext(DbContextOptions<TaskManagementApiContext> options) : base(options) { }

        public DbSet<Models.Account> Account { get; set; }
        public DbSet<Models.JWTCliam> JWTCliam { get; set; }
        public DbSet<Models.JWTConfig> JWTConfig { get; set; }
    }
}
