using MaxMind.GeoIP2;

namespace IPGeoData.WebService.Infrastructure
{
    public interface IGeoWebServicesClientFactory
    {
        WebServiceClient CreateClient();
    }
}
