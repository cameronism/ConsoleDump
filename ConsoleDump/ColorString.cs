using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleDump
{
    internal struct ColorString
    {
        public readonly String String;
        public readonly ConsoleColor Foreground;
        public readonly ConsoleColor Background;

        public ColorString(string s, ConsoleColor foreground, ConsoleColor background)
        {
            this.String = s;
            this.Foreground = foreground;
            this.Background = background;
        }
    }
}
