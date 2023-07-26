using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommandLine;

namespace WindowsWebBrowser
{
    public partial class Form1 : Form
    {
        CommandLineOptions options;
        WebHandler webHandler;

        public Form1()
        {
            InitializeComponent();

            if (ArgumentHandler().GetAwaiter().GetResult() == -1)
            {
                MessageBox.Show("Invalid arguments");
                this.Close();
            }
            else webHandler = new WebHandler(options.Port);
        }

        public async Task<int> ArgumentHandler()
        {
            var args = System.Environment.GetCommandLineArgs();


            return await Parser.Default.ParseArguments<CommandLineOptions>(args)
                .MapResult(async (CommandLineOptions opts) =>
                {
                    options = opts;
                    return 1;
                }, errs => Task.FromResult(-1));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            WebBrowser webBrowser = new WebBrowser()
            {
                Anchor = AnchorStyles.Top,
                Dock = DockStyle.Fill,
                ScriptErrorsSuppressed = true,
            };


            Controls.Add(webBrowser);
            webBrowser.Navigated += WebBrowser_Navigated;
            webBrowser.Navigate(options?.Url);
            webBrowser.Show();
        }

        private void WebBrowser_Navigated(object? sender, WebBrowserNavigatedEventArgs e)
        {
            webHandler.SendMessage(e.Url.AbsoluteUri);
        }
    }
}