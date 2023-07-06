using Azure.ResourceManager.Resources;
using Azure.ResourceManager.ServiceBus;
using AzurePricing;
using Workshop.Models;

namespace AzureCostReduction.Console.CostingModules
{
    public class ServiceBusModule
    {
        private IAzurePricingClient _pricingClient;

        public ServiceBusModule(IAzurePricingClient pricingClient)
        {
            _pricingClient = pricingClient;
        }

        public async Task<IEnumerable<UnusedAzureResource>> GetEmptyServiceBusNamespaces(SubscriptionCollection subscriptions)
        {
            var serviceBusPricing = await _pricingClient.GetServiceBusPricingAsync();

            var tasks = new List<Task>();
            var emptyServiceBusNamespaces = new List<UnusedAzureResource>();

            subscriptions.ToList().ForEach(subscription =>
            {
                tasks.Add(Task.Factory.StartNew(async () =>
                {
                    var serviceBusNamespaces = subscription.GetServiceBusNamespaces().ToList();
                    foreach (var serviceBus in serviceBusNamespaces)
                    {
                        var topics = serviceBus.GetServiceBusTopics();
                        var queues = serviceBus.GetServiceBusQueues();

                        if (!topics.Any() && !queues.Any())
                        {
                            var offer = serviceBusPricing.Offers.Keys.FirstOrDefault(x => x.Contains(serviceBus.Data.Sku.Name.ToString().ToLower()));
                            var region = serviceBus.Data.Location.DisplayName.ToLower();
                            region = region.Replace(" ", "-");
                            //.Replace("", "-");
                            emptyServiceBusNamespaces.Add(new UnusedAzureResource
                            {
                                Name = serviceBus.Data.Name,
                                Location = serviceBus.Data.Location,
                                Sku = serviceBus.Data.Sku.Name.ToString(),
                                Subscription = subscription.Data.DisplayName,
                                MonthlyCostUsd = string.IsNullOrEmpty(offer) ? null : serviceBusPricing.Offers[offer].GetMonthlyAzurePriceForRegion(region)
                            });
                        }
                    }
                }));
            });

            await Task.WhenAll(tasks);

            return emptyServiceBusNamespaces;
        }
    }
}