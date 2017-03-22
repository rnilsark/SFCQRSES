using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.DDD;

namespace Common.CQRS
{
    public abstract class ReadModelGenerator<TAggregateRootEventInterface, TReadModel> : IDisposable
        where TAggregateRootEventInterface : class, IAggregateRootEvent
        where TReadModel : class, new()
    {
        private readonly IEventStreamReader _eventStreamReader;
        private readonly EventDispatcher<TAggregateRootEventInterface> _eventDispatcher = new EventDispatcher<TAggregateRootEventInterface>();

        protected ReadModelGenerator(IEventStreamReader eventStreamReader)
        {
            _eventStreamReader = eventStreamReader;
        }

        protected EventDispatcher<TAggregateRootEventInterface>.RegistrationBuilder RegisterEventAppliers()
        {
            return _eventDispatcher.RegisterAppliers();
        }

        public async Task<TReadModel> TryGenerateAsync(Guid aggregateRootId, CancellationToken cancellationToken)
        {
            var eventStream = await _eventStreamReader.GetEventStream(aggregateRootId, cancellationToken);
            
            if (!eventStream.Any())
            {
                return null;
            }
            
            ReadModel = new TReadModel();
            foreach (var domainEvent in eventStream)
            {
                _eventDispatcher.Dispatch(domainEvent as TAggregateRootEventInterface);
            }
            var result = ReadModel;
            ReadModel = null;
            return result;
        }

        public TReadModel ReadModel { get; private set; }

        public void Dispose()
        { }
    }
}