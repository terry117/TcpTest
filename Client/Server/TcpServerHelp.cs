using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using CommunicationCore;

namespace Server
{
    public class TcpServerHelp
    {
        private TcpListener _tcpListener;
        public bool State => _tcpListener.Server.Connected;
        public TcpServerHelp()
        {
            _tcpListener = new TcpListener(IPAddress.Any, AgreementHelp.Port);
        }

        public event Action<string> BytesReceived;
        public event Action<string> MessageReceived;
        public event Action<string> OpenServiceState;

        public async Task StartAsync()
        {
            _tcpListener.Start();
            OpenServiceState?.Invoke(AgreementHelp.Port.ToString());
            Debug.WriteLine($"======TCP Server started on port {AgreementHelp.Port}");
            await Task.Run(async () =>
            {
                while (true)
                {
                    var client = await _tcpListener.AcceptTcpClientAsync();
                    Debug.WriteLine("Client connected");
                    _ = HandleClientAsync(client);
                }
            });
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            try
            {
                using var stream = client.GetStream();
                using var reader = new BinaryReader(stream);
                using var writer = new BinaryWriter(stream);
                while (client.Connected)
                {
                    //接收帧头
                    var headerBytes = new byte[2];
                    await stream.ReadAsync(headerBytes, 0, 2);

                    //内容长度
                    var contentLengthBytes = new byte[AgreementHelp.MsgLength];
                    await stream.ReadAsync(contentLengthBytes, 0, AgreementHelp.MsgLength);

                    var contentLength = CommunityHelper.ByteConvToDec(contentLengthBytes);
                    if (contentLength > 0)
                    {
                        //接收正文数据
                        var dataBytes = new byte[contentLength];
                        await stream.ReadAsync(dataBytes, 0, contentLength);

                        //接收帧尾
                        var endBytes = new byte[2];
                        await stream.ReadAsync(endBytes, 0, 2);

                        //整包
                        byte[] data = new byte[2 + AgreementHelp.MsgLength + dataBytes.Length + 2];
                        headerBytes.CopyTo(data, 0);
                        contentLengthBytes.CopyTo(data, 2);
                        dataBytes.CopyTo(data, 2 + AgreementHelp.MsgLength);
                        endBytes.CopyTo(data, data.Length -2);
                        string str = BitConverter.ToString(data);
                        BytesReceived?.Invoke($"{DateTime.Now:yyyy MM dd HH:mm:ss fff}:{System.Environment.NewLine}{str} {System.Environment.NewLine}总长度 {data.Length}");
                        //解析内容数据
                        var contentData = new byte[contentLength - 4];
                        Array.Copy(dataBytes, 4, contentData, 0, contentData.Length);
                        var message = string.Empty;
                        if (AgreementHelp.IsGBK)
                        {
                            message = Encoding.GetEncoding("gbk").GetString(contentData);
                        }
                        else
                        {
                            message = Encoding.UTF8.GetString(contentData);
                        }
                        Debug.WriteLine($"Received message: {message}");
                        MessageReceived?.Invoke(message);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error handling client: {ex}");
            }
            finally
            {
                client.Close();
                Debug.WriteLine("Client disconnected");
            }
        }
    }
}
