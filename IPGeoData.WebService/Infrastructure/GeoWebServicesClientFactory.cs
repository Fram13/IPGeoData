using MaxMind.GeoIP2;

namespace IPGeoData.WebService.Infrastructure
{
    public class GeoWebServicesClientFactory : IGeoWebServicesClientFactory
    {
        private readonly int _accountId;
        private readonly string _licenseKey;

        public GeoWebServicesClientFactory(int accountId, string licenseKey)
        {
            _accountId = accountId;
            _licenseKey = licenseKey;
        }

        public WebServiceClient CreateClient()
        {
            return new WebServiceClient(_accountId, _licenseKey);
        }
    }
}
