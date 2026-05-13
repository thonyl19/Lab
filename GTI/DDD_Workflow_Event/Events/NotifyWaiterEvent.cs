namespace Restaurant.Events
{
    public class NotifyWaiterEvent : FlowEvent
    {
        public string MenuName { get; }
        public NotifyWaiterEvent(string menuName)
        {
            MenuName = menuName;
        }
    }
}