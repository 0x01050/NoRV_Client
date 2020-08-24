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
		private bool isHttps(string scheme)
		{
			return scheme == "https";
		}

		[RestRoute(HttpMethod = HttpMethod.GET, PathInfo = "/")]
		public IHttpContext index(IHttpContext context)
		{
			if (isHttps(context.Request.Url.Scheme))
				context.Response.Redirect("index.html");
			else
				context.Response.Redirect("norv.crt");
			return context;
		}
		[RestRoute(HttpMethod = HttpMethod.GET, PathInfo = "")]
		public IHttpContext root(IHttpContext context)
		{
			context.Response.SendResponse("");
			return context;
		}

		public static int tzOffset = 0;
		[RestRoute(HttpMethod = HttpMethod.GET, PathInfo = "/api/time")]
		public IHttpContext Time(IHttpContext context)
		{
			if (!isHttps(context.Request.Url.Scheme))
				return context;
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

	}
}
