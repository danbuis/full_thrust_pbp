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
using System.Globalization;

namespace FT_Functions.ShipFunctions
{
    public class MoveShipToCoordinates
    {
        private readonly ILogger<MoveShipToCoordinates> _logger;

        public MoveShipToCoordinates(ILogger<MoveShipToCoordinates> log)
        {
            _logger = log;
        }

        [FunctionName("MoveShipToCoordinates")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Move Ship To Coordinates" })]
        [OpenApiParameter(name: "id", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Id** parameter")]
        [OpenApiParameter(name: "x", In = ParameterLocation.Query, Required = true, Type = typeof(float), Description = "The **X* parameter")]
        [OpenApiParameter(name: "y", In = ParameterLocation.Query, Required = true, Type = typeof(float), Description = "The **Y** parameter")]
        [OpenApiParameter(name: "bearing", In = ParameterLocation.Query, Required = true, Type = typeof(float), Description = "The **New Bearing** parameter")]
        [OpenApiParameter(name: "speed", In = ParameterLocation.Query, Required = true, Type = typeof(float), Description = "The **New Speed** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = null)]
            HttpRequest req,
            [CosmosDB(
                databaseName: "FullThrust",
                collectionName: "Ships",
                Id = "{Query.id}",
                PartitionKey = "{Query.id}",
                ConnectionStringSetting = "CosmosConnection")] Document shipDocument,
            [CosmosDB(
                databaseName: "FullThrust",
                collectionName: "Ships",
                ConnectionStringSetting = "CosmosConnection")] DocumentClient shipContainer
            )
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            float newX = float.Parse(req.Query["x"], CultureInfo.InvariantCulture.NumberFormat);
            float newY = float.Parse(req.Query["y"], CultureInfo.InvariantCulture.NumberFormat);
            int newSpeed = int.Parse(req.Query["speed"]);
            int newBearing = int.Parse(req.Query["bearing"]);

            shipDocument.SetPropertyValue("x", newX);
            shipDocument.SetPropertyValue("y", newY);
            shipDocument.SetPropertyValue("currentSpeed", newSpeed);
            shipDocument.SetPropertyValue("currentBearing", newBearing);

            await shipContainer.ReplaceDocumentAsync(shipDocument.SelfLink, shipDocument);

            return new OkResult();
        }
    }
}

