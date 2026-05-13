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
	public class t_ChainofCommand : _testBase
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
 

        

        [TestMethod]
		public void t_() {

            var groupLeaderHandler = new GroupLeaderHandler();
            var departmentLeaderHandler = new DepartmentLeaderHandler();
            var hrHandler = new HRHandler();
            groupLeaderHandler.setNext(departmentLeaderHandler);
            departmentLeaderHandler.setNext(hrHandler);

            Trace.WriteLine("收到面试通知，需要请假");
            string request = "家中有事，请假半天，望批准";
            Trace.WriteLine("发起请求：");
            groupLeaderHandler.handle(request);
        }


	}


}
