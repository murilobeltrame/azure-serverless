using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace RecorderFunctions.Functions
{
    public class DisableInactiveRecordsFunction
    {
        [FunctionName("DisableInactiveRecordsFunction")]
        public async Task Run(
            [TimerTrigger("0 */5 * * * *")] TimerInfo myTimer,
            [CosmosDB(
                databaseName: "Records",
                collectionName: "People",
                ConnectionStringSetting = "CosmosDbConnection"
            )] DocumentClient client,
            ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var collectionUri = UriFactory.CreateDocumentCollectionUri("Records", "People");
            var query = client.CreateDocumentQuery<Person>(collectionUri)
                .Where(w => w.CreatedAt < DateTime.Now.Subtract(TimeSpan.FromMinutes(5)))
                .AsDocumentQuery();

            while (query.HasMoreResults)
            {
                foreach (var person in await query.ExecuteNextAsync())
                {
                    person.Active = false;
                }
            }
        }
    }
}

