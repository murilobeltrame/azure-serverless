using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace RecorderFunctions.Functions
{
    public class GreetingsFunction
    {
        [FunctionName(nameof(GreetingsFunction))]
        [return: TwilioSms(AccountSidSetting = "TwillioAccountSid", AuthTokenSetting = "TwillioAuthToken", From = "+18657372425")]
        public CreateMessageOptions Run(
            [CosmosDBTrigger(
                databaseName: "Records",
                collectionName: "People",
                ConnectionStringSetting = "CosmosDbConnection",
                LeaseCollectionName = "leases",
                CreateLeaseCollectionIfNotExists = true
            )] IReadOnlyList<dynamic> people,
            ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var person = people.Last();
            string personJson = JsonConvert.SerializeObject(person);
            log.LogInformation(personJson);
            return new CreateMessageOptions(new PhoneNumber(person.Phone))
            {
                Body = $"Wellcome aboard {person.Name}. U`r now recorded."
            };
        }
    }
}

