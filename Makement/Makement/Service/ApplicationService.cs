using Makement.Tracker;
using Newtonsoft.Json.Linq;

namespace Makement.Service
{
    public static class ApplicationService
    {
        public static string GetVersion()
        {
            if (!InternetTrack.IsConnectedToInternet())
                return null;

            var response = App.HttpClient.GetAsync(App.BaseUrl + "Application/Get?type=0").Result;

            if (response.IsSuccessStatusCode)
            {
                var json = response.Content.ReadAsStringAsync().Result;
                JObject jObject = JObject.Parse(json);

                return jObject["version"].ToString();
            }

            return "";
        }
    }
}
