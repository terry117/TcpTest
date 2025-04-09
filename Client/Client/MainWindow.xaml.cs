using CommunicationCore;
using System;
using System.Windows;

namespace Client
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly TcpClientHelp tcpClientHelp;
        public MainWindow()
        {
            InitializeComponent();
            tcpClientHelp = new TcpClientHelp();
            tcpClientHelp.BytesSend += TcpClientHelp_BytesSend;

            string test = "A7A3A7E020A7D3A7E2A7D6A7DEA7D6A7DFA7D120A7E4A7E2A7D6A7E4A7EEA7D6A7DB20A7E3A7E4A7E0A7E2A7E0A7DFA7EDA7A3A7E0A7A3A7E0";
            var  lenght=test.Length/2;
        }

        private async void TcpClientHelp_BytesSend(string obj)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                DisplayBytes.Text = obj;
            });
        }

        /// <summary>
        /// 连接服务端
        /// </summary>
        private async void ConnectService_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(IpTextBox.Text))
                {
                    MessageBox.Show("请填写服务器IP");
                    return;
                }
                AgreementHelp.Ip = IpTextBox.Text.Trim();
                if (!tcpClientHelp.IsConnected())
                {
                    await tcpClientHelp.StartAsync();
                }

                if (tcpClientHelp.IsConnected())
                {
                    ClientStateLabel.Content = $"连接服务器： {AgreementHelp.Ip} {AgreementHelp.Port} 成功";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }           
        }

        /// <summary>
        /// 发送
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Send_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int value;
                if (string.IsNullOrWhiteSpace(IDTextBox.Text))
                {
                    MessageBox.Show("请输入ID！");
                    return;
                }
              
                if (!int.TryParse(IDTextBox.Text, out value))
                {
                    MessageBox.Show("请输入有效的数字ID！");
                    return;
                }
              

                if (string.IsNullOrWhiteSpace(SendMessage.Text))
                {
                    MessageBox.Show("发送的数据不能为空！");
                    return;
                }

                tcpClientHelp.Send(value, 0x9c, SendMessage.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}