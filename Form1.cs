using AudioSwitcher.AudioApi.CoreAudio;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace NoRV
{
    public partial class Form1 : Form
    {
        private Thread loadThread = null;
        private Thread volumeThread = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            txtNoRVMachineID.Text = L.v();
            ButtonManager.getInstance().turnOffLED();

            startLoadThread();
            startVolumeThread();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
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

        private void btnNext_Click(object sender, EventArgs e)
        {
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

            stopLoadThread();
            Form2 form = new Form2(InfoList);
            form.ShowDialog();
            startLoadThread();
            ButtonManager.getInstance().turnOffLED();
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
            Form3 form = new Form3();
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

        private void SetInfo(JObject appointItem)
        {
            Invoke(new Action(() =>
            {
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

                                    if (key == oneInfo.name.ToString())
                                    {
                                        if (name == "Witness")
                                        {
                                            Witness.Text = oneInfo.value;
                                        }
                                        if (name== "Template")
                                        {
                                            Template.SelectedItem = oneInfo.value.ToString().Trim();
                                        }
                                        if (name == "CaseName")
                                        {
                                            CaseName.Text = oneInfo.value;
                                        }
                                        if (name == "Counsel")
                                        {
                                            Counsel.SelectedItem = oneInfo.value.ToString().Trim();
                                        }
                                        if (name == "Address")
                                        {
                                            Address.Text = oneInfo.value;
                                        }
                                        if (name == "TimeZone")
                                        {
                                            TimeZone.SelectedItem = oneInfo.value.ToString().Trim();
                                        }
                                        if (name == "Videographer")
                                        {
                                            Videographer.Text = oneInfo.value;
                                        }
                                        if (name == "Commission")
                                        {
                                            Commission.Text = oneInfo.value;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }));
        }

        private void LoadAppointments()
        {
            int sleepTime = 10 * 10000;
            while (true)
            {
                sleepTime = 10 * 10000;
                try
                {
                    List<JObject> jobs = JobManager.getJobs();
                    if(jobs.Count > 0)
                    {
                        SetInfo(jobs[0]);
                        btnNext.Invoke(new Action(() =>
                        {
                            btnNext.PerformClick();
                        }));
                        sleepTime = 30 * 10000;
                    }
                }
                catch(ThreadAbortException)
                {
                    return;
                }
                catch(Exception)
                {
                }
                Thread.Sleep(sleepTime);
            }
        }

        private void VolumeCheck()
        {
            int sleepTime = 1 * 1000;
            while (true)
            {
                try
                {
                    CoreAudioDevice defaultPlaybackDevice = new CoreAudioController().DefaultPlaybackDevice;
                    double vol = defaultPlaybackDevice.Volume;
                    if(vol != Config.getInstance().getDefaultVolume())
                    {
                        defaultPlaybackDevice.Volume = Config.getInstance().getDefaultVolume();
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
