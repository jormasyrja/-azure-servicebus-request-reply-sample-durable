# Introduction 
This is a sample implementation of the request-response message bus pattern using Azure Functions and Azure Service Bus.
In this sample, a single queue for both requests and replies (instead of a temporary queue per request/reply) will be used together with some Durable Functions functionality.
- .NET Core 3.1
- Azure Functions v3

# Getting Started
Setup environment (or local.settings.json) values:

|Name|Description|
|---|---|
|AzureWebJobsServiceBus|Connection string/SAS with read+listen rights to a ServiceBus namespace
|RequestQueueName|Name of an existing queue in the above namespace, which will be used for sending the *request*|
|ReplyQueueName|Name of an existing queue in the above namespace, which will be used for sending the *reply*|

There will be an HTTP trigger function at *http://localhost:7071/api/input* that will accept a simple POST with a payload:
```json
{ "name": "string" }
```

The API starts a new Durable Functions instance, which will asynchronously handle the request/reply pattern.
On success, the API will respond with
```
{
    "id": "8d7f914cebda4b86aa74db135077b7ec",
    "statusQueryGetUri": "http://localhost:7071/runtime/webhooks/durabletask/instances/8d7f914cebda4b86aa74db135077b7ec?taskHub=TestHubName&connection=Storage&code=qncc7CfyyYK7u4T5AM6d6TyLa6JFj3e6D1Jxbw9IgYE2Grgq7/i8fA==",
    "sendEventPostUri": "http://localhost:7071/runtime/webhooks/durabletask/instances/8d7f914cebda4b86aa74db135077b7ec/raiseEvent/{eventName}?taskHub=TestHubName&connection=Storage&code=qncc7CfyyYK7u4T5AM6d6TyLa6JFj3e6D1Jxbw9IgYE2Grgq7/i8fA==",
    "terminatePostUri": "http://localhost:7071/runtime/webhooks/durabletask/instances/8d7f914cebda4b86aa74db135077b7ec/terminate?reason={text}&taskHub=TestHubName&connection=Storage&code=qncc7CfyyYK7u4T5AM6d6TyLa6JFj3e6D1Jxbw9IgYE2Grgq7/i8fA==",
    "purgeHistoryDeleteUri": "http://localhost:7071/runtime/webhooks/durabletask/instances/8d7f914cebda4b86aa74db135077b7ec?taskHub=TestHubName&connection=Storage&code=qncc7CfyyYK7u4T5AM6d6TyLa6JFj3e6D1Jxbw9IgYE2Grgq7/i8fA=="
}
```
To see progress, use URI provided in property `statusQueryGetUri`. It will return a response something like this:
```
{
	"name": "HandleRequest",
	"instanceId": "8d7f914cebda4b86aa74db135077b7ec",
	"runtimeStatus": "Completed",
	"input": {
		"Name": "<name given in API request>"
	},
	"customStatus": null,
	"output": {
		"OrchestrationId": "8d7f914cebda4b86aa74db135077b7ec",
		"Name": "<name given in API request>",
		"Timestamp": "2020-11-27T19:05:13.9803949Z",
		"Id": "3a34be4f-77b1-4546-ba82-854cd92fb38c",
		"Success": true
	},
	"createdTime": "2020-11-27T19:05:13Z",
	"lastUpdatedTime": "2020-11-27T19:05:14Z"
}
```