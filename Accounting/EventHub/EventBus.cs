using System;
using System.Collections.Generic;

namespace Accounting.EventHub
{
    public class EventBus : IEventBus
    {
        private Dictionary<Type, List<object>> _subsciptions = new();

        public void Subscribe<TEvent>(Action<TEvent> eventHandler)
            where TEvent : IEvent
        {
            if (_subsciptions.ContainsKey(typeof(TEvent)))
            {
                var handlers = _subsciptions[typeof(TEvent)];
                handlers.Add(eventHandler);
            }
            else
            {
                _subsciptions[typeof(TEvent)] = new List<object> { eventHandler };
            }
        }

        public void Unsubscribe<TEvent>(Action<TEvent> eventHandler)
            where TEvent : IEvent
        {
            if (_subsciptions.ContainsKey(typeof(TEvent)))
            {
                var handlers = _subsciptions[typeof(TEvent)];
                handlers.RemoveAll(x => x as Action<TEvent> == eventHandler);
            }
        }

        public void Publish<TEvent>(TEvent @event)
            where TEvent : IEvent
        {
            if (_subsciptions.TryGetValue(typeof(TEvent), out var handlers))
            {
                foreach (var handler in handlers)
                {
                    ((Action<TEvent>) handler)(@event);
                }
            }
        }
    }

    public interface IEventBus
    {
        void Subscribe<TEvent>(Action<TEvent> eventHandler)
            where TEvent : IEvent;

        void Publish<TEvent>(TEvent @event)
            where TEvent : IEvent;

        void Unsubscribe<TEvent>(Action<TEvent> eventHandler)
            where TEvent : IEvent;
    }
}
