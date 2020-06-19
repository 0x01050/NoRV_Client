using Google.Apis.Auth.OAuth2;
using Google.Cloud.TextToSpeech.V1;
using Grpc.Auth;
using Grpc.Core;
using NAudio.Wave;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NoRV
{
    public partial class SelectScreen : Form
    {
        public int selectedIdx = -1;
        
        private JObject[] appointList = new JObject[0];

        private List<string> Witness = new List<string>();
        private List<string> Type = new List<string>();

        private Thread selectThread = null;

        public SelectScreen(JObject[] jobs)
        {
            appointList = jobs;
            InitializeComponent();
        }

        private void SelectScreen_Load(object sender, EventArgs e)
        {
            InitGoogleCredential();
            foreach (JObject job in appointList)
            {
                string datetime = "";
                string witness = "";
                string type = "";
                if (job.ContainsKey("datetime"))
                {
                    datetime = job.GetValue("datetime").ToString();
                }
                if (job.ContainsKey("forms") && job.GetValue("forms") is JArray forms && forms.Count > 0)
                {
                    dynamic info = forms.ToArray<dynamic>()[0];
                    if (info.values != null && info.values is JArray)
                    {
                        dynamic[] infos = ((JArray)info.values).ToArray<dynamic>();
                        string[] keyList = Config.getInstance().getKeyList();
                        foreach (dynamic oneInfo in infos)
                        {
                            if (oneInfo.value != null && oneInfo.name != null)
                            {
                                for (int idx = 0; idx < keyList.Length; idx++)
                                {
                                    string key = keyList[idx];
                                    string name = Config.getInstance().getPairName(idx);
                                    if (name == oneInfo.name.ToString())
                                    {
                                        if (key == "Witness")
                                        {
                                            witness = oneInfo.value;
                                        }
                                        if (key == "Template")
                                        {
                                            type = oneInfo.value.ToString().Trim();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                lstAppointments.Items.Add(witness + " " + type + " " + datetime);
                Witness.Add(witness);
                Type.Add(type);
            }
            startButtonCheck();
            startLEDFlash();
        }
        private void SelectScreen_FormClosing(object sender, FormClosingEventArgs e)
        {
            DisposeAudioPlayer();
            File.Delete("tts.mp3");
            stopSelectThread();
            stopButtonCheck();
            stopLEDFlash();
            if(DialogResult != DialogResult.OK)
                ButtonManager.getInstance().turnOffLED();
        }

        private void lstAppointments_DoubleClick(object sender, EventArgs e)
        {
            selectedIdx = lstAppointments.SelectedIndex;
            if(selectedIdx >= 0)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private Thread ledFlashThread = null;
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

        private GoogleCredential googleCredential;
        private Channel channel;
        private TextToSpeechClient client;
        private string voiceName = "";

        private void InitGoogleCredential()
        {
            using (Stream m = new FileStream("NoRV TTS-c4a3e2c55a4f.json", FileMode.Open))
                googleCredential = GoogleCredential.FromStream(m);
            channel = new Channel(TextToSpeechClient.DefaultEndpoint.Host,
                googleCredential.ToChannelCredentials());
            client = TextToSpeechClient.Create(channel);

            ListVoicesRequest voiceReq = new ListVoicesRequest { LanguageCode = "en-US" };
            ListVoicesResponse voiceResp = this.client.ListVoices(voiceReq);

            int idx = 0;
            foreach (Voice voice in voiceResp.Voices)
            {
                if (voice.LanguageCodes.Contains("en-US") && voice.Name.Contains("Wavenet"))
                {
                    if(idx <= 1)
                    {
                        voiceName = voice.Name;
                    }
                    idx++;
                }
            }
        }

        private void GenerateGoogleTTS(string witness, string type)
        {
            SynthesisInput input = new SynthesisInput
            {
                Text = "To start with " + witness + " " + type + " deposition, press button now."
            };
            VoiceSelectionParams voice = new VoiceSelectionParams
            {
                LanguageCode = "en-US",
                Name = voiceName
            };
            AudioConfig config = new AudioConfig
            {
                AudioEncoding = AudioEncoding.Mp3
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

        private void startSelectThread()
        {
            stopSelectThread();
            selectThread = new Thread(new ThreadStart(SelectProc));
            selectThread.Start();
        }
        private void stopSelectThread()
        {
            if(selectThread != null)
            {
                selectThread.Abort();
                selectThread = null;
            }
        }
        private void SelectProc()
        {
            int retry = 0;
            while(true)
            {
                try
                {
                    selectedIdx++;
                    if(selectedIdx >= appointList.Length)
                    {
                        selectedIdx = 0;
                        retry++;
                        if (retry >= 2)
                            break;
                    }

                    GenerateGoogleTTS(Witness[selectedIdx], Type[selectedIdx]);
                    PlayMP3("tts.mp3", (s, e) =>
                    {
                        File.Delete("tts.mp3");
                    });
                }
                catch (ThreadAbortException) { return; }
                catch (Exception) { }
                Thread.Sleep(8 * 1000);
            }
            selectedIdx = -1;
        }
        IWavePlayer waveOutDevice = null;
        AudioFileReader audioFileReader = null;
        private void PlayMP3(string mp3File, EventHandler<StoppedEventArgs> stopHandler = null)
        {
            DisposeAudioPlayer();
            this.Invoke(new Action(() =>
            {
                waveOutDevice = new WaveOut();
                audioFileReader = new AudioFileReader(mp3File);
                waveOutDevice.Init(audioFileReader);
                waveOutDevice.Volume = 1f;
                waveOutDevice.Play();
                waveOutDevice.PlaybackStopped += (s, e) =>
                {
                    DisposeAudioPlayer();
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


        private Thread buttonCheckThread = null;
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
            bool isPressed = false;
            int buttonStatus = 0;
            int threshold = Config.getInstance().getButtonClickThreshold();
            while (true)
            {
                bool pressed = ButtonManager.getInstance().checkButtonPressed();
                if (pressed)
                    buttonStatus++;
                else
                    buttonStatus--;

                if (!isPressed && buttonStatus == threshold)
                {
                    isPressed = true;
                }
                if (isPressed && buttonStatus == 0)
                {
                    isPressed = false;
                    ButtonClicked();
                }

                if (buttonStatus > threshold)
                    buttonStatus = threshold;
                if (buttonStatus < 0)
                    buttonStatus = 0;

                Thread.Sleep(30);
            }
        }
        private void ButtonClicked()
        {
            if(selectThread == null || !selectThread.IsAlive)
            {
                Invoke(new Action(() =>
                {
                    startSelectThread();
                }));
                return;
            }

            Invoke(new Action(() =>
            {
                if (selectedIdx >= 0 && selectedIdx <= lstAppointments.Items.Count)
                {
                    lstAppointments.SelectedIndex = selectedIdx;

                    DialogResult = DialogResult.OK;
                    Close();
                }
            }));
        }
    }
}
