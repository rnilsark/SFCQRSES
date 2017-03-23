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
    public class SubjectsController : ApiController
    {
        private static readonly FabricClient FabricClient = new FabricClient();

        private static readonly Random Random = new Random();
        private readonly ActorProxyFactory _actorProxyFactory;
        private readonly Uri _actorServiceUri;
        private readonly ServiceProxyFactory _serviceProxyFactory;

        public SubjectsController()
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
        [Route("/subjects")]
        public async Task<IEnumerable<Student>> Get(string subject)
        {
            var students = new List<Student>();

            Enum.TryParse(subject, out Subject subjectEnum);

            var partitions = await FabricClient.QueryManager.GetPartitionListAsync(_actorServiceUri);

            foreach (var p in partitions)
            {
                // ReSharper disable once PossibleNullReferenceException
                var minKey = (p.PartitionInformation as Int64RangePartitionInformation).LowKey;
                var proxy = _serviceProxyFactory.CreateServiceProxy<IStudentActorService>(_actorServiceUri,
                    new ServicePartitionKey(minKey));

                var result = await proxy.GetStudentsBySubjectAsync(subjectEnum, CancellationToken.None);
                if (result != null)
                    students.AddRange(result);
            }

            return students;
        }
    }
}