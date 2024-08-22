using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace UnitTestProject._Test
{
    public interface Iface
    {
        string A { get; set; }
    }

    public class 原生類 
    {
        public string A { get; set; }
    }

    public abstract class AbsX1<TEntity> where TEntity : Iface
    {
        public string Test(TEntity entity)
        {
            return entity.A;
        }
    }

    public abstract class AbsX<TEntity> where TEntity : Iface
    {
        private readonly TEntity _entity;

        protected AbsX(TEntity entity)
        {
            _entity = entity;
        }

        public string A
        {
            get => _entity.A;
            set => _entity.A = value;
        }

        public string Test()
        {
            return A;
        }
    }


    public class 衍生類 : AbsX<原生類>, Iface
{
        public 衍生類() : base(new 原生類())
        {
        }
    }


public class Program
{
    public static void Main()
    {
        var testA = new 衍生類 { A = "Hello World" };
        string result = testA.Test();
 
        Console.WriteLine(result); // Output: Hello World
    }
}
 }