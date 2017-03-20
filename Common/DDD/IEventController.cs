using System.Threading.Tasks;

namespace Common.DDD
{
    public interface IEventController
    {
        Task RaiseDomainEvent<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent;
    }
}
