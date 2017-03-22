using System;
using System.Threading;
using System.Threading.Tasks;

namespace Common.DDD
{
    public interface IEventStreamReader
    {
        Task<IDomainEvent[]> GetEventStream(Guid id, CancellationToken cancellationToke);
    }
}