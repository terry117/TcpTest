using CommunicationCore;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    public class TcpClientHelp
    {
       private TcpClient _tcpClient;
        //网络流 
        private NetworkStream _networkStream;
        //写入 
        private BinaryWriter _binaryWrite;
        public bool IsConnected()
        {
            try
            {
                if (_tcpClient == null)
                {
                    return false;
                }
                return _tcpClient != null && _tcpClient.Connected;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public event Action<string> BytesSend;

        public async Task StartAsync()
        {
            if (IsConnected()) return;
            await Task.Run(async () =>
            {
                try
                {
                    _tcpClient = new TcpClient();
                    _tcpClient.ReceiveTimeout = 100000;
                    var ipAddress = IPAddress.Parse(AgreementHelp.Ip);
                    IPEndPoint endpoint = new IPEndPoint(ipAddress, AgreementHelp.Port);
                    using (var cts = new CancellationTokenSource(2000))
                    {
                        var connectTask = _tcpClient.ConnectAsync(endpoint.Address, endpoint.Port);
                        var completedTask = await Task.WhenAny(connectTask, Task.Delay(Timeout.Infinite, cts.Token));
                        if (completedTask == connectTask)
                        {
                            if (!IsConnected()) throw new TimeoutException($"无法连接{endpoint.Address}:{endpoint.Port}, 3s连接超时");
                            _networkStream = _tcpClient.GetStream();
                            _binaryWrite = new BinaryWriter(_networkStream);
                            _ = Task.Run(ReceiveMessagesAsync);
                        }
                        else
                        {
                            throw new TimeoutException($"无法连接{endpoint.Address}:{endpoint.Port}, 2s连接超时");
                        }
                    }
                }
                catch (Exception e)
                {
                    _tcpClient?.Close();
                    throw e;
                }
            });
        }

        public void Close()
        {
            _tcpClient?.Close();
        }

        public void Send(int id, byte msgType, string message)
        {
            if (!IsConnected())
                throw new Exception($"{AgreementHelp.Ip} {AgreementHelp.Port} 无法连接");

            var idBytes =   CommunityHelper.CalculteIntToHex(id, 2).ToArray();
            //协议内容
            //var idBytes = CommunityHelper.CalculteIntToHexArray(id, 2);
            byte[] messageByte = null;
            if (AgreementHelp.IsGBK)
            {
                messageByte = Encoding.GetEncoding("gbk").GetBytes(message);
            }
            else
            {
                messageByte = Encoding.UTF8.GetBytes(message);
            }

            // 广播id   （2字节） + 广播人名类型 （1字节）  +  内容数据的长度N  （1字节） +  内容数据的长度N字节
            byte[] contentBytes = new byte[2 + 1 + 1+ messageByte.Length];
            Array.Copy(idBytes, 0, contentBytes, 0, 2);
            contentBytes[2] = msgType;
            contentBytes[3] = (byte)messageByte.Length;
            Array.Copy(messageByte, 0, contentBytes, 4, messageByte.Length);
            Send(contentBytes);
        }



        /// <summary>
        /// 发送
        /// </summary>
        /// <param name="buffer">音频数据</param>
        public string Send(byte[] buffer)
        {
            var data = new byte[2 + AgreementHelp.MsgLength + buffer.Length + 2];

            var bufferLengthBytes = new byte[AgreementHelp.MsgLength];
            if (AgreementHelp.MsgLength == 1)
            {
                bufferLengthBytes[0] = (byte)buffer.Length;
            }
            else
            {
                bufferLengthBytes = CommunityHelper.CalculteIntToHexArray(buffer.Length, AgreementHelp.MsgLength);
            }
             
        
            Array.Copy(frameHeader, 0, data, 0, 2);
            Array.Copy(bufferLengthBytes, 0, data, 2, bufferLengthBytes.Length);
            Array.Copy(buffer, 0, data, 2+ bufferLengthBytes.Length, buffer.Length);
            Array.Copy(frameEnd, 0, data, 2 + bufferLengthBytes.Length + buffer.Length, 2);

            //数据内容： 广播id   （2字节） + 广播人名类型 （1字节）  +  内容数据的长度N  （1字节） +  内容数据的长度N字节
            // 帧头(2) + 数据内容长度(1) + 数据内容（N）+ 帧尾
            _binaryWrite?.Write(data);
            string str = BitConverter.ToString(data);

            return str;
            BytesSend?.Invoke($"{DateTime.Now:yyyy MM dd HH:mm:ss fff}: {System.Environment.NewLine}{str}{System.Environment.NewLine}总长度 {data.Length}");
        }

        /// <summary>
        /// 读起接收到的数据
        /// </summary>
        /// <returns></returns>
        private async Task ReceiveMessagesAsync()
        {
            try
            {
                while (IsConnected())
                {
                    var rec = await ReceiveVarDataAsync(_tcpClient.Client);
                    if (rec == null)
                    {
                        await Task.Delay(10);
                        continue;
                    }
                    var Str = BitConverter.ToString(rec);
                    Debug.WriteLine($"{DateTime.Now:yyyy MM dd HH:mm:ss fff}: Received raw data: {Str}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error receiving messages: {ex}");
                Close();
            }
        }

        /// <summary>
        /// 接收变长字符串
        /// 为了处理粘包问题 ,每次发送数据时 包头(数据字节长度) + 正文
        /// </summary>
        /// <param name="socket">客户端socket</param>
        /// <returns>byte[]数组</returns>
        private async Task<byte[]> ReceiveVarDataAsync(Socket socket)
        {
            //接收帧头
            var headerBytes = await ReceiveFixDataAsync(socket, 2);
            if (headerBytes == null) return null;

            //内容长度
            var contentLengthBytes = await ReceiveFixDataAsync(socket, AgreementHelp.MsgLength);
            if (contentLengthBytes == null) return null;
            var contentLength = CommunityHelper.ByteConvToDec(contentLengthBytes);

            //接收正文数据
            var receiveFixDataAsync = await ReceiveFixDataAsync(socket, contentLength);
            if (receiveFixDataAsync == null) return null;

            //帧尾
            var endBytes = await ReceiveFixDataAsync(socket, 2);

            //整包
            byte[] data = new byte[2 + AgreementHelp.MsgLength + receiveFixDataAsync.Length +2];
            headerBytes.CopyTo(data, 0);
            contentLengthBytes.CopyTo(data, 2);
            receiveFixDataAsync.CopyTo(data,2+ AgreementHelp.MsgLength);
            endBytes.CopyTo(data, data.Length-2);
            return data;
        }

        /// <summary>
        /// 接受固定长度字符串
        /// </summary>
        /// <param name="socket">socket对象</param>
        /// <param name="dataSize">数据长度</param>
        /// <returns>字节数据</returns>
        private async Task<byte[]> ReceiveFixDataAsync(Socket socket, int dataSize)
        {
            var buffer = new byte[dataSize];
            int totalRead = 0;

            while (totalRead < dataSize)
            {
                if (!socket.Connected) break;
                int read = await socket.ReceiveAsync(new ArraySegment<byte>(buffer, totalRead, dataSize - totalRead), SocketFlags.None);
                if (read == 0) break;
                totalRead += read;
            }
            return totalRead == dataSize ? buffer : null;
        }

        private byte[] frameHeader = { 0xAA, 0xEE };
        private byte[] frameEnd = { 0xEE, 0xFC };
    }
}
