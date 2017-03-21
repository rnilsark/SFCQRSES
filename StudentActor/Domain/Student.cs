using System;
using System.Collections;
using System.Collections.Generic;
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
                .For<IStudentRegisteredEvent>(e => Name = e.Name)
                .For<ISubjectAddedEvent>(e => Subjects.Add(Subject.Create(e.Name, e.Level)));
        }

        public string Name { get; set; }
        public IList<Subject> Subjects {get; set; } = new List<Subject>();

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

        public void AddSubject(Subject subject)
        {
            RaiseEvent(new SubjectAddedEvent(subject));
        }
    }
}