using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Query;
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

        [HttpPost]
        public async Task<IEnumerable<Guid>>  Post(int numberOfStudents)
        {
            var ids = new List<Guid>();
            for (var i = 0; i < numberOfStudents; i++)
            {
                var id = Guid.NewGuid();
                ids.Add(id);

                var proxy = _actorProxyFactory.CreateActorProxy<IStudentActor>(new ActorId(id));
                await proxy.RegisterAsync(new RegisterCommand
                {
                    Name = "Person" + i,
                    Address = new Address { Street = "Street" + i, ZipCode = "ZipCode" + i, City = "City" + i}
                });
            }

            return ids;
        }
    }
}
