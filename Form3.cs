using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace NoRV
{
    public partial class Form3 : Form
    {
        public int selectedIdx = -1;
        public List<JObject> appointList = new List<JObject>();
        private Thread loadThread = null;

        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            loadThread = new Thread(new ThreadStart(LoadAppointments));
            loadThread.Start();
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
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

                    if (job.ContainsKey("datetime"))
                    {
                        string datetime = job.GetValue("datetime").ToString();
                        lstAppointments.Invoke(new Action(() =>
                        {
                            lstAppointments.Items.Add(datetime);
                        }));
                        appointList.Add(job);
                    }
                }
                Application.UseWaitCursor = false;
            }
            catch (ThreadAbortException)
            {
                Application.UseWaitCursor = false;
                return;
            }
            catch (Exception e)
            {
                Application.UseWaitCursor = false;
                MessageBox.Show(e.Message);
                Close();
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
