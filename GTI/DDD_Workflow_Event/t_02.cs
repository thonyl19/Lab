using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RestaurantArchitecture
{
    // ============================================================
    // 1. 溝通的藝術：訊息單 (Events)
    // ============================================================

    public interface IEvent { }

    // 【Domain Event】 顧客點菜單 - 描述業務事實 (過去式、不可逆)
    public class OrderPlacedEvent : IEvent
    {
        public Guid OrderId { get; }
        public List<(string DishName, string Category)> Items { get; }
        public OrderPlacedEvent(Guid id, List<(string, string)> items)
        {
            OrderId = id;
            Items = items;
        }
    }

    public class OrderCancelledEvent : IEvent
    {
        public Guid OrderId { get; }
        public DateTime CancelledTime { get; }
        public string Reason { get; }

        public OrderCancelledEvent(Guid id, string reason)
        {
            OrderId = id;
            CancelledTime = DateTime.Now;
            Reason = reason;
        }
    }

    // 【Flow Event】 廚房出菜鈴 - 描述流程控制 (通知性質、可重送)
    public class DishReadyFlowEvent : IEvent
    {
        public Guid OrderId { get; set; }
        public string Category { get; set; } // "Appetizer", "MainCourse", "Dessert"
    }

    // ============================================================
    // 2. 菜單與廚房聖經 (Domain-Driven Design)
    // ============================================================

    public class Order
    {
        public Guid Id { get; }
        public List<(string DishName, string Category)> Items { get; }
        public bool IsPaid { get; private set; }

        public bool IsCancelled { get; private set; }
        public bool IsCooked { get; private set; } // 假設這是由 Workflow 轉回來的狀態

        public Order(Guid id)=>Id = id;

        public Order(Guid id, List<(string, string)> items)
        {
            Id = id;
            Items = items;
        }

        public void Pay() => IsPaid = true; // 廚房聖經：付錢是業務事實的終點

        public void MarkAsCooked() => IsCooked = true;

        // 【領域邏輯】：並非所有狀態都能取消
        // 使用 C# 7 元組 (ValueTuples) 回傳多個結果
        public (bool Success, string Reason) CanCancel()
        {
            if (IsCancelled) return (false, "訂單早已取消。");
            if (IsCooked) return (false, "大廚已經煮好了，不能取消！");

            return (true, "核准取消。");
        }

        public void ApplyCancellation() => IsCancelled = true;
    }

     

    // ============================================================
    // 3. 外場服務流程 (Workflow - State Pattern)
    // ============================================================

    public interface IServingState
    {
        string StepName { get; }
        void HandleBell(ServiceWorkflow context, string category);
    }

    public class ServiceWorkflow
    {
        public IServingState CurrentStep { get; set; }
        public Guid OrderId { get; }

        public ServiceWorkflow(Guid orderId)
        {
            OrderId = orderId;
            CurrentStep = new AppetizerStage(); // SOP 啟始：上沙拉
        }
    }

    public class AppetizerStage : IServingState
    {
        public string StepName => "上沙拉階段";
        public void HandleBell(ServiceWorkflow context, string category)
        {
            if (category == "Appetizer") context.CurrentStep = new MainCourseStage();
        }
    }

    public class MainCourseStage : IServingState
    {
        public string StepName => "上主餐階段";
        public void HandleBell(ServiceWorkflow context, string category)
        {
            if (category == "MainCourse") context.CurrentStep = new DessertStage();
        }
    }

    public class DessertStage : IServingState
    {
        public string StepName => "上甜點階段";
        public void HandleBell(ServiceWorkflow context, string category)
        {
            if (category == "Dessert") context.CurrentStep = new FinishedStage();
        }
    }

    public class FinishedStage : IServingState
    {
        public string StepName => "流程結束";
        public void HandleBell(ServiceWorkflow context, string cat) { /* 結案 */ }
    }

    // ============================================================
    // 4. 基礎設施：事件發布者 (Simple Event Bus)
    // ============================================================

    public class SimpleEventBus
    {
        private readonly List<Action<IEvent>> _handlers = new List<Action<IEvent>>();
        public void Subscribe(Action<IEvent> handler) => _handlers.Add(handler);
        public void Publish(IEvent @event) => _handlers.ForEach(h => h(@event));
    }

    // ============================================================
    // 5. 單元測試 (Unit Tests)
    // ============================================================
 
}