using AudioSwitcher.AudioApi.CoreAudio;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NoRV
{
    public partial class InfoScreen : Form
    {
        private Thread loadThread = null;
        private Thread volumeThread = null;
        public InfoScreen()
        {
            InitializeComponent();
        }

        private void InfoScreen_Load(object sender, EventArgs e)
        {
            txtNoRVMachineID.Text = L.v();
            ButtonManager.getInstance().turnOffLED();

            startLoadThread();
            startVolumeThread();
        }

        private void InfoScreen_FormClosing(object sender, FormClosingEventArgs e)
        {
            stopLoadThread();
            stopVolumeThread();
        }

        private void startLoadThread()
        {
            stopLoadThread();
            loadThread = new Thread(new ThreadStart(LoadAppointments));
            loadThread.Start();
        }
        private void stopLoadThread()
        {
            if (loadThread != null)
            {
                loadThread.Abort();
                loadThread = null;
            }
        }
        private void startVolumeThread()
        {
            stopVolumeThread();
            volumeThread = new Thread(new ThreadStart(VolumeCheck));
            volumeThread.Start();
        }
        private void stopVolumeThread()
        {
            if (volumeThread != null)
            {
                volumeThread.Abort();
                volumeThread = null;
            }
        }

        private string clickSource = "";
        private void btnNext_Click(object sender, EventArgs e)
        {
            string source = clickSource;
            clickSource = "";
            while(true)
            {
                if(!OBSManager.CheckOBSRunning())
                {
                    if(MessageBox.Show("Check if OBS is running please", "Warning", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                    {
                        return;
                    }
                }
                else
                {
                    break;
                }
            }

            Dictionary<string, string> InfoList = new Dictionary<string, string>();
            foreach (ComboBox cb in this.Controls.OfType<ComboBox>())
            {
                if (cb.SelectedItem != null)
                {
                    InfoList.Add(cb.Name, cb.SelectedItem.ToString());
                }
            }

            foreach (TextBox tb in this.Controls.OfType<TextBox>())
            {
                if (!String.IsNullOrEmpty(tb.Text))
                {
                    InfoList.Add(tb.Name, tb.Text);
                }
            }

            InfoList.Add("ID", lblJobID.Text);

            stopLoadThread();
            TranscribeManager.Start();
            MainScreen form = new MainScreen(InfoList, source);
            form.ShowDialog();
            if (form.DialogResult == DialogResult.OK && !String.IsNullOrEmpty(lblJobID.Text))
                JobManager.FinishJob(lblJobID.Text);
            TranscribeManager.Stop();
            lblJobID.Text = "";
            startLoadThread();
        }

        private void Validate(object sender, EventArgs e)
        {
            foreach (ComboBox cb in this.Controls.OfType<ComboBox>())
            {
                if (cb.SelectedItem == null)
                {
                    btnNext.Enabled = false;
                    return;
                }
            }

            foreach (TextBox tb in this.Controls.OfType<TextBox>())
            {
                if (String.IsNullOrEmpty(tb.Text))
                {
                    btnNext.Enabled = false;
                    return;
                }
            }
            btnNext.Enabled = true;
        }

        private void ClearFields()
        {
            foreach (ComboBox cb in this.Controls.OfType<ComboBox>())
            {
                cb.SelectedItem = null;
            }

            foreach (TextBox tb in this.Controls.OfType<TextBox>())
            {
                if(!tb.ReadOnly)
                {
                    tb.Text = String.Empty;
                }
            }
            btnNext.Enabled = false;
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            stopLoadThread();
            LoadScreen form = new LoadScreen();
            if(form.ShowDialog() == DialogResult.OK)
            {
                if(form.selectedIdx >= 0 && form.selectedIdx < form.appointList.Count)
                {
                    ClearFields();
                    JObject appointItem = form.appointList[form.selectedIdx];
                    SetInfo(appointItem);
                    
                }
            }
            startLoadThread();
        }

        private void selectJob(List<JObject> _jobs)
        {
            Invoke(new Action(() =>
            {
                stopLoadThread();
                SelectScreen form = new SelectScreen(_jobs.ToArray());
                if (form.ShowDialog() == DialogResult.OK)
                {
                    if (form.selectedIdx >= 0 && form.selectedIdx < form.appointList.Length)
                    {
                        ClearFields();
                        JObject appointItem = form.appointList[form.selectedIdx];
                        SetInfo(appointItem);
                        if (!btnNext.Enabled)
                            ButtonManager.getInstance().turnOffLED();
                        else
                        {
                            clickSource = "SelectScreen";
                            btnNext.PerformClick();
                        }
                    }
                }
                startLoadThread();
            }));
        }

        private void SetInfo(JObject appointItem)
        {
            Invoke(new Action(() =>
            {
                if(appointItem.ContainsKey("id"))
                {
                    lblJobID.Text = appointItem["id"].ToString();
                }
                if (appointItem.ContainsKey("forms") && appointItem.GetValue("forms") is JArray forms && forms.Count > 0)
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
                                for(int idx = 0; idx < keyList.Length; idx ++)
                                {
                                    string key = keyList[idx];
                                    string name = Config.getInstance().getPairName(idx);

                                    if (name == oneInfo.name.ToString())
                                    {
                                        if (key == "Witness")
                                        {
                                            Witness.Text = oneInfo.value;
                                        }
                                        if (key == "Template")
                                        {
                                            Template.SelectedItem = oneInfo.value.ToString().Trim();
                                        }
                                        if (key == "CaseName")
                                        {
                                            CaseName.Text = oneInfo.value;
                                        }
                                        if (key == "Counsel")
                                        {
                                            Counsel.SelectedItem = oneInfo.value.ToString().Trim();
                                        }
                                        if (key == "Address")
                                        {
                                            Address.Text = oneInfo.value;
                                        }
                                        if (key == "TimeZone")
                                        {
                                            TimeZone.SelectedItem = oneInfo.value.ToString().Trim();
                                        }
                                        //if (key == "Videographer")
                                        //{
                                        //    Videographer.Text = oneInfo.value;
                                        //}
                                        //if (key == "Commission")
                                        //{
                                        //    Commission.Text = oneInfo.value;
                                        //}
                                    }
                                }
                            }
                        }
                    }
                }

                Videographer.Text = Program.videographer;
                Commission.Text = Program.commission;
            }));
        }

        private void LoadAppointments()
        {
            int sleepTime = 10 * 1000;
            while (true)
            {
                sleepTime = 10 * 1000;
                try
                {
                    List<JObject> jobs = JobManager.getJobs();
                    sleepTime = 30 * 1000;
                    if (jobs.Count > 0)
                    {
                        selectJob(jobs);
                    }
                }
                catch (ThreadAbortException) { return; }
                catch (Exception) { }
                Thread.Sleep(sleepTime);
            }
        }

        private async void VolumeCheck()
        {
            int sleepTime = 1 * 1000;
            CoreAudioController controller = new CoreAudioController();
            while (true)
            {
                try
                {
                    if(controller.DefaultPlaybackDevice != null)
                    {
                        double vol = controller.DefaultPlaybackDevice.Volume;
                        if (vol != Config.getInstance().getDefaultVolume())
                        {
                            await controller.DefaultPlaybackDevice.SetVolumeAsync(Config.getInstance().getDefaultVolume());
                        }
                    }
                }
                catch (ThreadAbortException)
                {
                    return;
                }
                catch (Exception)
                {

                }
                Thread.Sleep(sleepTime);
            }
        }
    }
}
