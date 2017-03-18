using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CraftSynth.BuildingBlocks.UI.Web
{
	public class Misc
	{
        public static void AddOrUpdateQueryStringItemInUri(ref string uri, string key, string value)
        {
			string queryString = string.Empty;
			int index = uri.IndexOf('?');
			if (index > -1)
			{
				queryString = uri.Substring(index + 1);
			}

			AddOrUpdateQueryStringItemInQueryString(ref queryString, key, value);
			if (index > -1)
			{
				uri = uri.Substring(0, index) + "?" + queryString;
			}
			else
			{
				uri = uri + "?" + queryString;
			}
        }

		public static void AddOrUpdateQueryStringItemInQueryString(ref string queryString, string key, string value)
		{
			var qureyStringsNamesValues = HttpUtility.ParseQueryString(queryString);
			qureyStringsNamesValues.Set(key, value);
			queryString = ConstructQueryString(qureyStringsNamesValues);
		}

        /// <summary>
        /// Constructs a QueryString (string).  
        /// Consider this method to be the opposite of "System.Web.HttpUtility.ParseQueryString"
        /// </summary>
        /// <param name="nvc">NameValueCollection</param>
        /// <returns>String</returns> 
        public static String ConstructQueryString(NameValueCollection parameters)
        {
            List<String> items = new List<String>();
            foreach (String name in parameters)
            {
                items.Add(String.Concat(name, "=", System.Web.HttpUtility.UrlEncode(parameters[name])));
            }
            return String.Join("&", items.ToArray());
        }

		/// <summary>
		/// Example: 	D:\Inetpub\wwwroot\CambiaWeb\Cambia3\
		/// </summary>
		public static string ApplicationPhysicalPath
		{
			get
			{
				return HttpContext.Current.Request.PhysicalApplicationPath;
			}
		}

		/// <summary>
		/// Get Query String Parameter Value converts it to specified type and returns it.
		/// If not found or error occur null is returned.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="queryStringParameterKey"></param>
		/// <returns></returns>
		public static Guid? GetQueryStringParameterValueAsGuid(string queryStringParameterKey)
		{
			Guid? value = null;
			try
			{
				value = new Guid(HttpContext.Current.Request.QueryString[queryStringParameterKey]);
			}
			catch (Exception)
			{
				value = null;
			}

			return value;
		}

		/// <summary>
		/// Get Query String Parameter Value converts it to specified type and returns it.
		/// If not found or error occur null is returned.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="queryStringParameterKey"></param>
		/// <returns></returns>
		public static int? GetQueryStringParameterValueAsInt(string queryStringParameterKey)
		{
			int? value = null;
			try
			{
				value = int.Parse(HttpContext.Current.Request.QueryString[queryStringParameterKey]);
			}
			catch (Exception)
			{
				value = null;
			}

			return value;
		}

		/// <summary>
		/// Get Query String Parameter Value converts it to specified type and returns it.
		/// If not found or error occur null is returned.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="queryStringParameterKey"></param>
		/// <returns></returns>
		public static string GetQueryStringParameterValueAsString(string queryStringParameterKey)
		{
			string value = null;
			try
			{
				value = HttpContext.Current.Request.QueryString[queryStringParameterKey];
			}
			catch (Exception)
			{
				value = null;
			}

			return value;
		}

		/// <summary>
		/// Get Query String Parameter Value converts it to specified type and returns it.
		/// If not found or error occur null is returned.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="queryStringParameterKey"></param>
		/// <returns></returns>
		public static DateTime? GetQueryStringParameterValueAsDateTime(string queryStringParameterKey)
		{
			DateTime? value = null;
			try
			{
				value = DateTime.Parse(HttpContext.Current.Request.QueryString[queryStringParameterKey]);
			}
			catch (Exception)
			{
				value = null;
			}

			return value;
		}

		// Input: http://localhost:96/Cambia3/Temp/Test.aspx?q=item#fragment
		//Request.ApplicationPath:	        /Cambia3
		//Request.CurrentExecutionFilePath:	/Cambia3/Temp/Test.aspx
		//Request.FilePath:	                /Cambia3/Temp/Test.aspx
		//Request.Path:	                    /Cambia3/Temp/Test.aspx
		//Request.PathInfo:	
		//Request.PhysicalApplicationPath:	D:\Inetpub\wwwroot\CambiaWeb\Cambia3\
		//Request.QueryString:	            /Cambia3/Temp/Test.aspx?query=arg
		//Request.Url.AbsolutePath:	        /Cambia3/Temp/Test.aspx
		//Request.Url.AbsoluteUri:	        http://localhost:96/Cambia3/Temp/Test.aspx?query=arg
		//Request.Url.Fragment:	
		//Request.Url.Host:	                localhost
		//Request.Url.Authority:	        localhost:96
		//Request.Url.LocalPath:	        /Cambia3/Temp/Test.aspx
		//Request.Url.PathAndQuery:	        /Cambia3/Temp/Test.aspx?query=arg
		//Request.Url.Port:	96
		//Request.Url.Query:	            ?query=arg
		//Request.Url.Scheme:	            http
		//Request.Url.Segments:	            /
		//                                  Cambia3/
		//                                  Temp/
		//                                  Test.aspx

		/// <summary>
		/// For '/subdir/page1.aspx' returns 'D:\Inetpub\wwwroot\MyWebApp\subdir\page1.aspx
		/// </summary>
		/// <param name="relativeFilePath"></param>
		/// <returns></returns>
		public static string GetAbsoluteLocalFilePath(string relativeFilePath)
		{
			string absoluteFilePath = null;
			relativeFilePath = relativeFilePath.Replace('/', '\\');
			if (relativeFilePath.StartsWith(@"\"))
			{
				relativeFilePath = relativeFilePath.Remove(0, 1);
			}
			absoluteFilePath = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, relativeFilePath);
			return absoluteFilePath;
		}

		/// <summary>
		/// Gets the current URL after rewrite. 
		/// Source: http://stackoverflow.com/questions/15625927/get-rewritten-url-from-codebehind
		/// </summary>
		/// <returns></returns>
		public static string GetCurrentUrlAfterRewrite()
		{
			string url = String.Format("{0}://{1}{2}", HttpContext.Current.Request.Url.Scheme, HttpContext.Current.Request.Url.Host, (HttpContext.Current.Handler as System.Web.UI.Page).ResolveUrl(HttpContext.Current.Request.RawUrl));
			return url;
		}

		/// <summary>
		/// Resolves URL. 
		/// Source: http://stackoverflow.com/questions/15625927/get-rewritten-url-from-codebehind
		/// </summary>
		/// <returns></returns>
		public static string ResolveUrl(string url)
		{
			string r = (HttpContext.Current.Handler as System.Web.UI.Page).ResolveUrl(url);
			return r;
		}

		/// <summary>
		/// Recursively searches for control with specified type and id in whole current page.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="id"></param>
		/// <returns></returns>
		public static T FindControl<T>(string id) where T : Control
		{
			// this is null by default
			T r = default(T);

			MasterPage master = (HttpContext.Current.Handler as System.Web.UI.Page).Master;

			if (master.Controls.Count > 0)
			{
				for (int i = 0; i < master.Controls.Count; i++)
				{
					Control c = master.Controls[i];
					if (c is T && (string.Compare(id, (c as T).ID, true) == 0))
					{
						r = c as T;
						break;
					}
					else
					{
						Debug.WriteLine(GetPath(c));
						r = FindControl<T>(c, id);
						if (r != null) break;
					}
				}
			}

			return r;
		}

		/// <summary>
		/// Recursively searches for control with specified type and id
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="rootControl"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public static T FindControl<T>(Control rootControl, string id) where T : Control
		{
			// this is null by default
			T r = default(T);
			
			if (rootControl.Controls.Count > 0)
			{
				for (int i = 0; i < rootControl.Controls.Count; i++)
				{
					Control c = rootControl.Controls[i];
					if (c is T && (string.Compare(id, (c as T).ID, true) == 0))
					{
						r = c as T;
						break;
					}
					else
					{
						Debug.WriteLine(GetPath(c));
						r = FindControl<T>(c, id);
						if (r != null) break;
					}
				}
			}

			return r;
		}

		public static List<Control> GetAncestors(Control c, bool placeOldestAtBegining)
		{
			List<Control> r = new List<Control>();

			while (c!=null)
			{
				r.Add(c);
				c = c.Parent;
			}

			if (placeOldestAtBegining)
			{
				r.Reverse();
			}

			return r;
		}

		public static string GetPath(Control c)
		{
			string r = "";

			foreach (Control a in GetAncestors(c, true))
			{
				r += "\\" + a.ID + "|" + a.ClientID;
			}

			return r;
		}

		public static List<System.Web.UI.WebControls.MenuItem> GetMenuItemsByText(Menu menu, string text, bool caseSensitive)
		{
			List<MenuItem> r = new List<MenuItem>();

			r.AddRange(GetMenuItemsByText(menu.Items, text, caseSensitive));

			return r;
		}

		public static List<System.Web.UI.WebControls.MenuItem> GetMenuItemsByText(MenuItemCollection menuItems, string text, bool caseSensitive)
		{
			List<MenuItem> r = new List<MenuItem>();

			foreach (MenuItem menuItem in menuItems)
			{
				if (string.Compare(menuItem.Text, text, caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase) ==0)
				{
					r.Add(menuItem);
				}

				r.AddRange(GetMenuItemsByText(menuItem.ChildItems, text, caseSensitive));
			}


			return r;
		}
	}
}
