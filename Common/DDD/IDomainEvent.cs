using System;

namespace Common.DDD
{
    public interface IDomainEvent
    {
        Guid EventId { get; }
        DateTime UtcTimeStamp { get; }
    }
}