using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using MMD;
using OpenTK.Graphics.OpenGL4;
// ReSharper disable StringLiteralTypo

namespace App
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            
        }
        
        
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        enum Status
        {
            Gay,
            Cool,
            InJail
        }
/*
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(IntPtr zeroOnly, string lpName);
*/

        [DllImport("user32.dll")]
        static extern bool SetWindowText(IntPtr hWnd, string text);
        [Obsolete("Newer Version OFM")]
        private static void openfilemenu()
        {
            throw new NotImplementedException();
        }
    }

    internal class Player
    {
        public static void Set(string user, object hw)
        {
            throw new NotImplementedException();
        }
    }
}