using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace RecorderFunctions
{
    public static class CreateRecordFunction
    {
        [FunctionName("CreateRecordFunction")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [CosmosDB(databaseName: "Records", collectionName: "People", ConnectionStringSetting = "")] out dynamic document,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            document = JsonConvert.DeserializeObject(requestBody);

            return new OkObjectResult(document);
        }
    }
}
