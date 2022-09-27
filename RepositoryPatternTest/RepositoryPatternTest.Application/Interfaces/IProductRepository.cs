using RepositoryPatternTest.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryPatternTest.Application.Interfaces
{
    public interface IProductRepository : IRepositoryBase<Product>
    {
    }
}
