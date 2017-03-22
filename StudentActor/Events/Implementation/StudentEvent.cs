using System;
using System.Runtime.Serialization;
using Common.DDD;
using StudentActor.Domain;

namespace StudentActor.Events.Implementation
{
    [DataContract]
    public abstract class StudentEvent : DomainEventBase
    {
        [DataMember]
        public Guid AggregateRootId { get; set; }
    }

    [DataContract]
    public class AddressChangedEvent : StudentEvent, IAddressChangedEvent
    {
        [DataMember]
        public string Street { get; set; }

        [DataMember]
        public string ZipCode { get; set; }

        [DataMember]
        public string City { get; set; }
    }

    [DataContract]
    public abstract class EnrollmentEvent : StudentEvent
    {
        public int EnrollmentId { get; set; }
    }

    [DataContract]
    public class StudentRegisteredEvent : EnrollmentEvent, IStudentRegisteredEvent
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Street { get; set; }

        [DataMember]
        public string ZipCode { get; set; }

        [DataMember]
        public string City { get; set; }

        [DataMember]
        public Subject Subject { get; set; }
    }
}