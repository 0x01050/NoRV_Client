using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NoRV
{
    class Utils
    {
        public static string CaptureScreen(Screen window)
        {
            try
            {
                Rectangle s_rect = window.Bounds;
                using (Bitmap bmp = new Bitmap(s_rect.Width, s_rect.Height))
                {
                    using (Graphics gScreen = Graphics.FromImage(bmp))
                        gScreen.CopyFromScreen(s_rect.Location, System.Drawing.Point.Empty, s_rect.Size);
                    using (Bitmap newBmp = new Bitmap(bmp, new Size(428, 240)))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            newBmp.Save(ms, ImageFormat.Png);
                            byte[] byteImage = ms.ToArray();
                            return Convert.ToBase64String(ms.ToArray());
                        }
                    }
                }
            }
            catch (Exception) { }
            return "";
        }
        public static Image CaptureWindow(IntPtr handle)
        {
            IntPtr hdcSrc = WinSDK.User32.GetWindowDC(handle);
            WinSDK.User32.RECT windowRect = new WinSDK.User32.RECT();
            WinSDK.User32.GetWindowRect(handle, ref windowRect);
            int width = windowRect.right - windowRect.left;
            int height = windowRect.bottom - windowRect.top;
            IntPtr hdcDest = WinSDK.GDI32.CreateCompatibleDC(hdcSrc);
            IntPtr hBitmap = WinSDK.GDI32.CreateCompatibleBitmap(hdcSrc, width, height - Config.getInstance().getMirrorIgnore());
            IntPtr hOld = WinSDK.GDI32.SelectObject(hdcDest, hBitmap);
            WinSDK.GDI32.BitBlt(hdcDest, 0, 0, width, height - Config.getInstance().getMirrorIgnore(), hdcSrc, 0, Config.getInstance().getMirrorIgnore(), WinSDK.GDI32.SRCCOPY);
            WinSDK.GDI32.SelectObject(hdcDest, hOld);
            WinSDK.GDI32.DeleteDC(hdcDest);
            WinSDK.User32.ReleaseDC(handle, hdcSrc);
            Image img = Image.FromHbitmap(hBitmap);
            WinSDK.GDI32.DeleteObject(hBitmap);
            return img;
        }
        public static float CalculateDifference(Bitmap bitmap1, Bitmap bitmap2)
        {
            if (bitmap1.Size != bitmap2.Size)
            {
                return -1;
            }

            var rectangle = new Rectangle(0, 0, bitmap1.Width, bitmap1.Height);

            BitmapData bitmapData1 = bitmap1.LockBits(rectangle, ImageLockMode.ReadOnly, bitmap1.PixelFormat);
            BitmapData bitmapData2 = bitmap2.LockBits(rectangle, ImageLockMode.ReadOnly, bitmap2.PixelFormat);

            float diff = 0;
            var byteCount = rectangle.Width * rectangle.Height * 3;

            unsafe
            {
                byte* pointer1 = (byte*)bitmapData1.Scan0.ToPointer();
                byte* pointer2 = (byte*)bitmapData2.Scan0.ToPointer();

                for (int x = 0; x < byteCount; x++)
                {
                    diff += (float)Math.Abs(*pointer1 - *pointer2) / 255;
                    pointer1++;
                    pointer2++;
                }
            }

            bitmap1.UnlockBits(bitmapData1);
            bitmap2.UnlockBits(bitmapData2);

            return 100 * diff / byteCount;
        }
        public static void Restart3rdParty(string appName)
        {
            Stop3rdParty(appName);
            Start3rdParty(appName);
        }
        public static void Stop3rdParty(string appName)
        {
            try
            {
                foreach (var proc in Process.GetProcessesByName(appName))
                {
                    proc.Kill();
                    Logger.info("'" + appName + "' was killed");
                }
            }
            catch (Exception e)
            {
                Logger.info("'" + appName + "' was not killed", e.Message);
            }
        }
        public static void Start3rdParty(string appName)
        {
            try
            {
                Process.Start(appName);
                Logger.info("'" + appName + "' was started");
            }
            catch (Exception e)
            {
                Logger.info("'" + appName + "' was not started", e.Message);
            }
        }
    }

    enum AppStatus
    {
        STOPPED,
        PLAYING,
        PAUSED
    }
}
