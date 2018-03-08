using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;

namespace ConsoleDump
{
    public static class Extensions
    {
        public static ushort? RowLimit { get; set; }

        private static readonly ConsoleWriter _Writer = new ConsoleWriter();

        public static T Dump<T>(this T it, string label = null)
        {
            return _Writer.Dump(it, label, GetEnumerableLimit());
        }

        // Non generic version for easier calling from reflection, powershell, etc.
        public static void DumpObject(object it, string label = null)
        {
            _Writer.Dump(it, label, GetEnumerableLimit());
        }

        private static int GetEnumerableLimit()
        {
            if (RowLimit is ushort limit) return limit;

            int height;
            try
            {
                height = Console.WindowHeight;
            }
            catch (Exception)
            {
                return 24;
            }

            return Math.Max(height - 5, 16);
        }
    }
}
