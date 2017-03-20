using System;

namespace Common.DDD
{
    public abstract partial class AggregateRoot<TAggregateRootEventInterface> : IAggregateRoot
        where TAggregateRootEventInterface : class, IAggregateRootEvent
    {
        protected IEventController EventController { get; private set; }

        public void Initialize(IEventController eventController, IDomainEvent[] eventStream)
        {
            EventController = eventController;

            if (eventStream == null) return;

            foreach (var domainEvent in eventStream)
            {
                ApplyEvent(domainEvent as TAggregateRootEventInterface);
            }
        }

        public Guid AggregateRootId { get; private set; }
        private void SetId(Guid id) { AggregateRootId = id; }

        protected void RaiseEvent<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : TAggregateRootEventInterface
        {
            if(domainEvent is IAggregateRootCreatedEvent)
            {
                if(AggregateRootId != Guid.Empty)
                {
                    throw new AggregateRootException(
                        $"The {nameof(AggregateRootId)} can only be set once. " +
                        $"Only the first event should implement {typeof(IAggregateRootCreatedEvent)}.");
                }

                if(domainEvent.AggregateRootId == Guid.Empty)
                {
                    throw new AggregateRootException($"Missing {nameof(domainEvent.AggregateRootId)}");
                }
            }
            else
            {
                if(AggregateRootId == Guid.Empty)
                {
                    throw new AggregateRootException(
                        $"No {nameof(AggregateRootId)} set. " +
                        $"Did you forget to implement {typeof(IAggregateRootCreatedEvent)} in the first event?");
                }

                if(domainEvent.AggregateRootId != Guid.Empty && domainEvent.AggregateRootId != AggregateRootId)
                {
                    throw new AggregateRootException(
                        $"{nameof(AggregateRootId)} in event is  {domainEvent.AggregateRootId} which is different from the current {AggregateRootId}");
                }

                domainEvent.AggregateRootId = AggregateRootId;
            }

            ApplyEvent(domainEvent);
            AssertInvariantsAreMet();

            EventController.RaiseDomainEvent(domainEvent).GetAwaiter().GetResult();
                // this make the call sync and forces exceptions to boubble up. Should we make the domain async instead?
        }

        public virtual void AssertInvariantsAreMet()
        {
            if (AggregateRootId == Guid.Empty)
            {
                throw new InvariantsNotMetException($"{nameof(AggregateRootId)} not set.");
            }
        }
        
        private void ApplyEvent(TAggregateRootEventInterface domainEvent)
        {
            if (domainEvent is IAggregateRootCreatedEvent)
            {
                SetId(domainEvent.AggregateRootId);
            }

            _eventDispatcher.Dispatch(domainEvent);
        }

        private readonly EventDispatcher<TAggregateRootEventInterface> _eventDispatcher = new EventDispatcher<TAggregateRootEventInterface>();
        protected EventDispatcher<TAggregateRootEventInterface>.RegistrationBuilder RegisterEventAppliers()
        {
            return _eventDispatcher.RegisterAppliers();
        }
    }
}