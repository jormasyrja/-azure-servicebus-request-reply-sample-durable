using System;

namespace ServiceBus.RequestReply.Sample2.Dtos
{
    public class ApiReplyDto
    {
        public string Name { get; set; }
        public DateTime? Timestamp { get; set; }
        public Guid? Id { get; set; }
        public bool Success { get; set; }
    }
}
