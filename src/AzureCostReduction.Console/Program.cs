using Azure.Identity;
using AzureCostReduction.Console.CostingModules;
using AzurePricing;
using CsvHelper;
using System.Globalization;

namespace Workshop
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var azurePricingClient = new AzurePricingClient();

            var ibcredential = new InteractiveBrowserCredential();
            ibcredential.Authenticate();

            var armClient = new Azure.ResourceManager.ArmClient(ibcredential);
            var subscriptions = armClient.GetSubscriptions();

            Console.WriteLine("Scanned the following subscriptions:");
            foreach (var subscription in subscriptions)
            {
                Console.WriteLine(subscription.Data.DisplayName);
            }

            var appServiceModule = new AppServiceModule(azurePricingClient);
            var emptyAppservicePlans = (await appServiceModule.GetEmptyAppServicePlans(subscriptions)).ToList();

            using (var writer = new StreamWriter("C:\\Temp\\file.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                csv.WriteRecords(emptyAppservicePlans);

            Console.WriteLine("Completed");
        }
    }
}