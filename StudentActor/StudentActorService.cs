using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
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

        public Task<Student> GetStudentAsync(Guid studentId, CancellationToken cancellationToken)
        {
            Task<Student> student = null;
            using (var generator = new StudentReadModelGenerator(_eventStreamReader))
            {
                student = generator.TryGenerateAsync(studentId, cancellationToken);
            }
            return student;
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            await base.RunAsync(cancellationToken);
            //Possible cache.
            //while (true)
            //{
            //    if (cancellationToken.CanBeCanceled)
            //        cancellationToken.ThrowIfCancellationRequested();

            //    ContinuationToken continuationToken = null;
            //    var students = new List<Student>();

            //    do
            //    {
            //        var page = await
            //            StateProvider.GetActorsAsync(100, continuationToken, cancellationToken);

            //        foreach (var actorId in page.Items)
            //        {
            //            using (var generator = new StudentReadModelGenerator(_eventStreamReader))
            //            {
            //                students.Add(await generator.TryGenerateAsync(actorId.GetGuidId(), cancellationToken));
            //            }
            //        }

            //        continuationToken = page.ContinuationToken;
            //    } while (continuationToken != null);

            //    await Task.Delay(millisecondsDelay: 60 * 1000 * 5, cancellationToken: cancellationToken);
            //}          
        }

        public async Task<IEnumerable<Student>> GetStudentsAsync(CancellationToken cancellationToken)
        {
            ContinuationToken continuationToken = null;
            var students = new List<Student>();

            do
            {
                var page = await 
                    StateProvider.GetActorsAsync(100, continuationToken, cancellationToken);

                foreach (var actorId in page.Items)
                {
                    using (var generator = new StudentReadModelGenerator(_eventStreamReader))
                    {
                        students.Add(await generator.TryGenerateAsync(actorId.GetGuidId(), cancellationToken));
                    }
                }

                continuationToken = page.ContinuationToken;
            } while (continuationToken != null);

            return students;
        }

        public async Task<IEnumerable<Student>> GetStudentsBySubjectAsync(Subject subject, CancellationToken cancellationToken)
        {
            //This is the naive filter. TODO: investigate how an index can help to improve.
            return (await GetStudentsAsync(cancellationToken)).Where(student => student.Subjects.Any(s => s == subject));
        }
    }
}