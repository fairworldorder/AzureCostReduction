using Azure.Identity;
using Azure.ResourceManager.AppService;
using AzurePricing;
using CsvHelper;
using System.Globalization;
using Workshop.Models;

namespace Workshop
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var azurePricingClient = new AzurePricingClient();
            var appServicePricing = await azurePricingClient.GetAppServicePlanPricing();

            var ibcredential = new InteractiveBrowserCredential();
            var authenticated = ibcredential.Authenticate();

            var armClient = new Azure.ResourceManager.ArmClient(ibcredential);
            var subscriptions = armClient.GetSubscriptions().ToList();

            Console.WriteLine("Scanned the following subscriptions:");
            foreach (var subscription in subscriptions)
            {
                Console.WriteLine(subscription.Data.DisplayName);
            }

            var emptyAppservicePlans = new List<EmptyAppServicePlan>();

            var tasks = new List<Task>();

            subscriptions.ForEach(sub =>
            {
                var t = Task.Factory.StartNew(async () =>
                {
                    var plans = sub.GetAppServicePlans();
                    var empty = plans.Where(x => x.GetWebApps().ToArray().Length == 0).ToArray();
                    foreach (var p in empty)
                    {
                        emptyAppservicePlans.Add(new EmptyAppServicePlan
                        {
                            Name = p.Data.Name,
                            Sku = p.Data.Sku.Name,
                            Location = p.Data.Location,
                            Subscription = sub.Data.DisplayName,
                            MonthlyCostUsd = appServicePricing.GetMonthlyAzurePriceForOffer(p.Data.Sku.Name,
                                                                                            p.Data.Location.DisplayName)
                        });
                    }
                });

                tasks.Add(t);
            });

            await Task.WhenAll(tasks);

            using (var writer = new StreamWriter("C:\\Temp\\file.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                csv.WriteRecords(emptyAppservicePlans);

            Console.WriteLine("Completed");
            //foreach (var e in emptyAppservicePlans)
            //{
            //    Console.WriteLine($"{e.Subscription} {e.Name} {e.Location} {e.MonthlyCostUsd} {e.MonthlyCostAud}");
            //}
        }
    }
}