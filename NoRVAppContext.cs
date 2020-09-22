using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace NoRV
{
    class NoRVAppContext : ApplicationContext
    {
        private static NoRVAppContext _instance = null;
        public static NoRVAppContext getInstance()
        {
            if (_instance == null)
                _instance = new NoRVAppContext();
            return _instance;
        }

        private AppStatus _status = AppStatus.STOPPED;
        public AppStatus getStatus()
        {
            return _status;
        }
        public void setStatus(AppStatus newStatus)
        {
            _status = newStatus;
        }
        public bool startDeposition(Dictionary<string, string> param)
        {
            try
            {
                if (getStatus() == AppStatus.STOPPED)
                {

                    return true;
                }
            }
            catch(Exception e)
            {
                Logger.info("Starting Deposition Failed", e.Message);
            }
            return false;
        }


        NotifyIcon notifyIcon = new NotifyIcon();
        NoRVAppContext()
        {
            MenuItem exitMenuItem = new MenuItem("Exit", new EventHandler(Exit));

            notifyIcon.Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            notifyIcon.ContextMenu = new ContextMenu(new MenuItem[] { exitMenuItem });
            notifyIcon.Visible = true;

        }
        void Exit(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure to exit now?", "NoRV", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;

            notifyIcon.Visible = false;

            ExitThread();
        }
    }
}
