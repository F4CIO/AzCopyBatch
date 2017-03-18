using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CraftSynth.BuildingBlocks.Common;

namespace AzCopyBatch
{
	public class HandlerForCommon
	{
		public static string HideSensitiveInformation(string line)
		{
			try
			{
				if (line.Contains("SmtpPassword"))
				{
					line = line.Remove(line.Length - line.GetSubstringAfter("=").Length) + "...(hidden)...";
				}
			}
			catch (Exception) { }

			//extract destination key:
			string destinationKey = line.GetParameterValue<string>("/destkey", false, null, false, null, true, null, '/', ':');
			if (destinationKey != null)
			{
				line = line.Replace(destinationKey, destinationKey.Bubble(20, "...(hidden)..."));
			}

			string sourceKey = line.GetParameterValue<string>("/sourcekey", false, null, false, null, true, null, '/', ':');
			if (sourceKey != null)
			{
				line = line.Replace(sourceKey, sourceKey.Bubble(20, "...(hidden)..."));
			}

			return line;
		}
	}
}
