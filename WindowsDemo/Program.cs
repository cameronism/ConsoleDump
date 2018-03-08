using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            new WindowsDemo().Run();
        }

        class WindowsDemo : DemoLib.Demo
        {
            protected override void ServiceStackPrintDump<T>(T it)
            {
                ServiceStack.Text.TypeSerializer.PrintDump(it);
            }

            protected override void TakeScreenShot(string label, int example)
            {
                Thread.Sleep(1);
                // crop the edges of my powershell window
                var img = ScreenShotDemo.ScreenCapture.CaptureWindow(GetConsoleWindow(), 32, 10, 10, 27);
                img.Save(String.Format("{0:d2}_{1}.png", example, label), ImageFormat.Png);
            }

        }

        [DllImport("kernel32.dll")]
        internal static extern IntPtr GetConsoleWindow();
    }
}
