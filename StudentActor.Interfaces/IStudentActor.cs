using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace StudentActor.Interfaces
{
    public interface IStudentActor : IActor
    {
        Task RegisterAsync(RegisterCommand command);
    }
}
