using System;

namespace Common.DDD
{
    public interface IEventStream
    {
        IDomainEvent[] DomainEvents { get; }
        void Append(IDomainEvent domainEvent);

        event EventHandler<IDomainEvent> EventAppended;
    }
}