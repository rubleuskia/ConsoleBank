using System;
using System.Collections.Generic;

namespace Common
{
    public class EventBus : IEventBus
    {
        private readonly Dictionary<Type, List<object>> _subscribers = new ();

        public void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent
        {
            if (_subscribers.ContainsKey(typeof(TEvent)))
            {
                var handlers = _subscribers[typeof(TEvent)];
                handlers.Add(handler);
            }
            else
            {
                _subscribers.Add(typeof(TEvent), new List<object> { handler });
            }
        }

        public void Publish<TEvent>(TEvent @event) where TEvent : IEvent
        {
            if (_subscribers.ContainsKey(typeof(TEvent)))
            {
                var handlers = _subscribers[typeof(TEvent)];
                foreach (var handler in handlers)
                {
                    var casted = handler as Action<TEvent>;
                    casted?.Invoke(@event);
                }
            }
        }

        public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent
        {
            if (_subscribers.ContainsKey(typeof(TEvent)))
            {
                var handlers = _subscribers[typeof(TEvent)];
                handlers.RemoveAll(x => (Action<TEvent>) x == handler);
            }
        }
    }
}
