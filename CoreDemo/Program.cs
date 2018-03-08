using DemoLib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;

namespace CoreDemo
{
    public class Program
    {
        static void Main(string[] args)
        {
            new CoreDemo().Run();
        }

        class CoreDemo : Demo
        {
            protected override void ServiceStackPrintDump<T>(T it)
            {
                ServiceStack.Text.TypeSerializer.PrintDump(it);
            }
        }
    }
}
