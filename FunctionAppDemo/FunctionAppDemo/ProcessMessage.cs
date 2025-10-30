//using Azure.Cosmos;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace FunctionAppDemo
{
    public class ProcessMessage
    {
        private readonly ILogger<ProcessMessage> _logger;
        private readonly CosmosClient _cosmosClient;
        private readonly string _databaseName = "DemoCosmos";
        private readonly string _containerName = "mycontainer";

        public ProcessMessage(ILogger<ProcessMessage> logger)
        {
            _logger = logger;

            // Create CosmosClient from environment variable.
            // Put your Cosmos DB connection string in local.settings.json as "CosmosDBConnection"
            var cosmosConn = Environment.GetEnvironmentVariable("CosmosDBConnection");
            if (string.IsNullOrWhiteSpace(cosmosConn))
            {
                throw new InvalidOperationException("CosmosDBConnection environment variable is not set.");
            }

            _cosmosClient = new CosmosClient(cosmosConn);
        }

        [Function("ProcessMessage")]
        public async Task Run(
            [ServiceBusTrigger("rollinsqueue", Connection = "ServiceBusConnection")] string messageBody)
        {
            _logger.LogInformation("Service Bus message received.");

            try
            {
                // Try parse message body as JSON
                JObject doc;
                try
                {
                    doc = JObject.Parse(messageBody);
                }
                catch (Exception)
                {
                    // If not valid JSON, wrap it in an object
                    doc = new JObject { ["message"] = messageBody };
                }

                // Ensure an 'id' (Cosmos DB requires an 'id' property)
                if (doc["id"] == null || string.IsNullOrWhiteSpace(doc["id"]!.ToString()))
                {
                    doc["id"] = Guid.NewGuid().ToString();
                }

                // Add a timestamp for auditing
                doc["receivedAt"] = DateTime.UtcNow;

                // Get container (make sure database + container exist and partition key is correct)
                var container = _cosmosClient.GetContainer(_databaseName, _containerName);

                // Choose partition key value — change if your container uses a different PK
                // If your container's partition key is /id then use doc["id"].ToString()
                var partitionKey = doc["id"]!.ToString();

                _logger.LogInformation("About to insert document: {json}", doc.ToString());
                var result = await container.UpsertItemAsync(doc, new PartitionKey(doc["id"]!.ToString()));
                _logger.LogInformation("Insert successful with RU charge: {ru}", result.RequestCharge);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed processing Service Bus message.");
                // Let the function fail — the message will be retried according to Service Bus settings.
                throw;
            }
        }
    }
}
