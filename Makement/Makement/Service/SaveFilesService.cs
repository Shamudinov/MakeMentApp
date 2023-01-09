using System;
using System.Net.Http;
using System.Text;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Makement.Tracker;

namespace Makement.Service
{
    public static class SaveFilesService
    {
        private static string GetDate(string fileName)
        {
            var segment = fileName.Split('.');
            var day = (segment[segment.Length - 3]);
            var month = (segment[segment.Length - 4]);
            var year = (segment[segment.Length - 5]);

            if (day.Length < 2)
                day = "0" + day;
            if (month.Length < 2)
                month = "0" + month;

            return year + "-" + month + "-" + day + "T00:00:00.000Z";
        }
        private static string GetDateTime(string fileName)
        {
            var segment = fileName.Split('.');
            var minute = (segment[segment.Length - 3]);
            var hour = (segment[segment.Length - 4]);
            var day = (segment[segment.Length - 5]);
            var month = (segment[segment.Length - 6]);
            var year = (segment[segment.Length - 7]);

            if (minute.Length < 2)
                minute = "0" + minute;
            if (hour.Length < 2)
                hour = "0" + hour;
            if (day.Length < 2)
                day = "0" + day;
            if (month.Length < 2)
                month = "0" + month;

            return $"{year}-{month}-{day}T{hour}:{minute}:00.000Z";
        }

        private static string GetUserId(string file)
        {
            var segment = file.Split('.');
            return segment[segment.Length - 2];
        }
        private static string ReadFile(string path)
        {
            string file;

            using (StreamReader writer = new StreamReader(path))
            {
                file = writer.ReadToEnd();
            }

            return file;
        }
        private static void SendActivity()
        {
            var directory = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;
            var files = Directory.GetFiles(directory, "*.act");

            foreach (var path in files)
            {
                var date = GetDate(path);
                var userId = GetUserId(path);
                File.SetAttributes(path, FileAttributes.Normal);
                string file = ReadFile(path);

                var data = new
                {
                    userId = userId,
                    file = file,
                    date = date
                };

                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = App.HttpClient.PostAsync(App.BaseUrl + "Track/SaveActivity", content).Result;

                if (response.IsSuccessStatusCode) { File.Delete(path); }
                else { File.SetAttributes(path, FileAttributes.Hidden); }
            }
        }
        private static void SendAppInfo()
        {
            var directory = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;
            var files = Directory.GetFiles(directory, "*.apu");

            foreach (var path in files)
            {
                var date = GetDate(path);
                var userId = GetUserId(path);
                File.SetAttributes(path, FileAttributes.Normal);
                string file = ReadFile(path);

                var data = new
                {
                    userId = userId,
                    file = file,
                    date = date
                };

                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = App.HttpClient.PostAsync(App.BaseUrl + "Track/SaveAppInfo", content).Result;

                if (response.IsSuccessStatusCode) { File.Delete(path); }
                else { File.SetAttributes(path, FileAttributes.Hidden); }
            }
        }
        private static void SendScreenShot()
        {
            var directory = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;
            var files = Directory.GetFiles(directory, "*.sst");

            foreach (var path in files)
            {
                var dateTime = GetDateTime(path);
                var userId = GetUserId(path);
                File.SetAttributes(path, FileAttributes.Normal);
                byte[] file = File.ReadAllBytes(path);

                var data = new
                {
                    userId = userId,
                    file = Convert.ToBase64String(file),
                    dateTime = dateTime
                };

                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = App.HttpClient.PostAsync(App.BaseUrl + "Track/SaveScreenShot", content).Result;

                if (response.IsSuccessStatusCode) { File.Delete(path); }
                else { File.SetAttributes(path, FileAttributes.Hidden); }
            }
        }
        public static void Send()
        {
            if (!InternetTrack.IsConnectedToInternet())
                return;

            SendActivity();
            SendAppInfo();
            SendScreenShot();
        }
    }
}
