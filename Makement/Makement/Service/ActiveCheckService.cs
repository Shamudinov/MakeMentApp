using Makement.Tracker;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Threading;

namespace Makement.Service
{
    delegate void ActionDelegate();
    public static class ActiveCheckService
    {
        private static int ActiveSecond;
        private static int AbsenceSecond;
        private static bool Ongoing;
        private static DispatcherTimer timer;
        private static Point lastPoint;
        private static LastInputInfo lastInput;

        static ActiveCheckService()
        {
            ActiveSecond = 0;
            AbsenceSecond = 0;
        }
        public static void Begin()
        {
            Ongoing = true;
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += (s, e) => { Handle(); };
            lastPoint = MouseTrack.GetCursorPosition();
            lastInput = KeyBoardTrack.GetLastInput();

            timer.Start();
        }

        public static bool IsMouseActive()
        {
            Point currentPoint = MouseTrack.GetCursorPosition();

            if (lastPoint.X != currentPoint.X || lastPoint.Y != currentPoint.Y)
            {
                lastPoint = currentPoint;
                return true;
            }

            return false;
        }

        public static bool IsKeyBoardActive()
        {
            LastInputInfo CurrentInput = KeyBoardTrack.GetLastInput();

            if (CurrentInput.dwTime != lastInput.dwTime)
            {
                lastInput = CurrentInput;
                return true;
            }

            return false;
        }

        public static void Handle()
        {
            if (!Ongoing || !App.IsTrackActivity)
                return;

            if (IsKeyBoardActive() || IsMouseActive())
            {
                ActiveSecond++;
            }
            else
            {
                AbsenceSecond++;
            }
        }
        public static void Stop()
        {
            timer.Stop();
            Ongoing = false;
        }
        public static void SaveLocal()
        {
            string date = DateTime.Now.Year.ToString() + "." + DateTime.Now.Month.ToString() + "." + DateTime.Now.Day.ToString();
            string filename = date + "." + App.User.Id;
            var directory = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName + "/." + filename + ".act";

            if (File.Exists(directory))
            {
                File.SetAttributes(directory, FileAttributes.Normal);
            }
            using (StreamWriter writer = new StreamWriter(directory))
            {
                string row = ActiveSecond.ToString();
                string cryptrow = CryptService.EncryptString(row);
                writer.WriteLine(cryptrow);
                row = AbsenceSecond.ToString();
                cryptrow = CryptService.EncryptString(row);
                writer.WriteLine(cryptrow);
            }
            File.SetAttributes(directory, FileAttributes.Hidden);
        }
        public static void End()
        {
            Stop();
        }
    }
}
