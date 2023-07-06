using Azure.ResourceManager.AppService;
using Azure.ResourceManager.Resources;
using AzurePricing;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.Models;

namespace AzureCostReduction.Console.CostingModules
{
    public class AppServiceModule
    {
        private IAzurePricingClient _pricingClient;

        public AppServiceModule(IAzurePricingClient pricingClient)
        {
            _pricingClient = pricingClient;
        }

        public async Task<IEnumerable<EmptyAppServicePlan>> GetEmptyAppServicePlans(SubscriptionCollection subscriptions)
        {
            var appServicePricing = await _pricingClient.GetAppServicePlanPricing();
            var emptyAppservicePlans = new List<EmptyAppServicePlan>();

            var tasks = new List<Task>();

            subscriptions.ToList().ForEach(sub =>
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

            return emptyAppservicePlans;

            //using (var writer = new StreamWriter("C:\\Temp\\file.csv"))
            //using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            //    csv.WriteRecords(emptyAppservicePlans);

            //Console.WriteLine("Completed");
        }
    }
}
