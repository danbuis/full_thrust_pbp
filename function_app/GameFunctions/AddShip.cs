using System.IO;
using System.Net;
using System.Threading.Tasks;
using FT_Functions.Models;
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
using System.Linq;
using System;

namespace FT_Functions.GameFunctions;

public class AddShip
{
    private readonly ILogger<AddShip> _logger;

    public AddShip(ILogger<AddShip> log)
    {
        _logger = log;
    }

    [FunctionName("AddShip")]
    [OpenApiOperation(operationId: "Run", tags: new[] { "Add Ships" })]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
    [OpenApiParameter(name: "id", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **ID** parameter of the game")]
    [OpenApiParameter(name: "player", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Player** parameter")]
    [OpenApiParameter(name: "ship_name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Ship Name** parameter")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]
        HttpRequest req,
        [CosmosDB(
            databaseName: "FullThrust",
            collectionName: "Games",
            Id = "{Query.id}",
            PartitionKey = "{Query.id}",
            ConnectionStringSetting = "CosmosConnection")] Document gameDocument,
        [CosmosDB(
            databaseName: "FullThrust",
            collectionName: "Games",
            ConnectionStringSetting = "CosmosConnection")] DocumentClient gameContainer,
        [CosmosDB(
            databaseName: "FullThrust",
            collectionName: "Ships",
            ConnectionStringSetting = "CosmosConnection")
        ]
        IAsyncCollector<dynamic> ShipsDocumentsOut
        )
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        string id = req.Query["id"];
        string player = req.Query["player"];
        string newShip = req.Query["ship_names"];

        if (gameDocument == null || string.IsNullOrEmpty(id))
        {
            return new BadRequestResult();
        }

        Dictionary<string, List<string>> shipsDictionary = gameDocument.GetPropertyValue<Dictionary<string, List<string>>>("ships");

        if (!shipsDictionary.ContainsKey(player))
        {
            shipsDictionary.Add(player, new List<string>());
        }

        string new_id = Guid.NewGuid().ToString();
        shipsDictionary[player].Add(new_id);
        Ship shipToAdd = new()
        {
            Id = new_id,
            Name = newShip,
            Player = player,
            CurrentSpeed = 0,
            CurrentBearing = 0,
            X = 0,
            Y = 0,
            DateCreated = DateTime.Now
        };
        await ShipsDocumentsOut.AddAsync(shipToAdd);

        gameDocument.SetPropertyValue("ships", shipsDictionary);

        await gameContainer.ReplaceDocumentAsync(gameDocument.SelfLink, gameDocument);
        
        return new OkObjectResult(shipToAdd);
    }
}

