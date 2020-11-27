namespace ServiceBus.RequestReply.Sample2.Dtos
{
    /// <summary>
    /// Represents request object received via HttpTrigger
    /// </summary>
    public class ApiRequestDto
    {
        public string Name { get; set; }
    }
}
