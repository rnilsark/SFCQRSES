using Common.CQRS;

namespace StudentActor.Interfaces
{
    public class RegisterCommand : CommandBase
    {
        public string Name { get; set; }
    }
}