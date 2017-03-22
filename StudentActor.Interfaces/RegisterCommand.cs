using System.Runtime.Serialization;
using Common.CQRS;

namespace StudentActor.Interfaces
{
    [DataContract]
    public class RegisterCommand : CommandBase
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public Address Address { get; set; }
    }
}