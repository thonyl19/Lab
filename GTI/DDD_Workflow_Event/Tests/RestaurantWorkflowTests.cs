using Microsoft.VisualStudio.TestTools.UnitTesting;
using Restaurant.Domain;
using Restaurant.Events;
using Restaurant.Bus;
using Restaurant.Workflow;
using System.Collections.Generic;
using System;

namespace Restaurant.Tests
{
    [TestClass]
    public class RestaurantWorkflowTests
    {
        /// <summary>
        /// 顧客點餐應觸發廚房 並 通知 waiter
        /// </summary>
        [TestMethod]
        public void CustomerOrder_Should_Trigger_Kitchen_And_Notify_Waiter()
        {
            var domainBus = new EventBus<DomainEvent>();
            var flowBus = new EventBus<FlowEvent>();

            var workflow = new KitchenWorkflow();
            bool waiterNotified = false;

            domainBus.Subscribe(evt =>
            {
                var orderEvt = evt as CustomerOrderedEvent;
                if (orderEvt != null)
                {
                    var flowEvt = workflow.Handle(orderEvt);
                    flowBus.Publish(flowEvt);
                }
            });

            flowBus.Subscribe(evt =>
            {
                var notify = evt as NotifyWaiterEvent;
                if (notify != null)
                    waiterNotified = true;
            });

            var order = new Order();
            var domainEvent = order.PlaceOrder(new MenuItem("Steak"));

            domainBus.Publish(domainEvent);

            Assert.IsTrue(waiterNotified);
        }

        [TestMethod]
        public void CustomerOrder_Should_Trigger_FlowEvent()
        {
            var bus = new InMemoryEventBus();
            var workflow = new ServingWorkflow(bus);

            bool dishServed = false;

            bus.Subscribe<DishReadyToServeEvent>(e =>
            {
                dishServed = true;
                Assert.AreEqual("牛肉麵", e.Order.Item.Name);
            });

            var menu = new MenuItem("牛肉麵");
            var order = new Order("ORD001", menu);
            var domainEvent = new CustomerOrderedEvent(order);

            workflow.Handle(domainEvent);

            Assert.IsTrue(dishServed);
        }



    }

    public class InMemoryEventBus : IEventBus
    {
        private readonly Dictionary<Type, List<Action<object>>> _handlers
            = new Dictionary<Type, List<Action<object>>>();

        public void Subscribe<T>(Action<T> handler)
        {
            var type = typeof(T);
            if (!_handlers.ContainsKey(type))
                _handlers[type] = new List<Action<object>>();

            _handlers[type].Add(e => handler((T)e));
        }

        public void Publish<T>(T @event)
        {
            var type = typeof(T);
            if (!_handlers.ContainsKey(type)) return;

            foreach (var handler in _handlers[type])
                handler(@event);
        }
    }

}