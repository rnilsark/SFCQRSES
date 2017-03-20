using System.Runtime.Serialization;
using Common.DDD;
using StudentActor.Events.Implementation;

namespace StudentActor.Events
{
    [DataContract]
    [KnownType(typeof(StudentRegisteredEvent))]
    public class EventStream : EventStreamBase
    { }
}