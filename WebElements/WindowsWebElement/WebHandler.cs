using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace WindowsWebBrowser
{
    public class WebHandler
    {
        public int Port { get; set; }
        public bool IsStarted { get; set; }
        private TcpClient _TcpClient;
        private Stream _TcpStream;
        public WebHandler(int port)
        {
           
            this.Port = port;
        }
        private void TcpInint()
        {
            _TcpClient = new TcpClient();
        }
        private void Connect()
        {

            try
            {
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Port);
                _TcpClient.Connect(ipPoint);
                _TcpStream = _TcpClient.GetStream();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка:{0}", ex.Message);
                _TcpClient?.Close();
                _TcpClient = null;
            }
        }
        public void SendMessage(string message)
        {
            if (_TcpClient is null) TcpInint();
            if (!_TcpClient.Connected) Connect();
            byte[] data = Encoding.UTF8.GetBytes(message);
          
            _TcpStream.Write(data, 0, data.Length);
            
           
        }
    }
}
