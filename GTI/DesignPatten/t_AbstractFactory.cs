using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using UnitTestProject.TestUT;

namespace UnitTestProject
{
	/// <summary>
	/// https://refactoringguru.cn/design-patterns/chain-of-responsibility
	/// https://juejin.cn/post/7124337266645794823
	/// </summary>
	[TestClass]
	public class t_AbstractFactory : _testBase
	{
		static class _log
		{
			/// <summary>
			/// splitBIN 前端傳入的資料範例 
			/// </summary>
			internal static string t_splitBIN
			{
				get
				{
					return FileApp.ts_Log(@"WIP\t_splitBIN.json");
				}
			}
		}

		/// <summary>
		/// https://refactoring.guru/design-patterns/abstract-factory
		/// </summary>
		public interface Charir {
			void hasLegs();
			void sitOn();
		}

        public class VictorianChair : Charir
        {
            public void hasLegs()
            {
                throw new System.NotImplementedException();
            }

            public void sitOn()
            {
                throw new System.NotImplementedException();
            }
        }

		public class ModernChair : Charir
		{
			public void hasLegs()
			{
				throw new System.NotImplementedException();
			}

			public void sitOn()
			{
				throw new System.NotImplementedException();
			}
		}

		[TestMethod]
		public void _()
		{
			new DesignPatterns.AbstractFactory_Case1.Client().Main();
		}

	}


}
