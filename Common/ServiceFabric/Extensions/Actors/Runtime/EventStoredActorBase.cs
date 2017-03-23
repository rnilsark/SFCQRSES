using System;
using System.Threading.Tasks;
using Common.DDD;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;

namespace Common.ServiceFabric.Extensions.Actors.Runtime
{
    public abstract class EventStoredActorBase<TDomainAggregateRoot, TDomainEventStream> : Actor, IEventController
        where TDomainEventStream : IEventStream, new()
        where TDomainAggregateRoot : class, IAggregateRoot, new()
    {
        public const string EventStreamStateKey = @"__event_stream";
        protected TDomainAggregateRoot DomainState = null;

        protected EventStoredActorBase(ActorService actorService, ActorId actorId) : base(actorService, actorId)
        {
        }

        protected async Task<TDomainAggregateRoot> GetAndSetDomainAsync()
        {
            if (DomainState != null) return await Task.FromResult(DomainState);

            var eventStream = await this.StateManager.GetOrAddStateAsync<TDomainEventStream>(EventStreamStateKey, new TDomainEventStream());
            DomainState = new TDomainAggregateRoot();
            DomainState.Initialize(this, eventStream.DomainEvents);
            return DomainState;
        }
        
        protected async Task StoreDomainEventAsync(IDomainEvent domainEvent)
        {
            var eventStream = await StateManager.GetOrAddStateAsync<TDomainEventStream>(EventStreamStateKey, new TDomainEventStream());
            eventStream.Append(domainEvent);
            await StateManager.SetStateAsync(EventStreamStateKey, eventStream);
        }

        public async Task RaiseDomainEvent<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            var handleDomainEvent = this as IHandleDomainEvent<TDomainEvent>;
            if (handleDomainEvent == null)
            {
                throw new Exception($"No handler found for {domainEvent}.");
            }

            await handleDomainEvent.Handle(domainEvent);
        }

    }
}