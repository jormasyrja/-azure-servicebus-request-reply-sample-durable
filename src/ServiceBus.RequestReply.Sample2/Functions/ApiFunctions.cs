using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using ServiceBus.RequestReply.Sample2.Dtos;

namespace ServiceBus.RequestReply.Sample2.Functions
{
    public class ApiFunctions
    {
        [FunctionName(nameof(HandleInputRequest))]
        public async Task<IActionResult> HandleInputRequest(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "input")] ApiRequestDto input,
            HttpRequest req,
            [DurableClient] IDurableOrchestrationClient orchestrationClient)
        {
            var instanceId = await orchestrationClient.StartNewAsync(nameof(RequestProcessorDurableFunctions.HandleRequest), input);

            return orchestrationClient.CreateCheckStatusResponse(req, instanceId);
        }
    }
}
