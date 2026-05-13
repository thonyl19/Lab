using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RestaurantDDD.Core
{
    #region 1. Domain & Events (領域與事件)

    // 領域事件介面
    public interface IEvent { }

    // Domain Event: 顧客點餐
    public class OrderPlacedEvent : IEvent
    {
        public Guid OrderId { get; set; }
        public List<string> Items { get; set; }
    }

    // Flow Event: 廚房通知上菜 (某一階段完成)
    public class DishReadyEvent : IEvent
    {
        public Guid OrderId { get; set; }
        //菜餚類別
        public string DishCategory { get; set; } // e.g., "Appetizer"
    }

    // 簡單的記憶體內事件發布者 (C# 7 Pattern Matching 應用)
    public class SimpleEventBus
    {
        private readonly List<Action<IEvent>> _subscribers = new List<Action<IEvent>>();

        public void Subscribe(Action<IEvent> handler) => _subscribers.Add(handler);

        public void Publish(IEvent @event)
        {
            foreach (var sub in _subscribers) sub(@event);
        }
    }

    // 菜單領域模型
    public class Menu
    {
        // 使用 C# 7 Tuples 定義菜單內容
        public static (string Name, string Category) GetItem(string itemName)
        {
            switch (itemName)
            {
                case "Salad": return ("Salad", "Appetizer");
                case "Steak": return ("Steak", "MainCourse");
                case "Cake": return ("Cake", "Dessert");
                default: throw new ArgumentException("Unknown dish");
            }
        }
    }
    #endregion

    #region 2. Workflow - State Pattern (工作流 - 狀態模式)

    // 狀態介面
    public interface IOrderState
    {
        string StateName { get; }
        void HandleReady(Order context, string category);
    }

    // 前菜階段
    public class AppetizerState : IOrderState
    {
        public string StateName => "Serving Appetizer";
        public void HandleReady(Order context, string category)
        {
            if (category == "Appetizer") context.TransitionTo(new MainCourseState());
        }
    }

    // 主菜階段
    public class MainCourseState : IOrderState
    {
        public string StateName => "Serving Main Course";
        public void HandleReady(Order context, string category)
        {
            if (category == "MainCourse") context.TransitionTo(new DessertState());
        }
    }

    // 甜點階段
    public class DessertState : IOrderState
    {
        public string StateName => "Serving Dessert";
        public void HandleReady(Order context, string category)
        {
            if (category == "Dessert") context.TransitionTo(new CompletedState());
        }
    }

    public class CompletedState : IOrderState
    {
        public string StateName => "Completed";
        public void HandleReady(Order context, string category) { /* 已結束 */ }
    }
    #endregion

    #region 3. Aggregate Root (聚合根)

    public class Order
    {
        public Guid Id { get; }
        public IOrderState CurrentState { get; private set; }
        public List<string> Items { get; }

        public Order(Guid id, List<string> items)
        {
            Id = id;
            Items = items;
            CurrentState = new AppetizerState(); // 初始流程：從前菜開始
        }

        public void TransitionTo(IOrderState state) => CurrentState = state;

        public void ProcessDishReady(string category)
        {
            // 執行狀態轉換邏輯
            CurrentState.HandleReady(this, category);
        }
    }
    #endregion

    #region 4. Unit Tests (單元測試)

    [TestClass]
    public class RestaurantTests
    {
        private SimpleEventBus _bus;
        private Order _order;

        [TestInitialize]
        public void Setup()
        {
            _bus = new SimpleEventBus();
        }

        /// <summary>
        /// 確保 工作流程狀態轉換應遵循順序
        /// </summary>
        [TestMethod]
        public void Workflow_StateTransition_ShouldFollowSequence()
        {
            // Arrange
            var order = new Order(Guid.NewGuid()
                , new List<string> { "Salad", "Steak", "Cake" });

            // Act & Assert 1: 初始應為前菜
            Assert.AreEqual("Serving Appetizer", order.CurrentState.StateName);

            // Act 2: 前菜好了
            order.ProcessDishReady("Appetizer");
            Assert.AreEqual("Serving Main Course", order.CurrentState.StateName);

            // Act 3: 主菜好了
            order.ProcessDishReady("MainCourse");
            Assert.AreEqual("Serving Dessert", order.CurrentState.StateName);

            // Act 4: 甜點好了
            order.ProcessDishReady("Dessert");
            Assert.AreEqual("Completed", order.CurrentState.StateName);
        }

        [TestMethod]
        public void EventBased_OrderPlaced_ShouldTriggerWorkflow()
        {
            // 這個測試模擬 Event-Based 的聯鎖反應
            // Arrange
            Order capturedOrder = null;

            // 訂閱事件：當訂單成立時，建立領域模型 (這通常是 Application Service 的工作)
            _bus.Subscribe(e =>
            {
                // C# 7 Pattern Matching (Type Pattern)
                if (e is OrderPlacedEvent placed)
                {
                    capturedOrder = new Order(placed.OrderId, placed.Items);
                }
            });

            // 訂閱事件：當廚房通知上菜時，驅動工作流
            _bus.Subscribe(e =>
            {
                if (e is DishReadyEvent ready && capturedOrder != null && ready.OrderId == capturedOrder.Id)
                {
                    capturedOrder.ProcessDishReady(ready.DishCategory);
                }
            });

            // Act
            var orderId = Guid.NewGuid();
            _bus.Publish(new OrderPlacedEvent { OrderId = orderId, Items = new List<string> { "Steak" } });

            // 模擬廚房發出 Flow Event
            _bus.Publish(new DishReadyEvent { OrderId = orderId, DishCategory = "Appetizer" });

            // Assert
            Assert.IsNotNull(capturedOrder);
            // 驗證因為收到了 Appetizer Ready 事件，工作流已自動推動到 Main Course
            Assert.AreEqual("Serving Main Course", capturedOrder.CurrentState.StateName);
        }
    }
    #endregion
}