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
		public static int tzOffset = 0;

		[RestRoute(HttpMethod = HttpMethod.GET, PathInfo = "/api/time")]
		public IHttpContext Time(IHttpContext context)
		{
			context.Response.SendResponse(DateTime.UtcNow.AddHours(tzOffset).ToString("MMM d,yyyy h:mm:ss tt"));
			return context;
		}

	}
}
