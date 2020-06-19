using System;
using System.Threading;
using System.Windows.Forms;

namespace NoRV
{
    public partial class WaitScreen : Form
    {
        Thread obsCheckThread = null;
        public WaitScreen()
        {
            InitializeComponent();
            obsCheckThread = new Thread(new ThreadStart(OBSCheck));
            obsCheckThread.Start();
        }
        private void OBSCheck()
        {
            try
            {
                while (!OBSManager.CheckOBSRunning())
                {
                    Thread.Sleep(500);
                }
                Invoke(new Action(() => Close()));
            }
            catch(ThreadAbortException) { }
            catch(Exception) { }
        }

        private void WaitScreen_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(obsCheckThread != null)
            {
                obsCheckThread.Abort();
                obsCheckThread = null;
            }
        }
    }
}
