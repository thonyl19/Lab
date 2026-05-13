using System;

namespace Restaurant.Events
{
    public abstract class DomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.Now;
    }
}