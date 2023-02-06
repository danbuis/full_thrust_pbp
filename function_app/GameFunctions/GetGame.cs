using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace FT_Functions.ShipFunctions;

public class GetGame
{
    private readonly ILogger<GetShip> _logger;

    public GetGame(ILogger<GetShip> log)
    {
        _logger = log;
    }

    [FunctionName("GetGame")]
    [OpenApiOperation(operationId: "Run", tags: new[] { "Get Game" })]
    [OpenApiParameter(name: "id", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **id** parameter")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]
        HttpRequest req,
        [CosmosDB(
            databaseName: "FullThrust",
            collectionName: "Games",
            Id = "{Query.id}",
            PartitionKey = "{Query.id}",
            ConnectionStringSetting = "CosmosConnection")] Document document
        )
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        string id = req.Query["id"];

        if (document == null || string.IsNullOrEmpty(id))
        {
            return new BadRequestResult();
        }

        return new OkObjectResult(document);
    }
}

