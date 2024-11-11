using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HttpKDA
{
	public class HttpKDA : IHttpHandler, System.Web.SessionState.IRequiresSessionState
	{

		private Stack<int> Stack
		{
			get
			{
				if (HttpContext.Current.Session["Stack"] == null)
					HttpContext.Current.Session["Stack"] = new Stack<int>();
				return (Stack<int>)HttpContext.Current.Session["Stack"];
			}
		}

		private int Result
		{
			get
			{
				if (HttpContext.Current.Session["Result"] == null)
					HttpContext.Current.Session["Result"] = 0;
				return (int)HttpContext.Current.Session["Result"];
			}
			set
			{
				HttpContext.Current.Session["Result"] = value;
			}
		}

		public void ProcessRequest(HttpContext context)
		{
			context.Response.ContentType = "application/json";
			string method = context.Request.HttpMethod;

			switch (method)
			{
				case "GET":
					HandleGet(context);
					break;
				case "POST":
					HandlePost(context);
					break;
				case "PUT":
					HandlePut(context);
					break;
				case "DELETE":
					HandleDelete(context);
					break;
				default:
					context.Response.StatusCode = 405;
					break;
			}
		}

		private void HandleGet(HttpContext context)
		{
			var response = new { Result = Result };
			context.Response.Write(JsonConvert.SerializeObject(response));
		}

		private void HandlePost(HttpContext context)
		{
			if (int.TryParse(context.Request["RESULT"], out int newResult))
			{
				Result = newResult;
				var response = new { Result = Result };
				context.Response.Write(JsonConvert.SerializeObject(response));
			}
			else
			{
				context.Response.StatusCode = 400;
			}
		}

		private void HandlePut(HttpContext context)
		{
			if (int.TryParse(context.Request["ADD"], out int valueToAdd))
			{
				Stack.Push(valueToAdd);
				Result += valueToAdd;
				var response = new { Result = Result };
				context.Response.Write(JsonConvert.SerializeObject(response));
			}
			else
			{
				context.Response.StatusCode = 400;
			}
		}

		private void HandleDelete(HttpContext context)
		{
			if (Stack.Count > 0)
			{
				int poppedValue = Stack.Pop();
				Result -= poppedValue;
				var response = new { Result = Result };
				context.Response.Write(JsonConvert.SerializeObject(response));
			}
			else
			{
				context.Response.StatusCode = 400;
			}
		}

		public bool IsReusable => false;
	}
}