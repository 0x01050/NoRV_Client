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
    public partial class MainScreen : Form
    {
        private TextToSpeechClient client;

        private Dictionary<string, string> InfoList = new Dictionary<string, string>();
        private string id = "";
        private string witness = "";

        private string voiceText = "";
        private double speed = 0.9;
        private double pitch = 0.0;
        private string lastTime = "#Time#";
        private string source = "";

        IWavePlayer waveOutDevice = null;
        AudioFileReader audioFileReader = null;

        public MainScreen(Dictionary<string, string> InfoList = null, string source = "")
        {
            if (InfoList != null)
            {
                this.InfoList = InfoList;

                if (this.InfoList.ContainsKey("TimeZone"))
                {
                    string tz = this.InfoList["TimeZone"];
                    this.InfoList.Remove("TimeZone");
                    TimeManage.setTimezone(tz);
                    DateTime tzNow = TimeManage.getCurrentTime();
                    this.InfoList.Add("Date", tzNow.ToString("MMM dd, yyyy"));
                    this.InfoList.Add("Time", this.lastTime = tzNow.ToString("h:mm tt"));
                }
                if (this.InfoList.ContainsKey("ID"))
                    id = this.InfoList["ID"];
                if (this.InfoList.ContainsKey("Witness"))
                    witness = this.InfoList["Witness"];
                if (!this.InfoList.ContainsKey("Videographer"))
                    this.InfoList.Add("Videographer", Program.videographer);
                if (!this.InfoList.ContainsKey("Commission"))
                    this.InfoList.Add("Commission", Program.commission);
            }
            this.source = source;
            InitializeComponent();
        }

        private void MainScreen_Load(object sender, EventArgs e)
        {
            Application.UseWaitCursor = true;

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

            slSpeed.Value = Convert.ToInt32(Config.getInstance().getGoogleVoiceSpeed() * 10);
            slPitch.Value = Convert.ToInt32(Config.getInstance().getGoogleVoicePitch() * 10);

            InitGoogleCredential();
            LogInit();

            DoInitialize();
            Application.UseWaitCursor = false;
        }
        private void MainScreen_FormClosing(object sender, FormClosingEventArgs e)
        {
            DisposeAudioPlayer();
            if (DialogResult == DialogResult.OK)
            {
                OBSManager.StopOBSRecording(witness);
            }
            else
            {
                OBSManager.StopOBSRecording();
            }
            stopButtonCheck();
            stopLEDFlash();
            stopKillerThread();
            stopAlertThread();
            ButtonManager.getInstance().turnOffLED();

            if(DialogResult == DialogResult.OK)
            {
                InsertLog("End");
                SaveLog();
            }
            else
            {
                StatusManage.getInstance().changeWitness(id, witness, "", 3);
            }
        }

        private string logFile = "";
        private List<string> logs = new List<string>();
        private string lastLog = "";
        private DateTime lastLogTime = DateTime.Now;
        private int totalSeconds = 0;
        private void LogInit()
        {
            string type = "", date = "";
            if (InfoList.ContainsKey("Template"))
                type = InfoList["Template"];
            if (InfoList.ContainsKey("Date"))
                date = InfoList["Date"];
            logFile = witness + " - " + date;
            logs.Add(witness + ". " + date);
            logs.Add("");
            GenerateStartAudio(witness, type);
        }
        private void SaveLog()
        {
            logs.Add("total running time =" + buildElapsedTimeString(totalSeconds));
            logs.Add(String.Format("total breaks = {0}", logs.Count - 3));
            try
            {
                File.WriteAllLines(Config.getInstance().getLogPath() + "\\" + logFile + ".txt", logs);
            }
            catch(DirectoryNotFoundException)
            {
                File.WriteAllLines(logFile + ".txt", logs);
            }
            catch(Exception) { }
        }
        private void InsertLog(string type)
        {
            string action = type + TimeManage.getCurrentTime().ToString(": h:mmtt");
            if (type == "Start" || type == "On")
            {
                lastLog = action;
                lastLogTime = DateTime.Now;
            }
            if (type == "Off" || type == "End")
            {
                int elapSec = (int)(DateTime.Now - lastLogTime).TotalSeconds;
                logs.Add(lastLog + " - " + action + " =" + buildElapsedTimeString(elapSec));
                totalSeconds += elapSec;
            }

            string time = TimeManage.getCurrentTime().ToString("h:mmtt");
            if (type == "Start")
                StatusManage.getInstance().changeWitness(id, witness, time, 1);
            if (type == "End")
                StatusManage.getInstance().changeWitness(id, witness, time, 2);
        }

        private void InitGoogleCredential()
        {
            var builder = new TextToSpeechClientBuilder();
            builder.CredentialsPath = "NoRV TTS-c4a3e2c55a4f.json";
            client = builder.Build();

            ListVoicesRequest voiceReq = new ListVoicesRequest { LanguageCode = "en-US" };
            ListVoicesResponse voiceResp = this.client.ListVoices(voiceReq);
            int idx = 0, selected = 0;
            foreach (Voice voice in voiceResp.Voices)
            {
                if (voice.LanguageCodes.Contains("en-US") && voice.Name.Contains("Wavenet"))
                {
                    cbVoice.Items.Add(voice.Name);
                    if (voice.Name == Config.getInstance().getGoogleVoiceName())
                        selected = idx;
                    idx++;
                }
            }

            if (cbVoice.Items.Count <= 0)
            {
                MessageBox.Show("No available voice", "NoRV", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
            cbVoice.SelectedIndex = selected;
        }


        private static string STATUS_WAIT = "WAIT";
        private string STATUS_RECORD = "RECORD";
        private string STATUS_PAUSE = "PAUSE";

        private bool ignoreInput = false;
        private string status = STATUS_WAIT;

        private DateTime lastButtonClicked;
        private Thread buttonCheckThread = null;
        private Thread ledFlashThread = null;
        private void DoInitialize()
        {
            lblButtonStatus.Text = ButtonManager.getInstance().getButtonStatus();
            lastButtonClicked = DateTime.Now.AddSeconds(-15);
            startButtonCheck();
            startLEDFlash();

            if (source == "SelectScreen")
            {
                ButtonClicked();
            }
        }

        private void startButtonCheck()
        {
            stopButtonCheck();
            buttonCheckThread = new Thread(new ThreadStart(ButtonCheckProcedure));
            buttonCheckThread.Start();
        }
        private void stopButtonCheck()
        {
            if (buttonCheckThread != null)
            {
                buttonCheckThread.Abort();
                buttonCheckThread = null;
            }
        }
        private void ButtonCheckProcedure()
        {
            DateTime buttonPressed = DateTime.Now;
            bool isPressed = false;
            bool longPressClicked = false;
            int buttonStatus = 0;
            int threshold = Config.getInstance().getButtonClickThreshold();
            while (true)
            {
                bool pressed = ButtonManager.getInstance().checkButtonPressed();
                if (pressed)
                    buttonStatus++;
                else
                    buttonStatus--;

                if (isPressed && buttonStatus > threshold && !longPressClicked)
                {
                    TimeSpan elapse = DateTime.Now - buttonPressed;
                    if (elapse.TotalSeconds > 3)
                    {
                        longPressClicked = true;
                        ButtonClicked(true);
                    }
                }
                if (!isPressed && buttonStatus == threshold)
                {
                    isPressed = true;
                    buttonPressed = DateTime.Now;
                }
                if (isPressed && buttonStatus == 0)
                {
                    isPressed = false;
                    longPressClicked = false;
                    ButtonClicked();
                }

                if (buttonStatus > threshold)
                    buttonStatus = threshold;
                if (buttonStatus < 0)
                    buttonStatus = 0;

                Thread.Sleep(30);
            }
        }
        private void ButtonClicked(bool longPress = false)
        {
            if (ignoreInput)
            {
                return;
            }

            if (longPress)
            {
                if (status == STATUS_RECORD)
                {
                    btnSpeak.Invoke(new Action(() =>
                    {
                        btnSpeak.PerformClick();
                    }));
                }
            }
            else if (status == STATUS_WAIT)
            {
                TimeSpan elapse = DateTime.Now - lastButtonClicked;
                if (elapse.TotalSeconds > 10)
                {

                    PlayMP3("Audios/StartAudio.mp3", (s, e) =>
                    {
                        lastButtonClicked = DateTime.Now;
                        if (source == "SelectScreen")
                        {
                            startKillerThread();
                        }
                        File.Delete("Audios/StartAudio.mp3.mp3");
                    });
                }
                else
                {
                    stopKillerThread();
                    btnSpeak.Invoke(new Action(() =>
                    {
                        btnSpeak.PerformClick();
                    }));
                }
            }
            else if (status == STATUS_RECORD)
            {
                PauseRecording();
            }
            else if (status == STATUS_PAUSE)
            {
                UnpauseRecording();
            }
        }
        private Thread killerThread = null;
        private void startKillerThread()
        {
            stopKillerThread();
            killerThread = new Thread(new ThreadStart(KillProc));
            killerThread.Start();
        }
        private void stopKillerThread()
        {
            if (killerThread != null)
            {
                killerThread.Abort();
                killerThread = null;
            }
        }
        private void KillProc()
        {
            try
            {
                Thread.Sleep(10 * 1000);
                if (status == STATUS_WAIT)
                    Invoke(new Action(() => 
                    {
                        Close();
                    }));
            }
            catch (ThreadAbortException) { }
            catch (Exception) { }
        }

        private void PlayMP3(string mp3File, EventHandler<StoppedEventArgs> stopHandler = null, int volume = -1)
        {
            DisposeAudioPlayer();
            this.Invoke(new Action(() =>
            {
                ignoreInput = true;
                waveOutDevice = new WaveOut();
                audioFileReader = new AudioFileReader(mp3File);
                waveOutDevice.Init(audioFileReader);

                if (volume == -1)
                    waveOutDevice.Volume = 1f;
                else
                    waveOutDevice.Volume = volume * 1f / Config.getInstance().getDefaultVolume();

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
                Thread.Sleep(Config.getInstance().getFlashPeriod());
                if (ledStatus)
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
            if (ignoreInput)
            {
                return;
            }
            if (status == STATUS_WAIT)
            {
                StartRecording();
            }
            else if (status == STATUS_RECORD || status == STATUS_PAUSE)
            {
                StopRecording();
            }
        }
        private void StartRecording()
        {
            InsertLog("Start");
            status = STATUS_RECORD;
            ignoreInput = true;

            Application.UseWaitCursor = true;

            btnSpeak.Image = Properties.Resources.stop;
            btnSpeak.Text = "STOP IT";

            OBSManager.StartOBSRecording(witness);
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
                DateTime tzNow = TimeManage.getCurrentTime();
                SpeechSynthesizer stopAudio = new SpeechSynthesizer();
                stopAudio.SpeakAsync(Config.getInstance().getAnnounceTime() + tzNow.ToString(" h:mm tt"));
                stopAudio.SpeakCompleted += (ss, ee) =>
                {
                    int elapSec = (int)(DateTime.Now - lastLogTime).TotalSeconds;
                    SpeechSynthesizer totalAudio = new SpeechSynthesizer();
                    totalAudio.SpeakAsync(Config.getInstance().getEndTimeTemplate() + buildElapsedTimeString(totalSeconds + elapSec));
                    totalAudio.SpeakCompleted += (sss, eee) =>
                    {
                        ignoreInput = false;

                        btnSpeak.Image = Properties.Resources.play;
                        btnSpeak.Text = "SPEAK IT";

                        DialogResult = DialogResult.OK;
                        Close();
                    };
                };
            });
        }
        private void PauseRecording()
        {
            InsertLog("Off");                                                                                                                                                                           
            status = STATUS_PAUSE;
            PlayMP3("Audios/PauseAudio.mp3", (s, e) =>
            {
                ignoreInput = true;
                DisposeAudioPlayer();
                DateTime tzNow = TimeManage.getCurrentTime();
                SpeechSynthesizer pauseAudio = new SpeechSynthesizer();
                pauseAudio.SpeakAsync(Config.getInstance().getAnnounceTime() + tzNow.ToString(" h:mm tt"));
                pauseAudio.SpeakCompleted += (ss, ee) =>
                {
                    Thread.Sleep(1000);
                    OBSManager.PauseOBSRecording(witness);
                    startLEDFlash();
                    startAlertThread();

                    SpeechSynthesizer totalAudio = new SpeechSynthesizer();
                    totalAudio.SpeakAsync(Config.getInstance().getTotalTimeTemplate() + buildElapsedTimeString(totalSeconds));
                    totalAudio.SpeakCompleted += (sss, eee) =>
                    {
                        ignoreInput = false;
                    };
                };
            });
        }
        private void UnpauseRecording()
        {
            InsertLog("On");
            status = STATUS_RECORD;
            OBSManager.UnpauseOBSRecording(witness);
            solidLED();
            stopAlertThread();
            Thread.Sleep(1000);
            PlayMP3("Audios/UnpauseAudio.mp3", (s, e) =>
            {
                ignoreInput = true;
                DisposeAudioPlayer();
                DateTime tzNow = TimeManage.getCurrentTime();
                SpeechSynthesizer unpauseAudio = new SpeechSynthesizer();
                unpauseAudio.SpeakAsync(Config.getInstance().getAnnounceTime() + tzNow.ToString(" h:mm tt"));
                unpauseAudio.SpeakCompleted += (ss, ee) =>
                {
                    ignoreInput = false;
                };
            });
        }

        private Thread alertThread = null;
        private void startAlertThread()
        {
            stopAlertThread();
            alertThread = new Thread(new ThreadStart(AlertProc));
            alertThread.Start();
        }
        private void stopAlertThread()
        {
            if(alertThread != null)
            {
                alertThread.Abort();
                alertThread = null;
            }
        }
        private void AlertProc()
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(Config.getInstance().getAlertInterval() * 1000);
                    PlayMP3("Audios/BreakAlertAudio.mp3", null, Config.getInstance().getAlertVolume());
                }
            }
            catch (ThreadAbortException) { }
            catch (Exception) { }
        }


        private void GenerateGoogleTTS()
        {
            DateTime tzNow = TimeManage.getCurrentTime();
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
        private void GenerateStartAudio(string witness, string type)
        {
            SynthesisInput input = new SynthesisInput
            {
                Text = Config.getInstance().getStartTemplate().Replace("#Witness Type#", type).Replace("#Witness#", witness).Replace("30B6", "Thirty B 6")
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
            using (Stream output = File.Create("Audios/StartAudio.mp3"))
            {
                response.AudioContent.WriteTo(output);
            }
        }

        private string buildElapsedTimeString(int totalSec)
        {
            string elapse = " ";
            if (totalSec > 3600)
            {
                elapse += String.Format("{0} hours", totalSec / 3600);
                totalSec %= 3600;
                if (totalSec > 0)
                    elapse += ", ";
            }
            if (totalSec > 60)
            {
                elapse += String.Format("{0} minutes", totalSec / 60);
                totalSec %= 60;
                if (totalSec > 0)
                    elapse += " and ";
            }
            if (totalSec > 0)
                elapse += String.Format("{0} seconds", totalSec);
            return elapse;
        }
    }
}
