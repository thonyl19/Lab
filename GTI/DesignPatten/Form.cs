using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;

namespace UnitTestProject.DesignPatterns    
{
    public interface IResult
    {
        Guid ID
        {
            get;
        }

        bool Success
        {
            get;
            set;
        }
 

        List<string> MessageList
        {
            get;
            set;
        }

        dynamic Data
        {
            get;
            set;
        }
 

        Exception Exception
        {
            get;
            set;
        }
 
        string Code { get; set; }

        void ThrowException();
        T parseData<T>();
        ExpandoObject DataTransExpandoObject(object obj = null);
    }
    public interface iForm {
        IResult Read();
        IResult Save();
        IResult Del();

    }
     
}
