using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleDump;
using System.Net;

namespace Demo
{
	class Program
	{
		static void Main(string[] args)
		{
			int? i = null;
			object o = i;

			foreach (var label in new[] { "test", null })
			{
				"foo".Dump(label);

				o.Dump(label);

				new { 
					Int32 = 1, 
					String = "sssstring",
					NullObject = (object)null,
					Boolean = true,
					DateTime = DateTime.UtcNow,
					SomeClass = Version.Parse("1.0"),
					SomeStruct = new KeyValuePair<int, int>(1, 1),
				}.Dump(label);

				new[] { 1, 2, 3, 4 }.Dump(label);

				Enumerable.Range(0, 15).Dump(label);

				new[] { new { a = 1 }, null, new { a = 1 }, }.Dump(label);

				IPAddress.Parse("1.1.1.1").Dump(label);

				(new ArgumentException("my message", "someParam")).Dump(label);

				Enumerable.Range(4, 8).ToDictionary(
					n => n,  
					n => Convert.ToString(-1 + (long)Math.Pow(2, n), 2)).Dump(label);
			}
		}
	}
}
