using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.DDD;

namespace StudentActor.Events
{
    public interface IStudentEvent : IAggregateRootEvent
    {

    }

    public interface IStudentRegisteredEvent : IStudentEvent, IAggregateRootCreatedEvent
    {
        string Name { get; set; }
    }
}
