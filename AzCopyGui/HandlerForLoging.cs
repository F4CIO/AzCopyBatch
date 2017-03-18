using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CraftSynth.BuildingBlocks.Logging;

namespace AzCopyGui
{
	public class HandlerForLoging
	{
		public static void LogAction(string line, bool inNewLine = true)
		{
			//if (Settings.Current.logActions)
			//{
			CraftSynth.BuildingBlocks.Logging.Misc.AddTimestampedLineToApplicationWideLog(line, inNewLine, null, false);
			//}
		}

		public static void LogException(Exception e, CustomTraceLog log)
		{
			//if (Settings.Current.logErrors)
			//{
			//CraftSynth.BuildingBlocks.Logging.Misc.AddTimestampedExceptionInfoToApplicationWideLog(e);
			//}
			string[] lines = CraftSynth.BuildingBlocks.Logging.Misc.GetExceptionDescription(e, true, false).Split('\n');
			foreach (string line in lines)
			{
				log.AddLine(line.Trim('\r'));
			}
		}

		public static void LogDebugInfo(string line, bool inNewLine = true)
		{
			//if (Settings.Current.logDebugInfo)
			//{
			CraftSynth.BuildingBlocks.Logging.Misc.AddTimestampedLineToApplicationWideLog(line, inNewLine, null, false);
			//}
		}

		

	}
}
