using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using FT_Functions.Models;
using FT_Functions.ShipFunctions;
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

namespace FT_Functions.GameFunctions
{
    public class PlayerSignoff
    {
        private readonly ILogger<PlayerSignoff> _logger;

        public PlayerSignoff(ILogger<PlayerSignoff> log)
        {
            _logger = log;
        }

        [FunctionName("PlayerSignoff")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiParameter(name: "id", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **id** parameter")]
        [OpenApiParameter(name: "player", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **player** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = null)] 
                HttpRequest req,
            [CosmosDB(
                databaseName: "FullThrust",
                collectionName: "Games",
                Id = "{Query.id}",
                PartitionKey = "{Query.id}",
                ConnectionStringSetting = "CosmosConnection")]
            Document gameDocument,
            [CosmosDB(
                databaseName: "FullThrust",
                collectionName: "Games",
                ConnectionStringSetting = "CosmosConnection")] DocumentClient gameContainer)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string player = req.Query["player"];

            List<string> signoffList = gameDocument.GetPropertyValue<List<string>>("playerSignoffs");

            if (!signoffList.Contains(player))
            {
                signoffList.Add(player);
                gameDocument.SetPropertyValue("playerSignoffs", signoffList);
            }
            await gameContainer.ReplaceDocumentAsync(gameDocument.SelfLink, gameDocument);
        
            return new OkObjectResult(gameDocument);
        }
    }
}

