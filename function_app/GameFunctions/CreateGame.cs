using System.Collections.Generic;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using FT_Functions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace FT_Functions.GameFunctions;

public class CreateGame
{
    private readonly ILogger<CreateGame> _logger;

    public CreateGame(ILogger<CreateGame> log)
    {
        _logger = log;
    }

    [FunctionName("CreateGame")]
    [OpenApiOperation(operationId: "Run", tags: new[] { "Create Game" })]
    [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]
        HttpRequest req,
        [CosmosDB(
            databaseName: "FullThrust",
            collectionName: "Games",
            ConnectionStringSetting = "CosmosConnection")
        ]
        IAsyncCollector<dynamic> documentsOut,
        ILogger log)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        string name = req.Query["name"];

        Game testGameToAdd = new()
        {
            // Create a random ID.
            Id = Guid.NewGuid().ToString(),
            Name = name,
            Players = new Dictionary<string, string>(),
            Ships = new Dictionary<string, List<string>>(),
            DateCreated = DateTime.Now
        };

        // Add a JSON document to the output container.
        await documentsOut.AddAsync(testGameToAdd);

        return new OkObjectResult(testGameToAdd);
    }
}


