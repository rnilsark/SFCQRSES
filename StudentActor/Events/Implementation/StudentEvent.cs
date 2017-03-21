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
    public class StudentRegisteredEvent : StudentEvent, IStudentRegisteredEvent
    {
        [DataMember]
        public string Name { get; set; }
    }

    [DataContract]
    public class SubjectAddedEvent : StudentEvent, ISubjectAddedEvent
    {
        public SubjectAddedEvent(Subject subject)
        {
            Name = subject.Name;
            Level = subject.Level;
        }

        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public Level Level { get; set; }
    }
}