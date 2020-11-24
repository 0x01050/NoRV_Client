using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace NoRV
{
    public partial class ControlForm : Form
    {
        private static ControlForm _instance = null;
        public static ControlForm getInstance()
        {
            if (_instance == null)
                _instance = new ControlForm();
            return _instance;
        }


        public ControlForm()
        {
            InitializeComponent();
        }

        private NotifyIcon notifyIcon = new NotifyIcon();
        private void ControlForm_Load(object sender, EventArgs e)
        {
            MenuItem exitMenuItem = new MenuItem("Exit", new EventHandler(Exit));

            notifyIcon.Icon =Properties.Resources.icon;
            notifyIcon.ContextMenu = new ContextMenu(new MenuItem[] { exitMenuItem });
            notifyIcon.Visible = true;

            Visible = false;
            WindowState = FormWindowState.Minimized;
            ShowInTaskbar = false;
        }
        void Exit(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure to exit now?", "NoRV", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;
            Close();
        }

        private void ControlForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            notifyIcon.Visible = false;
            if(!Utils.MainFormClosed(_mainForm))
            {
                _mainForm.CancelRecording();
            }
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
        public bool getIgnorable()
        {
            if(!Utils.MainFormClosed(_mainForm))
            {
                return _mainForm.isIgnoreInput();
            }
            return false;
        }
        public bool getReadonFinished()
        {
            if(!Utils.MainFormClosed(_mainForm))
            {
                return _mainForm.isReadonFinished();
            }
            return false;
        }
        public string getRunningTime()
        {
            if (!Utils.MainFormClosed(_mainForm))
            {
                return _mainForm.getRunningTime();
            }
            return "?";
        }
        public string getBreaksNumber()
        {
            if (!Utils.MainFormClosed(_mainForm))
            {
                return _mainForm.getBreaksNumber().ToString();
            }
            return "";
        }

        public bool loadDeposition(Dictionary<string, string> param)
        {
            try
            {
                if (getStatus() == AppStatus.STOPPED && Utils.MainFormClosed(_mainForm))
                {
                    Invoke(new Action(() => {
                        _mainForm = new MainScreen(param);
                        _mainForm.Show();
                    }));
                    return true;
                }
            }
            catch (Exception e)
            {
                Logger.info("Deposition Loading Failed On Context", e.Message);
            }
            return false;
        }
        public bool cancelDeposition()
        {
            try
            {
                if (getStatus() == AppStatus.LOADED && !Utils.MainFormClosed(_mainForm) && !_mainForm.isIgnoreInput())
                {
                    Invoke(new Action(() =>
                    {
                        _mainForm.CancelRecording();
                    }));
                    return true;
                }
            }
            catch (Exception e)
            {
                Logger.info("Deposition Starting Failed On Context", e.Message);
            }
            return false;
        }
        public bool startDeposition()
        {
            try
            {
                if (getStatus() == AppStatus.LOADED && !Utils.MainFormClosed(_mainForm) && !_mainForm.isIgnoreInput())
                {
                    Invoke(new Action(() =>
                    {
                        _mainForm.StartRecording();
                    }));
                    return true;
                }
            }
            catch (Exception e)
            {
                Logger.info("Deposition Starting Failed On Context", e.Message);
            }
            return false;
        }
        public bool pauseDeposition()
        {
            try
            {
                if (getStatus() == AppStatus.STARTED && !Utils.MainFormClosed(_mainForm) && !_mainForm.isIgnoreInput())
                {
                    Invoke(new Action(() =>
                    {
                        _mainForm.PauseRecording();
                    }));
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
                if (getStatus() == AppStatus.PAUSED && !Utils.MainFormClosed(_mainForm) && !_mainForm.isIgnoreInput())
                {
                    Invoke(new Action(() =>
                    {
                        _mainForm.ResumeRecording();
                    }));
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
                if (getStatus() == AppStatus.STARTED && !Utils.MainFormClosed(_mainForm) && !_mainForm.isIgnoreInput())
                {
                    Invoke(new Action(() =>
                    {
                        _mainForm.StopRecording();
                    }));
                    return true;
                }
            }
            catch (Exception e)
            {
                Logger.info("Deposition Stopping Failed On Context", e.Message);
            }
            return false;
        }

        private void ControlForm_Shown(object sender, EventArgs e)
        {
            Visible = false;
            WindowState = FormWindowState.Minimized;
            ShowInTaskbar = false;
        }
    }
}
