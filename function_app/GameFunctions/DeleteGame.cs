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

namespace FT_Functions.GameFunctions
{
    public class DeleteGame
    {
        private readonly ILogger<DeleteGame> _logger;

        public DeleteGame(ILogger<DeleteGame> log)
        {
            _logger = log;
        }

        [FunctionName("DeleteGame")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Delete Game" })]
        [OpenApiParameter(name: "id", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **ID** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = null)]
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
            _logger.LogInformation($"C# HTTP trigger function processed a request. Function name: {nameof(DeleteGame)}");

            string id = httpRequest.Query["id"];

            if (document == null || string.IsNullOrEmpty(id))
            {
                return new BadRequestResult();
            }

            await client.DeleteDocumentAsync(document.SelfLink, new RequestOptions() { PartitionKey = new PartitionKey(id) });

            return new OkResult();
        }
    }
}

