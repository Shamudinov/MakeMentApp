using System;
using System.Windows.Threading;

namespace Makement.Service
{
    public delegate void LocalSaverDelegate();
    public static class LocalSaverService
    {
        public static event LocalSaverDelegate save;

        public static void Begin()
        {
            var timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 5, 0);
            timer.Tick += TimerOnTick;
            timer.IsEnabled = true;
        }

        private static void TimerOnTick(object sender, EventArgs e)
        {
            save.Invoke();
        }
    }
}
