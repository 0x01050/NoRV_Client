using System;
using System.Threading;
using System.Windows.Forms;

namespace NoRV
{
    public partial class WaitScreen : Form
    {
        public WaitScreen()
        {
            InitializeComponent();
            Thread thread = new Thread(new ThreadStart(OBSCheck));
            thread.Start();
        }
        private void OBSCheck()
        {
            while(!OBSManager.CheckOBSRunning())
            {
                Thread.Sleep(500);
            }
            Invoke(new Action(() => Close()));
        }
    }
}
