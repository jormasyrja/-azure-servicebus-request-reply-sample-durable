using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using ServiceBus.RequestReply.Sample2.Dtos;

namespace ServiceBus.RequestReply.Sample2.Functions
{
    public class RequestProcessorDurableFunctions
    {
        [FunctionName(nameof(HandleRequest))]
        public async Task HandleRequest([OrchestrationTrigger] IDurableOrchestrationContext orchestrationContext)
        {
            var input = orchestrationContext.GetInput<ApiRequestDto>();
            var queueRequest = new QueueRequestDto
            {
                Name = input.Name,
                OrchestrationId = orchestrationContext.InstanceId
            };

            await orchestrationContext.CallActivityAsync(nameof(HandleRequestEnqueue), queueRequest);
            
            var cancellationTokenSource = new CancellationTokenSource();

            var requestTask = orchestrationContext.WaitForExternalEvent<ApiReplyDto>(Constants.RequestProcessedEventName);
            var timeoutTask = orchestrationContext.CreateTimer(orchestrationContext.CurrentUtcDateTime.AddMinutes(5), cancellationTokenSource.Token);

            await Task.WhenAny(requestTask, timeoutTask);

            // Reply received
            if (requestTask.IsCompletedSuccessfully)
            {
                cancellationTokenSource.Cancel();
                orchestrationContext.SetOutput(requestTask.Result);
            }
            else
            {
                // Timeout
                orchestrationContext.SetOutput(new QueueReplyDto
                {
                    Name = input.Name,
                    Success = false
                });
            }
        }

        [FunctionName(nameof(HandleRequestEnqueue))]
        public Task HandleRequestEnqueue(
            [ActivityTrigger] IDurableActivityContext activityContext,
            [ServiceBus("%" + Constants.RequestQueueNameVariableName + "%")] IAsyncCollector<QueueRequestDto> collector)
        {
            var dto = activityContext.GetInput<QueueRequestDto>();
            return collector.AddAsync(dto);
        }

        [FunctionName(nameof(HandleReplyDequeue))]
        public Task HandleReplyDequeue(
            [ServiceBusTrigger("%" + Constants.ReplyQueueNameVariableName + "%")] Message queueMessage,
            [DurableClient] IDurableOrchestrationClient orchestrationClient)
        {
            var reply = JsonSerializer.Deserialize<QueueReplyDto>(queueMessage.Body);
            return orchestrationClient.RaiseEventAsync(reply.OrchestrationId, Constants.RequestProcessedEventName, reply);
        }
    }
}
