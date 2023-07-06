namespace Workshop.Models
{
    public class UnusedAzureResource
    {
        public string Name { get; set; }

        public string Sku { get; set; }

        public string Location { get; set; }

        public string Subscription { get; set; }

        public Double? MonthlyCostUsd { get; set; }

        public Double? MonthlyCostAud { get => MonthlyCostUsd != null ? MonthlyCostUsd * 1.5 : null; }
    }
}