using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;

namespace ConsoleDump
{
	public static class DumpExtensions
	{
		private static readonly ConsoleWriter _Writer = new ConsoleWriter();
		public static T Dump<T>(this T it, string label = null)
		{
			return _Writer.Dump(it, label);
		}
	}
}
