using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationCore
{
    /// <summary>
    /// 自定义投票
    /// </summary>
    public  class DefineVoteHelp
    {
        /// <summary>
        /// 获取自定义投票命令
        /// </summary>
        /// <param name="opNum">选项个数: 2、3、4、5</param>
        /// <param name="voteParam">表决方式 ：1 先签到后表决; 0 直接表决</param>
        /// <param name="billParam">计票方式： 1 第一次有效； 0 最后一次有效</param>
        /// <returns></returns>
        private static byte GetDefineVoteCmd(int opNum, int voteParam, int billParam)
        {
            switch (opNum)
            {
                case 2:
                    {
                        if (billParam == 1)
                        {
                            if (voteParam == 1)
                            {
                                return 0x1c;
                            }
                            if (voteParam == 0)
                            {
                                return 0x1d;
                            }
                        }

                        if (billParam == 0)
                        {
                            if (voteParam == 1)
                            {
                                return 0x1e;
                            }

                            if (voteParam == 0)
                            {
                                return 0x1f;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        if (billParam == 1)
                        {
                            if (voteParam == 1)
                            {
                                return 0x20;
                            }
                            if (voteParam == 0)
                            {
                                return 0x21;
                            }
                        }

                        if (billParam == 0)
                        {
                            if (voteParam == 1)
                            {
                                return 0x22;
                            }

                            if (voteParam == 0)
                            {
                                return 0x23;
                            }
                        }
                        break;
                    }
                case 4:
                    {
                        if (billParam == 1)
                        {
                            if (voteParam == 1)
                            {
                                return 0x30;
                            }
                            if (voteParam == 0)
                            {
                                return 0x31;
                            }
                        }

                        if (billParam == 0)
                        {
                            if (voteParam == 1)
                            {
                                return 0x32;
                            }

                            if (voteParam == 0)
                            {
                                return 0x33;
                            }
                        }
                        break;
                    }
                case 5:
                    {
                        if (billParam == 1)
                        {
                            if (voteParam == 1)
                            {
                                return 0x34;
                            }
                            if (voteParam == 0)
                            {
                                return 0x35;
                            }
                        }

                        if (billParam == 0)
                        {
                            if (voteParam == 1)
                            {
                                return 0x36;
                            }

                            if (voteParam == 0)
                            {
                                return 0x37;
                            }
                        }
                        break;
                    }
            }
            return 0;
        }


        /// <summary>
        /// 获取自定义投票
        /// </summary>
        /// <param name="opNum">选项个数: 2、3、4、5</param>
        /// <param name="voteParam">表决方式 ：1 先签到后表决; 0 直接表决</param>
        /// <param name="billParam">计票方式： 1 第一次有效； 0 最后一次有效</param>
        public static byte[] GetDefineVoteContent(int opNum, int voteParam, int billParam)
        {
            byte[] content = new byte[5];
            content[0] = 0xff;
            content[1] = 0xf0;
            content[2] = 0x80;
            content[3] = 0x02;
            content[4] = GetDefineVoteCmd(opNum, voteParam, billParam);
            return content;
        }

        /// <summary>
        /// 获取自定义投票内容文本
        /// </summary>
        /// <param name="opNum">项序号</param>
        /// <param name="opText">文本内容</param>
        /// <returns></returns>
        public static byte[] GetDefineVoteOpText(int opNum, string opText)
        {
            byte[] messageByte = Encoding.GetEncoding("gbk").GetBytes(opText);
            //0xfff0 + 0x86 +0x24 + 序号(1个字节)  + 内容(N个字节)
            byte[] contentBytes = new byte[5+ messageByte.Length];
            contentBytes[0] = 0xff;
            contentBytes[1] = 0xf0;
            contentBytes[2] = 0x86;
            contentBytes[3] = 0x24;
            contentBytes[4] = (byte)opNum;
            Array.Copy(messageByte, 0, contentBytes, 5, messageByte.Length);
            return contentBytes;
        }

    }
}
