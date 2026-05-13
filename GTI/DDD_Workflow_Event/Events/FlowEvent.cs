using System;

namespace Restaurant.Events
{
    public abstract class FlowEvent
    {
        public Guid FlowId { get; } = Guid.NewGuid();
    }
}