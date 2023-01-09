using Makement.Model;
using Makement.Views;
using System;
using System.Net.Http;
using System.Windows;
using System.Windows.Threading;

namespace Makement
{
    public partial class App : Application
    {
        public static DateTime BeginTime { get; set; }
        public static Int64 WorkSecond { get; set; }
        public static bool IsRunning { get; set; }
        public static bool IsTrackActivity { get; set; }
        public static bool IsTrackAppUsage { get; set; }
        public static bool IsTrackScreenShot { get; set; }
        public static User User { get; set; }
        public readonly static string key = "c12ca5898a4e4134bbce2ea3316a1916";
        public readonly static string BaseUrl = "https://localhost:44392/";
        public static HttpClient HttpClient { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            try {
                base.OnStartup(e);

                HttpClient = new HttpClient();
                MainWindow mainWindow = new MainWindow();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
