using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NoRV
{
	class WebServer
	{
		private readonly HttpListener _listener = new HttpListener();

		private readonly Func<HttpListenerRequest, string> _responderMethod;

		public static int tzOffset = 0;

		public WebServer(string[] prefixes, Func<HttpListenerRequest, string> method)
		{
			if (!HttpListener.IsSupported)
			{
				throw new NotSupportedException("Needs Windows XP SP2, Server 2003 or later.");
			}
			if (prefixes == null || prefixes.Length == 0)
			{
				throw new ArgumentException("prefixes");
			}
			if (method == null)
			{
				throw new ArgumentException("method");
			}
			foreach (string uriPrefix in prefixes)
			{
				_listener.Prefixes.Add(uriPrefix);
			}
			_responderMethod = method;
			_listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
			_listener.Start();
		}

		public WebServer(Func<HttpListenerRequest, string> method, params string[] prefixes)
			: this(prefixes, method)
		{
		}

		public void Run()
		{
			ThreadPool.QueueUserWorkItem(delegate
			{
				try
				{
					while (_listener.IsListening)
					{
						ThreadPool.QueueUserWorkItem(delegate (object c)
						{
							HttpListenerContext httpListenerContext = c as HttpListenerContext;
							try
							{
								string s = _responderMethod(httpListenerContext.Request);
								byte[] bytes = Encoding.UTF8.GetBytes(s);
								httpListenerContext.Response.ContentLength64 = bytes.Length;
								httpListenerContext.Response.OutputStream.Write(bytes, 0, bytes.Length);
								httpListenerContext.Response.StatusCode = 200;
								httpListenerContext.Response.StatusDescription = "OK";
							}
							catch
							{
							}
							finally
							{
								httpListenerContext.Response.OutputStream.Close();
							}
						}, _listener.GetContext());
					}
				}
				catch
				{
				}
			});
		}

		public void Stop()
		{
			_listener.Stop();
			_listener.Close();
		}
		public static string SendResponse(HttpListenerRequest request)
		{
			try
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine(DateTime.UtcNow.AddHours(tzOffset).ToString("MMM d,yyyy h:mm:ss tt"));
				return stringBuilder.ToString();
			}
			catch (Exception)
			{
				return "";
			}
		}
	}
}
