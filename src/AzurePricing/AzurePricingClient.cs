using AzurePricing.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AzurePricing
{
    public class AzurePricingClient
    {
        private HttpClient _client;

        public AzurePricingClient(HttpClient client = null)
        {
            _client = client == null ? new HttpClient() : client;
            _client.BaseAddress = new Uri("https://azure.microsoft.com/api/v2/pricing/app-service/calculator/");
        }

        public async Task<AzurePricingResponse> GetAppServicePlanPricing()
        {
            var today = DateTime.Today.ToString("yyyyMMdd");
            var route = $"?culture=en-au&discount=mca&billingAccount=&billingProfile=&v={today}";
            var request = new HttpRequestMessage(HttpMethod.Get, route);
            var response = await _client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            var azurePricingOffer = JsonConvert.DeserializeObject<AzurePricingResponse>(content);
            return azurePricingOffer;
        }
    }
}