using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System;

namespace RecorderFunctions
{
    public static class CreateRecordFunction
    {
        [FunctionName(nameof(CreateRecordFunction))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(
                AuthorizationLevel.Function,
                "post",
                Route = null
            )] HttpRequest req,
            [CosmosDB(
                databaseName: "Records",
                collectionName: "People",
                ConnectionStringSetting = "CosmosDbConnection"
            )] IAsyncCollector<Person> people,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                string requestBody = new StreamReader(req.Body).ReadToEnd();
                var person = JsonConvert.DeserializeObject<Person>(requestBody);
                await people.AddAsync(person);
                return new OkObjectResult(person);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "An error has ocurred");
                return new ObjectResult("An error has occurred") { StatusCode = 500 };
            }
        }
    }
}
