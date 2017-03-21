using System;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.DDD;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using StudentActor.Events;
using StudentActor.Interfaces;

namespace StudentActor
{
    internal class StudentActorService : ActorService, IStudentActorService
    {
        public StudentActorService
        (
            StatefulServiceContext context,
            ActorTypeInformation actorTypeInfo,
            Func<ActorService, ActorId, ActorBase> actorFactory = null,
            Func<ActorBase, IActorStateProvider, IActorStateManager> stateManagerFactory = null,
            IActorStateProvider stateProvider = null,
            ActorServiceSettings settings = null) :
            base(context, actorTypeInfo, actorFactory, stateManagerFactory, stateProvider, settings)
        {
        }

        //Note! This just works when having partition key 1. Unless, we need to make sure we call the correct actor service.
        public async Task<Student> GetStudentAsync(Guid studentId, CancellationToken cancellationToken)
        {
            var generator = new StudentReadModelGenerator(this.StateProvider);
            return await generator.TryGenerateAsync(new ActorId(studentId), cancellationToken);
        }
    }
    
    public class StudentReadModelGenerator
    {
        private readonly IActorStateProvider _stateProvider;
        private readonly EventDispatcher<IStudentEvent> _eventDispatcher = new EventDispatcher<IStudentEvent>();

        public StudentReadModelGenerator(IActorStateProvider stateProvider)
        {
            _stateProvider = stateProvider;
            _eventDispatcher.RegisterAppliers()
                .For<IStudentEvent>(e => Model.Id = e.AggregateRootId)
                .For<IStudentRegisteredEvent>(e => Model.Name = e.Name);
        }

        public async Task<Student> TryGenerateAsync(ActorId actorId, CancellationToken cancellationToken)
        {
            var eventStream = await _stateProvider.LoadStateAsync<EventStream>(
                actorId,
                StudentActor.EventStreamKey,
                cancellationToken);

            if (!eventStream.DomainEvents.Any())
            {
                return null;
            }

            var model = new Student();
            Model = model;
            foreach (var domainEvent in eventStream.DomainEvents)
            {
                _eventDispatcher.Dispatch(domainEvent as IStudentEvent);
            }
            var result = Model;
            Model = null;
            return result;
        }

        public Student Model { get; set; }
    }
}