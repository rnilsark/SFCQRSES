using Common.DDD;
using StudentActor.Domain;

namespace StudentActor.Events
{
    public interface IStudentEvent : IAggregateRootEvent
    {
    }

    public interface IStudentRegisteredEvent : IStudentEvent, IAggregateRootCreatedEvent
    {
        string Name { get; set; }
    }

    public interface ISubjectAddedEvent : IStudentEvent
    {
        string Name { get; set; }
        Level Level { get; set; }
    }
}