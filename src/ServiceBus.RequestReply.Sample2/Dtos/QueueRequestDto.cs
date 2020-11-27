namespace ServiceBus.RequestReply.Sample2.Dtos
{
    /// <summary>
    /// Represents a request that passed to the processor via ServiceBus
    /// </summary>
    public class QueueRequestDto
    {
        public string OrchestrationId { get; set; }
        public string Name { get; set; }
    }
}
