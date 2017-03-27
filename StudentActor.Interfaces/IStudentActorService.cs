using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Services.Remoting;

namespace StudentActor.Interfaces
{
    public interface IStudentActorService : IActorService, IService
    {
        Task<Student> GetStudentAsync(Guid studentId, CancellationToken cancellationToken);
        Task<IEnumerable<Student>> GetStudentsWithIdCacheAsync(CancellationToken cancellationToken);
        Task<IEnumerable<Student>> GetStudentsAsync(CancellationToken cancellationToken);
        Task<IEnumerable<Student>> GetStudentsBySubjectAsync(Subject subject, CancellationToken cancellationToken);
        Task<IEnumerable<Guid>> GetAllGuids(int numItemsToReturn, CancellationToken cancellationToken);
    }
}