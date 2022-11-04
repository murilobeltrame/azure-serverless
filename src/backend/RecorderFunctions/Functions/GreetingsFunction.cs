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
            log.LogInformation("Greeting for recently changed people.");

            var person = people.Last();
            var message = person.Active ?
                 $"Wellcome aboard {person.Name}. U'r now recorded." :
                 $"We are missing you {person.Name}";
            return new CreateMessageOptions(new PhoneNumber(person.Phone))
            {
                Body = message
            };
        }
    }
}

