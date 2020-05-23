using Google.Apis.Auth.OAuth2;
using Google.Cloud.TextToSpeech.V1;
using Grpc.Auth;
using Grpc.Core;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
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
                    {"PDT (GMT -7)", -7},
                    {"EDT (GMT -4)", -4},
                    {"CDT (GMT -5)", -5},
                    {"MDT (GMT -6)", -6},
                    {"ADT (GMT -8)", -8},
                    {"HAST (GMT -10)", -10}
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

            string template = File.ReadAllText("template.txt");
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
                MessageBox.Show("No available voice", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
            cbVoice.SelectedIndex = 1;
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
            btnSpeak.Image = Properties.Resources.stop;
            btnSpeak.Text = "STOP IT";
            status = "speak";

            sound();

            waveOutDevice = new WaveOut();
            audioFileReader = new AudioFileReader("tts.mp3");
            waveOutDevice.Init(audioFileReader);
            waveOutDevice.Play();
            waveOutDevice.PlaybackStopped += Stop;
        }

        private void Stop(object sender = null, EventArgs e = null)
        {
            btnSpeak.Image = Properties.Resources.play;
            btnSpeak.Text = "SPEAK IT";
            status = "stop";

            if (waveOutDevice != null)
            {
                waveOutDevice.Stop();
            }
            if(audioFileReader != null)
            {
                audioFileReader.Dispose();
                audioFileReader = null;
            }
            if (waveOutDevice != null)
            {
                waveOutDevice.Dispose();
                waveOutDevice = null;
            }

            File.Delete("tts.mp3");
        }

        private void btnSpeak_Click(object sender, EventArgs e)
        {
            if(status == "stop")
            {
                Play();
            }
            else if(status == "speak")
            {
                Stop();
            }
        }

        private void sound()
        {
            DateTime tzNow = DateTime.UtcNow.AddHours(offset);

            // Set the text input to be synthesized.
            SynthesisInput input = new SynthesisInput
            {
                Text = voiceText.Replace(this.lastTime, tzNow.ToString("h:mm tt"))
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
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Stop();
        }
    }
}
