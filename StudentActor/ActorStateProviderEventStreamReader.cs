using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.DDD;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using StudentActor.Events;

namespace StudentActor
{
    public class ActorStateProviderEventStreamReader : IEventStreamReader
    {
        private readonly IActorStateProvider _stateProvider;
        private readonly string _stateKey;

        public ActorStateProviderEventStreamReader(IActorStateProvider stateProvider, string stateKey)
        {
            _stateProvider = stateProvider;
            _stateKey = stateKey;
        }

        public async Task<IDomainEvent[]> GetEventStream(Guid id, CancellationToken cancellationToken)
        {
            var eventStream = await _stateProvider.LoadStateAsync<EventStream>(
                new ActorId(id), 
                _stateKey,
                cancellationToken);

            return eventStream.DomainEvents;
        }
    }
}