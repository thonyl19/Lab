using EasyNetQ;
using Genesis.Gtimes.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMQ.Client;
using System;
using System.Threading.Tasks;
using UnitTestProject.TestUT;

namespace UnitTestProject
{
	[TestClass]
	public class t_EasynetQ : _testBase
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
 
		static string host = @"host=10.96.0.217;username=DTS;password=Manager1;VirtualHost=Test";
		static string host_x = @"host=10.96.0.217;username=DTS;password=Manager1;VirtualHost=Test";
		static string host_s = @"host=10.96.0.217;VirtualHost=Test";

		public class MyMessage
		{
			public string Text { get; set; }
		}

		IBus bus = RabbitHutch.CreateBus(host);

		[TestMethod]
		public void t_con() {
			ConnectionFactory factory = new ConnectionFactory()
			{
				UserName = "DTS",
				Password = "Manager1",
				HostName = "10.96.0.217",
				//Port = 15672,
			};
			IConnection conn = factory.CreateConnection();
		}

		/// <summary>
		///
		/// </summary>
		[TestMethod]
		public void t_Producer()
		{
			this.t_Producer4();

		}

        private void t_Producer4()
        {
			//bus.SendReceive.Send("Test",new MyMessage() { Text="test" });
			bus.SendReceive.SendAsync("Test",new MyMessage() { Text="test" }).ContinueWith(task => {
				if (task.IsCompleted)
				{
					//Console.Out.WriteLine("{0} Completed", count);
				}
				if (task.IsFaulted)
				{
					//Console.Out.WriteLine("\n\n");
					//Console.Out.WriteLine(task.Exception);
					//Console.Out.WriteLine("\n\n");
				}
			});

		}

        /// <summary>
        ///  https://changyuhao625.github.io/changyuhao625.github.io/tech/2019/07/17/rabbitmq-with-easynetq/
        /// </summary>
        void t_Producer1() {
			
			//監聽 CommandQueue
			/* 不 work 會出現以下錯誤 
			嚴重性	程式碼	說明	專案	檔案	行	隱藏項目狀態
			錯誤	CS1929	'IBus' 未包含 'Receive' 的定義，且最佳擴充方法多載 'SendReceiveExtensions.Receive<string>(ISendReceive, string, Action<string>, CancellationToken)' 需要類型 'ISendReceive' 的接收器	UT_SSMES	P:\UnitTestProject\t_EasynetQ.cs	43	作用中
			bus.Receive<string>("CommandQueue", (x) => { Console.WriteLine("Got the command!"); });
			 */
		}
		async void t_Producer2()
		{
			var bus = RabbitHutch.CreateBus(host);
			var input = "";
			var msg = new MyMessage() { 
				Text = "Got the command!"
			} ;
			await bus.PubSub.PublishAsync(msg).ContinueWith(task=> {
				if(task.IsCompleted){
					//Console.Out.WriteLine("{0} Completed", count);
				}
				if (task.IsFaulted)
				{
					//Console.Out.WriteLine("\n\n");
					//Console.Out.WriteLine(task.Exception);
					//Console.Out.WriteLine("\n\n");
				}
			});
		}

		/// <summary>
		/// https://github.com/EasyNetQ/EasyNetQ/wiki/Quick-Start
		/// </summary>
		void t_Producer3()
		{
			var bus = RabbitHutch.CreateBus(host_s);
			var input = "";
			var msg = new MyMessage()
			{
				Text = "Got the command!"
			};
			bus.PubSub.Publish(msg);
		}
		/// <summary>
		/// https://www.javaer101.com/en/article/17174327.html
		/// </summary>
		/// <returns></returns>
		ConnectionFactory x() {
			return new ConnectionFactory() {
				UserName = "DTS",
				Password = "Manager1",
				HostName = "10.96.0.217",
				Port = 15672,
				//VirtualHost = "/"
			};
 		}


		[TestMethod]
		public void t_Consumer()
		{
			this.t_Consumer4();

   		}

        private void t_Consumer4()
        {
			bus.PubSub.Subscribe<MyMessage>("Test", msg => { 
				Console.WriteLine(msg.Text); 
			});
		}

        private void t_Consumer3()
        {
			bus.SendReceive.Receive<MyMessage>("Test",(x)=> {
				var z = x;
			});


			//bus.SendReceive.ReceiveAsync("Test", cust => Task.Factory.StartNew(async () =>
			//{
			//	try
			//	{
			//		// await some function

			//		// code here won't get trigger
			//	}
			//	catch (Exception ex)
			//	{
			//		throw ex;
			//	}
			//}).ContinueWith(task => {
			//	if (task.IsCompleted)
			//	{
			//		//Console.Out.WriteLine("{0} Completed", count);
			//	}
			//	if (task.IsFaulted)
			//	{
			//		//Console.Out.WriteLine("\n\n");
			//		//Console.Out.WriteLine(task.Exception);
			//		//Console.Out.WriteLine("\n\n");
			//	}
			//});
		}

        private void t_Consumer1()
        {
			//var bus = RabbitHutch.CreateBus(host);
            var subscriptionResult = bus.PubSub.Subscribe<MyMessage>("Test", msg =>
            {
				var x = msg;
            });

		}
		private async void t_Consumer2()
		{
			var bus = RabbitHutch.CreateBus(host);

            await bus.PubSub.SubscribeAsync<MyMessage>("Test",
            message => Task.Factory.StartNew(() =>
            {
                // Perform some actions here
                // If there is a exception it will result in a task complete but task faulted which
                // is dealt with below in the continuation
            }).ContinueWith(task =>
            {
                if (task.IsCompleted && !task.IsFaulted)
                {
                    // Everything worked out ok
                }
                else
                {
                    // Don't catch this, it is caught further up the hierarchy and results in being sent to the default error queue
                    // on the broker
                    throw new EasyNetQException("Message processing exception - look in the default error queue (broker)");
                }
            }));

        }

		[TestMethod]
		public void t_() { }


	}


}
 