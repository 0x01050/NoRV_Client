using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NoRV
{
    class TranscribeManager
    {
        private static string xmlFile = @"Transcribe.xml";
        public static XDocument Open(bool wait = true)
        {
            XDocument xml = null;
            for (int i = 0; (i < 3 && !wait || i < 5 && wait); i++)
            {
                try
                {
                    xml = XDocument.Load(xmlFile);
                    break;
                }
                catch (IOException) { }
                catch (Exception)
                {
                    break;
                }
            }
            return xml;
        }
        public static void Save(XDocument xml)
        {
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    xml.Save(xmlFile);
                    break;
                }
                catch (IOException) { }
                catch (Exception)
                {
                    break;
                }
            }
        }
        public static void Start()
        {
            try
            {
                var xml = Open();
                if (xml == null)
                    return;
                xml.Root.Descendants("Status").Remove();
                XElement status = new XElement("Status");
                status.Value = "Recording";
                xml.Root.Add(status);
                Clear(xml);
                Save(xml);
            }
            catch (Exception) { }
        }
        public static void Stop()
        {
            try
            {
                var xml = Open();
                if (xml == null)
                    return;
                xml.Root.Descendants("Status").Remove();
                XElement status = new XElement("Status");
                status.Value = "Offline";
                xml.Root.Add(status);
                Clear(xml);
                Save(xml);
            }
            catch (Exception) { }
        }
        public static void Clear(XDocument xml)
        {
            xml.Root.Descendants("Transcripts").Remove();
            XElement transcripts = new XElement("Transcripts");
            xml.Root.Add(transcripts);

            xml.Root.Descendants("Candidate").Remove();
            XElement candidate = new XElement("Candidate");
            xml.Root.Add(candidate);
        }

        public static string getStatus(XDocument xml)
        {
            XElement status = xml.Root.Element("Status");
            if (status == null)
            {
                status = new XElement("Status");
                status.Value = "Offline";
                xml.Root.Add(status);
            }
            return status.Value;
        }
        public static XElement getTranscripts(XDocument xml)
        {
            XElement transcripts = xml.Root.Element("Transcripts");
            if (transcripts == null)
            {
                transcripts = new XElement("Transcripts");
                xml.Root.Add(transcripts);
            }
            return transcripts;
        }
        public static XElement getCandidate(XDocument xml)
        {
            XElement candidate = xml.Root.Element("Candidate");
            if (candidate == null)
            {
                candidate = new XElement("Candidate");
                xml.Root.Add(candidate);
            }
            return candidate;
        }
        public static void setCandidate(XDocument xml, string transcript)
        {
            XElement candidate = getCandidate(xml);
            candidate.Value = transcript;
        }

        public static void NewTranscript(string transcript, List<string>[] candidates)
        {
            try
            {
                var xml = Open();
                if (xml == null)
                    return;
                if (getStatus(xml) == "Recording")
                {
                    Logger.info("New Transcript", transcript);
                    Int64 timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                    XElement newElem = new XElement("Transcript");
                    newElem.Add(new XAttribute("Timestamp", timestamp));
                    newElem.Value = JsonConvert.SerializeObject(candidates);

                    getTranscripts(xml).Add(newElem);
                    setCandidate(xml, "");

                    Save(xml);
                }
            }
            catch (Exception) { }
        }
        public static void NewCandidate(string transcript)
        {
            try
            {
                var xml = Open(false);
                if (xml == null)
                    return;
                if (getStatus(xml) == "Recording")
                {
                    setCandidate(xml, transcript);
                    Save(xml);
                }
            }
            catch (Exception) { }
        }
        public static string getTranscripts(Int64 lastTimestamp)
        {
            try
            {
                var xml = Open();
                if (xml == null)
                    return "{\"status\":\"error\"}";
                string status = getStatus(xml);
                if (status != "Recording")
                {
                    return "{\"status\":\"" + status + "\"}";
                }
                var transcripts = xml.Root.Descendants("Transcript")
                        .Where(x => (Int64)x.Attribute("Timestamp") > lastTimestamp)
                        .OrderBy(x => (Int64)x.Attribute("Timestamp"));
                XElement candidate = getCandidate(xml);
                List<Dictionary<string, string>> trans = new List<Dictionary<string, string>>();
                foreach (var transcript in transcripts)
                {
                    Dictionary<string, string> newDic = new Dictionary<string, string>();
                    newDic.Add("Timestamp", (string)transcript.Attribute("Timestamp"));
                    newDic.Add("Transcript", transcript.Value);
                    trans.Add(newDic);
                }

                Dictionary<string, object> result = new Dictionary<string, object>();
                result.Add("status", status);
                result.Add("transcripts", trans);
                result.Add("candidate", candidate.Value);
                return JsonConvert.SerializeObject(result);
            }
            catch (Exception)
            {
                return "{\"status\":\"error\"}";
            }
        }
    }
}
