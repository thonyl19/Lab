using RepositoryPatternTest.Application.Interfaces;
using RepositoryPatternTest.Domain.Entities;
using RepositoryPatternTest.Infrastructure.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryPatternTest.Infrastructure.Service.Repositories
{
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }
    }
}
