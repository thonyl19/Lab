using System;
using NUnit.Framework;
using EventBusTemplate.Bus;
using EventBusTemplate.Events;
using EventBusTemplate.Handlers;
using EventBusTemplate.Tests.Handlers;

namespace UnitTestProject
{



	[TestFixture]
	public class t_EDF
	{
		[SetUp]
		public void Setup()
		{
			TestDomainEventHandler.HandleCount = 0;
			TestFlowEventHandler.HandleCount = 0;
		}


        [Test]
        public void DomainEvent_Should_Invoke_Handler()
        {
            // Arrange
            var domainBus = new InMemoryEventBus<IDomainEvent>();
            domainBus.Subscribe<TestDomainEventHandler>();

            var evt = new LotReleasedDomainEvent { LotNo = "LOT-001" };

            // Act
            domainBus.Publish(evt);

            // Assert
            Assert.AreEqual(1, TestDomainEventHandler.HandleCount,
                "DomainEvent handler should be invoked exactly once.");
        }

        [Test]
        public void FlowEvent_Should_Invoke_Handler()
        {
            // Arrange
            var flowBus = new InMemoryEventBus<IFlowEvent>();
            flowBus.Subscribe<TestFlowEventHandler>();

            var evt = new TestFlowEvent(Guid.NewGuid());

            // Act
            flowBus.Publish(evt);

            // Assert
            Assert.AreEqual(1, TestFlowEventHandler.HandleCount,
                "FlowEvent handler should be invoked exactly once.");
        }

        [Test]
        public void DomainAndFlowEvent_Should_Be_Isolated()
        {
            // Arrange
            var domainBus = new InMemoryEventBus<IDomainEvent>();
            var flowBus = new InMemoryEventBus<IFlowEvent>();

            domainBus.Subscribe<TestDomainEventHandler>();
            flowBus.Subscribe<TestFlowEventHandler>();

            var domainEvt = new LotReleasedDomainEvent { LotNo = "LOT-002" };
            var flowEvt = new TestFlowEvent(Guid.NewGuid());

            // Act
            domainBus.Publish(domainEvt);
            flowBus.Publish(flowEvt);

            // Assert
            Assert.AreEqual(1, TestDomainEventHandler.HandleCount,
                "DomainEvent handler should be invoked exactly once.");
            Assert.AreEqual(1, TestFlowEventHandler.HandleCount,
                "FlowEvent handler should be invoked exactly once.");
        }

        [Test]
        public void MultipleHandlers_Should_All_Be_Called()
        {
            // Arrange
            var domainBus = new InMemoryEventBus<IDomainEvent>();
            domainBus.Subscribe<TestDomainEventHandler>();
            domainBus.Subscribe<TestDomainEventHandler>(); // 訂閱兩次測試

            var evt = new LotReleasedDomainEvent { LotNo = "LOT-003" };

            // Act
            domainBus.Publish(evt);

            // Assert
            Assert.AreEqual(2, TestDomainEventHandler.HandleCount,
                "Both handlers should be invoked.");
        }
    }
}
