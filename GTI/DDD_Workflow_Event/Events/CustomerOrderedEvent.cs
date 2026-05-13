namespace Restaurant.Events
{
    public class CustomerOrderedEvent : DomainEvent
    {
        public string MenuName { get; }
        public CustomerOrderedEvent(string menuName)
        {
            MenuName = menuName;
        }
    }
}