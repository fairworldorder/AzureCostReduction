using AzurePricing.Models;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AzurePricing
{
    public class AzurePricingClient : IAzurePricingClient
    {
        private HttpClient _client;

        public AzurePricingClient(HttpClient client = null)
        {
            _client = client == null ? new HttpClient() : client;
            _client.BaseAddress = new Uri("https://azure.microsoft.com/api/v2/pricing/");
        }

        public async Task<AzurePricingResponse> GetAppServicePlanPricing()
        {
            var today = DateTime.Today.ToString("yyyyMMdd");
            var route = $"app-service/calculator/?culture=en-au&discount=mca&billingAccount=&billingProfile=&v={today}";
            var request = new HttpRequestMessage(HttpMethod.Get, route);
            var response = await _client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            var azurePricingOffer = JsonConvert.DeserializeObject<AzurePricingResponse>(content);
            return azurePricingOffer;
        }

        public async Task<AzurePricingResponse> GetServiceBusPricing()
        {
            var today = DateTime.Today.ToString("yyyyMMdd");
            var route = $"service-bus/calculator/?culture=en-au&discount=mca&billingAccount=&billingProfile=&v={today}";
            var request = new HttpRequestMessage(HttpMethod.Get, route);
            var response = await _client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            var azurePricingOffer = JsonConvert.DeserializeObject<AzurePricingResponse>(content);

            var zzz = azurePricingOffer.Offers.Where(x => x.Key.StartsWith("messages"));
            
            // filter out hybrid-connections
            azurePricingOffer.Offers = azurePricingOffer.Offers.Where(x => x.Key.StartsWith("messages"))
                                                               .ToDictionary(x => x.Key,
                                                                             x => x.Value);
            return azurePricingOffer;
        }
    }
}