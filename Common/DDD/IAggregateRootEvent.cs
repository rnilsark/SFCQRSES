using System;

namespace Common.DDD
{
    public interface IAggregateRootDeletedEvent : IAggregateRootEvent
    {

    }

    public interface IAggregateRootCreatedEvent : IAggregateRootEvent
    {

    }

    public interface IAggregateRootEvent : IDomainEvent
    {
        Guid AggregateRootId { get; set; }
    }
}
