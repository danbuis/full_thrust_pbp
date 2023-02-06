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
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace FT_Functions.GameFunctions;

public class AddPlayer
{
    private readonly ILogger<AddPlayer> _logger;

    public AddPlayer(ILogger<AddPlayer> log)
    {
        _logger = log;
    }
    [FunctionName("AddPlayer")]
    [OpenApiOperation(operationId: "Run", tags: new[] { "Add player" })]
    [OpenApiParameter(name: "id", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Game ID** parameter")]
    [OpenApiParameter(name: "player", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Player** parameter")]
    [OpenApiParameter(name: "password", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Password** parameter")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
    public async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = null)]
    HttpRequest httpRequest,
    [CosmosDB(
        databaseName: "FullThrust",
        collectionName: "Games",
        Id = "{Query.id}",
        PartitionKey = "{Query.id}",
        ConnectionStringSetting = "CosmosConnection")] Document document,
    [CosmosDB(
        databaseName: "FullThrust",
        collectionName: "Games",
        ConnectionStringSetting = "CosmosConnection")] DocumentClient client
    )
    {
        _logger.LogInformation($"C# HTTP trigger function processed a request. Function name: {nameof(AddPlayer)}");

        string id = httpRequest.Query["id"];

        if (document == null || string.IsNullOrEmpty(id))
        {
            return new BadRequestResult();
        }

        string player = httpRequest.Query["player"];
        string password = httpRequest.Query["password"];

        Dictionary<string,string> currentPlayers = document.GetPropertyValue<Dictionary<string,string>>("players");

        currentPlayers.Add(player, password);

        document.SetPropertyValue("players", currentPlayers);

        await client.ReplaceDocumentAsync(document.SelfLink, document);

        return new OkResult();
    }
}

