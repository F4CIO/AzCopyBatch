using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using CraftSynth.BuildingBlocks.Common;
using CraftSynth.BuildingBlocks.Logging;

namespace AzCopyGui
{
	
	public class CommandAzCopy
	{
		#region Private Members
		#endregion

		#region Properties

		private string line;
		public string CommandName;		
		public int TimeOutInMinutes = 30;
		public bool SkipFixingEmptyFolders;
		public string SourceLocation;
		public string SourceKey;
		public string DestinationLocation;
		public string DestinationKey;
		public bool Overwrite;

		#endregion

		#region Public Methods

		public override string ToString()
		{
			return line;
		}

		public string ToString(bool removeCustomCommands)
		{
			string r;
			if (!removeCustomCommands)
			{
				r = line;
			}
			else
			{
				r = AzCopyBatch.HandlerForTask_AzCopy.RemoveCustomParameters(line);
			}

			return r;
		}

		#endregion

		#region Constructors And Initialization
		public static CommandAzCopy Parse(string line, bool handleCommentsAsNormalCommands, CustomTraceLog log)
		{
			CommandAzCopy r = null;
			try
			{
				r = new CommandAzCopy();

				line = line.Trim();

				if (handleCommentsAsNormalCommands)
				{
					line = line.TrimStart('-').Trim();
				}

				if (line.ToLower().Split(' ')[0] != "azcopy")
				{
					throw new Exception("This is not AzCopy command.");
				}

				r.line = line;
				r.CommandName = "AzCopy";
				r.SkipFixingEmptyFolders = line.GetParameterPresence("/skipFixingEmptyFolders", false, false, '/', null);

				var parameters = line.GetParameters(false, false, '"');
				if (parameters.Count < 2)
				{
					throw new Exception("Invalid number of parameters.");
				}

				r.SourceLocation = parameters[0];
				r.DestinationLocation = parameters[1];
				if (line.GetParameterPresence("/sourceKey", false, false, '/', ':'))
				{
					r.SourceKey = line.GetParameterValue<string>("/sourceKey", true, null, true, null, false, null, '/', ':');
				}
				if (line.GetParameterPresence("/destKey", false, false, '/', ':'))
				{
					r.DestinationKey = line.GetParameterValue<string>("/destKey", true, null, true, null, false, null, '/', ':');
				}
				
			}
			catch (Exception e)
			{
				HandlerForLoging.LogException(e, log);
				r = null;
			}
			return r;
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
