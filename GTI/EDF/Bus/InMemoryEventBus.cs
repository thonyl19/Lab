using System;
using System.Collections.Generic;

namespace EventBusTemplate.Bus
{
    public class InMemoryEventBus<TEvent> : IEventBus<TEvent>
    {
        private readonly List<Action<TEvent>> _handlers = new List<Action<TEvent>>();

        public void Publish(TEvent @event)
        {
            foreach (var handler in _handlers)
            {
                handler(@event);
            }
        }

        public void Subscribe<THandler>() where THandler : class
        {
            var handler = Activator.CreateInstance<THandler>();
            var method = typeof(THandler).GetMethod("Handle");
            _handlers.Add(e => method.Invoke(handler, new object[] { e }));
        }
    }
}