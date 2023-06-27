using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace AzurePricing.Models
{
    public class AzurePricingResponse
    {
        [JsonProperty("offers")]
        public Dictionary<string, AzurePricingOffer> Offers { get; set; } = new Dictionary<string, AzurePricingOffer>();

        public double? GetMonthlyAzurePriceForOffer(string offerName, string regionName)
        {
            var normalisedRegionName = regionName.ToLower().Replace(" ", "-");
            var normalisedOfferName = $"{offerName}-payg".ToLower();

            var matchingKey = Offers.Keys.FirstOrDefault(x => x.Contains(normalisedOfferName) && x.Contains("windows"));
            if (!string.IsNullOrEmpty(matchingKey))
            {
                return Offers[matchingKey].GetMonthlyAzurePriceForRegion(regionName);
            }
            return null;
        }
    }
}