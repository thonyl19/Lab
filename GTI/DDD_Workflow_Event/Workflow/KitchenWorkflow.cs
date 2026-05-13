using Restaurant.Events;

namespace Restaurant.Workflow
{
    public class KitchenWorkflow
    {
        public NotifyWaiterEvent Handle(CustomerOrderedEvent evt)
        {
            return new NotifyWaiterEvent(evt.MenuName);
        }
    }
}