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
 

        /// <summary>
        /// https://refactoringguru.cn/design-patterns/chain-of-responsibility
        /// https://juejin.cn/post/7124337266645794823
        /// </summary>
        public abstract class DayOffHandler
        {
            public DayOffHandler next { get; private set; }
            public DayOffHandler getNext()
            {
                return next;
            }
            public void setNext(DayOffHandler next)
            {
                this.next = next;
            }
            public abstract void handle(string request);

        }
 
        // 直属 leader 处理
        public class GroupLeaderHandler : DayOffHandler
        {

            public override void handle(string request)
            {
                Trace.WriteLine("直属 leader 审查: " + request);
                Trace.WriteLine("同意请求");
                next?.handle(request);
            }
        }
        // 部门 leader 处理
        public class DepartmentLeaderHandler : DayOffHandler
        {
            public override void handle(string request)
            {
                Trace.WriteLine("部门 leader 审查: " + request);
                Trace.WriteLine("同意请求");
                next?.handle(request);
            }
        }
        // 人事处处理
        public class HRHandler : DayOffHandler
        {
            public override void handle(string request)
            {
                Trace.WriteLine("人事处审查: " + request);
                Trace.WriteLine("同意请求，记录请假");
                next?.handle(request);
            }
        }

        [TestMethod]
		public void _ChainofCommand() {

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

        [TestMethod]
        public void _ChainofCommand模式混合工廠模式()
        {

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
