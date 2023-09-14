using System.Drawing;
using FortniteKillTracker.Services;
using OpenCvSharp;

public class ScreenCaptureService
{
    private Mat _screenFrame;
    private readonly string _windowName = "Fortnite Kill Tracker";

    private int _windowLeft;
    private int _windowTop;
    private int _windowWidth;
    private int _windowHeight;

    private ImageProcessorService _imageProcessorService;
    public ScreenCaptureService(ImageProcessorService imageProcessorService)
    {
        _imageProcessorService = imageProcessorService;
    }

    public void StartCapture()
    {
        CreateWindow();
        try
        {
            while (true)
            {
                WindowService.ShowWindow(WindowService.FindWindow(null, _windowName), WindowService.SW_HIDE);
                UpdateCaptureWindow();

                using Bitmap bitmap = new(_windowWidth, _windowHeight);
                using Graphics graphics = Graphics.FromImage(bitmap);
                graphics.CopyFromScreen(_windowLeft, _windowTop, 0, 0, new(_windowWidth, _windowHeight));
                graphics.Dispose();
                WindowService.ShowWindow(WindowService.FindWindow(null, _windowName), WindowService.SW_SHOW);

                _screenFrame?.Dispose();
                _screenFrame = OpenCvSharp.Extensions.BitmapConverter.ToMat(bitmap);
                bitmap.Dispose();
                Cv2.ImShow(_windowName, _screenFrame);
                _imageProcessorService.ExtractTextFromImage(_screenFrame);
                Cv2.WaitKey(1000);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

        Cv2.DestroyAllWindows();
    }


    private void CreateWindow()
    {
        Cv2.NamedWindow(_windowName, WindowFlags.Normal);
        Cv2.SetWindowProperty(_windowName, WindowPropertyFlags.AutoSize, 1);
        Cv2.SetWindowProperty(_windowName, WindowPropertyFlags.Topmost, 1);

        // Set the initial window location and size
        _windowLeft = 100; // Replace with your desired left coordinate
        _windowTop = 100; // Replace with your desired top coordinate
        _windowWidth = 500; // Replace with your desired width
        _windowHeight = 500; // Replace with your desired height

        Cv2.MoveWindow(_windowName, _windowLeft, _windowTop);
        Cv2.ResizeWindow(_windowName, _windowWidth, _windowHeight);
    }

    private void UpdateCaptureWindow()
    {
        IntPtr windowHandle = WindowService.FindWindow(null, _windowName);

        var (scaleX, scaleY) = WindowService.GetDPIScale(windowHandle);

        // Get the dimensions of the window's client area.
        WindowService.GetClientRect(windowHandle, out WindowService.WindowRECT clientRect);

        // Get the dimensions of the entire window (including non-client areas).
        WindowService.GetWindowRect(windowHandle, out WindowService.WindowRECT windowRect);

        // Calculate the border width and title bar height.
        double borderWidth = ((windowRect.Right - windowRect.Left) - (clientRect.Right - clientRect.Left)) / 2;
        double titleBarHeight = (windowRect.Bottom - windowRect.Top) - (clientRect.Bottom - clientRect.Top);

        // Scale the values for DPI.
        borderWidth *= scaleX;
        titleBarHeight *= scaleY;

        // Calculate other dimensions as needed.
        var right = windowRect.Right * scaleX;
        var bottom = windowRect.Bottom * scaleY;
        var left = windowRect.Left * scaleX;
        var top = windowRect.Top * scaleY;

        _windowWidth = (int)(right - left - (2 * borderWidth));
        _windowHeight = (int)(bottom - top - titleBarHeight);
        _windowLeft = (int)(left + borderWidth);
        _windowTop = (int)(top + titleBarHeight - borderWidth);
    }
}
