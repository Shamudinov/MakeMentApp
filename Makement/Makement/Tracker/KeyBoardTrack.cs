using System.Runtime.InteropServices;

namespace Makement.Tracker
{
    public struct LastInputInfo
    {
        public uint cbSize;
        public uint dwTime;
    }

    public static class KeyBoardTrack
    {
        [DllImport("User32.dll")]
        private static extern bool GetLastInputInfo(ref LastInputInfo inputInfo);

        public static LastInputInfo GetLastInput()
        {
            var lastInputInfo = new LastInputInfo();
            lastInputInfo.cbSize = (uint)Marshal.SizeOf(lastInputInfo);

            GetLastInputInfo(ref lastInputInfo);

            return lastInputInfo;
        }
    }
}
