using Azure.Identity;
using AzureCostReduction.Console.CostingModules;
using AzureCostReduction.Console.Services;
using AzurePricing;

namespace Workshop
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var csvWriter = new CsvWriterService();
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
            csvWriter.Write("appserviceplans.csv", emptyAppservicePlans);

            var emptyServiceBusModule = new ServiceBusModule(azurePricingClient);
            var emptyServiceBusNamespaces = await emptyServiceBusModule.GetEmptyServiceBusNamespaces(subscriptions);
            csvWriter.Write("servicebus.csv", emptyServiceBusNamespaces);

            Console.WriteLine("Completed");
        }
    }
}