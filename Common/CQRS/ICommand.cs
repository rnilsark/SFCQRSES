using System;

namespace Common.CQRS
{
    public interface ICommand
    {
        Guid CommandId { get; }
    }
}