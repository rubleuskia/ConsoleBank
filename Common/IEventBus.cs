using System;

namespace Common
{
    // TODO add async support
    public interface IEventBus
    {
        void Subscribe<TEvent>(Action<TEvent> handler)
            where TEvent : IEvent;

        void Publish<TEvent>(TEvent @event)
            where TEvent : IEvent;

        void Unsubscribe<TEvent>(Action<TEvent> handler)
            where TEvent : IEvent;
    }
}
