using System;
using Common.DDD;
using StudentActor.Events;
using StudentActor.Events.Implementation;

namespace StudentActor.Domain
{
    public class Student : AggregateRoot<IStudentEvent>
    {
        public Student()
        {
            RegisterEventAppliers()
                .For<IStudentRegisteredEvent>(e => Name = e.Name);
        }

        public string Name { get; set; }

        public void Register(Guid studentId, string name)
        {
            if (name.Length < 5)
            {
                throw new ArgumentException("Name should be at least 5 characters.");
            }
            
            RaiseEvent(new StudentRegisteredEvent
            {
                AggregateRootId = studentId,
                Name = name
            });
        }
    }
}