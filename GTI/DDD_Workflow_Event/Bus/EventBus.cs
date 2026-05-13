using System;
using System.Collections.Generic;

namespace Restaurant.Bus
{
    public class EventBus<T>
    {
        private readonly List<Action<T>> _handlers = new List<Action<T>>();

        public void Subscribe(Action<T> handler)
        {
            _handlers.Add(handler);
        }

        public void Publish(T evt)
        {
            foreach (var h in _handlers)
                h(evt);
        }
    }
}