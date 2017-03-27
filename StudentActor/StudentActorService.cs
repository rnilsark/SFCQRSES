using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.DDD;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Query;
using Microsoft.ServiceFabric.Actors.Runtime;
using StudentActor.Events;
using StudentActor.Events.Implementation;
using StudentActor.Interfaces;
using StudentActor.Services;
using Student = StudentActor.Interfaces.Student;
using Subject = StudentActor.Interfaces.Subject;

namespace StudentActor
{

    internal class StudentActorService : ActorService, IStudentActorService, IHandleDomainEvent<StudentRegisteredEvent>
    {
        private readonly ActorStateProviderEventStreamReader _stateProviderEventStreamReader;

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
            _stateProviderEventStreamReader = new ActorStateProviderEventStreamReader(StateProvider, StudentActor.EventStreamStateKey);
        }

        private readonly ConcurrentDictionary<Guid, object> _cache = new ConcurrentDictionary<Guid, object>();
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            await base.RunAsync(cancellationToken);

            if (cancellationToken.CanBeCanceled)
                cancellationToken.ThrowIfCancellationRequested();
            
            ContinuationToken continuationToken = null;
            do
            {
                var page = await StateProvider.GetActorsAsync(int.MaxValue, continuationToken, cancellationToken);

                foreach (var actorId in page.Items)
                {
                    _cache.AddOrUpdate(actorId.GetGuidId(), new object(), (id, o) => o);
                }
                
                continuationToken = page.ContinuationToken;
            } while (continuationToken != null);
           
        }
        
        internal async Task Publish<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent
        {
            var handleDomainEvent = this as IHandleDomainEvent<TDomainEvent>;
            if (handleDomainEvent != null)
            {
                await handleDomainEvent.Handle(domainEvent);
            }

            //Here we could map it to a general "StudentChangedEvent" and include the complete readmodel for any consumers and publish on a service bus.
        }

        public Task<Student> GetStudentAsync(Guid studentId, CancellationToken cancellationToken)
        {
            Task<Student> student = null;
            using (var generator = new StudentReadModelGenerator(_stateProviderEventStreamReader))
            {
                student = generator.TryGenerateAsync(studentId, cancellationToken);
            }
            return student;
        }

        public async Task<IEnumerable<Student>> GetStudentsWithIdCacheAsync(CancellationToken cancellationToken)
        {
            var tasks = _cache.Select(kvp => GetStudentAsync(kvp.Key, cancellationToken));
            return (await Task.WhenAll(tasks));
        }

        public async Task<IEnumerable<Student>> GetStudentsAsync(CancellationToken cancellationToken)
        {
            var ids = new List<Guid>();
            ContinuationToken continuationToken = null;
            do
            {
                var page = await StateProvider.GetActorsAsync(int.MaxValue, continuationToken, cancellationToken);

                ids.AddRange(page.Items.Select(actorId => actorId.GetGuidId()));

                continuationToken = page.ContinuationToken;
            } while (continuationToken != null);

            var tasks = ids.Select(id => GetStudentAsync(id, cancellationToken));
            return await Task.WhenAll(tasks);
        }

        public async Task<IEnumerable<Student>> GetStudentsBySubjectAsync(Subject subject,
            CancellationToken cancellationToken)
        {
            var tasks = _cache.Select(kvp => GetStudentAsync(kvp.Key, cancellationToken));
            return (await Task.WhenAll(tasks)).Where(s => s.Subjects.Contains(subject));
        }

        //To test performance of paging.
        public async Task<IEnumerable<Guid>> GetAllGuids(int numItemsToReturn, CancellationToken cancellationToken)
        {
            ContinuationToken continuationToken = null;

            var ids = new List<Guid>();
            do
            {
                var page = await
                    StateProvider.GetActorsAsync(numItemsToReturn, continuationToken, cancellationToken);

                foreach (var actorId in page.Items)
                {
                    ids.Add(actorId.GetGuidId());
                }

                continuationToken = page.ContinuationToken;
            } while (continuationToken != null);

            return ids;
        }

        public Task Handle(StudentRegisteredEvent domainEvent)
        {
            _cache.AddOrUpdate(domainEvent.AggregateRootId, new object(), (id, o) => o);
            return Task.FromResult(true);
        }
    }
}