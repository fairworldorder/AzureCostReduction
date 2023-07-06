using AzurePricing.Models;
using System.Threading.Tasks;

namespace AzurePricing
{
    public interface IAzurePricingClient
    {
        Task<AzurePricingResponse> GetAppServicePlanPricing();

        Task<AzurePricingResponse> GetServiceBusPricing();
    }
}