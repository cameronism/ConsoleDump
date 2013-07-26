ConsoleDump
===========

**Visualize your collections and objects in color at the console.**

![Example output](http://cameronism.github.io/ConsoleDump/images/consoledump-example.png)

`Console.WriteLine(...)` 
cannot begin to compare to [LINQPad](http://www.linqpad.net/)'s `.Dump()` extension method.  This library
provides a `.Dump()` extension method that can be used in console apps
and in [scriptcs](http://scriptcs.net/).

The output above was created with the following code:
```csharp
new[] { 200, 201, 202, 400, 404 }.Select(CreateSillyExample).Dump();
	
IPAddress.Loopback.Dump(".Dump() output can be labeled.");
```
	
Features
---------

- Available via NuGet: `PM> Install-Package ConsoleDump` **TODO**
- Single dll, depends only on .NET 4.0 Client Profile
- Output colors based on type:
  * `null` is green
  * Strings are cyan
  * Primitives, enums and nullable primitives are white
  * `.ToString()` from a class is purple
  * `.ToString()` from a struct is yellow
  * If a property throws an exception the exception is shown in red
- Numbers are right aligned
- `IEnumerable<>` support
  * Displays count (if available)
  * Safe for infinite `IEnumerable<>`
  * Only the first 24 items are displayed
- Much more concise and readable than JSON in the console



Disclaimer
-------------

This project is not affiliated with [LINQPad](http://www.linqpad.net/) or its author [Joseph Albahari](http://www.albahari.com/).
I've been completely spoiled by an amazing tool and I am trying to keep some of the convenience when working at the console.
Download [LINQPad](http://www.linqpad.net/), it's free; [activate autocompletion](http://www.linqpad.net/Purchase.aspx), it's far
and away the best .NET tool.



TODO
-----

- Improve this document
- Improve "count" wording on enumerable views
- Refine colors
- Publish NuGet package
- Truncate long strings
- Truncate or omit columns if `IEnumerable<>` view is too wide for screen
- Investigate [scriptcs REPL](http://scriptcs.net/) integration


License
---------

Apache 2
