using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using System.Runtime.InteropServices;

namespace setBTscanner
{
    public class Win32
    {
        [DllImport("coredll.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool MessageBeep(beepType uType);
        /// <summary>
        /// Enum type that enables intellisense on the private <see cref="Beep"/> method.
        /// </summary>
        /// <remarks>
        /// Used by the public Beep <see cref="Beep"/></remarks>
        public enum beepType : uint
        {
            /// <summary>
            /// A simple windows beep
            /// </summary>            
            SimpleBeep = 0xFFFFFFFF,
            /// <summary>
            /// A standard windows OK beep
            /// </summary>
            OK = 0x00,
            /// <summary>
            /// A standard windows Question beep
            /// </summary>
            Question = 0x20,
            /// <summary>
            /// A standard windows Exclamation beep
            /// </summary>
            Exclamation = 0x30,
            /// <summary>
            /// A standard windows Asterisk beep
            /// </summary>
            Asterisk = 0x40,
        }

        public static void doGoodBeeps()
        {
            MessageBeep(beepType.OK);
            System.Threading.Thread.Sleep(200);
            MessageBeep(beepType.OK);
            System.Threading.Thread.Sleep(200);
            MessageBeep(beepType.OK);
        }

        public static void doBadBeeps()
        {
            MessageBeep(beepType.Exclamation);
        }
    }
}
