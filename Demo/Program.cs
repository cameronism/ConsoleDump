using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleDump;
using System.Net;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Threading;

namespace Demo
{
	public class Program
	{
		public struct WhateverStruct
		{
			public readonly int Top;
			public readonly int Right;
			public readonly int Bottom;
			public readonly int Left;

			public WhateverStruct(int t, int r, int b, int l)
			{
				Top = t;
				Right = r;
				Bottom = b;
				Left = l;
			}

			public override string ToString()
			{
				return String.Format("t: {0}, r: {1}, b: {2}, l = {3}", Top, Right, Bottom, Left);
			}
		}

		static void Main(string[] args)
		{
			Show("foo");

			Show(42);

			Show(new
			{
				Int32 = 1,
				String = "sssstring",
				NullObject = (object)null,
				Boolean = true,
				DateTime = DateTime.UtcNow,
				SomeClass = Version.Parse("1.0"),
				SomeStruct = new KeyValuePair<int, int>(1, 1),
			});

			Show(new[] { 1, 22, 333, 4444 });

			Show(IPAddress.Parse("1.1.1.1"));

			Show(Enumerable.Range(0, 100).ToList());

			Show(new[] { new { a = 1 }, null, new { a = 1 }, });


			Show(Enumerable.Range(4, 8).ToDictionary(
					n => n,  
					n => Convert.ToString(-1 + (long)Math.Pow(2, n), 2)));

			Show(new ArgumentException("my message", "someParam"));

			Show(new WhateverStruct(1,2,3,4));

			var bigExample = new[] { 200, 201, 202, 400, 404 }
				.Select((c, i) => new
				{
					StringProp = ((HttpStatusCode)c).ToString(),
					EnumProp = (HttpStatusCode)c,
					NullableInt = i % 3 == 0 ? null : (int?)c,
					BiggerInt = (int)Math.Pow(2, i * 4),
					IPAddress = i % 3 == 2 ? null : IPAddress.Parse("1.1.1." + (i * 32)),
				});
			Show(bigExample);


			ConsoleDump.DumpExtensions.Dump(bigExample);
			ConsoleDump.DumpExtensions.Dump(IPAddress.Loopback, ".Dump() output can be labeled.");
			TakeScreenShot("simple-ip", ++Count);

		}

		static int Count = 0;
		static void Show<T>(T it)
		{
			Console.WriteLine("// Example: " + (++Count));

			Console.WriteLine();
			Console.WriteLine("// Console.WriteLine");
			Console.WriteLine(it);
			TakeScreenShot("console", Count);

			Console.WriteLine();
			Console.WriteLine("// ServiceStack.Text.TypeSerializer.PrintDump");
			try
			{
				ServiceStack.Text.TypeSerializer.PrintDump(it);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
			TakeScreenShot("servicestack", Count);

			Console.WriteLine();
			Console.WriteLine("// ConsoleDump");
			ConsoleDump.DumpExtensions.Dump(it);
			TakeScreenShot("consoledump", Count);

			ConsoleDump.DumpExtensions.Dump(it, "ConsoleDump label");
		}

		static void TakeScreenShot(string label, int example)
		{
			Thread.Sleep(1);
			// crop the edges of my powershell window
			var img = ScreenShotDemo.ScreenCapture.CaptureWindow(GetConsoleWindow(), 32, 10, 10, 27);
			img.Save(String.Format("{0:d2}_{1}.png", example, label), ImageFormat.Png);
		}

		[DllImport("kernel32.dll")]
		internal static extern IntPtr GetConsoleWindow();
	}
}
