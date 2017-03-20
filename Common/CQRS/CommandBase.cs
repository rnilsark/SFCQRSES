using System;
using System.Runtime.Serialization;

namespace Common.CQRS
{
    [DataContract]
    public abstract class CommandBase : ICommand
    {
        protected CommandBase()
        {
            CommandId = Guid.NewGuid();
        }

        [DataMember]
        public Guid CommandId { get; private set; }

    }
}