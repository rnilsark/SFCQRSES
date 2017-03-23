using System;
using System.Threading;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace StudentActor
{
    internal static class Program
    {
        private static void Main()
        {
            try
            {
                ActorRuntime.RegisterActorAsync<StudentActor>(
                    (context, actorType) => new StudentActorService(context, actorType)).GetAwaiter().GetResult();

                ServiceRuntime.RegisterServiceAsync("StudentReadServiceType",
                    context => new StudentReadService(context)).GetAwaiter().GetResult();

                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                ActorEventSource.Current.ActorHostInitializationFailed(e.ToString());
                throw;
            }
        }
    }
}