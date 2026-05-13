using System;
using EventBusTemplate.Events;

namespace EventBusTemplate.Handlers
{
    public class LotReleasedDomainEvent : IDomainEvent
    {
        public string LotNo { get; set; }
        public DateTime OccurredOn { get; private set; } = DateTime.Now;
    }
}