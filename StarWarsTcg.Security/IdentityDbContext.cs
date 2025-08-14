using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace StarWarsTcg.Security
{
    public class IdentityDbContext : IdentityDbContext<User, Role, string>
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) { }

       public DbSet<User> Users { get; set; }
       public DbSet<Role> Roles { get; set; }
    }
}