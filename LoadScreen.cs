using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NoRV
{
    public partial class LoadScreen : Form
    {
        public int selectedIdx = -1;
        public List<JObject> appointList = new List<JObject>();
        private Thread loadThread = null;

        public LoadScreen()
        {
            InitializeComponent();
        }

        private void LoadScreen_Load(object sender, EventArgs e)
        {
            loadThread = new Thread(new ThreadStart(LoadAppointments));
            loadThread.Start();
        }

        private void LoadScreen_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (loadThread != null)
            {
                loadThread.Abort();
                loadThread = null;
            }
        }

        private void LoadAppointments()
        {
            try
            {
                Application.UseWaitCursor = true;
                List<JObject> jobs = JobManager.getJobs();
                foreach(JObject job in jobs)
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

                    if(datetime != "" && witness != "" && type != "")
                    {
                        lstAppointments.Invoke(new Action(() =>
                        {
                            lstAppointments.Items.Add(witness + " " + type + " " + datetime);
                        }));
                        appointList.Add(job);
                    }
                }
                Application.UseWaitCursor = false;
            }
            catch (ThreadAbortException) { Application.UseWaitCursor = false; }
            catch (Exception e)
            {
                Application.UseWaitCursor = false;
                MessageBox.Show(e.Message);
                Invoke(new Action(() => Close()));
            }

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
    }
}
