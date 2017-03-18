using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CraftSynth.BuildingBlocks.Common.Types
{
	public enum UnspecifiedYesNoEnum
	{
		Unspecified = 0,
		Yes = 1,
		No = -1
	}

	public class StringTripple
	{
		public string A { get; set; }
		public string B { get; set; }
		public string C { get; set; }
		public StringTripple(string a, string b, string c)
		{
			this.A = a;
			this.B = b;
			this.C = c;
		}
	}
}
