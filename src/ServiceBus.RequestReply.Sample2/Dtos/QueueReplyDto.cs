using System;

namespace ServiceBus.RequestReply.Sample2.Dtos
{
    public class QueueReplyDto : ApiReplyDto
    {
        public string OrchestrationId { get; set; }
    }
}
