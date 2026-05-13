namespace EventBusTemplate.Events
{
    public interface IFlowEvent
    {
        System.Guid FlowInstanceId { get; }
    }
}