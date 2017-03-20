using System;
using System.Collections.Generic;

namespace Common.DDD
{
    public class EventDispatcher<TEvent>
        where TEvent : class, IAggregateRootEvent
    {
        private readonly List<KeyValuePair<Type, Action<object>>> _handlers = new List<KeyValuePair<Type, Action<object>>>();
        
        public RegistrationBuilder RegisterAppliers()
        {
            return new RegistrationBuilder(this);
        }
        
        public class RegistrationBuilder : IEventApplierRegistrar<TEvent>
        {
            private readonly EventDispatcher<TEvent> _dispather;

            public RegistrationBuilder(EventDispatcher<TEvent> dispather)
            {
                _dispather = dispather;
            }
            
            public RegistrationBuilder For<THandledEvent>(Action<THandledEvent> handler) where THandledEvent : TEvent
            {
                return ForGenericEvent(handler);
            }
            
            private RegistrationBuilder ForGenericEvent<THandledEvent>(Action<THandledEvent> handler)
            {
                var eventType = typeof(THandledEvent);

                _dispather._handlers.Add(new KeyValuePair<Type, Action<object>>(eventType, e => handler((THandledEvent)e)));
                return this;
            }
            
            IEventApplierRegistrar<TEvent> IEventApplierRegistrar<TEvent>.For<THandledEvent>(Action<THandledEvent> handler)
            {
                return For(handler);
            }
        }
        
        private Action<object>[] GetAppliers(Type type)
        {
            var result = new List<Action<object>>();
            foreach(KeyValuePair<Type, Action<object>> t in _handlers) {
                if (t.Key.IsAssignableFrom(type))
                {
                    result.Add(t.Value);
                }
            }
            
            return result.ToArray();
        }

        public void Dispatch(TEvent evt)
        {
            var handlers = GetAppliers(evt.GetType());

            if(handlers.Length == 0)
            {
                throw new AggregateRootException($"No handler found for event {evt.GetType()}.");
            }

            for (var i = 0; i < handlers.Length; i++)
            {
                handlers[i](evt);
            }
        }
    }
    
    public interface IEventApplierRegistrar<in TBaseEvent>
        where TBaseEvent : class
    {
        IEventApplierRegistrar<TBaseEvent> For<THandledEvent>(Action<THandledEvent> handler) where THandledEvent : TBaseEvent;
    }
}
