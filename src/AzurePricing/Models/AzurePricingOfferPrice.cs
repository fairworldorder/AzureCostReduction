using Newtonsoft.Json;

namespace AzurePricing.Models
{
    public class AzurePricingOfferPrice
    {
        [JsonProperty("value")]
        public double Value { get; set; }
    }
}
