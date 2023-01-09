using System;
using System.Windows.Threading;

namespace Makement.Service
{
    public delegate void ServerSaveDelegate();

    public static class ServerSaverService
    {
        public static event ServerSaveDelegate save;

        public static void Begin()
        {
            var timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 10, 0);
            timer.Tick += TimerOnTick;
            timer.IsEnabled = true;
        }

        private static void TimerOnTick(object sender, EventArgs e)
        {
            save.Invoke();
        }
    }
}
