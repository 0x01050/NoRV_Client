using Accord.Video;
using Accord.Video.DirectShow;
using Emgu.CV;
using Emgu.CV.Structure;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NoRV
{
    public partial class TrackForm : Form
    {
        private int XOffset, XSpeed, YOffset, YSpeed, ZOffset, ZSpeed;
        private Size mainDetectArea;
        private int outsideSec;
        private int topIgnore, bottomIgnore;
        private double topPadding, totalHeight;
        private Size inputResolution, outputResolution;
        private string cameraName;

        private Thread cameraStart = null;
        private CascadeClassifier faceDetector;
        private VideoCaptureDevice cameraSource = null;
        private Thread detectThread = null;

        private bool _needRedraw = false, detected = false;
        private Bitmap curBitmap = null;
        private Size curSize, detectedSize;

        private Point curPoint, detectedPoint;
        private DateTime lastMidPersonDetect;

        public TrackForm()
        {
            InitializeComponent();
        }

        private void TrackForm_Load(object sender, EventArgs e)
        {
            LoadInitialValues();
            Width = outputResolution.Width;
            Height = outputResolution.Height;
            cameraStart = new Thread(StartCamera);
            cameraStart.Start();
        }

        private void TrackForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Terminate();
        }

        private void LoadInitialValues()
        {
            Config config = Config.getInstance();

            topIgnore = config.getTopIgnorePercent();
            bottomIgnore = config.getBottomIgnorePercent();

            mainDetectArea = config.getDetectMainArea();
            outsideSec = config.getDetectOutsideWaitSeconds();

            topPadding = config.getZoomTopPadding();
            totalHeight = config.getZoomTotalHeight();

            XOffset = config.getSmoothingXOffset();
            XSpeed = config.getSmoothingXSpeed();
            YOffset = config.getSmoothingYOffset();
            YSpeed = config.getSmoothingYSpeed();
            ZOffset = config.getSmoothingZOffset();
            ZSpeed = config.getSmoothingZSpeed();

            inputResolution = config.getCameraInputResolution();
            outputResolution = config.getOutputOutputResolution();
            cameraName = config.getCameraName();

            faceDetector = new CascadeClassifier("Detect/haarcascade_frontalface.xml");
        }

        private void StartCamera()
        {
            while(true)
            {
                try
                {
                    var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                    foreach (var videoDevice in videoDevices)
                    {
                        if(videoDevice.Name == cameraName)
                        {
                            cameraSource = new VideoCaptureDevice(videoDevice.MonikerString);
                            var resolutions = cameraSource.VideoCapabilities;
                            Size approxSize = new Size(0, 0);
                            foreach (var cap in resolutions)
                            {
                                var FrameSize = cap.FrameSize;
                                if (FrameSize.Width == inputResolution.Width && FrameSize.Height == inputResolution.Height)
                                {
                                    cameraSource.VideoResolution = cap;
                                    break;
                                }
                                if (approxSize.Width * approxSize.Height < FrameSize.Width * FrameSize.Height)
                                {
                                    approxSize = FrameSize;
                                    cameraSource.VideoResolution = cap;
                                }
                            }
                            if(cameraSource.VideoResolution != null)
                            {
                                inputResolution = cameraSource.VideoResolution.FrameSize;
                            }

                            curPoint = detectedPoint = new Point(inputResolution.Width / 2, inputResolution.Height / 2);
                            curSize = detectedSize = new Size((int)(outputResolution.Height / totalHeight), (int)(outputResolution.Height / totalHeight));

                            cameraSource.NewFrame += NewFrame_EventHandler;
                            cameraSource.Start();
                            return;
                        }
                    }
                    
                }
                catch (Exception) { }
            }

        }

        private void NewFrame_EventHandler(object sender, NewFrameEventArgs e)
        {
            try
            {
                curBitmap = (Bitmap)e.Frame.Clone();
                Bitmap tmp = (Bitmap)e.Frame.Clone();
                if (detectThread == null || !detectThread.IsAlive)
                {
                    detectThread = new Thread(DetectWork);
                    detectThread.Start(tmp);
                }
                Invoke(new Action(() =>
                {
                    if(!_needRedraw)
                    {
                        _needRedraw = true;
                        resultImage.Invalidate();
                    }
                }));
            }
            catch (Exception) { }
        }
        private void DetectWork(object param)
        {
            Bitmap frame = (Bitmap)param;
            Bitmap target = new Bitmap(frame.Width, frame.Height * (100 - topIgnore - bottomIgnore) / 100);
            lastMidPersonDetect = DateTime.Now.AddDays(-1);

            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(frame, new Rectangle(0, 0, target.Width, target.Height),
                                 new Rectangle(0, frame.Height * topIgnore / 100, target.Width, target.Height),
                                 GraphicsUnit.Pixel);
            }


            Rectangle[] faces;
            using (var userPicture = target.ToImage<Gray, byte>())
            {
                faces = faceDetector.DetectMultiScale(userPicture, 1.3, 6);
            }


            if (faces != null && faces.Length > 0)
            {
                int minD = -1;
                Rectangle approx = new Rectangle();

                int midD = -1;
                Rectangle middle = new Rectangle();
                Point center = new Point(inputResolution.Width / 2, inputResolution.Height / 2);

                foreach (var face in faces)
                {
                    face.Offset(0, inputResolution.Height * topIgnore / 100);
                    Point tmp = new Point(face.X + face.Width / 2, face.Y + face.Height / 2);
                    int dist = (tmp.X - center.X) * (tmp.X - center.X) + (tmp.Y - center.Y) * (tmp.Y - center.Y);
                    if ((Math.Abs(tmp.X - center.X) < mainDetectArea.Width / 2 && Math.Abs(tmp.Y - center.Y) < mainDetectArea.Height / 2) && (midD == -1 || midD > dist))
                    {
                        midD = dist;
                        middle = face;
                    }
                    if ((Math.Abs(tmp.X - center.X) >= mainDetectArea.Width / 2 || Math.Abs(tmp.Y - center.Y) >= mainDetectArea.Height / 2) && (minD == -1 || minD > dist))
                    {
                        minD = dist;
                        approx = face;
                    }
                }

                if (midD >= 0)
                {
                    lastMidPersonDetect = DateTime.Now;
                    detectedPoint = new Point(middle.X + middle.Width / 2, middle.Y + middle.Height / 2);
                    detectedSize = middle.Size;
                    detected = true;
                }
                else if (minD >= 0 && (DateTime.Now - lastMidPersonDetect).TotalSeconds > outsideSec)
                {
                    detectedPoint = new Point(approx.X + approx.Width / 2, approx.Y + approx.Height / 2);
                    detectedSize = approx.Size;
                    detected = true;
                }
                else
                {
                    detected = false;
                }

            }
            else
            {
                detected = false;
            }
        }

        private void resultImage_Paint(object sender, PaintEventArgs e)
        {
            if (_needRedraw)
            {
                Graphics g = e.Graphics;

                int deltaZoom = 0;
                if (Math.Abs(detectedSize.Width - curSize.Width) > ZOffset)
                {
                    deltaZoom = (detectedSize.Width - curSize.Width) / Math.Abs(detectedSize.Width - curSize.Width) * ZSpeed;
                }
                curSize = new Size(curSize.Width + deltaZoom, curSize.Height + deltaZoom);

                int deltaX = 0, deltaY = 0;
                if (Math.Abs(detectedPoint.X - curPoint.X) > XOffset)
                {
                    deltaX = (detectedPoint.X - curPoint.X) / Math.Abs(detectedPoint.X - curPoint.X) * XSpeed;
                }
                if (Math.Abs(detectedPoint.Y - curPoint.Y) > YOffset)
                {
                    deltaY = (detectedPoint.Y - curPoint.Y) / Math.Abs(detectedPoint.Y - curPoint.Y) * YSpeed;
                }
                curPoint = new Point(curPoint.X + deltaX, curPoint.Y + deltaY);

                int realHeight = (int)(curSize.Height * totalHeight);
                int realWidth = realHeight * outputResolution.Width / outputResolution.Height;
                int x = curPoint.X - realWidth / 2;
                int y = curPoint.Y - (int)(curSize.Height * (0.5 + topPadding));

                if (x < 0) x = 0;
                if (x > inputResolution.Width - realWidth) x = inputResolution.Width - realWidth;

                if (y < 0) y = 0;
                if (y > inputResolution.Height - realHeight) y = inputResolution.Height - realHeight;

                if (curBitmap != null)
                    g.DrawImage(curBitmap, new Rectangle(0, 0, outputResolution.Width, outputResolution.Height), new Rectangle(x, y, realWidth, realHeight), GraphicsUnit.Pixel);

                if (Config.getInstance().mainAreaVisible())
                    g.DrawRectangle(new Pen(Color.Blue, 2), new Rectangle((inputResolution.Width / 2 - mainDetectArea.Width / 2 - x) * outputResolution.Width / realWidth, (inputResolution.Height / 2 - mainDetectArea.Height / 2 - y) * outputResolution.Height / realHeight, mainDetectArea.Width * outputResolution.Width / realWidth, mainDetectArea.Height * outputResolution.Height / realHeight));
                _needRedraw = false;
            }
        }

        public void Terminate()
        {
            if (cameraStart != null && !cameraStart.IsAlive)
            {
                cameraStart.Abort();
            }
            if (cameraSource != null && cameraSource.IsRunning)
            {
                cameraSource.SignalToStop();
            }
            if (detectThread != null && !detectThread.IsAlive)
            {
                detectThread.Abort();
            }
        }
    }
}
