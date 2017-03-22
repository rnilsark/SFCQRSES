using Common.DDD;
using StudentActor.Domain;

namespace StudentActor.Events
{
    public interface IStudentEvent : IAggregateRootEvent
    {
    }

    public interface IStudentNameChangedEvent : IStudentEvent
    {
        string Name { get; set; }
    }

    public interface IAddressChangedEvent : IStudentEvent
    {
        string Street { get; set; }
        string ZipCode { get; set; }
        string City { get; set; }
    }

    public interface IEnrollmentEvent : IStudentEvent
    {
        int EnrollmentId { get; set; }
    }

    public interface IStudentEnrolledEvent : IEnrollmentEvent
    {
        Subject Subject { get; set; }
    }

    public interface IStudentRegisteredEvent : IAggregateRootCreatedEvent, IStudentNameChangedEvent, IAddressChangedEvent, IStudentEnrolledEvent
    { }
}