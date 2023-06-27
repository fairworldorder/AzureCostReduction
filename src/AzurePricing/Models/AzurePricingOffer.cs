using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AzurePricing.Models
{
    public class AzurePricingOffer
    {
        [JsonProperty("cores")]
        public int Cores { get; set; }

        [JsonProperty("diskSpace")]
        public int DiskSpace { get; set; }

        [JsonProperty("prices")]
        public Dictionary<string, AzurePricingOfferPrice> Prices { get; set; } = new Dictionary<string, AzurePricingOfferPrice>();

        public double? GetMonthlyAzurePriceForRegion(string regionName)
        {
            var normalisedRegionName = regionName.ToLower().Replace(" ", "-");
            if (Prices.ContainsKey(normalisedRegionName))
            {
                return Math.Round(Prices[normalisedRegionName].Value * 730, 2);
            }
            return null;
        }
    }
}
