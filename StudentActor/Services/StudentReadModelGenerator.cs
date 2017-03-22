using Common.CQRS;
using Common.DDD;
using StudentActor.Events;
using StudentActor.Interfaces;

namespace StudentActor.Services
{
    public class StudentReadModelGenerator : ReadModelGenerator<IStudentEvent, Student>
    {
        public StudentReadModelGenerator(IEventStreamReader eventStreamReader) : base(eventStreamReader)
        {
            RegisterEventAppliers()
                .For<IStudentEvent>(e => ReadModel.Id = e.AggregateRootId)
                .For<IStudentRegisteredEvent>(e => ReadModel.Name = e.Name)
                .For<IStudentAddressChangedEvent>(e => ReadModel.Address = new Address {Street = e.Street, ZipCode = e.ZipCode, City = e.City});
        }
    }
}