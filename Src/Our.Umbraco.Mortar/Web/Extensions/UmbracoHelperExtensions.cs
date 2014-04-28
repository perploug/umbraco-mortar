﻿using System;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Umbraco.Web;
using Umbraco.Web.Mvc;

namespace Our.Umbraco.Mortar.Web.Extensions
{
	internal static class UmbracoHelperExtensions
	{
		public static bool SurfaceControllerExists(this UmbracoHelper helper, string name, string actionName = "Index")
		{
			// Setup dummy route data
			var rd = new RouteData();
			rd.DataTokens.Add("area", "umbraco");
			rd.DataTokens.Add("umbraco", "true");

			// Setup dummy request context
			var rc = new RequestContext(
				new HttpContextWrapper(HttpContext.Current),
				rd);

			// Get controller factory
			var cf = ControllerBuilder.Current.GetControllerFactory();

			// Try and create the controller
			try
			{
				var ctrl = cf.CreateController(rc, name);
				if (ctrl == null)
					return false;

				var ctrlInstance = ctrl as SurfaceController;
				if (ctrlInstance == null)
					return false;

				foreach (var method in ctrlInstance.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
					.Where(x => typeof(ActionResult).IsAssignableFrom(x.ReturnType)))
				{
					if (method.Name == actionName)
					{
						return true;
					}

					var attr = method.GetCustomAttribute<ActionNameAttribute>();
					if (attr != null && attr.Name == actionName)
					{
						return true;
					}
				}

				return false;
			}
			catch (Exception ex)
			{
				return false;
			}
		}
	}
}

