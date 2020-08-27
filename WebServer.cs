using Grapevine.Interfaces.Server;
using Grapevine.Server;
using Grapevine.Server.Attributes;
using Grapevine.Shared;
using System;

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

		public static int tzOffset = 0;
		[RestRoute(HttpMethod = HttpMethod.GET, PathInfo = "/api/time")]
		public IHttpContext Time(IHttpContext context)
		{
			context.Response.AddHeader("Access-Control-Allow-Origin", "*");
			context.Response.SendResponse(DateTime.UtcNow.AddHours(tzOffset).ToString("MMM d,yyyy h:mm:ss tt"));
			return context;
		}
		[RestRoute(HttpMethod = HttpMethod.GET, PathInfo = "/getTranscripts")]
		public IHttpContext getTranscripts(IHttpContext context)
		{
			var lastTimestamp = context.Request.QueryString["lastTimestamp"];
			context.Response.SendResponse(TranscribeManager.getTranscripts(Int64.Parse(lastTimestamp)));
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
