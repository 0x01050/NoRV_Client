using Grapevine.Interfaces.Server;
using Grapevine.Server;
using Grapevine.Server.Attributes;
using Grapevine.Shared;
using System;
using System.Collections.Generic;
using System.Web;

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
			context.Response.AddHeader("Access-Control-Allow-Origin", "*");
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



		[RestRoute(HttpMethod = HttpMethod.GET, PathInfo = "")]
		public IHttpContext root(IHttpContext context)
		{
			context.Response.SendResponse("");
			return context;
		}

	}
}
