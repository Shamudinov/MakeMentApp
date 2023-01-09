using System.Drawing.Imaging;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Threading;
using System;
using System.Reflection;
using System.IO;

namespace Makement.Service
{
    public static class ScreenCaptureService
    {
        private static DispatcherTimer timer;
        private static DateTime CaptureTime;
        private static int maxMinute = 10;
        private static Random random = new Random();
        public static void Begin()
        {
            CaptureTime = DateTime.Now.AddMinutes(1);

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += (s, e) => { Capture(); };
            timer.Start();
        }

        public static void Stop()
        {
            timer.Stop();
        }

        private static bool IsEqualDateTime(DateTime a, DateTime b)
        {
            return (a.Year == b.Year && a.Month == b.Month && a.Day == b.Day && a.Hour == b.Hour && a.Minute == b.Minute);
        }

        public static void Capture()
        {
            if (!App.IsTrackScreenShot)
                return;

            var currentTime = DateTime.Now;

            if (!IsEqualDateTime(currentTime, CaptureTime))
                return;

            CaptureTime = CaptureTime.AddMinutes(random.Next(maxMinute));

            int width = Screen.PrimaryScreen.WorkingArea.Width;
            int height = Screen.PrimaryScreen.WorkingArea.Height;

            Bitmap captureBitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Rectangle captureRectangle = Screen.AllScreens[0].Bounds;
            Graphics captureGraphics = Graphics.FromImage(captureBitmap);

            captureGraphics.CopyFromScreen(captureRectangle.Left, captureRectangle.Top, 0, 0, captureRectangle.Size);

            var now = DateTime.Now;
            var name = $".{now.Year}.{now.Month}.{now.Day}.{now.Hour}.{now.Minute}.{App.User.Id}.sst";
            var directory = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName + "/" + name;

            captureBitmap = new Bitmap(captureBitmap, new Size(500, 281));

            captureBitmap.Save(directory, ImageFormat.Jpeg);
            File.SetAttributes(directory, FileAttributes.Hidden);
        }
    }
}
