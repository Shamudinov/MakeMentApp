using System;
using System.Windows.Controls;
using System.Windows.Threading;


namespace Makement.Service
{
    public static class TimerService
    {
        public static DateTime BeginTime { get; private set; }
        public static Label TimerLabel { get; private set; }
        private static DispatcherTimer timer;

        public static void Begin(Label timerLabel)
        {
            TimerLabel = timerLabel;

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 1, 0);
            timer.Tick += TimerOnTick;
            timer.IsEnabled = true;
        }

        public static void Stop()
        {
            timer.IsEnabled = false;
        }
        public static void End()
        {
            TimerLabel.Content = "00:00:00";
            App.WorkSecond = 0;
            timer.IsEnabled = false;
        }
        private static void TimerOnTick(object sender, EventArgs e)
        {
            string hour = (App.WorkSecond / 3600).ToString();
            string minute = ((App.WorkSecond / 60) % 60).ToString(); ;
            string second = (App.WorkSecond % 60).ToString();
            App.WorkSecond++;

            if (hour.Length < 2)
                hour = "0" + hour;
            if (minute.Length < 2)
                minute = "0" + minute;
            if (second.Length < 2)
                second = "0" + second;


            TimerLabel.Content = hour + ":" + minute + ":" + second;
        }
    }
}
