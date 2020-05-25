using Google.Apis.Auth.OAuth2;
using Google.Cloud.TextToSpeech.V1;
using Google.Protobuf.WellKnownTypes;
using Grpc.Auth;
using Grpc.Core;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NoRV
{
    public partial class Form2 : Form
    {


        private GoogleCredential googleCredential;
        private Channel channel;
        private TextToSpeechClient client;
        
        private Dictionary<string, string> InfoList = new Dictionary<string, string>();

        private string voiceText = "";
        private double speed = 1.0;
        private double pitch = 0.0;
        private string status = "stop";
        private string lastTime = "#Time#";
        private int offset = 0;

        IWavePlayer waveOutDevice = null;
        AudioFileReader audioFileReader = null;

        public Form2(Dictionary<string, string> InfoList = null)
        {
            if(InfoList != null)
            {
                this.InfoList = InfoList;

                Dictionary<string, int> tzAbbrev = new Dictionary<string, int>() {
                    {"Pacific Time (PDT)", -7},
                    {"Eastern Time (EDT)", -4},
                    {"Central Time (CDT)", -5},
                    {"Mountain Time (MDT)", -6},
                    {"Alaska Time (ADT)", -8},
                    {"Hawaii-Aleutian Standard Time (HAST)", -10}
                };

                if(this.InfoList.ContainsKey("TimeZone"))
                {
                    string tz = this.InfoList["TimeZone"];
                    this.InfoList.Remove("TimeZone");
                    offset = tzAbbrev[tz];
                    DateTime tzNow = DateTime.UtcNow.AddHours(offset);
                    this.InfoList.Add("Date", tzNow.ToString("MMM dd, yyyy"));
                    this.InfoList.Add("Time", this.lastTime = tzNow.ToString("h:mm tt"));
                }
            }

            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

            string template = "Normal";
            if (this.InfoList.ContainsKey("Template"))
            {
                template = this.InfoList["Template"];
            }
            template = File.ReadAllText(template + ".txt");
            foreach (var info in this.InfoList)
            {
                template = template.Replace("#" + info.Key + "#", info.Value);
            }
            txtSource.Text = this.voiceText = template;

            using (Stream m = new FileStream("NoRV TTS-c4a3e2c55a4f.json", FileMode.Open))
                this.googleCredential = GoogleCredential.FromStream(m);
            this.channel = new Channel(TextToSpeechClient.DefaultEndpoint.Host,
                googleCredential.ToChannelCredentials());
            this.client = TextToSpeechClient.Create(channel);

            ListVoicesRequest voiceReq = new ListVoicesRequest { LanguageCode = "en-US" };
            ListVoicesResponse voiceResp = this.client.ListVoices(voiceReq);
            foreach (Voice voice in voiceResp.Voices)
            {
                if (voice.LanguageCodes.Contains("en-US") && voice.Name.Contains("Wavenet"))
                {
                    cbVoice.Items.Add(voice.Name);
                }
            }

            if(cbVoice.Items.Count <= 0)
            {
                MessageBox.Show("No available voice", "NoRV", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
            cbVoice.SelectedIndex = 1;

            DeviceOpen();
        }

        private Thread buttonCheckThread = null;
        private uint deviceHandle = 0;
        private int ledColor = Delcom.GREENLED;
        private int buttonStatus = 0; // 0: Released, 1: Pressed
        private int ledStatus = 0; // 0: Off, 1: On, 2: Flash Off, 3: Flash On
        private DateTime lastButtonClicked;
        private void DeviceOpen()
        {
            StringBuilder DeviceName = new StringBuilder(Delcom.MAXDEVICENAMELEN);
            if (Delcom.DelcomGetNthDevice(0, 0, DeviceName) == 0)
            {
                lblButtonStatus.Text = "No button";
                //MessageBox.Show("No button found, you can control with the button on the screen.", "NoRV", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            deviceHandle = Delcom.DelcomOpenDevice(DeviceName, 0);
            if (deviceHandle == 0)
            {
                lblButtonStatus.Text = "Can not open button";
                //MessageBox.Show("Button open failed, you can control with the button on the screen.", "NoRV", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            lblButtonStatus.Text = "Initiated";
            Delcom.DelcomLEDControl(deviceHandle, ledColor, Delcom.LEDON);
            LEDFlash();

            lastButtonClicked = DateTime.Now.AddSeconds(-15);
            buttonCheckThread = new Thread(new ThreadStart(ButtonCheckProcedure));
            buttonCheckThread.Start();
        }
        private Thread ledFlashThread = null;
        private void LEDFlash()
        {
            DestroyLEDFlashThread();
            ledStatus = 2;
            ledFlashThread = new Thread(new ThreadStart(LEDFlashProcedure));
            ledFlashThread.Start();
        }
        private void LEDOn()
        {
            Delcom.DelcomLEDPower(deviceHandle, ledColor, 100);
        }
        private void LEDOff()
        {
            Delcom.DelcomLEDPower(deviceHandle, ledColor, 0);

        }
        private void DestroyLEDFlashThread()
        {
            if (ledFlashThread != null)
            {
                ledFlashThread.Abort();
                ledFlashThread = null;
            }
        }
        private void LEDFlashProcedure()
        {
            while (true)
            {
                Thread.Sleep(1000);
                if (deviceHandle == 0)
                    continue;
                if (ledStatus < 2)
                    continue;
                ledStatus = 5 - ledStatus;
                if (ledStatus == 2)
                {
                    LEDOff();
                }
                if (ledStatus == 3)
                {
                    LEDOn();
                }
            }
        }

        private void ButtonCheckProcedure()
        {
            while (true)
            {
                Thread.Sleep(100);
                if (deviceHandle == 0)
                    continue;
                int btnStat = Delcom.DelcomGetButtonStatus(deviceHandle);
                if (buttonStatus == 1 && btnStat == 0)
                {
                    ButtonClicked();
                }
                buttonStatus = btnStat;
            }
        }
        private int recordStatus = 0; // 0: before record, 1: during record
        private void ButtonClicked()
        {
            if(status != "stop")
            {
                return;
            }

            DateTime now = DateTime.Now;
            TimeSpan span = now - lastButtonClicked;
            if(span.TotalSeconds > 10)
            {
                if(recordStatus == 0)
                {
                    this.Invoke(new Action(() =>
                    {
                        status = "instruction";
                        DisposeAudioPlayer();
                        waveOutDevice = new WaveOut();
                        audioFileReader = new AudioFileReader("Audio_1.mp3");
                        waveOutDevice.Init(audioFileReader);
                        waveOutDevice.Play();
                        waveOutDevice.PlaybackStopped += InstructionStop;
                    }));
                }
                if(recordStatus == 1)
                {
                    this.Invoke(new Action(() =>
                    {
                        status = "instruction";
                        DisposeAudioPlayer();
                        waveOutDevice = new WaveOut();
                        audioFileReader = new AudioFileReader("Audio_2.mp3");
                        waveOutDevice.Init(audioFileReader);
                        waveOutDevice.Play();
                        waveOutDevice.PlaybackStopped += InstructionStop;
                    }));
                }
            }
            else
            {
                DisposeAudioPlayer();
                ScreenButtonClick();
            }
        }

        private void InstructionStop(object sender = null, EventArgs e = null)
        {
            this.Invoke(new Action(() =>
            {
                status = "stop";
                lastButtonClicked = DateTime.Now;
            }));
        }

        private void slSpeed_ValueChanged(object sender, EventArgs e)
        {
            this.speed = slSpeed.Value / 10.0;
            lblSpeedValue.Text = String.Format("{0:N2}", this.speed);
        }

        private void slPitch_ValueChanged(object sender, EventArgs e)
        {
            this.pitch = slPitch.Value / 10.0;
            lblPitchValue.Text = String.Format("{0:N2}", this.pitch);
        }

        private void Play()
        {
            this.Invoke(new Action(() =>
            {
                btnSpeak.Image = Properties.Resources.stop;
                btnSpeak.Text = "STOP IT";
                status = "speak";
                recordStatus = 1;
                ledStatus = 1;

                StartOBS();
                LEDOn();
                Thread.Sleep(2000);
                sound();

                DisposeAudioPlayer();
                waveOutDevice = new WaveOut();
                audioFileReader = new AudioFileReader("tts.mp3");
                waveOutDevice.Init(audioFileReader);
                waveOutDevice.Play();
                waveOutDevice.PlaybackStopped += Stop;
            }));
        }
        private void DisposeAudioPlayer()
        {
            this.Invoke(new Action(() =>
            {
                if (waveOutDevice != null)
                {
                    waveOutDevice.Stop();
                }
                if (audioFileReader != null)
                {
                    audioFileReader.Dispose();
                    audioFileReader = null;
                }
                if (waveOutDevice != null)
                {
                    waveOutDevice.Dispose();
                    waveOutDevice = null;
                }
            }));
        }

        private void Stop(object sender = null, EventArgs e = null)
        {
            this.Invoke(new Action(() =>
            {
                status = "stop";

                DisposeAudioPlayer();

                File.Delete("tts.mp3");
            }));
        }

        private void btnSpeak_Click(object sender, EventArgs e)
        {

            this.Invoke(new Action(() =>
            {
                ScreenButtonClick();
            }));
        }
        private void ScreenButtonClick()
        {
            lastButtonClicked = DateTime.Now.AddSeconds(-15);
            if (status == "stop")
            {
                if(recordStatus == 0)
                {
                    Play();
                }
                else if(recordStatus == 1)
                {
                    StopRecord();
                }
            }
        }
        private void StopRecord()
        {
            this.Invoke(new Action(() =>
            {
                status = "finalize";
                DisposeAudioPlayer();

                DateTime tzNow = DateTime.UtcNow.AddHours(offset);
                SpeechSynthesizer speech = new SpeechSynthesizer();
                string announce = File.ReadAllText("announce.txt");
                speech.SpeakAsync(announce + tzNow.ToString(" h:mm tt"));
                speech.SpeakCompleted += Speech_SpeakCompleted;
            }));
        }

        private void Speech_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            this.Invoke(new Action(() =>
            {
                btnSpeak.Image = Properties.Resources.play;
                btnSpeak.Text = "SPEAK IT";
                recordStatus = 0;
                status = "stop";
                StopOBS();
                Close();
            }));
        }

        private void sound()
        {
            this.Invoke(new Action(() =>
            {

                DateTime tzNow = DateTime.UtcNow.AddHours(offset);

                // Set the text input to be synthesized.
                SynthesisInput input = new SynthesisInput
                {
                    Text = voiceText.Replace(this.lastTime, tzNow.ToString("h:mm tt"))
                    //Text = "Simple Text.Simple Text.Simple Text."
                };

                // Build the voice request, select the language code ("en-US"),
                // and the SSML voice gender ("neutral").
                VoiceSelectionParams voice = new VoiceSelectionParams
                {
                    LanguageCode = "en-US",
                    Name = cbVoice.SelectedItem.ToString()
                };

                // Select the type of audio file you want returned.
                AudioConfig config = new AudioConfig
                {
                    AudioEncoding = AudioEncoding.Mp3,
                    Pitch = pitch,
                    SpeakingRate = speed
                };

                // Perform the Text-to-Speech request, passing the text input
                // with the selected voice parameters and audio file type
                var response = client.SynthesizeSpeech(new SynthesizeSpeechRequest
                {
                    Input = input,
                    Voice = voice,
                    AudioConfig = config
                });

                // Write the binary AudioContent of the response to an MP3 file.
                using (Stream output = File.Create("tts.mp3"))
                {
                    response.AudioContent.WriteTo(output);
                }
            }));
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Stop();
            StopOBS();
            DestroyLEDFlashThread();
            if (buttonCheckThread != null)
            {
                buttonCheckThread.Abort();
                buttonCheckThread = null;
            }
            if (deviceHandle != 0)
            {
                LEDOff();
                Delcom.DelcomCloseDevice(deviceHandle);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ButtonClicked();
        }

        private void StartOBS()
        {
            this.Invoke(new Action(() =>
            {

                Process[] obs64 = Process.GetProcessesByName("obs64");
                if (obs64.Length == 0)
                    return;
                foreach (Process obs in obs64)
                {
                    if (obs.MainWindowHandle != (IntPtr)0)
                    {
                        SetForegroundWindow(obs.MainWindowHandle);
                        Thread.Sleep(100);
                        SendKeys.Send("^R");
                    }
                }
            }));
        }
        private void StopOBS()
        {
            this.Invoke(new Action(() =>
            {
                Process[] obs64 = Process.GetProcessesByName("obs64");
                if (obs64.Length == 0)
                    return;
                foreach (Process obs in obs64)
                {
                    if(obs.MainWindowHandle != (IntPtr)0)
                    {
                        SetForegroundWindow(obs.MainWindowHandle);
                        Thread.Sleep(100);
                        SendKeys.Send("^S");
                    }
                }
            }));
        }
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern int ShowWindow(IntPtr hWnd, int nCmdShow);
    }
}
