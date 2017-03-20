using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Services.Remoting;

namespace StudentActor.Interfaces
{
    public interface IStudentActor : IActor
    {
        Task RegisterAsync(RegisterCommand command);
    }

    public interface IStudentActorService : IActorService, IService
    {
        Task<Student> GetStudentAsync(Guid studentId, CancellationToken cancellationToken);
    }
}
