using System;
using System.Windows.Forms;

namespace WindowsWebBrowser
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            args = new[]
            {
                "https://oauth.vk.com/oauth/authorize?client_id=6463690&scope=1073737727&redirect_uri=https://oauth.vk.com/blank.html&display=mobile&response_type=token&revoke=1 2654"
            };
            //ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}