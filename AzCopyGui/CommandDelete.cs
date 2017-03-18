using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using CraftSynth.BuildingBlocks.Common;
using CraftSynth.BuildingBlocks.Logging;

namespace AzCopyGui
{
	
	public class CommandDelete
	{
		#region Private Members
		#endregion

		#region Properties

		private string line;
		public string CommandName;		
		public int TimeOutInMinutes = 30;
		public string DestinationLocation;
		public string DestinationKey;
		public bool DeleteOnlyContent;

		#endregion

		#region Public Methods

		public override string ToString()
		{
			return line;
		}

		#endregion

		#region Constructors And Initialization
		public static CommandDelete Parse(string line, bool handleCommentsAsNormalCommands, CustomTraceLog log)
		{
			CommandDelete r = null;
			try
			{
				r = new CommandDelete();

				line = line.Trim();

				if (handleCommentsAsNormalCommands)
				{
					line = line.TrimStart('-').Trim();
				}

				if (line.ToLower().Split(' ')[0] != "delete")
				{
					throw new Exception("This is not delete command.");
				}

				r.line = line;
				r.CommandName = "delete";
				var parameters = line.GetParameters(false, false, '"');
				if (parameters.Count < 1)
				{
					throw new Exception("Invalid number of parameters.");
				}

				r.DestinationLocation = parameters[0];

				if (line.GetParameterPresence("/destKey", false, false, '/', ':'))
				{
					r.DestinationKey = line.GetParameterValue<string>("/destKey", true, null, true, null, false, null, '/', ':');
				}
					
				r.DeleteOnlyContent = line.GetParameterPresence("/DeleteOnlyContent", false, false, '/', null);
			
			}
			catch (Exception e)
			{
				HandlerForLoging.LogException(e, log);
				r = null;
			}
			return r;
		}

		public CommandDelete()
		{
			
		}

		public CommandDelete(string destinationLocation, string destinationKey=null, bool deleteOnlyContent=false)
		{
			if (string.IsNullOrEmpty(destinationLocation))
			{
				throw new Exception("destinationLocation can not be null.");
			}
			destinationLocation = destinationLocation.Trim();
			this.DestinationLocation = destinationLocation;
			if (destinationKey != null)
			{
				this.DestinationKey = destinationKey.Trim();
			}
			this.DeleteOnlyContent = deleteOnlyContent;

			if (destinationLocation.Contains(" "))
			{
				destinationLocation = "\"" + destinationLocation + "\"";
			}
			this.line = "delete "+ destinationLocation;
			
			if (!string.IsNullOrWhiteSpace(this.DestinationKey))
			{
				this.line = this.line + " /destKey:" + destinationKey;
			}

			if (this.DeleteOnlyContent)
			{
				this.line = this.line + " /DeleteOnlyContent";
			}
		}
		#endregion

		#region Deinitialization And Destructors
		#endregion

		#region Event Handlers
		#endregion

		#region Private Methods
		#endregion

		#region Helpers
		#endregion
		
	}
}
