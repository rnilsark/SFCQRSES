using System;
using System.Runtime.Serialization;
using Common.DDD;

namespace StudentActor.Events.Implementation
{
    [DataContract]
    public abstract class StudentEvent : DomainEventBase
    {
        [DataMember]
        public Guid AggregateRootId { get; set; }
    }

    [DataContract]
    public class StudentRegisteredEvent : StudentEvent, IStudentRegisteredEvent
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Street { get; set; }

        [DataMember]
        public string ZipCode { get; set; }

        [DataMember]
        public string City { get; set; }
    }

    [DataContract]
    public class StudentAddressChangedEvent : StudentEvent, IStudentAddressChangedEvent
    {
        [DataMember]
        public string Street { get; set; }

        [DataMember]
        public string ZipCode { get; set; }

        [DataMember]
        public string City { get; set; }
    }
}