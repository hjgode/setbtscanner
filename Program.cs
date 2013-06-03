using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;

namespace setBTscanner
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [MTAThread]
        static void Main(String[] args)
        {
            if (args.Length == 2)
            {
                if (args[0].ToLower() == "-connect")
                {
                    string sBT = args[1];
                    if (sBT.Length == 12)
                        Application.Run(new setBTscanner(sBT));
                }
            }
            else
                Application.Run(new setBTscanner());
        }
    }
}