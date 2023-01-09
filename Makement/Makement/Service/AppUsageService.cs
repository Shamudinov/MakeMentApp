using System.Runtime.InteropServices;
using System.IO;
using System;
using System.Text;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Reflection;

namespace Makement.Service
{

    public static class AppUsageService
    {
        public static Dictionary<string, int> AppSeconds { get; private set; }
        public static Label Text { get; private set; }
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
        public static string GetActiveWindowsTXT()
        {
            try
            {
                int chars = 256;
                StringBuilder buff = new StringBuilder(chars);
                IntPtr handle = GetForegroundWindow();
                string RET = "";
                if (handle != null)
                {
                    if (GetWindowText(handle, buff, chars) > 0)
                    {
                        RET = buff.ToString();
                        //listBox2.Items.Add(handle.ToString());                   
                    }
                }

                return RET;
            }
            catch
            {
                return "NO Windows Active";
            }
        }
        public static DispatcherTimer timer;
        public static void Begin()
        {
            if (AppSeconds == null)
            {
                AppSeconds = new Dictionary<string, int>();
            }
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 1, 0);
            timer.Tick += TimerOnTick;
            timer.IsEnabled = true;
        }
        private static void TimerOnTick(object sender, EventArgs e)
        {
            if (!App.IsTrackAppUsage)
                return;

            string title = GetActiveWindowsTXT();

            if (AppSeconds.ContainsKey(title))
                AppSeconds[title]++;
            else
                AppSeconds.Add(title, 1);
        }
        public static void SaveLocal()
        {
            if (AppSeconds == null)
                return;

            string date = DateTime.Now.Year.ToString() + "." + DateTime.Now.Month.ToString() + "." + DateTime.Now.Day.ToString();
            string filename = date + "." + App.User.Id;
            var directory = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName + "/." + filename + ".apu";

            if (File.Exists(directory))
            {
                File.SetAttributes(directory, FileAttributes.Normal);
            }
            using (StreamWriter writer = new StreamWriter(directory, false, Encoding.UTF8))
            {
                foreach (var item in AppSeconds)
                {
                    string row = item.Key + "@" + item.Value.ToString();
                    string cryptrow = CryptService.EncryptString(row);
                    writer.WriteLine(cryptrow);
                }
            }
            File.SetAttributes(directory, FileAttributes.Hidden);
        }
        public static void Stop()
        {
            timer.IsEnabled = false;
        }
    }
}
