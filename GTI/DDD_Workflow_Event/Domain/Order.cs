using System.Collections.Generic;
using Restaurant.Events;

namespace Restaurant.Domain
{
    public class Order
    {
        public List<MenuItem> Items { get; } = new List<MenuItem>();

        public DomainEvent PlaceOrder(MenuItem item)
        {
            Items.Add(item);
            return new CustomerOrderedEvent(item.Name);
        }
    }
}