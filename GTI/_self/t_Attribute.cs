using Genesis.Library.BLL.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace UnitTestProject
{

	public static class StatusExtensions
	{
		public static string AttrShortName(this Enum _enum)
		{
			var memberInfo = _enum.GetType().GetMember(_enum.ToString());
			var displayAttribute = (DisplayAttribute)Attribute.GetCustomAttribute(memberInfo[0], typeof(DisplayAttribute));
			return displayAttribute?.ShortName ?? _enum.ToString();
		}
	}


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




		[TestMethod]
		public void t_TTTT()
		{
			//var z = BulkHelper.GetProperty(Genesis.Library.BLL.ICM.Definition.Status.Verifying.GetType(), "ShortName");

			//var status = Genesis.Library.BLL.ICM.Definition.Status.Verifying;
			//var memberInfo = status.GetType().GetMember(status.ToString());
			//var displayAttribute = (DisplayAttribute)Attribute.GetCustomAttribute(memberInfo[0], typeof(DisplayAttribute));

			//var z = displayAttribute?.ShortName ?? status.ToString();


			//var z1 = Genesis.Library.BLL.ICM.Definition.Status.Used.AttrShortName();

		}

		

	}
}
