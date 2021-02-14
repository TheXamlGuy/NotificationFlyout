using Microsoft.Windows.Sdk;
using System;
using System.Runtime.InteropServices;
using Windows.Foundation;

namespace NotificationFlyout.Shared.UI.Helpers
{
    public class Screen
    {
        private const int CCHDEVICENAME = 32;
        private const int PRIMARY_MONITOR = unchecked((int)0xBAADF00D);
        private const int SM_CMONITORS = 80;
        private static readonly bool _multiMonitorSupport;

        private readonly IntPtr _monitorHandle;

        static Screen()
        {
            _multiMonitorSupport = PInvoke.GetSystemMetrics(SM_CMONITORS) != 0;
        }

        internal Screen(IntPtr monitorHandle)
        {
            if (!_multiMonitorSupport || monitorHandle == (IntPtr)PRIMARY_MONITOR)
            {
                Bounds = SystemInformationHelper.VirtualScreen;
                Primary = true;
                DeviceName = "DISPLAY";
            }
            else
            {
                var monitorData = GetMonitorData(monitorHandle);

                Bounds = new Rect(monitorData.MonitorRect.left, monitorData.MonitorRect.top, monitorData.MonitorRect.right - monitorData.MonitorRect.left, monitorData.MonitorRect.bottom - monitorData.MonitorRect.top);
                Primary = (monitorData.Flags & (int)MonitorFlag.MONITOR_DEFAULTTOPRIMARY) != 0;
                DeviceName = monitorData.DeviceName;
            }

            _monitorHandle = monitorHandle;
        }

        private enum MonitorFlag : uint
        {
            MONITOR_DEFAULTTONULL = 0,
            MONITOR_DEFAULTTOPRIMARY = 1,
            MONITOR_DEFAULTTONEAREST = 2
        }

        public Rect Bounds { get; }

        public string DeviceName { get; }

        public bool Primary { get; }

        public Rect WorkingArea => GetWorkingArea();

        public static Screen FromHandle(IntPtr handle)
        {
            return _multiMonitorSupport ? new Screen(PInvoke.MonitorFromWindow((HWND)handle, (uint)MonitorFlag.MONITOR_DEFAULTTONEAREST)) : new Screen((IntPtr)PRIMARY_MONITOR);
        }

        public override bool Equals(object obj)
        {
            if (obj is not Screen monitor) return false;
            return _monitorHandle == monitor._monitorHandle;
        }

        public override int GetHashCode()
        {
            return (int)_monitorHandle;
        }

        [DllImport("user32.dll", EntryPoint = "GetMonitorInfo", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool GetMonitorInfoEx(IntPtr hMonitor, ref MonitorData lpmi);

        private MonitorData GetMonitorData(IntPtr monitorHandle)
        {
            var monitorData = new MonitorData();
            monitorData.Size = Marshal.SizeOf(monitorData);
            GetMonitorInfoEx(monitorHandle, ref monitorData);

            return monitorData;
        }

        private Rect GetWorkingArea()
        {
            if (!_multiMonitorSupport || _monitorHandle == (IntPtr)PRIMARY_MONITOR)
            {
                return SystemInformationHelper.WorkingArea;
            }

            var monitorData = GetMonitorData(_monitorHandle);
            return new Rect(monitorData.WorkAreaRect.left, monitorData.WorkAreaRect.top, monitorData.WorkAreaRect.right - monitorData.WorkAreaRect.left, monitorData.WorkAreaRect.bottom - monitorData.WorkAreaRect.top);
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct MonitorData
        {
            public int Size;
            public RECT MonitorRect;
            public RECT WorkAreaRect;
            public uint Flags;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
            public string DeviceName;
        }
    }
}