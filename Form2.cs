using Google.Apis.Auth.OAuth2;
using Google.Cloud.TextToSpeech.V1;
using Grpc.Auth;
using Grpc.Core;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Speech.Synthesis;
using System.Threading;
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
        private string lastTime = "#Time#";
        private int tzOffset = 0;

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
                    tzOffset = tzAbbrev[tz];
                    DateTime tzNow = DateTime.UtcNow.AddHours(tzOffset);
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
            template = Config.getInstance().getTemplate(template);
            
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
            cbVoice.SelectedIndex = 2;

            DoInitialize();
        }
        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            DisposeAudioPlayer();
            OBSManager.StopOBSRecording();
            stopButtonCheck();
            stopLEDFlash();
        }


        private static string STATUS_WAIT = "WAIT";
        private string STATUS_RECORD = "RECORD";
        private string STATUS_PAUSE = "PAUSE";

        private bool ignoreInput = false;
        private string status = STATUS_WAIT;

        private DateTime lastButtonClicked;
        private Thread buttonCheckThread = null;
        int buttonStatus = 0;
        private Thread ledFlashThread = null;
        private void DoInitialize()
        {
            lblButtonStatus.Text = ButtonManager.getInstance().getButtonStatus();
            lastButtonClicked = DateTime.Now.AddSeconds(-15);
            startButtonCheck();
            startLEDFlash();
        }

        private void startButtonCheck()
        {
            stopButtonCheck();
            buttonCheckThread = new Thread(new ThreadStart(ButtonCheckProcedure));
            buttonCheckThread.Start();
        }
        private void stopButtonCheck()
        {
            if(buttonCheckThread != null)
            {
                buttonCheckThread.Abort();
                buttonCheckThread = null;
            }
        }
        private void ButtonCheckProcedure()
        {
            DateTime buttonPressed = DateTime.Now;
            bool isPressed = false;
            while (true)
            {
                bool pressed = ButtonManager.getInstance().checkButtonPressed();
                if (pressed)
                    buttonStatus++;
                else
                    buttonStatus--;

                if (buttonStatus == 2)
                {
                    isPressed = true;
                    buttonPressed = DateTime.Now;
                }
                if (isPressed && buttonStatus == 0)
                {
                    TimeSpan elapse = DateTime.Now - buttonPressed;
                    if (elapse.TotalSeconds > 3)
                        ButtonClicked(true);
                    else
                        ButtonClicked();
                }

                if (buttonStatus > 2)
                    buttonStatus = 2;
                if (buttonStatus < 0)
                    buttonStatus = 0;
                
                Thread.Sleep(30);
            }
        }
        private void ButtonClicked(bool longPress = false)
        {
            if(ignoreInput)
            {
                return;
            }

            if (status == STATUS_WAIT)
            {
                TimeSpan elapse = DateTime.Now - lastButtonClicked;
                if (elapse.TotalSeconds > 10)
                {
                    PlayMP3("Audios/StartAudio.mp3", (s, e) =>
                    {
                        lastButtonClicked = DateTime.Now;
                    });
                }
                else
                {
                    btnSpeak.Invoke(new Action(() =>
                    {
                        btnSpeak.PerformClick();
                    }));
                }
            }
            else if (longPress)
            {
                btnSpeak.Invoke(new Action(() =>
                {
                    btnSpeak.PerformClick();
                }));
            }
            else if (status == STATUS_RECORD)
            {
                PauseRecording();
            }
            else if(status == STATUS_PAUSE)
            {
                UnpauseRecording();
            }
        }
        private void PlayMP3(string mp3File, EventHandler<StoppedEventArgs> stopHandler = null)
        {
            DisposeAudioPlayer();
            this.Invoke(new Action(() =>
            {
                ignoreInput = true;
                waveOutDevice = new WaveOut();
                audioFileReader = new AudioFileReader(mp3File);
                waveOutDevice.Init(audioFileReader);
                waveOutDevice.Play();
                waveOutDevice.PlaybackStopped += (s, e) =>
                {
                    DisposeAudioPlayer();
                    ignoreInput = false;
                };
                if (stopHandler != null)
                {
                    waveOutDevice.PlaybackStopped += stopHandler;
                }
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



        private void startLEDFlash()
        {
            stopLEDFlash();
            ledFlashThread = new Thread(new ThreadStart(LEDFlashProcedure));
            ledFlashThread.Start();
        }
        private void stopLEDFlash()
        {
            if (ledFlashThread != null)
            {
                ledFlashThread.Abort();
                ledFlashThread = null;
            }
        }
        private void LEDFlashProcedure()
        {
            bool ledStatus = true;
            while (true)
            {
                Thread.Sleep(1000);
                if(ledStatus)
                {
                    ButtonManager.getInstance().turnOnLED();
                }
                else
                {
                    ButtonManager.getInstance().turnOffLED();
                }
                ledStatus = !ledStatus;
            }
        }
        private void solidLED()
        {
            stopLEDFlash();
            ButtonManager.getInstance().turnOnLED();
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


        private void btnSpeak_Click(object sender, EventArgs e)
        {
            if(ignoreInput)
            {
                return;
            }
            if(status == STATUS_WAIT)
            {
                StartRecording();
            }
            else if(status == STATUS_RECORD || status == STATUS_PAUSE)
            {
                StopRecording();
            }
        }
        private void StartRecording()
        {
            status = STATUS_RECORD;
            ignoreInput = true;

            Application.UseWaitCursor = true;

            btnSpeak.Image = Properties.Resources.stop;
            btnSpeak.Text = "STOP IT";

            OBSManager.StartOBSRecording();
            solidLED();
            Thread.Sleep(2000);

            Application.UseWaitCursor = false;

            GenerateGoogleTTS();
            PlayMP3("tts.mp3", (s, e) =>
            {
                File.Delete("tts.mp3");
            });
        }
        private void StopRecording()
        {
            status = STATUS_WAIT;
            PlayMP3("Audios/StopAudio.mp3", (s, e) =>
            {
                ignoreInput = true;
                DateTime tzNow = DateTime.UtcNow.AddHours(tzOffset);
                SpeechSynthesizer speech = new SpeechSynthesizer();
                speech.SpeakAsync("The time is" + tzNow.ToString(" h:mm tt"));
                speech.SpeakCompleted += (ss, ee) =>
                {
                    btnSpeak.Image = Properties.Resources.play;
                    btnSpeak.Text = "SPEAK IT";
                    Close();
                };
            });
        }
        private void PauseRecording()
        {
            status = STATUS_PAUSE;
            PlayMP3("Audios/PauseAudio.mp3", (s, e) =>
            {
                ignoreInput = true;
                DisposeAudioPlayer();
                DateTime tzNow = DateTime.UtcNow.AddHours(tzOffset);
                SpeechSynthesizer speech = new SpeechSynthesizer();
                speech.SpeakAsync("The time is" + tzNow.ToString(" h:mm tt"));
                speech.SpeakCompleted += (ss, ee) =>
                {
                    ignoreInput = false;
                };
            });
            OBSManager.PauseOBSRecording();
            startLEDFlash();
        }
        private void UnpauseRecording()
        {
            status = STATUS_RECORD;
            PlayMP3("Audios/UnpauseAudio.mp3", (s, e) =>
            {
                ignoreInput = true;
                DisposeAudioPlayer();
                DateTime tzNow = DateTime.UtcNow.AddHours(tzOffset);
                SpeechSynthesizer speech = new SpeechSynthesizer();
                speech.SpeakAsync("The time is" + tzNow.ToString(" h:mm tt"));
                speech.SpeakCompleted += (ss, ee) =>
                {
                    ignoreInput = false;
                };
            });
            OBSManager.UnpauseOBSRecording();
            solidLED();
        }

        private void GenerateGoogleTTS()
        {
            DateTime tzNow = DateTime.UtcNow.AddHours(tzOffset);
            SynthesisInput input = new SynthesisInput
            {
                Text = voiceText.Replace(this.lastTime, tzNow.ToString("h:mm tt"))
            };
            VoiceSelectionParams voice = new VoiceSelectionParams
            {
                LanguageCode = "en-US",
                Name = cbVoice.SelectedItem.ToString()
            };
            AudioConfig config = new AudioConfig
            {
                AudioEncoding = AudioEncoding.Mp3,
                Pitch = pitch,
                SpeakingRate = speed
            };
            var response = client.SynthesizeSpeech(new SynthesizeSpeechRequest
            {
                Input = input,
                Voice = voice,
                AudioConfig = config
            });
            using (Stream output = File.Create("tts.mp3"))
            {
                response.AudioContent.WriteTo(output);
            }
        }
    }
}
