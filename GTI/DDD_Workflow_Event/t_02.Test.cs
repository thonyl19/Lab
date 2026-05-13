using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RestaurantArchitecture
{
    // ============================================================
    // 5. 單元測試 (Unit Tests)
    // ============================================================

    [TestClass]
    public class RestaurantManagementTests
    {
        private SimpleEventBus _eventBus;
        private Dictionary<Guid, Order> _orderRepository;
        private Dictionary<Guid, ServiceWorkflow> _workflowEngine;

        [TestInitialize]
        public void Setup()
        {
            _eventBus = new SimpleEventBus();
            _orderRepository = new Dictionary<Guid, Order>();
            _workflowEngine = new Dictionary<Guid, ServiceWorkflow>();

            // 訂閱事件：當點菜單成立 (Domain Event)
            _eventBus.Subscribe(e =>
            {
                // C# 7 Pattern Matching
                if (e is OrderPlacedEvent placed)
                {
                    // 1. 建立 Domain 事實
                    _orderRepository[placed.OrderId] = new Order(placed.OrderId, placed.Items);
                    // 2. 初始化 Workflow SOP
                    _workflowEngine[placed.OrderId] = new ServiceWorkflow(placed.OrderId);
                }
            });

            // 訂閱事件：當廚房鈴響 (Flow Event)
            _eventBus.Subscribe(e =>
            {
                if (e is DishReadyFlowEvent bell && _workflowEngine.TryGetValue(bell.OrderId, out var flow))
                {
                    // 驅動 Workflow 演進
                    flow.CurrentStep.HandleBell(flow, bell.Category);
                }
            });
        }

        [TestMethod]
        public void Workflow_ShouldFollowSequence_WhenBellsRing()
        {
            // Arrange: 顧客點餐 (Domain Event)
            var orderId = Guid.NewGuid();
            var dishes = new List<(string, string)> { ("沙拉", "Appetizer"), ("牛排", "MainCourse") };

            // Act 1: 送出點菜單
            _eventBus.Publish(new OrderPlacedEvent(orderId, dishes));

            // Assert: 目前應該在沙拉階段
            Assert.AreEqual("上沙拉階段", _workflowEngine[orderId].CurrentStep.StepName);

            // Act 2: 廚房響起「前菜好了」的鈴聲 (Flow Event)
            _eventBus.Publish(new DishReadyFlowEvent { OrderId = orderId, Category = "Appetizer" });

            // Assert: Workflow 應自動演進到主餐階段
            Assert.AreEqual("上主餐階段", _workflowEngine[orderId].CurrentStep.StepName);
        }

        [TestMethod]
        public void Workflow_ShouldIgnoreWrongBell_WhenSequenceIsInvalid()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            _eventBus.Publish(new OrderPlacedEvent(orderId, new List<(string, string)>()));

            // Act: 廚房在沙拉還沒好時，先按了「主餐好了」的鈴 (順序錯誤)
            _eventBus.Publish(new DishReadyFlowEvent { OrderId = orderId, Category = "MainCourse" });

            // Assert: 流程手冊應發揮作用，狀態不應改變 (仍停留在沙拉階段)
            Assert.AreEqual("上沙拉階段", _workflowEngine[orderId].CurrentStep.StepName);
        }

        [TestMethod]
        public void Domain_Facts_ShouldStayStable_RegardlessOfWorkflow()
        {
            // 驗證「職責分離」：Workflow 的變化不應影響 Domain 的基礎事實
            var orderId = Guid.NewGuid();
            _eventBus.Publish(new OrderPlacedEvent(orderId, new List<(string, string)> { ("沙拉", "Appetizer") }));

            // 就算 Workflow 結束了
            _eventBus.Publish(new DishReadyFlowEvent { OrderId = orderId, Category = "Appetizer" });
            _eventBus.Publish(new DishReadyFlowEvent { OrderId = orderId, Category = "MainCourse" });
            _eventBus.Publish(new DishReadyFlowEvent { OrderId = orderId, Category = "Dessert" });

            // Assert: Domain 中的訂單依然存在且內容不變
            Assert.IsTrue(_orderRepository.ContainsKey(orderId));
            Assert.IsFalse(_orderRepository[orderId].IsPaid); // 除非明確呼叫 Pay()
            _orderRepository[orderId].Pay();

            Assert.IsTrue(_orderRepository[orderId].IsPaid); // 除非明確呼叫 Pay()
        }

        [TestMethod]
        public void Test_Cancellation_Logic_Divergence()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order(orderId);
            var bus = new SimpleEventBus();

            // 模擬 Workflow 進行中：牛排已經煎好了
            order.MarkAsCooked();

            // Act: 客人嘗試取消
            // 使用 C# 7 變數解構 (Deconstruction)
            var (isSuccess, message) = order.CanCancel();

            if (isSuccess){
                order.ApplyCancellation();
                bus.Publish(new OrderCancelledEvent(orderId, "客人反悔了"));
            }

            // Assert
            Assert.IsFalse(isSuccess, "牛排煮好了應該不能取消");
            Assert.AreEqual("大廚已經煮好了，不能取消！", message);
        }
    }
}