using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;

namespace UnitTestProject
{
	[TestClass]
	public class t_Attribute
	{
		[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
		public class DeveloperAttribute : Attribute
		{
			// Private fields.
			private string name;
			private string level;
			private bool reviewed;

			// This constructor defines two required parameters: name and level.

			public DeveloperAttribute(string name, string level = "A", bool reviewed = false)
			{
				this.name = name;
				this.level = level;
				this.reviewed = reviewed;
			}

			// Define Name property.
			// This is a read-only attribute.

			public virtual string Name
			{
				get { return name; }
			}

			// Define Level property.
			// This is a read-only attribute.

			public virtual string Level
			{
				get { return level; }
			}

			// Define Reviewed property.
			// This is a read/write attribute.

			public virtual bool Reviewed
			{
				get { return reviewed; }
				set { reviewed = value; }
			}
		}

		[Developer("Joan", "1")]
		[Developer("Smith")]
		[TestMethod]
		public void t_擷取單一屬性執行個體()
		{
			DeveloperAttribute att;
			MemberInfo[] MyMemberInfo = this.GetType().GetMethods();
			for (int i = 0; i < MyMemberInfo.Length; i++)
			{
				att = (DeveloperAttribute)Attribute.GetCustomAttribute(MyMemberInfo[i], typeof(DeveloperAttribute));
				if (att == null)
				{
					Console.WriteLine("No attribute in member function {0}.\n", MyMemberInfo[i].ToString());
				}
				else
				{
					var x = new string[] { att.Name, att.Level, att.Reviewed.ToString() };

				}
			}
		}



	}
}
