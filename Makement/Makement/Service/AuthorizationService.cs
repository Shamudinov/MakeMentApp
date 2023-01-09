using Makement.Model;
using Makement.Tracker;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Windows.Threading;

namespace Makement.Service
{
    public static class AuthorizationService
    {
        public static string Token { get; private set; }

        public static void InitOption()
        {
            var timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 10);
            timer.Tick += (s, e) => { GetOption(); };
            timer.Start();
        }

        public static void GetOption()
        {
            if (!InternetTrack.IsConnectedToInternet())
                return;

            var response = App.HttpClient.GetAsync(App.BaseUrl + "Organization/GetCompanyOption").Result;
            var json = response.Content.ReadAsStringAsync().Result;

            JObject jObject = JObject.Parse(json);
            App.IsTrackActivity = (bool)jObject["isTrackActivity"];
            App.IsTrackAppUsage = (bool)jObject["isTrackAppUsage"];
            App.IsTrackScreenShot = (bool)jObject["isTrackScreenShot"];
        }

        public static bool Authorize(string email, string password)
        {
            if (!InternetTrack.IsConnectedToInternet())
                return false;

            var data = "{ \"email\": \"" + email + "\", \"password\": \"" + password + "\" }";

            var content = new StringContent(data.ToString(), Encoding.UTF8, "application/json");
            var response = App.HttpClient.PostAsync(App.BaseUrl + "user/login", content).Result;

            if (response.IsSuccessStatusCode)
            {
                var json = response.Content.ReadAsStringAsync().Result;

                JObject jObject = JObject.Parse(json);
                Token = jObject["token"].ToString();

                FileService.Set("token", email + " " + password);

                App.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                return true;
            }

            return false;
        }

        public static User GetCurrent()
        {
            if (!InternetTrack.IsConnectedToInternet())
                return null;

            var response = App.HttpClient.GetAsync(App.BaseUrl + "User/GetCurrent").Result;

            if (response.IsSuccessStatusCode)
            {
                var json = response.Content.ReadAsStringAsync().Result;
                JObject jObject = JObject.Parse(json);
                var user = new User();

                user.Id = jObject["id"].ToString();
                user.Email = jObject["email"].ToString();
                user.FirstName = jObject["firstName"].ToString();
                user.SecondName = jObject["secondName"].ToString();

                return user;
            }

            return null;
        }
    }
}
