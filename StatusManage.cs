using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NoRV
{
    class StatusManage
    {
        private static StatusManage _instance = null;

        private PerformanceCounter theCPUCounter;
        private PerformanceCounter theMemCounter;

        private string usage = "";
        private string obs = "Awaiting Recording (Unknown)";
        private string button = "";
        private string depo = "Awaiting Start (Unknown)";
        private List<string[]> witness = new List<string[]>();

        StatusManage()
        {
            theCPUCounter = new PerformanceCounter("Process", "% Processor Time", Process.GetCurrentProcess().ProcessName);
            theMemCounter = new PerformanceCounter("Process", "Working Set - Private", Process.GetCurrentProcess().ProcessName);
        }
        public static StatusManage getInstance()
        {
            if (_instance == null)
                _instance = new StatusManage();
            return _instance;
        }
        public void reportBase()
        {
            try
            {
                string CPUUsage = Math.Round(theCPUCounter.NextValue(), 1) + "%";
                string RAMUsage = Math.Round(theMemCounter.NextValue() / 1048576, 1) + "MB";
                usage = CPUUsage + " / " + RAMUsage;
                button = ButtonManager.getInstance().getButtonStatus() == ButtonManager.INITIATED ? "Operational" : "Not Operational";
                string screenshot = Utils.CaptureScreen(Screen.PrimaryScreen);
                string joinedWitness = "";
                filterWitness();
                if (witness.Count > 0)
                    joinedWitness += formatWitness(witness[0]);
                for (int i = 1; i < witness.Count; i++)
                    joinedWitness += ", " + formatWitness(witness[i]);
                string jobs = witness.FindAll(x => (x != null && x.Length == 5 && x[3] == "")).Count.ToString();

                var httpClient = new HttpClient();
                string url = Config.getInstance().getServerUrl() + "/status/master";

                MultipartFormDataContent httpContent = new MultipartFormDataContent();
                httpContent.Add(new StringContent(L.v()), "id");
                httpContent.Add(new StringContent(usage), "usage");
                httpContent.Add(new StringContent(obs), "obs");
                httpContent.Add(new StringContent(button), "button");
                httpContent.Add(new StringContent(depo), "depo");
                httpContent.Add(new StringContent(jobs), "jobs");
                httpContent.Add(new StringContent(joinedWitness), "witness");
                httpContent.Add(new StringContent(screenshot), "screenshot");
                httpClient.PostAsync(url, httpContent);
            }
            catch (Exception e)
            {
                Logger.info("Base Reporting Failed", e.Message);
            }
        }

        public void changeOBS(string newOBS)
        {
            if (obs != newOBS)
            {
                obs = newOBS;
                reportBase();
            }
        }
        public void changeWitness(string id, string title, string time, int type)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string[] found = witness.Find(x => x[0] == id);
            if (found == null)
                witness.Add(new string[] { id, title, date, "", "" });
            else if (type == 1)
            {
                found[3] = time;
                found[4] = "Not concluded";
            }
            else if (type == 2)
            {
                found[4] = time;
            }
            else if (type == 3)
            {
                found[3] = "";
                found[4] = "";
            }
            else
                return;
            reportBase();
        }
        private void filterWitness()
        {
            witness.Sort((l, r) =>
            {
                if (l == null || l.Length != 5)
                    return 1;
                if (r == null || r.Length != 5)
                    return -1;
                if (l[3] == "")
                    return 1;
                if (r[3] == "")
                    return -1;
                return l[3].CompareTo(r[3]);
            });
            witness = witness.FindAll(x =>
            {
                if (x == null || x.Length != 5)
                    return false;
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                return x[2] == date;
            });
        }

        private string formatWitness(string[] onelog)
        {
            string curStr = "";
            if (onelog.Length > 1)
                curStr = onelog[1];
            if (onelog.Length > 3 && onelog[3] != "")
                curStr += " (" + onelog[3] + "/" + onelog[4] + ")";
            return curStr;
        }
    }
}
