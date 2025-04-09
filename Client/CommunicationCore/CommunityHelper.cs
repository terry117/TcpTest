using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationCore
{
    /// <summary>
    /// 公共辅助类
    /// </summary>
    public class CommunityHelper
    {
        /// <summary>
        /// 将10进制整型转换成16进制byte集合
        /// 高字节在前，低字节在后
        /// 大端
        /// 例如100，0x00,0x64 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="size">一个整型为2 （N*2）:N为整型的位数</param>
        /// <returns></returns>
        public static List<byte> CalculteIntToHex(int source, int size = 2)
        {
            try
            {
                size = size * 2;
                List<byte> DateLis = new List<byte>();
                string sourceHex = string.Format("{0:x}", source, 16).PadLeft(size, '0').ToUpper();

                for (int index = 0; index < size; index++)
                {
                    byte moneyByte = Convert.ToByte(string.Format($"{sourceHex.Substring(index, 2)}"), 16);
                    index++;
                    DateLis.Add(moneyByte);
                }
                return DateLis;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public static byte[] CalculteIntToHexArray(int source, int size = 4)
        {
            try
            {
                //size = size * 2;
                byte[] DateLis = new byte[size];
                string sourceHex = string.Format("{0:x}", source, 16).PadLeft(size, '0').ToUpper();

                for (int index = 0; index < size; index++)
                {
                    byte moneyByte = Convert.ToByte(string.Format($"{sourceHex.Substring(index, 2)}"), 16);
                    DateLis[index / 2] = moneyByte;
                    index++;
                }
                return DateLis;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        #region 字符串转成utf-8 16进制数组
        /// <summary>
        /// 字符串按utf-8编码转成16进制数组
        /// </summary>
        /// <param name="source"></param>
        /// <param name="length">固定长度</param>
        /// <returns></returns>
        public static byte[] StringConvToHexByte(string source, int length)
        {
            var HexStr = CommonStrToHexStr(source);
            var HexBytes = HexStr2HexBytes(HexStr, length);
            return HexBytes;
        }

        /// <summary>
        /// 普通字符串转16进制字符串
        /// </summary>
        /// <param name="mStr">普通字符串</param>
        /// <returns>16进制类型的字符串</returns>
        private static string CommonStrToHexStr(string mStr) //返回处理后的十六进制字符串
        {
            try
            {
                if (string.IsNullOrEmpty(mStr)) mStr = "";
                return BitConverter.ToString(Encoding.GetEncoding("utf-8").GetBytes(mStr)).Replace("-", " ");
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 16进制字符串转换成16进制的字节数组(非GBK编码)
        /// </summary>
        /// <param name="HexSendstr">16进制的字符串</param>
        /// <returns>16进制字节数组</returns>
        public static byte[] HexStr2HexBytes(string HexSendstr, int max_send_size)
        {
            try
            {
                string errMsg = string.Empty;
                byte[] send_buf = new byte[max_send_size];
                string text_content = HexSendstr;
                string new_content = text_content.Insert(text_content.Length, " ");
                int txt_len = new_content.Length;
                char[] txt_buf = new_content.ToCharArray();
                int index = 0;
                char[] strHex = new char[3];
                byte uc;
                for (int i = 0; i < txt_len - 2; i += 3)
                {
                    if (txt_buf[i + 2] != ' ')
                    {
                        errMsg += "16 进制数据格式不正确";
                    }
                    strHex[0] = txt_buf[i];
                    strHex[1] = txt_buf[i + 1];
                    strHex[2] = (char)0;
                    for (int j = 0; j < 2; j++)
                    {
                        if (strHex[j] < '0' || (strHex[j] > '9' && strHex[j] < 'A') || (strHex[j] > 'F' &&
                            strHex[j] < 'a') || strHex[j] > 'f')
                        {
                            errMsg += "16 进制数据格式不正确";
                        }
                    }
                    string hex_value = new string(strHex);
                    uc = byte.Parse(hex_value, System.Globalization.NumberStyles.HexNumber);
                    if (index == max_send_size)
                        return send_buf;

                    send_buf[index] = uc;
                    index++;
                }
                return send_buf;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 16进制字符串转换成16进制的字节数组(非GBK编码)
        /// </summary>
        /// <param name="HexSendstr">16进制的字符串</param>
        /// <returns>16进制字节数组</returns>
        public static byte[] HexStr2HexBytesTwo(string HexSendstr, int max_send_size)
        {
            try
            {
                string errMsg = string.Empty;
                byte[] send_buf = new byte[max_send_size];
                string text_content = HexSendstr;
                int txt_len = text_content.Length;
                char[] txt_buf = text_content.ToCharArray();
                int index = 0;
                char[] strHex = new char[2];
                byte uc;

                for (int i = 0; i <= txt_len - 2; i += 2)
                {
                    strHex[0] = txt_buf[i];
                    strHex[1] = txt_buf[i + 1];
                    for (int j = 0; j < 2; j++)
                    {
                        if (strHex[j] < '0' || (strHex[j] > '9' && strHex[j] < 'A') || (strHex[j] > 'F' &&
                            strHex[j] < 'a') || strHex[j] > 'f')
                        {
                            errMsg += "16 进制数据格式不正确";
                        }
                    }
                    string hex_value = new string(strHex);
                    uc = byte.Parse(hex_value, System.Globalization.NumberStyles.HexNumber);
                    if (index == max_send_size)
                        return send_buf;

                    send_buf[index] = uc;
                    index++;
                }
                return send_buf;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion


        #region byte数组按编码utf-8转成字符串

        /// <summary>
        /// 字符串按UTF-8编码转成16进制数组
        /// 排除多余不足补零信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string ByteConvToString(byte[] data)
        {
            var HexString = ByteArrayToHexString(data);
            var Str = CommonHexStrToStr(HexString);
            return Str;
        }

        /// <summary>
        /// 字节数组转换成十六进制字符串(带空格)
        /// </summary>
        /// <param name="data">要转换的字节数组 </param>
        /// <returns>转换后的字符串 AA BB ....  CC </returns>
        public static string ByteArrayToHexString(byte[] data)
        {
            try
            {
                StringBuilder sb = new StringBuilder(data.Length * 3);
                foreach (byte b in data)
                {
                    if (b == 0)
                        continue;
                    sb.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
                }
                return sb.ToString().Trim().ToUpper();
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 将16进制字符串转为字符串
        /// </summary>
        /// <param name="HexStr"></param>
        /// <returns></returns>
        private static string CommonHexStrToStr(string hs)
        {
            try
            {
                string strTemp = "";
                byte[] b = new byte[hs.Length / 2];
                for (int i = 0; i < hs.Length / 2; i++)
                {
                    strTemp = hs.Substring(i * 2, 2);
                    b[i] = Convert.ToByte(strTemp, 16);
                }
                //按照指定编码将字节数组变为字符串
                return Encoding.UTF8.GetString(b);
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        #endregion

        //byte数组转成10进制    
        public static int ByteConvToDec(byte[] data)
        {
            string HexString = ByteArrayToHexString(data);
            try
            {
                StringBuilder sb = new StringBuilder();
                return Convert.ToInt32("0x" + HexString, 16);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
     
    }
}