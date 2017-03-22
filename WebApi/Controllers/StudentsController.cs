using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Query;
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
        private readonly ActorProxyFactory _actorProxyFactory;
        private readonly ServiceProxyFactory _serviceProxyFactory;
        private static readonly FabricClient FabricClient = new FabricClient();
        private readonly Uri _serviceUri;

        public StudentsController()
        {
            _actorProxyFactory = new ActorProxyFactory();
            _serviceProxyFactory = new ServiceProxyFactory();
            _serviceUri = new Uri($@"{FabricRuntime.GetActivationContext().ApplicationName}/{"StudentActorService"}");
        }

        [HttpGet]
        public async Task<Student> Get(Guid id)
        {
            var proxy = _actorProxyFactory.CreateActorServiceProxy<IStudentActorService>(
                _serviceUri, new ActorId(id));

            var student = await proxy.GetStudentAsync(id, CancellationToken.None);

            return student;
        }

        [HttpGet]
        public async Task<IEnumerable<Student>> Get()
        {
            var students = new List<Student>();

            var partitions = await FabricClient.QueryManager.GetPartitionListAsync(_serviceUri);

            foreach (var p in partitions)
            {
                // ReSharper disable once PossibleNullReferenceException
                var minKey = (p.PartitionInformation as Int64RangePartitionInformation).LowKey;
                var proxy = _serviceProxyFactory.CreateServiceProxy<IStudentActorService>(_serviceUri, new ServicePartitionKey(minKey));

                var result = await proxy.GetStudentsAsync(CancellationToken.None);
                if (result != null)
                {
                    students.AddRange(result);
                }
            }

            return students;
        }

        [HttpGet]
        public async Task<IEnumerable<Student>> Get(string subject)
        {
            var students = new List<Student>();

            Enum.TryParse(subject, out Subject subjectEnum);

            var partitions = await FabricClient.QueryManager.GetPartitionListAsync(_serviceUri);

            foreach (var p in partitions)
            {
                // ReSharper disable once PossibleNullReferenceException
                var minKey = (p.PartitionInformation as Int64RangePartitionInformation).LowKey;
                var proxy = _serviceProxyFactory.CreateServiceProxy<IStudentActorService>(_serviceUri, new ServicePartitionKey(minKey));
                
                var result = await proxy.GetStudentsBySubjectAsync(subjectEnum,  CancellationToken.None);
                if (result != null)
                {
                    students.AddRange(result);
                }
            }

            return students;
        }

        [HttpPost]
        public async Task<IEnumerable<RandomStudent>>  Post(int numberOfStudents)
        {
            var tasks = new List<Task<RandomStudent>>();
            for (var i = 0; i < numberOfStudents; i++)
            {
                tasks.Add(RegisterStudent());
            }

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
                Address = new Address { Street = RandomString(5), ZipCode = RandomStringNumber(5), City = RandomString(5) },
                Subject = student.Subject
            });

            return student;
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

        private static readonly Random _random = new Random();
        
        public static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxy";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        public static string RandomStringNumber(int length)
        {
            const string chars = "1234567890";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        public static Subject RandomSubject()
        {
            var  subjects = new List<Subject> { Subject.Math, Subject.Physics, Subject.Language, Subject.IT };
            return subjects[_random.Next(subjects.Count)];
        }
    }
}
