using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NoRV
{
    public partial class Form3 : Form
    {
        public int selectedIdx = -1;
        public List<JObject> appointList = new List<JObject>();
        Thread thread = null;

        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            thread = new Thread(new ThreadStart(LoadAppointments));
            thread.Start();
        }

        private void LoadAppointments()
        {
            try
            {
                var httpClient = new HttpClient()
                {
                    Timeout = new TimeSpan(0, 0, 5)
                };
                httpClient.DefaultRequestHeaders.Add("ContentType", "application/json");
                var apiKeyPwd = Encoding.UTF8.GetBytes("19487502:30fce42b9991bbd32cea500a49c7d3b9");
                string basicAuth = Convert.ToBase64String(apiKeyPwd);
                httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + basicAuth);

                var method = new HttpMethod("GET");
                string now = DateTime.Now.ToString("yyyy-MM-dd");
                HttpResponseMessage response = httpClient.GetAsync("https://acuityscheduling.com/api/v1/appointments?direction=ASC&minDate=" + now + "&maxDate=" + now).Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string content = response.Content.ReadAsStringAsync().Result;
                    object respObj = JsonConvert.DeserializeObject(content);
                    if(respObj is JArray respArray)
                    {
                        Invoke(new Action(() =>
                        {
                            lstAppointments.Items.Clear();
                        }));
                        appointList.Clear();
                        selectedIdx = -1;

                        string mapping = File.ReadAllText("mapping.txt");
                        string[] keyMaps = mapping.Split(new char[] { ';' });
                        string machineIDKey = "";
                        foreach(string key in keyMaps)
                        {
                            string[] names = key.Split(new char[] { ',' });
                            if (names.Length == 2 && names[1] == "MachineID")
                                machineIDKey = names[0];
                        }
                        foreach (JObject appointItem in respArray)
                        {
                            if(appointItem.ContainsKey("datetime"))
                            {
                                string datetime = appointItem.GetValue("datetime").ToString();
                                string NoRVID = "";

                                if (appointItem.ContainsKey("forms") && appointItem.GetValue("forms") is JArray forms && forms.Count > 0)
                                {
                                    dynamic info = forms.ToArray<dynamic>()[0];
                                    if (info.values != null && info.values is JArray)
                                    {
                                        dynamic[] infos = ((JArray)info.values).ToArray<dynamic>();
                                        foreach (dynamic oneInfo in infos)
                                        {
                                            if (oneInfo.value != null && oneInfo.name != null)
                                            {
                                                if (oneInfo.name == machineIDKey)
                                                {
                                                    NoRVID = oneInfo.value;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                if(NoRVID == L.v())
                                {
                                    Invoke(new Action(() =>
                                    {
                                        lstAppointments.Items.Add(datetime);
                                    }));
                                    appointList.Add(appointItem);
                                }
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show(response.ReasonPhrase);
                    Close();
                }
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception e)
            {
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

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(thread != null)
            { 
                thread.Abort();
                thread = null;
            }
        }
    }
}
