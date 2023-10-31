using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace FT_Functions.ShipFunctions
{
    public class AssignNavigationOrders
    {
        private readonly ILogger<AssignNavigationOrders> _logger;

        public AssignNavigationOrders(ILogger<AssignNavigationOrders> log)
        {
            _logger = log;
        }

        [FunctionName("AssignNavigationOrders")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Assign Navigation Orders" })]
        [OpenApiParameter(name: "id", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **id** parameter")]
        [OpenApiParameter(name: "speed", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **speed** parameter")]
        [OpenApiParameter(name: "bearing", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **bearing** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = null)] 
                HttpRequest req,
            [CosmosDB(
                databaseName: "FullThrust",
                collectionName: "Ships",
                Id = "{Query.id}",
                PartitionKey = "{Query.id}",
                ConnectionStringSetting = "CosmosConnection")]
            Document shipDocument,
            [CosmosDB(
                databaseName: "FullThrust",
                collectionName: "Ships",
                ConnectionStringSetting = "CosmosConnection")] DocumentClient shipContainer)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            int newSpeed = int.Parse(req.Query["speed"]);
            int newBearing = int.Parse(req.Query["bearing"]);

            shipDocument.SetPropertyValue("speedChange", newSpeed);
            shipDocument.SetPropertyValue("bearingChange", newBearing);

            await shipContainer.ReplaceDocumentAsync(shipDocument.SelfLink, shipDocument);

            return new OkObjectResult(shipDocument);
        }
    }
}

