using System;
using System.Collections.Generic;
using System.Fabric;
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

        public StudentsController()
        {
            _actorProxyFactory = new ActorProxyFactory();
            _serviceProxyFactory = new ServiceProxyFactory();
        }

        [HttpGet]
        public async Task<Student> Get(Guid id)
        {
            var proxy = _serviceProxyFactory.CreateServiceProxy<IStudentActorService>(
                new Uri($@"{FabricRuntime.GetActivationContext().ApplicationName}/{"StudentActorService"}"), new ServicePartitionKey(1));

            var state = await proxy.GetStudentAsync(id, CancellationToken.None);

            return state;
        }

        [HttpPost]
        public async Task Post()
        {
            var proxy = _actorProxyFactory.CreateActorProxy<IStudentActor>(new ActorId(Guid.Parse("F1DEEC4E-AFAE-439A-8976-0BE1F40BF0C2")));
            await proxy.RegisterAsync(new RegisterCommand { Name = "Kalle"});
        }
    }
}
