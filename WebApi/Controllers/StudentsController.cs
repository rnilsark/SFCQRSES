using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using StudentActor.Interfaces;

namespace WebApi.Controllers
{
    [ServiceRequestActionFilter]
    public class StudentsController : ApiController
    {
        private static readonly FabricClient FabricClient = new FabricClient();

        private static readonly Random Random = new Random();
        private readonly ActorProxyFactory _actorProxyFactory;
        private readonly Uri _actorServiceUri;
        private readonly ServiceProxyFactory _serviceProxyFactory;

        public StudentsController()
        {
            _actorProxyFactory = new ActorProxyFactory();
            _serviceProxyFactory = new ServiceProxyFactory();
            _actorServiceUri = new Uri($@"{FabricRuntime.GetActivationContext().ApplicationName}/{"StudentActorService"}");
        }

        [HttpGet]
        public async Task<Student> Get(Guid id)
        {
            var proxy = _actorProxyFactory.CreateActorServiceProxy<IStudentActorService>(
                _actorServiceUri, new ActorId(id));

            var student = await proxy.GetStudentAsync(id, CancellationToken.None);

            return student;
        }

        [HttpGet]
        public async Task<IEnumerable<Student>> Get(bool useCache)
        {
            var students = new List<Student>();

            var partitions = await FabricClient.QueryManager.GetPartitionListAsync(_actorServiceUri);

            foreach (var p in partitions)
            {
                // ReSharper disable once PossibleNullReferenceException
                var minKey = (p.PartitionInformation as Int64RangePartitionInformation).LowKey;
                var proxy = _serviceProxyFactory.CreateServiceProxy<IStudentActorService>(_actorServiceUri,
                    new ServicePartitionKey(minKey));

                IEnumerable<Student> result;
                if (useCache)
                {
                    result = await proxy.GetStudentsWithIdCacheAsync(CancellationToken.None);
                }
                else
                {
                    result = await proxy.GetStudentsAsync(CancellationToken.None);
                }
                
                if (result != null)
                    students.AddRange(result);
            }

            return students;
        }

        [HttpGet]
        public async Task<IEnumerable<Guid>> Get(int numItemsToReturnPerPage)
        {
            var students = new List<Guid>();

            var partitions = await FabricClient.QueryManager.GetPartitionListAsync(_actorServiceUri);

            foreach (var p in partitions)
            {
                // ReSharper disable once PossibleNullReferenceException
                var minKey = (p.PartitionInformation as Int64RangePartitionInformation).LowKey;
                var proxy = _serviceProxyFactory.CreateServiceProxy<IStudentActorService>(_actorServiceUri,
                    new ServicePartitionKey(minKey));

                var result = await proxy.GetAllGuids(numItemsToReturnPerPage, CancellationToken.None);
                if (result != null)
                    students.AddRange(result);
            }

            return students;
        }
        
        [HttpPost]
        public async Task<IEnumerable<RandomStudent>> Post(int numberOfStudents)
        {
            var tasks = new List<Task<RandomStudent>>();
            for (var i = 0; i < numberOfStudents; i++)
                tasks.Add(RegisterStudent());

            var result = await Task.WhenAll(tasks);

            return result;
        }

        private async Task<RandomStudent> RegisterStudent()
        {
            var student = new RandomStudent();
            var proxy = _actorProxyFactory.CreateActorProxy<IStudentActor>(new ActorId(student.Id));
            await proxy.RegisterAsync(new RegisterCommand
            {
                Name = student.Name,
                Address =
                    new Address {Street = RandomString(5), ZipCode = RandomStringNumber(5), City = RandomString(5)},
                Subject = student.Subject
            });

            return student;
        }

        #region Random test data generators

        public static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxy";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }

        public static string RandomStringNumber(int length)
        {
            const string chars = "1234567890";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }

        public static Subject RandomSubject()
        {
            var subjects = new List<Subject> {Subject.Math, Subject.Physics, Subject.Language, Subject.IT};
            return subjects[Random.Next(subjects.Count)];
        }

        public class RandomStudent
        {
            public RandomStudent()
            {
                Id = Guid.NewGuid();
                Name = RandomString(8);
                Subject = RandomSubject();
            }

            public Guid Id { get; }
            public string Name { get; set; }
            public Subject Subject { get; set; }
        }

    #endregion
    }
}