using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject.T4.Htm
{
    public struct MyData {
        public List<MyDataItem> Items;
    }
    public class MyDataItem
    {
        public string Name;
        public string Value;
        public MyDataItem(string Name, string Value) {
            this.Name = Name;
            this.Value = Value;
        }
    }

    partial class MyWebPage1
    {
        private MyData m_data;
        public MyWebPage1(MyData data) { this.m_data = data; }
    }
}
