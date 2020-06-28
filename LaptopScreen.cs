using Accord.Video;
using Accord.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NoRV
{
    public partial class LaptopScreen : Form
    {
        private VideoCaptureDevice videoSource = null;
        private Bitmap prevBitmap = null;
        private DateTime lastChanged = DateTime.Now.AddSeconds(-10);

        public LaptopScreen()
        {
            InitializeComponent();

            this.Size = Config.getInstance().getLaptopResolution();
            this.MaximumSize = Config.getInstance().getLaptopResolution();
            this.MinimumSize = Config.getInstance().getLaptopResolution();
        }

        private void LaptopScreen_Load(object sender, EventArgs e)
        {
            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (var videoDevice in videoDevices)
            {
                if(videoDevice.Name == Config.getInstance().getLaptopDeviceName())
                {
                    videoSource = new VideoCaptureDevice(videoDevice.MonikerString);
                    int found = -1, maximum = 0;
                    VideoCapabilities[] resolutions = videoSource.VideoCapabilities;
                    Size desiredSize = Config.getInstance().getLaptopResolution();
                    for(int i = 0; i < resolutions.Length; i ++)
                    {
                        Size newSize = resolutions[i].FrameSize;
                        if(newSize == desiredSize)
                        {
                            found = i;
                            break;
                        }
                        Size maxSize = resolutions[maximum].FrameSize;
                        if(newSize.Width * newSize.Height > maxSize.Width * maxSize.Height)
                        {
                            maximum = i;
                        }
                    }
                    if (found >= 0 && found < resolutions.Length)
                    {
                        videoSource.VideoResolution = videoSource.VideoCapabilities[found];
                    }
                    else if (maximum < resolutions.Length)
                    {
                        videoSource.VideoResolution = videoSource.VideoCapabilities[found];
                    }
                    videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
                    videoSource.Start();
                }
            }
        }

        private void LaptopScreen_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(videoSource != null)
            {
                videoSource.SignalToStop();
                videoSource = null;
            }
        }
        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            if (videoSource == null)
                return;
            // get new frame
            using (Bitmap bitmap = eventArgs.Frame)
            {
                Invoke(new Action(() =>
                {
                    Bitmap cloned = (Bitmap)bitmap.Clone();
                    picCamera.InitialImage = null;
                    picCamera.Image = cloned;
                    float difference = 0f;
                    if (prevBitmap != null)
                    {
                        difference = CalculateDifference(prevBitmap, bitmap);
                        if (difference > Config.getInstance().getDetectThreshold())
                        {
                            OBSManager.SwitchToExhibits();
                            lastChanged = DateTime.Now;
                        }
                        else
                        {
                            TimeSpan span = DateTime.Now - lastChanged;
                            if(span.TotalMilliseconds > Config.getInstance().getSwitchTime())
                            {
                                OBSManager.SwitchToWitness();
                            }
                        }
                    }
                    prevBitmap = cloned;
                }));

            }
        }

        static float CalculateDifference(Bitmap bitmap1, Bitmap bitmap2)
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
                // scan to first byte in bitmaps
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
    }
}
