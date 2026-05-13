using System.Data.Entity;
using System.Data.Common;
using md = YourApp.Entities;

namespace YourApp.Db
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("Name=AppDbContext")
        {
        }

        public AppDbContext(DbConnection connection) : base(connection, true)
        {
        }

        public virtual DbSet<md.User> Users { get; set; }
    }
}
