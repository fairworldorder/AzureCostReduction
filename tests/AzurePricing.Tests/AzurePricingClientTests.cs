namespace AzurePricing.Tests
{
    public class AzurePricingClientTests
    {
        private IAzurePricingClient _pricingClient;

        public AzurePricingClientTests()
        {
            _pricingClient = new AzurePricingClient();
        }

        [Fact]
        public async Task Test1()
        {
            var serviceBusPricing = await _pricingClient.GetServiceBusPricingAsync();
        }
    }
}