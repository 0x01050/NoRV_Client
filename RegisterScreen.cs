using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NoRV
{
    public partial class RegisterScreen : Form
    {
        Thread registerThread = null;

        public RegisterScreen()
        {
            InitializeComponent();
        }

        private void RegisterScreen_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(registerThread != null)
            {
                registerThread.Abort();
                registerThread = null;
            }
        }

        private void RegisterScreen_Load(object sender, EventArgs e)
        {
            txtID.Text = L.v();
            registerThread = new Thread(new ThreadStart(RegisterProc));
            registerThread.Start();
        }

        private void RegisterProc()
        {
            while(true)
            {
                try
                {

                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Add("ContentType", "application/json");
                    string url = Config.getInstance().getServerUrl() + "/machine/master/" + L.v();
                    HttpResponseMessage response = httpClient.GetAsync(url).Result;
                    if(response.StatusCode == HttpStatusCode.OK)
                    {
                        string content = response.Content.ReadAsStringAsync().Result;
                        var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
                        if(result["status"] == "ok")
                        {
                            Invoke(new Action(() =>
                            {
                                L.setID(result["id"]);
                                DialogResult = DialogResult.OK;
                                Close();
                            }));
                            return;
                        }
                    }
                }
                catch (ThreadAbortException)
                {
                    return;
                }
                catch (Exception) { }
                Thread.Sleep(5 * 1000);
            }
        }
    }
}
