using CommunicationCore;
using System;
using System.Threading;
using System.Threading.Tasks;
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

        private async void SendDefine_Click(object sender, RoutedEventArgs e)
        {
            DisplayBytes.Text = "";
            await Task.Delay(1000);

            DisplayBytes.Text = "三键自定义表决，直接表决 , 最后一次有效";
            // 自定义3 ; 先签到后表决  第一次有效
            var VoteContent =  DefineVoteHelp.GetDefineVoteContent(3, 0, 0);
            var data = tcpClientHelp.Send(VoteContent);
            DisplayBytes.Text += $"{System.Environment.NewLine} {data}";

            DisplayBytes.Text += $"{System.Environment.NewLine}";
            DisplayBytes.Text += $"自定义表决内容,遍历发送";
            //自定义选项内容
            var opText = "反对,赞同,中立";
            var opTextArray = opText.Split(',');
            for (int i = 0; i < opTextArray.Length; i++)
            {
                DisplayBytes.Text += $"{System.Environment.NewLine} {opTextArray[i]}";
                Thread.Sleep(150);
                var opTextBytes = DefineVoteHelp.GetDefineVoteOpText(i+1, opTextArray[i]);
                var d = tcpClientHelp.Send(opTextBytes);
                DisplayBytes.Text += $"{System.Environment.NewLine} {d}";
            }

        }
    }
}