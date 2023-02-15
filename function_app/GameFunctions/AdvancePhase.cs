using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Collections.Generic;
using System.Numerics;

namespace FT_Functions.GameFunctions
{
    public static class AdvancePhase
    {
        [FunctionName("AdvancePhase")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiParameter(name: "id", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **id** parameter")]
        [OpenApiParameter(name: "phase", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **phase** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]

        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put",Route = null)] HttpRequest req,
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
                ConnectionStringSetting = "CosmosConnection")] DocumentClient gameContainer,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string nextPhase = req.Query["phase"];

            gameDocument.SetPropertyValue("phase", nextPhase);
            gameDocument.SetPropertyValue("playerSignoffs", new List<string>());

            await gameContainer.ReplaceDocumentAsync(gameDocument.SelfLink, gameDocument);

            return new OkObjectResult(gameDocument);
        }
    }
}
