ConsoleDump
===========

**Visualize your collections and objects in color at the console.**

`Console.WriteLine(...)` 
cannot begin to compare to [LINQPad](http://www.linqpad.net/)'s `.Dump()` extension method.  This library
provides a `.Dump()` extension method that can be used in console apps
and in [scriptcs](http://scriptcs.net/).

	
	Console.WriteLine(new[] { 1, 2, 3, 4 });
	// result
	System.Int32[]


	new[] { 1, 2, 3, 4 }.Dump();
	// result
	Int32[] (4 items)
	 1
	 2
	 3
	 4


	Console.WriteLine(IPAddress.Parse("1.1.1.1"));
	// result
	1.1.1.1


	IPAddress.Parse("1.1.1.1").Dump();
	// result
	System.Net.IPAddress
	 Address            16843009
	 AddressFamily      InterNetwork
	 ScopeId            The attempted operation is not supported for the type of object referenced
	 IsIPv6Multicast    False
	 IsIPv6LinkLocal    False
	 IsIPv6SiteLocal    False
	 IsIPv6Teredo       False
	 IsIPv4MappedToIPv6 False


`myObj.Dump();` replaces the following:

- `Console.WriteLine("{0} {1} {2}", myObj.Prop1, myObj.Prop2, myObj.Prop3);`
- `Console.WriteLine(JsonConvert.SerializeObject(myObj));`


Disclaimer
-------------

This project is not affiliated with [LINQPad](http://www.linqpad.net/) or its author [Joseph Albahari](http://www.albahari.com/).
I've been completely spoiled by an amazing tool and I am trying to keep some of the convenience when working at the console.
Download [LINQPad](http://www.linqpad.net/), it's free; [activate autocompletion](http://www.linqpad.net/Purchase.aspx), it's far
and away the best .NET tool.


TODO
-----

- Improve this document
- Add ToString() to output for object (not enumerable) view
- Refine colors
- Publish NuGet package
- Truncate long strings
- Truncate or omit columns if `IEnumerable<>` view is too wide for screen
- Investigate [scriptcs REPL](http://scriptcs.net/) integration


License
---------

Apache 2
