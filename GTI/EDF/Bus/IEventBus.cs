namespace EventBusTemplate.Bus
{
    public interface IEventBus<TEvent>
    {
        void Publish(TEvent @event);
        void Subscribe<THandler>() where THandler : class;
    }
}