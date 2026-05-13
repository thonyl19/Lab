using System;
using EventBusTemplate.Events;

namespace EventBusTemplate.Handlers
{
    public class LotReleasedHandler
    {
        public void Handle(IDomainEvent evt)
        {
            Console.WriteLine("Domain Event handled: " + evt.GetType().Name);
        }
    }
}