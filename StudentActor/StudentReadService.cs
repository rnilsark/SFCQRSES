using System.Collections.Generic;
using System.Fabric;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace StudentActor
{
    //Idea: Act as a pure read model store.
    //Todo: How to keep up to date? How to handle reindexing after changing the read model?
    internal sealed class StudentReadService : StatefulService, IService
    {
        public StudentReadService(StatefulServiceContext context)
            : base(context)
        {
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            yield return new ServiceReplicaListener(this.CreateServiceRemotingListener, "StudentReadServiceEndpoint");
        }
    }
}