using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;

namespace NoRV
{
    class JobManager
    {
        public static List<JObject> getJobs()
        {
            List<JObject> _jobs = new List<JObject>();
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
                                            if (oneInfo.name == Config.getInstance().getMachineIDKey())
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
                                _jobs.Add(appointItem);
                            }
                        }
                    }
                }
            }
            else
            {
                throw new Exception(response.StatusCode.ToString());
            }
            return _jobs;
        }
    }
}
