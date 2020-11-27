using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using ServiceBus.RequestReply.Sample2.Dtos;

namespace ServiceBus.RequestReply.Sample2.Functions
{
    public class ReplyFunction
    {
        [FunctionName(nameof(HandleRequestFromQueue))]
        public Task HandleRequestFromQueue(
            [ServiceBusTrigger("%" + Constants.RequestQueueNameVariableName + "%")] Message message,
            [ServiceBus("%" + Constants.ReplyQueueNameVariableName + "%")] IAsyncCollector<QueueReplyDto> collector)
        {
            var request = JsonSerializer.Deserialize<QueueRequestDto>(message.Body);
            var reply = new QueueReplyDto
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Timestamp = DateTime.UtcNow,
                Success = true,
                OrchestrationId = request.OrchestrationId
            };

            return collector.AddAsync(reply);
        }
    }
}
