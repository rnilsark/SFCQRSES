using System.Threading.Tasks;
using Common.DDD;
using Common.ServiceFabric.Extensions.Actors.Runtime;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using StudentActor.Domain;
using StudentActor.Events;
using StudentActor.Events.Implementation;
using StudentActor.Interfaces;
using StudentActor.Mapping;
using Student = StudentActor.Domain.Student;

namespace StudentActor
{
    [StatePersistence(StatePersistence.Persisted)]
    internal class StudentActor : EventStoredActorBase<Student, EventStream>, IStudentActor,
        IHandleDomainEvent<StudentRegisteredEvent>
    {
        private readonly StudentActorService _actorService;
        public const string ForeignKeysStateKey = @"__fk";

        public StudentActor(StudentActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
            _actorService = actorService;
        }
        
        public Task RegisterAsync(RegisterCommand command)
        {
            DomainState.Register(this.GetActorId().GetGuidId(), command.Name,
                command.Address.ToDomainModel(), command.Subject.ToDomainModel());

            return Task.FromResult(true);
        }

        protected override async Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");
            await GetAndSetDomainAsync();
        }

        public async Task Handle(StudentRegisteredEvent domainEvent)
        {
            await StoreDomainEventAsync(domainEvent);
            await _actorService.Publish(domainEvent);
        }
    }
}