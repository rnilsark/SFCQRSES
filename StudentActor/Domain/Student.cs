using System;
using System.Collections.Generic;
using System.Linq;
using Common.DDD;
using StudentActor.Events;
using StudentActor.Events.Implementation;

namespace StudentActor.Domain
{
    public partial class Student : AggregateRoot<IStudentEvent>
    {
        public Student()
        {
            RegisterEventAppliers()
                .For<IStudentRegisteredEvent>(Apply)
                .For<IAddressChangedEvent>(Apply)
                .For<IStudentEnrolledEvent>(e => Enrollments.Add(new Enrollment(this, e.EnrollmentId)))
                .For<IEnrollmentEvent>(
                    e => Enrollments.Single(enrollment => enrollment.Id == e.EnrollmentId).ApplyEvent(e));
        }

        private void Apply(IStudentRegisteredEvent @event)
        {
            Name = @event.Name;
        }

        private void Apply(IAddressChangedEvent @event)
        {
            Address = Address.Create(@event.Street, @event.ZipCode, @event.City);
        }
        
        public string Name { get; set; }
        public Address Address { get; set; }
        public IList<Enrollment> Enrollments { get; set; } = new List<Enrollment>();


        public void Register(Guid studentId, string name, Address address, Subject subject)
        {
            if (name.Length < 5)
                throw new ArgumentException("Name should be at least 5 characters.");

            RaiseEvent(new StudentRegisteredEvent
            {
                AggregateRootId = studentId,
                Name = name,
                Street = address.Street,
                ZipCode = address.ZipCode,
                City = address.City,
                EnrollmentId = 1, //First enrollment upon registration.
                Subject = subject
            });
        }
        
        public void EditAddress(Address address)
        {
            RaiseEvent(new AddressChangedEvent
            {
                Street = address.Street,
                ZipCode = address.ZipCode,
                City = address.City
            });
        }
    }
}