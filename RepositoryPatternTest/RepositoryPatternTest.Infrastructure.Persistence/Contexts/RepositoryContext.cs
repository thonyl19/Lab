using Microsoft.EntityFrameworkCore;
using RepositoryPatternTest.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryPatternTest.Infrastructure.Persistence.Contexts
{
    public class RepositoryContext : DbContext
    {
        public RepositoryContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
    }
}
