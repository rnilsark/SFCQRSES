namespace Common.DDD
{
    public interface IAggregateRoot
    {
        void Initialize(IEventController eventController, IDomainEvent[] eventStream);
    }
}