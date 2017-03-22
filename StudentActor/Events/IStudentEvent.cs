using Common.DDD;
using StudentActor.Domain;

namespace StudentActor.Events
{
    public interface IStudentEvent : IAggregateRootEvent
    {
    }

    public interface IStudentRegisteredEvent : IStudentEvent, IAggregateRootCreatedEvent, IStudentAddressChangedEvent
    {
        string Name { get; set; }
    }

    public interface IStudentAddressChangedEvent : IStudentEvent
    {
        string Street { get; set; }
        string ZipCode { get; set; }
        string City { get; set; }
    }
    
}