using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;

namespace NoRV
{
    class JobManager
    {
        public static List<JObject> getJobs()
        {
            List<JObject> _jobs = new List<JObject>();

            var httpClient = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true })
            {
                Timeout = new TimeSpan(0, 0, 8)
            };
            httpClient.DefaultRequestHeaders.Add("ContentType", "application/json");
            var apiKeyPwd = Encoding.UTF8.GetBytes(Config.getInstance().getAucityAPIUser() + ":" + Config.getInstance().getAucityAPIPass());
            string basicAuth = Convert.ToBase64String(apiKeyPwd);
            httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + basicAuth);
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            string now = DateTime.Now.ToString("yyyy-MM-dd");
            if (Program.DEBUG)
                now = "2020-06-23";
            string url = Config.getInstance().getAucityAPIUrl().Replace("%MINDATE%", now).Replace("%MAXDATE%", now);
            HttpResponseMessage response = httpClient.GetAsync(url).Result;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                string content = response.Content.ReadAsStringAsync().Result;
                object respObj = JsonConvert.DeserializeObject(content);
                if (respObj is JArray respArray)
                {
                    foreach (JObject appointItem in respArray)
                    {
                        string jobID = "";
                        if(appointItem.ContainsKey("id"))
                        {
                            jobID = appointItem["id"].ToString();
                            if(checkFinishedJob(jobID))
                                continue;
                        }
                        if (appointItem.ContainsKey("datetime"))
                        {
                            string datetime = appointItem.GetValue("datetime").ToString();
                            string NoRVID = "";
                            string witness = "";

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
                                            }
                                            if(oneInfo.name == Config.getInstance().getWitnessKey())
                                            {
                                                witness = oneInfo.value;
                                            }
                                        }
                                    }
                                }
                            }
                            if (NoRVID == L.v())
                            {
                                Program.changeWitness(jobID, witness, "", 0);
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

        public static void FinishJob(string jobID)
        {
            removeUnnecessaryLog();
            if(!checkFinishedJob(jobID))
            {
                string now = DateTime.Now.ToString("yyyy-MM-dd");
                var xml = XDocument.Load(@"Log.xml");


                if(xml.XPathSelectElements(String.Format("//Group[@Date='{0}']", now)).Count() == 0)
                {
                    XElement newElem = new XElement("Group");
                    newElem.Add(new XAttribute("Date", now));
                    xml.Root.Add(newElem);
                    xml.Save(@"Log.xml");
                }
                var query = from c in xml.Root.Descendants("Group")
                        where (string)c.Attribute("Date") == now
                        select c;
                foreach(var item in query)
                {
                    XElement newElem = new XElement("Log");
                    newElem.Add(new XAttribute("ID", jobID));
                    item.Add(newElem);
                    xml.Save(@"Log.xml");
                }
            }
        }

        private static bool checkFinishedJob(string jobID)
        {
            string now = DateTime.Now.ToString("yyyy-MM-dd");
            var xml = XDocument.Load(@"Log.xml");
            return xml.XPathSelectElements(String.Format("//Group[@Date='{0}']/Log[@ID='{1}']", now, jobID)).Count() > 0;
        }

        private static void removeUnnecessaryLog()
        {
            string now = DateTime.Now.ToString("yyyy-MM-dd");
            var xml = XDocument.Load(@"Log.xml");
            xml.Root.Descendants("Group")
                    .Where(x => (string)x.Attribute("Date") != now)
                    .Remove();
            xml.Save(@"Log.xml");
        }
    }
}
