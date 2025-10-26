using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

public class Function1
{
    private readonly ILogger _logger;
    public Function1(ILoggerFactory loggerFactory) => _logger = loggerFactory.CreateLogger<Function1>();

    [Function("Function1")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", "options", Route = null)]
        HttpRequestData req,
        FunctionContext context)
    {
        _logger.LogInformation("Function1 processed a request.");

        // Handle preflight
        if (string.Equals(req.Method, "OPTIONS", StringComparison.OrdinalIgnoreCase))
        {
            var preflight = req.CreateResponse(HttpStatusCode.OK);
           // AddCorsHeaders(preflight);
            return preflight;
        }

        // Normal request handling
        var response = req.CreateResponse(HttpStatusCode.OK);
        //AddCorsHeaders(response);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

        string favWrestler = Environment.GetEnvironmentVariable("REACT_APP_FAV_WRESTLER") ?? "Not Found sorry";
        _logger.LogInformation($"Favorite Wrestler from react: {favWrestler}");

        await response.WriteStringAsync($"Hello from Function1, Favorite Wrestler: {favWrestler}");
        return response;
    }

    private static void AddCorsHeaders(HttpResponseData response)
    {
        // Only for local/dev � restrict origins in production!
        response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:3000");
        response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
        response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
        // If you need credentials uncomment:
        // response.Headers.Add("Access-Control-Allow-Credentials", "true");
    }
}
