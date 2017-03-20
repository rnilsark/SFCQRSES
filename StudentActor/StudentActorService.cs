using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
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

        public async Task<Student> GetStudentAsync(Guid studentId, CancellationToken cancellationToken)
        {
            var state = await StateProvider.LoadStateAsync<EventStream>(
                new ActorId(studentId),
                StudentActor.EventStreamKey,
                cancellationToken);

            return new Student
            {
                Id = ((IStudentRegisteredEvent) state.DomainEvents[0]).AggregateRootId,
                Name = ((IStudentRegisteredEvent) state.DomainEvents[0]).Name
            };
        }
    }
}