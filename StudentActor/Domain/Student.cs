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
                .For<IStudentRegisteredEvent>(Apply)
                .For<IStudentAddressChangedEvent>(Apply);
        }

        private void Apply(IStudentRegisteredEvent @event)
        {
            Name = @event.Name;
        }

        private void Apply(IStudentAddressChangedEvent @event)
        {
            Address = Address.Create(@event.Street, @event.ZipCode, @event.City);
        }
        
        public string Name { get; set; }
        public Address Address { get; set; }


        public void Register(Guid studentId, string name, Address address)
        {
            if (name.Length < 5)
                throw new ArgumentException("Name should be at least 5 characters.");

            RaiseEvent(new StudentRegisteredEvent
            {
                AggregateRootId = studentId,
                Name = name,
                Street = address.Street,
                ZipCode = address.ZipCode,
                City = address.City
            });
        }

        public void EditAddress(Address address)
        {
            RaiseEvent(new StudentAddressChangedEvent
            {
                Street = address.Street,
                ZipCode = address.ZipCode,
                City = address.City
            });
        }
    }
}