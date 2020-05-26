using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NoRV
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void btnNext_Click(object sender, EventArgs e)
        {
            while(true)
            {
                bool bFound = false;
                Process[] obs64 = Process.GetProcessesByName("obs64");
                foreach (Process obs in obs64)
                {
                    if(obs.MainWindowHandle != (IntPtr)0)
                    {
                        bFound = true;
                        break;
                    }
                }
                if(!bFound)
                {
                    if(MessageBox.Show("Check if OBS is running please", "Warning", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                    {
                        return;
                    }
                }
                else
                {
                    break;
                }
            }
            Dictionary<string, string> InfoList = new Dictionary<string, string>();
            foreach (ComboBox cb in this.Controls.OfType<ComboBox>())
            {
                if (cb.SelectedItem != null)
                {
                    InfoList.Add(cb.Name, cb.SelectedItem.ToString());
                }
            }

            foreach (TextBox tb in this.Controls.OfType<TextBox>())
            {
                if (!String.IsNullOrEmpty(tb.Text))
                {
                    InfoList.Add(tb.Name, tb.Text);
                }
            }

            Form2 form = new Form2(InfoList);
            form.ShowDialog();
            ButtonOff();
        }

        private void ButtonOff()
        {
            StringBuilder DeviceName = new StringBuilder(Delcom.MAXDEVICENAMELEN);
            if (Delcom.DelcomGetNthDevice(0, 0, DeviceName) == 0)
            {
                return;
            }
            uint deviceHandle = Delcom.DelcomOpenDevice(DeviceName, 0);
            if (deviceHandle == 0)
            {
                return;
            }
            const int ledColor = Delcom.GREENLED;
            Delcom.DelcomLEDControl(deviceHandle, ledColor, Delcom.LEDON);
            Delcom.DelcomLEDPower(deviceHandle, ledColor, 0);
            Delcom.DelcomCloseDevice(deviceHandle);
        }

        private void Validate(object sender, EventArgs e)
        {
            foreach (ComboBox cb in this.Controls.OfType<ComboBox>())
            {
                if (cb.SelectedItem == null)
                {
                    btnNext.Enabled = false;
                    return;
                }
            }

            foreach (TextBox tb in this.Controls.OfType<TextBox>())
            {
                if (String.IsNullOrEmpty(tb.Text))
                {
                    btnNext.Enabled = false;
                    return;
                }
            }
            btnNext.Enabled = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            txtNoRVMachineID.Text = L.v();
            ButtonOff();
            new Thread(new ThreadStart(LoadAppointments)).Start();
        }

        private void ClearFields()
        {
            foreach (ComboBox cb in this.Controls.OfType<ComboBox>())
            {
                cb.SelectedItem = null;
            }

            foreach (TextBox tb in this.Controls.OfType<TextBox>())
            {
                if(!tb.ReadOnly)
                {
                    tb.Text = String.Empty;
                }
            }
            btnNext.Enabled = false;
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            Form3 form = new Form3();
            if(form.ShowDialog() == DialogResult.OK)
            {
                if(form.selectedIdx >= 0 && form.selectedIdx < form.appointList.Count)
                {
                    ClearFields();
                    JObject appointItem = form.appointList[form.selectedIdx];
                    SetInfo(appointItem);
                    
                }
            }
        }

        private void SetInfo(JObject appointItem)
        {
            Invoke(new Action(() =>
            {
                if (appointItem.ContainsKey("forms") && appointItem.GetValue("forms") is JArray forms && forms.Count > 0)
                {
                    dynamic info = forms.ToArray<dynamic>()[0];
                    if (info.values != null && info.values is JArray)
                    {
                        dynamic[] infos = ((JArray)info.values).ToArray<dynamic>();

                        string mapping = File.ReadAllText("mapping.txt");
                        string[] keyMaps = mapping.Split(new char[] { ';' });
                        foreach (dynamic oneInfo in infos)
                        {
                            if (oneInfo.value != null && oneInfo.name != null)
                            {
                                foreach (string key in keyMaps)
                                {
                                    string[] names = key.Split(new char[] { ',' });
                                    if (names.Length == 2 && names[0] == oneInfo.name.ToString())
                                    {
                                        if (names[1] == "Witness")
                                        {
                                            Witness.Text = oneInfo.value;
                                        }
                                        if (names[1] == "Template")
                                        {
                                            Template.SelectedItem = oneInfo.value.ToString().Trim();
                                        }
                                        if (names[1] == "CaseName")
                                        {
                                            CaseName.Text = oneInfo.value;
                                        }
                                        if (names[1] == "Counsel")
                                        {
                                            Counsel.SelectedItem = oneInfo.value.ToString().Trim();
                                        }
                                        if (names[1] == "Address")
                                        {
                                            Address.Text = oneInfo.value;
                                        }
                                        if (names[1] == "TimeZone")
                                        {
                                            TimeZone.SelectedItem = oneInfo.value.ToString().Trim();
                                        }
                                        if (names[1] == "Videographer")
                                        {
                                            Videographer.Text = oneInfo.value;
                                        }
                                        if (names[1] == "Commission")
                                        {
                                            Commission.Text = oneInfo.value;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }));
        }

        private void LoadAppointments()
        {
            while (true)
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
                        if (respObj is JArray respArray)
                        {
                            JObject item = null;

                            string mapping = File.ReadAllText("mapping.txt");
                            string[] keyMaps = mapping.Split(new char[] { ';' });
                            string machineIDKey = "";
                            foreach (string key in keyMaps)
                            {
                                string[] names = key.Split(new char[] { ',' });
                                if (names.Length == 2 && names[1] == "MachineID")
                                    machineIDKey = names[0];
                            }
                            foreach (JObject appointItem in respArray)
                            {
                                if (appointItem.ContainsKey("datetime"))
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
                                    if (NoRVID == L.v())
                                    {
                                        if (item != null)
                                        {
                                            return;
                                        }
                                        item = appointItem;
                                    }
                                }
                            }

                            if (item != null)
                            {
                                Invoke(new Action(() =>
                                {
                                    SetInfo(item);
                                    btnNext.PerformClick();
                                }));
                                return;
                            }

                            if (item == null)
                            {
                                return;
                            }
                        }
                    }
                    else
                    {
                    }
                }
                catch (Exception)
                {
                }
                Thread.Sleep(10000);
            }
        }
    }
}
