namespace CommunicationCore
{
    public class AgreementHelp
    {
        public static string Ip = "127.0.0.1";
        public const int Port = 51688;

        /// <summary>
        /// 协议内容N字节的长度，单位为byte
        /// </summary>
        public const int MsgLength = 1;

        public const bool IsGBK = true;

        //Tcp通信格式
        //1 帧头 0xAA 0xEE 
        //2 协议长度(1个字节)  N
        //3 协议内容(N个字节)
        //4 帧尾 0xEE 0xFC

        //协议内容
        //1 ID(2个字节)
        //2 类型(1个字节)
        //3 扩展内容(长度不定)
        //例如： 发送铭牌内容 ->  id = 2(设备ID) 类型0xEE
    }
}
