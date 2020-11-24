using Google.Cloud.TextToSpeech.V1;
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
        private string witness = "";
        private string casename = "";

        private string voiceText = "";
        private double speed = 0.9;
        private double pitch = 0.0;
        private string lastTime = "#Time#";

        IWavePlayer waveOutDevice = null;
        AudioFileReader audioFileReader = null;

        private bool ignoreInput = false;
        private bool readonFinished = false;

        public MainScreen(Dictionary<string, string> InfoList = null)
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
                if (this.InfoList.ContainsKey("Witness"))
                    witness = this.InfoList["Witness"];
                if (this.InfoList.ContainsKey("CaseName"))
                    casename = this.InfoList["CaseName"];
                if (!this.InfoList.ContainsKey("Videographer"))
                    this.InfoList.Add("Videographer", Program.videographer);
                if (!this.InfoList.ContainsKey("Commission"))
                    this.InfoList.Add("Commission", Program.commission);
            }
            InitializeComponent();
        }
        private void MainScreen_Load(object sender, EventArgs evt)
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

            PlayMP3("Audios/LoadingAudio.mp3", (s, e) =>
            {
                ControlForm.getInstance().setStatus(AppStatus.LOADED);
            });

            Application.UseWaitCursor = false;
        }
        private void MainScreen_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(DialogResult == DialogResult.None)
            {
                e.Cancel = true;
                return;
            }

            DisposeAudioPlayer();

            stopAlertThread();

            ControlForm.getInstance().setStatus(AppStatus.STOPPED);
            InsertLog("End");

            OBSManager.StopOBSRecording(witness);

            if(DialogResult == DialogResult.OK)
                SaveLog();
        }

        // For Logging
        private List<string> logs = new List<string>();
        private string lastLog = "";
        private DateTime lastLogTime = DateTime.Now;
        private int totalSeconds = 0;
        private void LogInit()
        {
            string date = "";
            if (InfoList.ContainsKey("Date"))
                date = InfoList["Date"];
            logs.Add(witness + ". " + date);
            logs.Add("");
        }
        private void SaveLog()
        {
            string depoid = HistoryManager.getInstance().generateID();

            logs.Add("total running time =" + Utils.buildElapsedTimeString(totalSeconds));
            logs.Add(String.Format("total breaks = {0}", logs.Count - 3));

            string logpath = Path.GetFullPath(Config.getInstance().getLogPath() + "\\" + depoid + ".txt");
            try
            {
                File.WriteAllLines(Config.getInstance().getLogPath() + "\\" + depoid + ".txt", logs);
            }
            catch(Exception) { }

            string videopath = Path.GetFullPath(Config.getInstance().getVideoPath() + "\\" + depoid + ".mkv");
            DateTime waitForFinish = DateTime.Now;
            while(true)
            {
                // Wait for 30s for video file to be saved completely.
                if ((DateTime.Now - waitForFinish).TotalSeconds > 30)
                    break;
                try
                {
                    File.Move(Config.getInstance().getVideoPath() + "\\Video.mkv", Config.getInstance().getVideoPath() + "\\" + depoid + ".mkv");
                }
                catch(IOException)
                {
                    continue;
                }
                catch (Exception) { }
                break;
            }

            HistoryManager.getInstance().saveDepo(depoid, witness, casename, videopath, logpath);
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
                logs.Add(lastLog + " - " + action + " =" + Utils.buildElapsedTimeString(elapSec));
                totalSeconds += elapSec;
            }
        }
        public int getBreaksNumber()
        {
            return logs.Count - 2;
        }
        public string getRunningTime()
        {
            return Utils.buildSimpleElapsedTimeString(totalSeconds);
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
        private void PlayMP3(string mp3File, EventHandler<StoppedEventArgs> stopHandler = null, int volume = -1)
        {
            ignoreInput = true;

            DisposeAudioPlayer();

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
        }
        private void DisposeAudioPlayer()
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

        public bool isIgnoreInput()
        {
            return ignoreInput;
        }
        public bool isReadonFinished()
        {
            return readonFinished;
        }
        public void CancelRecording()
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
        public void StartRecording()
        {
            ignoreInput = true;
            ControlForm.getInstance().setStatus(AppStatus.STARTED);
            InsertLog("Start");

            Application.UseWaitCursor = true;
            OBSManager.StartOBSRecording(witness);
            Thread.Sleep(2000);
            Application.UseWaitCursor = false;

            GenerateGoogleTTS();
            PlayMP3("tts.mp3", (s, e) =>
            {
                File.Delete("tts.mp3");
                readonFinished = true;
            });
        }
        public void StopRecording()
        {
            ignoreInput = true;
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
                    totalAudio.SpeakAsync(Config.getInstance().getEndTimeTemplate() + Utils.buildElapsedTimeString(totalSeconds + elapSec));
                    totalAudio.SpeakCompleted += (sss, eee) =>
                    {
                        ignoreInput = false;

                        DialogResult = DialogResult.OK;
                        Close();
                    };
                };
            });
        }
        public void PauseRecording()
        {
            PlayMP3("Audios/PauseAudio.mp3", (s, e) =>
            {
                ignoreInput = true;
                DisposeAudioPlayer();
                DateTime tzNow = TimeManage.getCurrentTime();
                SpeechSynthesizer pauseAudio = new SpeechSynthesizer();
                pauseAudio.SpeakAsync(Config.getInstance().getAnnounceTime() + tzNow.ToString(" h:mm tt"));
                pauseAudio.SpeakCompleted += (ss, ee) =>
                {
                    ControlForm.getInstance().setStatus(AppStatus.PAUSED);
                    InsertLog("Off");

                    Application.UseWaitCursor = true;
                    Thread.Sleep(1000);
                    OBSManager.PauseOBSRecording(witness);
                    startAlertThread();
                    Application.UseWaitCursor = false;

                    SpeechSynthesizer totalAudio = new SpeechSynthesizer();
                    totalAudio.SpeakAsync(Config.getInstance().getTotalTimeTemplate() + Utils.buildElapsedTimeString(totalSeconds));
                    totalAudio.SpeakCompleted += (sss, eee) =>
                    {
                        ignoreInput = false;
                    };
                };
            });
        }
        public void ResumeRecording()
        {
            ControlForm.getInstance().setStatus(AppStatus.STARTED);
            InsertLog("On");
            
            Application.UseWaitCursor = true;
            OBSManager.UnpauseOBSRecording(witness);
            stopAlertThread();
            Thread.Sleep(1000);
            Application.UseWaitCursor = false;

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
            if (alertThread != null)
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
                    Invoke(new Action(() =>
                    {
                        PlayMP3("Audios/BreakAlertAudio.mp3", null, Config.getInstance().getAlertVolume());
                    }));
                }
            }
            catch (Exception e)
            {
                Logger.info("BreakAlert Exception", e.Message);
            }
        }

    }
}
