using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Common.DDD
{
    [DataContract]
    public abstract class EventStreamBase : IEventStream
    {
        protected EventStreamBase()
        {
            DomainEvents = new IDomainEvent[] { };
        }

        [DataMember]
        public IDomainEvent[] DomainEvents { get; private set; }

        public void Append(IDomainEvent domainEvent)
        {
            DomainEvents = DomainEvents.Union(new IDomainEvent[] { domainEvent }).ToArray();

            // Raise the event
            EventAppended?.Invoke(this, domainEvent);
        }

        public event EventHandler<IDomainEvent> EventAppended;
    }
}