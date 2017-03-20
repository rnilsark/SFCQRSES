using System;
using System.Runtime.Serialization;

namespace Common.DDD
{
    [DataContract]
    public abstract class DomainEventBase : IDomainEvent
    {
        protected DomainEventBase()
        {
            EventId = Guid.NewGuid();
            UtcTimeStamp = DateTime.UtcNow;
        }

        [DataMember]
        public Guid EventId { get; private set; }
        [DataMember]
        public DateTime UtcTimeStamp { get; private set; }
    }
}