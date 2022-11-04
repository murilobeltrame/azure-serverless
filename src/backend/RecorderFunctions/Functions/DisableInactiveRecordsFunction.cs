using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace RecorderFunctions.Functions
{
    public class DisableInactiveRecordsFunction
    {
        [FunctionName("DisableInactiveRecordsFunction")]
        public async Task Run(
            [TimerTrigger("0 * * * * *")] TimerInfo myTimer,
            [CosmosDB(
                databaseName: "Records",
                collectionName: "People",
                ConnectionStringSetting = "CosmosDbConnection"
            )] DocumentClient client,
            ILogger log)
        {
            log.LogInformation($"Disabling old people now, at: {DateTime.Now}");

            var collectionUri = UriFactory.CreateDocumentCollectionUri("Records", "People");
            var query = client.CreateDocumentQuery<Person>(collectionUri, new FeedOptions { EnableCrossPartitionQuery = true })
                .Where(person => person.CreatedAt < DateTime.Now.Subtract(TimeSpan.FromMinutes(5)) && person.Active == true)
                .AsDocumentQuery();

            while (query.HasMoreResults)
            {
                foreach (var person in await query.ExecuteNextAsync<Person>())
                {
                    log.LogInformation($"Disabling {person.Name} with id {person.id}");
                    person.Active = false;
                    await client.UpsertDocumentAsync(collectionUri, person);
                }
            }
        }
    }
}
