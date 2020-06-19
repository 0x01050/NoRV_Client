using System;
using System.Windows.Forms;

namespace NoRV
{
    static class Program
    {
        public static bool DEBUG = false;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            WebServer webServer = new WebServer(WebServer.SendResponse, "http://127.0.0.1:4001/now/");
            webServer.Run();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (!OBSManager.CheckOBSRunning())
                Application.Run(new WaitScreen());
            Application.Run(new InfoScreen());
        }
    }
}
