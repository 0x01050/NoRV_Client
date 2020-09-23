using Emgu.CV.Aruco;
using Grapevine.Interfaces.Server;
using Grapevine.Server;
using Grapevine.Server.Attributes;
using Grapevine.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

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
			context.Response.SendResponse(NoRVAppContext.getInstance().getStatus().ToString());
			return context;
        }
		[RestRoute(HttpMethod = HttpMethod.POST, PathInfo = "/loadDeposition")]
		public IHttpContext loadDeposition(IHttpContext context)
		{
			try
			{
				string payload = context.Request.Payload;
				Dictionary<string, string> param = JsonConvert.DeserializeObject<Dictionary<string, string>>(payload);
				if (NoRVAppContext.getInstance().loadDeposition(param))
				{
					context.Response.SendResponse("Loading Succeessed");
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
				if (NoRVAppContext.getInstance().startDeposition())
				{
					context.Response.SendResponse("Starting Succeessed");
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
				if (NoRVAppContext.getInstance().pauseDeposition())
				{
					context.Response.SendResponse("Pausing Succeessed");
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
				if (NoRVAppContext.getInstance().resumeDeposition())
				{
					context.Response.SendResponse("Resuming Succeessed");
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
				if (NoRVAppContext.getInstance().stopDeposition())
				{
					context.Response.SendResponse("Stopping Succeessed");
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
