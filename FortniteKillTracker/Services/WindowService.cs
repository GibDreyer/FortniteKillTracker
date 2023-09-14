using System.Management;
using System.Runtime.InteropServices;

public partial class WindowService
{
    public const int SW_HIDE = 0;
    public const int SW_SHOW = 5;

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetWindowRect(IntPtr hWnd, out WindowRECT lpRect);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetClientRect(IntPtr hWnd, out WindowRECT lpRect);

    public static (double x, double y) GetDPIScale(IntPtr hwnd)
    {
        var (x, y) = GetWindowDPI(hwnd);

        try
        {
            // Query the WMI for monitor DPI
            using ManagementObjectSearcher searcher = new("SELECT * FROM Win32_DesktopMonitor");
            
            foreach ((int dpiX, int dpiY) in from ManagementObject mo in searcher.Get().Cast<ManagementObject>()
                                             let dpiX = Convert.ToInt32(mo["PixelsPerXLogicalInch"])
                                             let dpiY = Convert.ToInt32(mo["PixelsPerYLogicalInch"])
                                             where dpiX > 0 && dpiY > 0
                                             select (dpiX, dpiY))
            {
                return (dpiX / x, dpiY / y);
            }
        }
        catch { }

        return (1, 1);
    }

    private static (double x, double y) GetWindowDPI(IntPtr hwnd)
    {
        IntPtr monitor = MonitorFromWindow(hwnd, 2); // MONITOR_DEFAULTTONEAREST
        if (monitor != IntPtr.Zero)
        {
            if (GetDpiForMonitor(monitor, 0, out uint dpiX, out uint dpiY) == 0)
            {
                return (dpiX, dpiY);
            }
        }
        return (96, 96);
    }

    [LibraryImport("user32.dll")]
    private static partial IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

    [LibraryImport("shcore.dll")]
    private static partial int GetDpiForMonitor(IntPtr hmonitor, int dpiType, out uint dpiX, out uint dpiY);
}