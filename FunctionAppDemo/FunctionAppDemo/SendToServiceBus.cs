using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;

namespace FunctionAppDemo;

public class SendToServiceBus
{
    private readonly ILogger<SendToServiceBus> _logger;

    public SendToServiceBus(ILogger<SendToServiceBus> logger)
    {
        _logger = logger;
    }
    [Function("SendToServiceBus")]
    [ServiceBusOutput("rollinsqueue", Connection = "ServiceBusConnection")]
    public async Task<string> Run(
    [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        _logger.LogInformation("HTTP trigger function processed a request.");

        // Read raw JSON from body
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

        // Send the exact JSON to Service Bus (no wrapping or re-serialization)
        var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
        await response.WriteStringAsync("Message sent to Service Bus queue.");

        return requestBody;
    }

    //[Function("SendToServiceBus")]
    //[ServiceBusOutput(queueOrTopicName: "rollinsqueue", Connection = "ServiceBusConnection")]
    //public async Task<string> Run(
    //    [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    //{
    //    _logger.LogInformation("HTTP trigger function processed a request.");

    //    // Read the request body
    //    var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

    //    // Serialize message for the queue
    //    var outputMessage = JsonSerializer.Serialize(new { message = requestBody });

    //    // Optionally respond to the HTTP caller
    //    var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
    //    await response.WriteStringAsync("Message sent to Service Bus queue.");

    //    // Return message to send to Service Bus
    //    return outputMessage;
    //}
}
