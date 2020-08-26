using Grapevine.Interfaces.Server;
using Grapevine.Server;
using Grapevine.Server.Attributes;
using Grapevine.Shared;
using System;
using System.Net;

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
			var identity = context.User.Identity;
			if (identity is HttpListenerBasicIdentity)
			{
				HttpListenerBasicIdentity basicId = (HttpListenerBasicIdentity)identity;
				if (basicId.Name == "norv" && basicId.Password == "norv_2020!")
					context.Response.SendResponse("");
				else
				{
					context.Response.Redirect("/");
					context.Response.TrySendResponse(context.Server.Logger, Grapevine.Shared.HttpStatusCode.Unauthorized);
				}
			}
			else
			{
				context.Response.Redirect("/");
				context.Response.TrySendResponse(context.Server.Logger, Grapevine.Shared.HttpStatusCode.Unauthorized);
			}
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
