using RepositoryPatternTest.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryPatternTest.Application
{
    public interface IRepositoryWrapper
    {
        IProductRepository Product { get; }
        void Save();
    }
}
