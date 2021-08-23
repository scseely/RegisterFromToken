using System;

using System.Web;
using Azure.Identity;
using Microsoft.Marketplace.SaaS.Models;

namespace RegisterFromToken
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 4)
            {
                Console.WriteLine("Expected 4 parameters: TenantID ClientId ClientSecret TokenUrl");
                return;
            }

            var tenantId = args[0];
            var clientId = args[1];
            var clientSecret = args[2];
            var tokenUrl = new Uri(args[3]);

            var cred = new ClientSecretCredential(tenantId, clientId, clientSecret);

            var tokens = HttpUtility.ParseQueryString(tokenUrl.Query);
            var token = tokens["token"];
            var client = new Microsoft.Marketplace.SaaS.MarketplaceSaaSClient(cred);
            var sub = client.Fulfillment.Resolve(token);
            Console.WriteLine($"Resolved token HTTP Status: {sub.GetRawResponse().Status}");
            var plan = new SubscriberPlan()
            {
                PlanId = sub.Value.PlanId,
                Quantity = sub.Value.Quantity
            };
            var result = client.Fulfillment.ActivateSubscription(sub.Value.Id.GetValueOrDefault(), plan);
            Console.WriteLine($"Activated HTTP Status: {result.Status}");

            Console.WriteLine($"Activated {sub.Value.SubscriptionName}");
        }
    }
}
