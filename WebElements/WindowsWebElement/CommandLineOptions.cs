using CommandLine;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsWebBrowser
{
    public class CommandLineOptions
    {
        [Value(index: 0, Required = false, HelpText = "vl")]
        public string RunDll { get; set; }

        [Value(index: 1,  Required = true, HelpText = "Open URL")]
        public string Url { get; set; }

        [Value(index: 2,  Required = true, HelpText = "ServerPort")]
        public int Port { get; set; }
    }
}
