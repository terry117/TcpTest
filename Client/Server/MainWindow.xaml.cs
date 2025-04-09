using Server;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Server
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private TcpServerHelp tcpServerHelp;
        public MainWindow()
        {
            InitializeComponent();
            tcpServerHelp = new TcpServerHelp();
            tcpServerHelp.MessageReceived += MessageReceived;
            tcpServerHelp.OpenServiceState += OpenServiceState;
            tcpServerHelp.BytesReceived += TcpServerHelp_BytesReceived;
        }

        private async void TcpServerHelp_BytesReceived(string obj)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                DisplayBytes.Text = obj;
            });
        }

        private async void MessageReceived(string obj)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                DisplayMessage.Text = obj;
            });
        }

        private async void OpenServiceState(string obj)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                ServiceStateLabel.Content = $"服务已开启： 监听端口 {obj}";
            });
        }

        /// <summary>
        /// 开启服务
        /// </summary>
        private async void OpenService_Click(object sender, RoutedEventArgs e)
        {
            if (tcpServerHelp.State) return;
            await tcpServerHelp.StartAsync();
        }
    }
}