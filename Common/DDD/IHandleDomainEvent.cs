using System.Threading.Tasks;

namespace Common.DDD
{
    public interface IHandleDomainEvent<in TDomainEvent>
        where TDomainEvent : IDomainEvent
    {
        Task Handle(TDomainEvent domainEvent);
    }
}