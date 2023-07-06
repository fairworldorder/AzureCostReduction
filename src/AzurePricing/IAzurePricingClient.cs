using AzurePricing.Models;
using System.Threading.Tasks;

namespace AzurePricing
{
    public interface IAzurePricingClient
    {
        Task<AzurePricingResponse> GetAppServicePlanPricingAsync();

        Task<AzurePricingResponse> GetServiceBusPricingAsync();
    }
}