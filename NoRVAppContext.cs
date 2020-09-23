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
        private MainScreen _mainForm = null;
        private void closeMainForm()
        {
            if (_mainForm != null)
            {
                _mainForm.Close();
            }
            _mainForm = null;
        }
        public bool loadDeposition(Dictionary<string, string> param)
        {
            try
            {
                if (getStatus() == AppStatus.STOPPED && Utils.MainFormClosed(_mainForm))
                {
                    _mainForm = new MainScreen(param);
                    _mainForm.Show();
                    return true;
                }
            }
            catch(Exception e)
            {
                Logger.info("Deposition Loading Failed On Context", e.Message);
            }
            return false;
        }
        public bool startDeposition()
        {
            try
            {
                if(getStatus() == AppStatus.LOADED && !Utils.MainFormClosed(_mainForm) && _mainForm.StartRecording())
                {
                    return true;
                }
            }
            catch(Exception e)
            {
                Logger.info("Deposition Starting Failed On Context", e.Message);
            }
            return false;
        }
        public bool pauseDeposition()
        {
            try
            {
                if (getStatus() == AppStatus.STARTED && !Utils.MainFormClosed(_mainForm) && _mainForm.PauseRecording())
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                Logger.info("Deposition Pausing Failed On Context", e.Message);
            }
            return false;
        }
        public bool resumeDeposition()
        {
            try
            {
                if (getStatus() == AppStatus.PAUSED && !Utils.MainFormClosed(_mainForm) && _mainForm.ResumeRecording())
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                Logger.info("Deposition Resuming Failed On Context", e.Message);
            }
            return false;
        }
        public bool stopDeposition()
        {
            try
            {
                if (getStatus() == AppStatus.STARTED && !Utils.MainFormClosed(_mainForm) && _mainForm.StopRecording())
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                Logger.info("Deposition Stopping Failed On Context", e.Message);
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
