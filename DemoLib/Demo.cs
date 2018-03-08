using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;

namespace DemoLib
{
    public abstract class Demo
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

        static IEnumerable<uint> Forever()
        {
            uint num = 0;
            while (true)
            {
                yield return unchecked(num++);
            }
        }

        public void Run()
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

            Show(new WhateverStruct(1, 2, 3, 4));

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

            ConsoleDump.Extensions.Dump(Forever().Take(5).ToList(), "small list");
            ConsoleDump.Extensions.Dump(Forever().Take(50).ToList(), "bigger list");
            ConsoleDump.Extensions.Dump(Forever(), "infinite enumerable");

            ConsoleDump.Extensions.Dump(bigExample);
            ConsoleDump.Extensions.Dump(IPAddress.Loopback, ".Dump() output can be labeled.");
            TakeScreenShot("simple-ip", ++Count);

            var datatable = GetDataTable();
            ConsoleDump.Extensions.Dump(datatable, "datatable label");
            ConsoleDump.Extensions.RowLimit = 64;
            ConsoleDump.Extensions.Dump(datatable);
            TakeScreenShot("datatable", ++Count);

            ConsoleDump.Extensions.RowLimit = null;
            var dataset = new DataSet();
            dataset.Tables.Add(datatable);
            ConsoleDump.Extensions.Dump(dataset, "dataset label");
            ConsoleDump.Extensions.Dump(dataset);
            TakeScreenShot("dataset", ++Count);

            //Extensions.Dump(new
            //{
            //    Console.BufferHeight,
            //    Console.LargestWindowHeight,
            //    Console.WindowHeight,
            //});
            //Console.ReadKey();
        }

        private static DataTable GetDataTable()
        {
            var dt = new DataTable();
            dt.Columns.Add("foo", typeof(int));
            dt.Columns.Add("bar", typeof(string));
            dt.Columns.Add("bop", typeof(DateTime));

            var rand = new Random(42);

            for (int i = 0; i < 30; i++)
            {
                var row = dt.NewRow();
                row[0] = rand.Next() % 4 == 0 ? (object)DBNull.Value : rand.Next(256);
                row[1] = rand.Next() % 4 == 0 ? (object)DBNull.Value : "_" + rand.Next(256);
                row[2] = rand.Next() % 4 == 0 ? (object)DBNull.Value : new DateTime(2018, 1, rand.Next(1, 31), 0, 0, 0, DateTimeKind.Utc);

                dt.Rows.Add(row);
            }

            return dt;
        }

        int Count = 0;
        protected virtual void Show<T>(T it)
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
                ServiceStackPrintDump(it);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            TakeScreenShot("servicestack", Count);

            Console.WriteLine();
            Console.WriteLine("// ConsoleDump");
            ConsoleDump.Extensions.Dump(it);
            TakeScreenShot("consoledump", Count);

            ConsoleDump.Extensions.Dump(it, "ConsoleDump label");

            ConsoleDump.Extensions.DumpObject(it, "as object");
        }

        protected abstract void ServiceStackPrintDump<T>(T it);

        protected virtual void TakeScreenShot(string label, int example)
        {
        }
    }
}
