using Grapevine.Interfaces.Server;
using Grapevine.Server;
using Grapevine.Server.Attributes;
using Grapevine.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.UI.WebControls;

namespace NoRV
{
    [RestResource]
	public class WebServer
	{
		[RestRoute(HttpMethod = HttpMethod.GET, PathInfo = "/")]
		public IHttpContext index(IHttpContext context)
		{
			context.Response.Redirect("index.html");
			return context;
		}

		[RestRoute(HttpMethod = HttpMethod.GET, PathInfo = "/api/time")]
		public IHttpContext Time(IHttpContext context)
		{
			context.Response.SendResponse(TimeManage.getCurrentTime().ToString("MMM d,yyyy h:mm:ss tt"));
			return context;
		}
		[RestRoute(HttpMethod = HttpMethod.GET, PathInfo = "/getTranscripts")]
		public IHttpContext getTranscripts(IHttpContext context)
		{
			var lastTimestamp = context.Request.QueryString["lastTimestamp"];
			context.Response.SendResponse(TranscribeManager.getTranscripts(Int64.Parse(lastTimestamp)));
			return context;
		}


		// For NoRV Player
		[RestRoute(HttpMethod = HttpMethod.GET, PathInfo = "/getStatus")]
		public IHttpContext getStatus(IHttpContext context)
        {
			AppStatus status = ControlForm.getInstance().getStatus();
			bool reading = false;
			string totaltime = "?";
			string breaks = "?";

			if(status != AppStatus.STOPPED)
            {
				reading = ControlForm.getInstance().getIgnorable();
				if(status == AppStatus.PAUSED)
                {
					totaltime = ControlForm.getInstance().getRunningTime();
					breaks = ControlForm.getInstance().getBreaksNumber();
                }
            }

			context.Response.SendResponse(status + "," + reading + "," + totaltime + "," + breaks);
			return context;
        }
		[RestRoute(HttpMethod = HttpMethod.POST, PathInfo = "/loadDeposition")]
		public IHttpContext loadDeposition(IHttpContext context)
		{
			try
			{
				string payload = context.Request.Payload;
				var paramCollection = HttpUtility.ParseQueryString(payload);
				Dictionary<string, string> param = new Dictionary<string, string>();
				foreach (var k in paramCollection.AllKeys)
				{
					param.Add(k, paramCollection[k]);
				}
				if (ControlForm.getInstance().loadDeposition(param))
				{
					context.Response.SendResponse("Loading Succeed");
					return context;
				}
			}
			catch (Exception e)
            {
				Logger.info("Deposition Not Loaded On Webserver", e.Message);
			}
			context.Response.SendResponse("Loading Failed");
			return context;
        }
		[RestRoute(HttpMethod = HttpMethod.POST, PathInfo = "/startDeposition")]
		public IHttpContext startDeposition(IHttpContext context)
        {
			try
			{
				if (ControlForm.getInstance().startDeposition())
				{
					context.Response.SendResponse("Starting Succeed");
					return context;
				}
			}
			catch (Exception e)
			{
				Logger.info("Deposition Not Started On Webserver", e.Message);
			}
			context.Response.SendResponse("Starting Failed");
			return context;
		}
		[RestRoute(HttpMethod = HttpMethod.POST, PathInfo = "/cancelDeposition")]
		public IHttpContext cancelDeposition(IHttpContext context)
		{
			try
			{
				if (ControlForm.getInstance().cancelDeposition())
				{
					context.Response.SendResponse("Starting Succeed");
					return context;
				}
			}
			catch (Exception e)
			{
				Logger.info("Deposition Not Started On Webserver", e.Message);
			}
			context.Response.SendResponse("Starting Failed");
			return context;
		}
		[RestRoute(HttpMethod = HttpMethod.POST, PathInfo = "/pauseDeposition")]
		public IHttpContext pauseDeposition(IHttpContext context)
		{
			try
			{
				if (ControlForm.getInstance().pauseDeposition())
				{
					context.Response.SendResponse("Pausing Succeed");
					return context;
				}
			}
			catch (Exception e)
			{
				Logger.info("Deposition Not Paused On Webserver", e.Message);
			}
			context.Response.SendResponse("Pausing Failed");
			return context;
		}
		[RestRoute(HttpMethod = HttpMethod.POST, PathInfo = "/resumeDeposition")]
		public IHttpContext resumeDeposition(IHttpContext context)
		{
			try
			{
				if (ControlForm.getInstance().resumeDeposition())
				{
					context.Response.SendResponse("Resuming Succeed");
					return context;
				}
			}
			catch (Exception e)
			{
				Logger.info("Deposition Not Resumed On Webserver", e.Message);
			}
			context.Response.SendResponse("Resuming Failed");
			return context;
		}
		[RestRoute(HttpMethod = HttpMethod.POST, PathInfo = "/stopDeposition")]
		public IHttpContext stopDeposition(IHttpContext context)
		{
			try
			{
				if (ControlForm.getInstance().stopDeposition())
				{
					context.Response.SendResponse("Stopping Succeed");
					return context;
				}
			}
			catch (Exception e)
			{
				Logger.info("Deposition Not Stopped On Webserver", e.Message);
			}
			context.Response.SendResponse("Stopping Failed");
			return context;
		}
		
		[RestRoute(HttpMethod = HttpMethod.GET, PathInfo = "/getHistory")]
		public IHttpContext getHistory(IHttpContext context)
        {
			string case_name = "", witness_name = "";
			try
            {
				case_name = context.Request.QueryString["case_name"];
            }
			catch(Exception)
            {
				case_name = "";
			}
			try
			{
				witness_name = context.Request.QueryString["witness_name"];
			}
			catch (Exception)
			{
				witness_name = "";
			}

			int limit, page, pages = 0;

			try
            {
				limit = Int32.Parse(context.Request.QueryString["rows"]);
            }
			catch(Exception)
            {
				limit = 20;
            }
			if (limit <= 0)
				limit = 20;

			int total = HistoryManager.getInstance().getTotalCount(case_name, witness_name);
			if (total > 0)
				pages = (total + limit - 1) / limit;

			try
			{
				page = Int32.Parse(context.Request.QueryString["page"]);
			}
			catch (Exception)
			{
				page = 1;
			}
			if (page > pages)
				page = pages;
			if (page < 1)
				page = 1;

			object list = HistoryManager.getInstance().getHistory(page, limit, case_name, witness_name);

			context.Response.SendResponse(JsonConvert.SerializeObject(new Dictionary<string, object>
			{
				{ "records", total },
				{ "total", pages },
				{ "page", page },
				{ "rows", list }
			}));
			return context;
		}

		[RestRoute(HttpMethod = HttpMethod.GET, PathInfo = "/download")]
		public IHttpContext download(IHttpContext context)
		{
			string type = context.Request.QueryString["type"];
			string id = context.Request.QueryString["id"];

			string path = HistoryManager.getInstance().getByID(type, id);
			context.Response.ContentType = ContentType.CUSTOM_BINARY;
			context.Response.AddHeader("Content-Disposition", "inline; filename=" + type + (type == "video" ? ".mkv" : ".txt"));
			byte[] fileContent = new byte[0];
			if(!String.IsNullOrEmpty(path))
				fileContent = File.ReadAllBytes(path);
			context.Response.SendResponse(fileContent);
			return context;
		}


		[RestRoute(HttpMethod = HttpMethod.GET, PathInfo = "")]
		public IHttpContext root(IHttpContext context)
		{
			context.Response.SendResponse("");
			return context;
		}

	}
}
