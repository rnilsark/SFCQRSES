using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Query;
using Microsoft.ServiceFabric.Actors.Runtime;
using StudentActor.Interfaces;
using StudentActor.Services;

namespace StudentActor
{
    internal class StudentActorService : ActorService, IStudentActorService
    {
        private readonly ActorEventStreamReader _eventStreamReader;

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
            _eventStreamReader = new ActorEventStreamReader(StateProvider, StudentActor.EventStreamKey);
        }

        public async Task<Student> GetStudentAsync(Guid studentId, CancellationToken cancellationToken)
        {
            using (var generator = new StudentReadModelGenerator(_eventStreamReader))
            {
                return await generator.TryGenerateAsync(studentId, cancellationToken);
            }
        }

        public async Task<IEnumerable<Student>> GetStudentsAsync(CancellationToken cancellationToken)
        {
            ContinuationToken continuationToken = null;
            var students = new List<Student>();

            do
            {
                var page = await StateProvider.GetActorsAsync(100, continuationToken, cancellationToken);

                foreach (var actorId in page.Items)
                {
                    var generator = new StudentReadModelGenerator(_eventStreamReader);
                    students.Add(await generator.TryGenerateAsync(actorId.GetGuidId(), cancellationToken));
                }

                continuationToken = page.ContinuationToken;
            } while (continuationToken != null);

            return students;
        }
    }
}