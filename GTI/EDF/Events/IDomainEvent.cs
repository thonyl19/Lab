namespace EventBusTemplate.Events
{
    public interface IDomainEvent
    {
        System.DateTime OccurredOn { get; }
    }
}