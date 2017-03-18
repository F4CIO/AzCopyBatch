using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

namespace CraftSynth.BuildingBlocks.UI.Web
{
	class NavigationHistoryItem
	{
		public string pageName;
		public System.Collections.Specialized.NameValueCollection queryString;
	}

	/// <summary>
	/// Class used for simple navigation functionality. 
	/// 1. View GetApplicationRootUrl() method body.
	/// 2. View GetNavigationHistorySessionVariableId() method body.
	/// 3. Call Navigation.RegisterPage from Page_Load event to add page to history stack.
	/// 4. Call Navigation.GoBack to return to previous page that has been registered.
	/// </summary>
	public class Navigation : Page
	{
		/// <summary>
		/// Returns string that is used as prefix to page name when calling Navigation.GoBack() 
		/// </summary>
		/// <returns></returns>
		public static string GetApplicationRootUrl()
		{
			string applicationRootUrl = "";
			//Set applicationRootUrl here to fit your applications global settings 
			return applicationRootUrl;
		}

		private static string GetNavigationHistorySessionVariableId()
		{
			//Set variable id to fit your applications global settings 
			return "NavigationHistory";
		}

		private static void UpdateHistory()
		{
			HttpContext.Current.Session.Add(Navigation.GetNavigationHistorySessionVariableId(), Navigation._history);
		}

		/// <summary>
		/// Stores history information in session object. Stack structure is used
		/// </summary>
		private static Stack<NavigationHistoryItem> _history;
		private static Stack<NavigationHistoryItem> history
		{
			get
			{
				try
				{
					Navigation._history = new Stack<NavigationHistoryItem>();
					Stack<NavigationHistoryItem> storedHistory = (Stack<NavigationHistoryItem>)HttpContext.Current.Session[Navigation.GetNavigationHistorySessionVariableId()];
					if (storedHistory != null)
					{
						Navigation._history = storedHistory;
					}
				}
				catch (Exception)
				{
					Navigation._history = new Stack<NavigationHistoryItem>();
				}
				return Navigation._history;
			}
			set
			{
				Navigation.UpdateHistory();
			}
		}

		/// <summary>
		/// Returns query string based on provided parameter (in form A=B&C=D&E=F)
		/// </summary>
		/// <param name="nameValueCollection"></param>
		/// <returns></returns>
		private static string BuildQueryString(System.Collections.Specialized.NameValueCollection nameValueCollection)
		{
			string queryString = "";
			for (int i = 0; i < nameValueCollection.Count; i++)
			{
				if (i > 0)
				{
					queryString += "&";
				}
				queryString += nameValueCollection.GetKey(i) + "=" + nameValueCollection[i];
			}
			return queryString;
		}


		private static string AddQuestionMarkIfNotEmpty(string queryString)
		{
			if (queryString != string.Empty)
			{
				queryString = "?" + queryString;
			}
			return queryString;
		}

		/// <summary>
		/// Clears navigation history. 
		/// </summary>
		public static void Reset()
		{
			Navigation.history.Clear();
			Navigation.UpdateHistory();
		}

		/// <summary>
		/// Registers current page in navigation history so that user can come back to it later.
		/// Put it in Page_Load event in "if(!PostBack)" structure in every page that participates in navigation functionality (that uses Navigation class). 
		/// </summary>
		/// <param name="pageName">Name of current page in form PageName.aspx</param>
		public static void RegisterPage(string pageName)
		{
			NavigationHistoryItem newItem = new NavigationHistoryItem();
			newItem.pageName = pageName;
			newItem.queryString = HttpContext.Current.Request.QueryString;
			Navigation.history.Push(newItem);
			Navigation.UpdateHistory();
		}

		/// <summary>
		/// Redirects back to previous page that exist in navigation history stack structure. 
		/// (For example if history is A->B->C and current page is C it redirects to B page)
		/// To return to page it needs to be registered (see Navigation.RegisterPage method).
		/// </summary>
		/// <param name="useStoredQueryString">Append (to Url) query string that existed at time the page was registered?</param>
		/// <param name="queryStringToAppend">Optionaly add key/value pairs in form: A=B ampersand C=D ampersand E=F. Do not include question mark nor spaces.</param>
		public static void GoBack(bool useStoredQueryString, string queryStringToAppend)
		{
			if (Navigation.history.Count > 0)
			{
				//Release current page history item
				Navigation.history.Pop();
				Navigation.UpdateHistory();
			}

			if (Navigation.history.Count == 0)
			{
				throw new Exception("Navigation history is empty. Can not go back.");
			}
			else
			{
				//Get previous page history item
				NavigationHistoryItem previousItem = Navigation.history.Pop();
				Navigation.UpdateHistory();

				//TODO: If key from queryStringToAppend exist in stored query string also it should replace that key in stored query string

				//Build query string
				string queryString = "";
				if (useStoredQueryString)
				{
					queryString = BuildQueryString(previousItem.queryString);
				}
				if (queryStringToAppend != null && queryStringToAppend != string.Empty)
				{
					queryString += ((queryString != string.Empty) ? "&" : "") + queryStringToAppend;
				}
				queryString = Navigation.AddQuestionMarkIfNotEmpty(queryString);

				//Go back using history item
				HttpContext.Current.Response.Redirect(
					Navigation.GetApplicationRootUrl()
					+ "/"
					+ previousItem.pageName
					+ queryString
					);
			}

		}
	}
}
