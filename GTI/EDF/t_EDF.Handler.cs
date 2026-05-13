using EventBusTemplate.Events;

namespace EventBusTemplate.Tests.Handlers
{
    public class TestDomainEventHandler
    {
        public static int HandleCount = 0;

        public void Handle(IDomainEvent evt)
        {
            HandleCount++;
        }
    }

    public class TestFlowEventHandler
    {
        public static int HandleCount = 0;

        public void Handle(IFlowEvent evt)
        {
            HandleCount++;
        }
    }
}
