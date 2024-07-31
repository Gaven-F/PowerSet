using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Net.Mime.MediaTypeNames;

namespace PowerSet.Main
{
    public class PowerController
    {
        // 电源相关参数
        //定义电源电压最大值、电流最大值(在IV电源的最大电压和最大电流分别为20V和10A)
        //private static double Vmax_IV = 20.0;
        private static double Imax_IV = 10000; //10.5;

        //定义电源电压最大值、电流最大值(在HSP-3010电源的最大电压和最大电流分别为31V和10.5A)
        private static double Vmax_HSP = 31.0;
        private static double Imax_HSP = 10500; //10.5;

        //定义电源输出控制参数，OVPmax和VCPmax(在HSP-3010型O.V.P 34.0V，O.C.P 12.0A）
        //private static double OVPmax_HSP = 34.0;
        //private static double OCPmax_HSP = 12000; //12A

        //定义电源电压最大值、电流最大值(在PLD-3010型电源的最大电压和最大电流分别为32V和11A)
        private static double Vmax_PLD = 32.0;
        private static double Imax_PLD = 11000; //11

        //定义电源输出控制参数，OVPmax和VCPmax(在PLD-3010型电的O.V.P 36.0V，O.C.P 12.0A）
        //private static double OVPmax_PLD = 36.0;
        //private static double OCPmax_PLD = 12000; //12A


        Byte[] Send_OFForONByte1 = new byte[8]; //发送电源开关命令字节缓冲区
        Byte[] Send_OFForONByte2 = new byte[8]; //发送电源开关命令字节缓冲区
        Byte[] Send_OFForONByte3 = new byte[8]; //发送电源开关命令字节缓冲区
        Byte[] Send_OFForONByte4 = new byte[8]; //发送电源开关命令字节缓冲区
        Byte[] Send_OFForONByte5 = new byte[8]; //发送电源开关命令字节缓冲区

        Byte[] Send_VCmdByte1 = new byte[13]; //发送电源电压设定命令字节缓冲区
        Byte[] Send_VCmdByte2 = new byte[13]; //发送电源电压设定命令字节缓冲区
        Byte[] Send_VCmdByte3 = new byte[13]; //发送电源电压设定命令字节缓冲区
        Byte[] Send_VCmdByte4 = new byte[13]; //发送电源电压设定命令字节缓冲区
        Byte[] Send_VCmdByte5 = new byte[8]; //发送电源电压设定命令字节缓冲区

        Byte[] Send_ICmdByte1 = new byte[13]; //发送电源电流命令字节缓冲区
        Byte[] Send_ICmdByte2 = new byte[13]; //发送电源电流命令字节缓冲区
        Byte[] Send_ICmdByte3 = new byte[13]; //发送电源电流命令字节缓冲区
        Byte[] Send_ICmdByte4 = new byte[13]; //发送电源电流命令字节缓冲区
        Byte[] Send_ICmdByte5 = new byte[8]; //发送电源电流命令字节缓冲区

        Byte[] Send_ReadCmdByte1 = new byte[13]; //发送电源电流读取命令字节缓冲区
        Byte[] Send_ReadCmdByte2 = new byte[13]; //发送电源电流读取命令字节缓冲区
        Byte[] Send_ReadCmdByte3 = new byte[13]; //发送电源电流读取命令字节缓冲区
        Byte[] Send_ReadCmdByte4 = new byte[13]; //发送电源电流读取命令字节缓冲区
        Byte[] Send_ReadCmdByte5 = new byte[8]; //发送电源电流读取命令字节缓冲区

        private static int CurOldVal_K; //K电源
        private static int CurOldVal_Na; //Na电源
        private static int CurOldVal_Cs; //Cs电源
        private static int CurOldVal_Sb; //Sb电源

        //private static int CurOldVal_DY; //灯源电源

        //电源电流当前设定值
        private static int CurSetVal_K; //K电源
        private static int CurSetVal_Na; //Na电源
        private static int CurSetVal_Cs; //Cs电源
        private static int CurSetVal_Sb; //Sb电源

        //private static int CurSetVal_DY; //灯源电源

        //电源电流返回值（读取值）
        private static int CurReadVal_K; //K电源
        private static int CurReadVal_Na; //Na电源
        private static int CurReadVal_Cs; //Cs电源
        private static int CurReadVal_Sb; //Sb电源

        //private static int CurReadVal_DY; //灯源电源

        //电源开关标志，old表示上次的状态，new表示当前状态,false表示关，true表示开
        private static bool NewOnFlag_K; //K电源
        private static bool NewOnFlag_Na; //Na电源
        private static bool NewOnFlag_Cs; //Cs电源
        private static bool NewOnFlag_Sb; //Sb电源

        //private static bool NewOnFlag_DY; //灯源电源

        private static bool OldOnFalg_K; //K电源
        private static bool OldOnFalg_Na; //Na电源
        private static bool OldOnFalg_Cs; //Cs电源
        private static bool OldOnFalg_Sb; //Sb电源

        //private static bool OldOnFalg_DY; //灯源电源


        private static bool K_Vlot_Flag; //K电源电压设定标志
        private static bool KoffFlag; //叉掉弹窗时，设置电流值为零的标志
        private static bool Na_Vlot_Flag; //Na电源电压设定标志
        private static bool NaoffFlag; //叉掉弹窗时，设置电流值为零的标志
        private static bool Cs_Vlot_Flag; //Cs电源电压设定标志
        private static bool CsoffFlag; //叉掉弹窗时，设置电流值为零的标志
        private static bool Sb_Vlot_Flag; //Sb电源电压设定标志
        private static bool SboffFlag; //叉掉弹窗时，设置电流值为零的标志

        //private static bool DY_Vlot_Flag; //灯源电源电压设定标志
        //private static bool DYoffFlag; //叉掉弹窗时，设置电流值为零的标志

        private static bool KOnFlag; //K电源开标志。
        private static bool NaOnFlag; //K电源开标志。
        private static bool CsOnFlag; //K电源开标志。
        private static bool SbOnFlag; //K电源开标志。

        //private static bool DYOnFlag; //K电源开标志。


        private static int KVoltage = 25,
            NaVoltage = 25,
            CsVoltage = 25,
            SbVoltage = 25; //电源电压

        //读数异常标志
        private static bool[] ReadErrorFlag = new bool[5];
        private static int DelayTimeNet = 100; //网络端口收发信息延时

        //private static int DelayTimeNet280 = 280; //2022-9-14 由于电源开关失控，故将延时提升到110ms,110ms不够，于是将延时改为280
        //private static int DelayTimeSerial = 100; //串口收发信息延时
        private static int DelayReconnectiuion = 1500; //断线重连延时

        //网络定义
        Socket SocketWatch1 = null; //负责监听客户端的套接字
        Socket SocketWatch2 = null;
        Socket SocketWatch3 = null;
        Socket SocketWatch4 = null;

        //创建一个负责和客户端通信的套接字
        Socket SocConnection1,
            SocConnection2,
            SocConnection3,
            SocConnection4;
        Thread Thread1,
            Thread2,
            Thread3,
            Thread4;

        public PowerController()
        {
            try
            {
                NetConnections();
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 网络连接函数
        /// </summary>
        void NetConnections()
        {
            //定义一个套接字用于监听客户端发来的信息  包含3个参数(IP4寻址协议,流式连接,TCP协议)
            SocketWatch1 = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp
            );
            IPAddress ipaddress1 = IPAddress.Parse("192.168.0.201"); //获取文本框输入的IP地址
            //将IP地址和端口号绑定到网络节点endpoint上
            IPEndPoint endpoint1 = new IPEndPoint(ipaddress1, 23); //获取文本框上输入的端口号
            //监听绑定的网络节点
            SocketWatch1.Bind(endpoint1);
            Thread1 = new Thread(ServerRecMsg1);
            Thread1.Start();

            SocketWatch2 = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp
            );
            IPAddress ipaddress2 = IPAddress.Parse("192.168.0.201");
            IPEndPoint endpoint2 = new IPEndPoint(ipaddress2, 26);
            SocketWatch2.Bind(endpoint2);
            Thread2 = new Thread(ServerRecMsg2);
            Thread2.Start();

            SocketWatch3 = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp
            );
            IPAddress ipaddress3 = IPAddress.Parse("192.168.0.201");
            IPEndPoint endpoint3 = new IPEndPoint(ipaddress2, 29);
            SocketWatch3.Bind(endpoint3);
            Thread3 = new Thread(ServerRecMsg3);
            Thread3.Start();

            SocketWatch4 = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp
            );
            IPAddress ipaddress4 = IPAddress.Parse("192.168.0.201");
            IPEndPoint endpoint4 = new IPEndPoint(ipaddress4, 32);
            SocketWatch4.Bind(endpoint4);
            Thread4 = new Thread(ServerRecMsg4);
            Thread4.Start();
        }

        /// <summary>
        /// K电源控制函数
        /// </summary>
        private void ServerRecMsg1()
        {
            float TemCurVal1; //K电源暂存电流值
            //创建一个内存缓冲区 其大小为1024*1024字节  即1M
            byte[] arrServerRecMsg1 = new byte[8];
            byte[] arrServerRecMsg2 = new byte[9];
            int length1;

            bool ErrorFlag1 = false;

            bool CurRdFlag = false;

            //将套接字的监听队列长度限制为20
            SocketWatch1.Listen(20);
            SocConnection1 = SocketWatch1.Accept();
            SocConnection1.ReceiveTimeout = 1;
            while (true)
            {
                try
                { //断网重连
                    if (SocConnection1.Connected == false)
                    {
                        SocConnection1 = null;
                        SocketWatch1.Listen(20);
                        SocConnection1 = SocketWatch1.Accept();
                        SocConnection1.ReceiveTimeout = 1;
                    }
                    //读电流值
                    //if (TotalPauseFlag)
                    {
                        //读电流值
                        if (!CurRdFlag)
                        {
                            ReadErrorFlag[0] = true;
                            //异常处理，先清空缓存，等待1.5s重发。
                            SocConnection1.SendBufferSize = 0;
                            Thread.Sleep(DelayReconnectiuion);
                            NewPower_OF_or_ON(1, 1);
                            SocConnection1.Send(Send_OFForONByte1);
                            Thread.Sleep(DelayTimeNet);
                            length1 = SocConnection1.Receive(arrServerRecMsg1);
                            if (length1 == 8)
                            {
                                if (
                                    arrServerRecMsg1[0] == 0x01
                                    && arrServerRecMsg1[1] == 0x06
                                    && arrServerRecMsg1[2] == 0x00
                                    && arrServerRecMsg1[3] == 0x1B
                                ) //判断帧头 01 06 00 1B
                                {
                                    ReadErrorFlag[0] = false;
                                    CurOldVal_K = 0; //保证掉电重启后，能恢复电源值。
                                }
                            }
                        }
                        else
                        {
                            if (ReadErrorFlag[0])
                            {
                                ReadErrorFlag[0] = false;
                            }
                        }
                        NewPower_Read_Cur(1);
                        SocConnection1.Send(Send_ReadCmdByte1);
                        Thread.Sleep(DelayTimeNet);
                        CurRdFlag = false;
                        //将接收到的信息存入到内存缓冲区,并返回其字节数组的长度
                        length1 = SocConnection1.Receive(arrServerRecMsg2);
                        //收到数据后，命令解析
                        if (length1 == 9)
                        {
                            if (
                                arrServerRecMsg2[0] == 0x01
                                && arrServerRecMsg2[1] == 0x03
                                && arrServerRecMsg2[2] == 0x04
                            ) //判断帧头 01 03 04
                            {
                                TemCurVal1 =
                                    (
                                        BitConverter.ToSingle(
                                            BitConverter.GetBytes(
                                                (arrServerRecMsg2[3]) << 24
                                                    | (arrServerRecMsg2[4]) << 16
                                                    | (arrServerRecMsg2[5]) << 8
                                                    | arrServerRecMsg2[6]
                                            ),
                                            0
                                        )
                                    ) * 1000;
                                if (TemCurVal1 <= 10)
                                {
                                    CurReadVal_K = 0;
                                }
                                else
                                {
                                    CurReadVal_K = (int)TemCurVal1;
                                }
                                CurRdFlag = true;
                            }
                        }
                    }

                    //打开电源
                    if (KOnFlag)
                    {
                        NewPower_OF_or_ON(1, 1);
                        SocConnection1.Send(Send_OFForONByte1);
                        Thread.Sleep(DelayTimeNet);
                        length1 = SocConnection1.Receive(arrServerRecMsg1);
                        if (length1 == 8)
                        {
                            if (
                                arrServerRecMsg1[0] == 0x01
                                && arrServerRecMsg1[1] == 0x06
                                && arrServerRecMsg1[2] == 0x00
                                && arrServerRecMsg1[3] == 0x1B
                            ) //判断帧头 01 06 00 1B
                            {
                                OldOnFalg_K = NewOnFlag_K;
                            }
                            else
                            {
                                NewPower_OF_or_ON(1, 1);
                                SocConnection1.Send(Send_OFForONByte1);
                                Thread.Sleep(DelayTimeNet);
                                length1 = SocConnection1.Receive(arrServerRecMsg1);

                                if (length1 == 8)
                                {
                                    if (
                                        arrServerRecMsg1[0] == 0x01
                                        && arrServerRecMsg1[1] == 0x06
                                        && arrServerRecMsg1[2] == 0x00
                                        && arrServerRecMsg1[3] == 0x1B
                                    ) //判断帧头 01 06 00 1B
                                    {
                                        OldOnFalg_K = NewOnFlag_K;
                                    }
                                }
                            }
                        }
                        else
                        {
                            NewPower_OF_or_ON(1, 1);
                            SocConnection1.Send(Send_OFForONByte1);
                            Thread.Sleep(DelayTimeNet);
                            length1 = SocConnection1.Receive(arrServerRecMsg1);

                            if (length1 == 8)
                            {
                                if (
                                    arrServerRecMsg1[0] == 0x01
                                    && arrServerRecMsg1[1] == 0x06
                                    && arrServerRecMsg1[2] == 0x00
                                    && arrServerRecMsg1[3] == 0x1B
                                ) //判断帧头 01 06 00 1B
                                {
                                    OldOnFalg_K = NewOnFlag_K;
                                }
                            }
                        }
                        KOnFlag = false;
                        continue;
                    }

                    if (NewOnFlag_K != OldOnFalg_K)
                    {
                        if (NewOnFlag_K) //打开K电源
                        {
                            NewPower_OF_or_ON(1, 1);
                            SocConnection1.Send(Send_OFForONByte1);
                            Thread.Sleep(DelayTimeNet);
                            length1 = SocConnection1.Receive(arrServerRecMsg1);
                            if (length1 == 8)
                            {
                                if (
                                    arrServerRecMsg1[0] == 0x01
                                    && arrServerRecMsg1[1] == 0x06
                                    && arrServerRecMsg1[2] == 0x00
                                    && arrServerRecMsg1[3] == 0x1B
                                ) //判断帧头 01 06 00 1B
                                {
                                    OldOnFalg_K = NewOnFlag_K;
                                }
                                else
                                {
                                    NewPower_OF_or_ON(1, 1);
                                    SocConnection1.Send(Send_OFForONByte1);
                                    Thread.Sleep(DelayTimeNet);
                                    length1 = SocConnection1.Receive(arrServerRecMsg1);

                                    if (length1 == 8)
                                    {
                                        if (
                                            arrServerRecMsg1[0] == 0x01
                                            && arrServerRecMsg1[1] == 0x06
                                            && arrServerRecMsg1[2] == 0x00
                                            && arrServerRecMsg1[3] == 0x1B
                                        ) //判断帧头 01 06 00 1B
                                        {
                                            OldOnFalg_K = NewOnFlag_K;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                NewPower_OF_or_ON(1, 1);
                                SocConnection1.Send(Send_OFForONByte1);
                                Thread.Sleep(DelayTimeNet);
                                length1 = SocConnection1.Receive(arrServerRecMsg1);

                                if (length1 == 8)
                                {
                                    if (
                                        arrServerRecMsg1[0] == 0x01
                                        && arrServerRecMsg1[1] == 0x06
                                        && arrServerRecMsg1[2] == 0x00
                                        && arrServerRecMsg1[3] == 0x1B
                                    ) //判断帧头 01 06 00 1B
                                    {
                                        OldOnFalg_K = NewOnFlag_K;
                                    }
                                }
                            }
                        }
                        else
                        {
                            NewPower_OF_or_ON(0, 1);
                            SocConnection1.Send(Send_OFForONByte1);
                            Thread.Sleep(DelayTimeNet);
                            length1 = SocConnection1.Receive(arrServerRecMsg1);
                            if (length1 == 8)
                            {
                                if (
                                    arrServerRecMsg1[0] == 0x01
                                    && arrServerRecMsg1[1] == 0x06
                                    && arrServerRecMsg1[2] == 0x00
                                    && arrServerRecMsg1[3] == 0x1B
                                ) //判断帧头 01 06 00 1B
                                {
                                    OldOnFalg_K = NewOnFlag_K;
                                }
                                else
                                {
                                    NewPower_OF_or_ON(0, 1);
                                    SocConnection1.Send(Send_OFForONByte1);
                                    Thread.Sleep(DelayTimeNet);
                                    length1 = SocConnection1.Receive(arrServerRecMsg1);

                                    if (length1 == 8)
                                    {
                                        if (
                                            arrServerRecMsg1[0] == 0x01
                                            && arrServerRecMsg1[1] == 0x06
                                            && arrServerRecMsg1[2] == 0x00
                                            && arrServerRecMsg1[3] == 0x1B
                                        ) //判断帧头 01 06 00 1B
                                        {
                                            OldOnFalg_K = NewOnFlag_K;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                NewPower_OF_or_ON(0, 1);
                                SocConnection1.Send(Send_OFForONByte1);
                                Thread.Sleep(DelayTimeNet);
                                length1 = SocConnection1.Receive(arrServerRecMsg1);

                                if (length1 == 8)
                                {
                                    if (
                                        arrServerRecMsg1[0] == 0x01
                                        && arrServerRecMsg1[1] == 0x06
                                        && arrServerRecMsg1[2] == 0x00
                                        && arrServerRecMsg1[3] == 0x1B
                                    ) //判断帧头 01 06 00 1B
                                    {
                                        OldOnFalg_K = NewOnFlag_K;
                                    }
                                }
                            }
                        }
                    }
                    //当电流值不等时，设置电流
                    if (CurOldVal_K != CurSetVal_K)
                    {
                        NewPower_Set_Cur(CurSetVal_K, 1);
                        SocConnection1.Send(Send_ICmdByte1);
                        Thread.Sleep(DelayTimeNet);
                        length1 = SocConnection1.Receive(arrServerRecMsg1);
                        if (length1 == 8)
                        {
                            if (
                                arrServerRecMsg1[0] == 0x01
                                && arrServerRecMsg1[1] == 0x10
                                && arrServerRecMsg1[2] == 0x00
                                && arrServerRecMsg1[3] == 0x03
                            ) //判断帧头 01 10 00 03
                            {
                                CurOldVal_K = CurSetVal_K;
                                ErrorFlag1 = true;
                            }
                            else
                            {
                                ErrorFlag1 = false;
                            }
                        }
                        else
                        {
                            ErrorFlag1 = false;
                        }

                        if (ErrorFlag1 == false)
                        {
                            NewPower_Set_Cur(CurSetVal_K, 1);
                            SocConnection1.Send(Send_ICmdByte1);
                            Thread.Sleep(DelayTimeNet);
                            length1 = SocConnection1.Receive(arrServerRecMsg1);
                            if (length1 == 8)
                            {
                                if (
                                    arrServerRecMsg1[0] == 0x01
                                    && arrServerRecMsg1[1] == 0x10
                                    && arrServerRecMsg1[2] == 0x00
                                    && arrServerRecMsg1[3] == 0x03
                                ) //判断帧头 01 10 00 03
                                {
                                    CurOldVal_K = CurSetVal_K;
                                    ErrorFlag1 = true;
                                }
                            }
                            else { }
                        }
                    }
                    //设置电压
                    if (K_Vlot_Flag)
                    {
                        NewPower_Set_Volt(KVoltage, 1); //载入文件中的电压设定值
                        SocConnection1.Send(Send_VCmdByte1);
                        Thread.Sleep(DelayTimeNet);
                        length1 = SocConnection1.Receive(arrServerRecMsg1);

                        if (length1 == 8)
                        {
                            if (
                                arrServerRecMsg1[0] == 0x01
                                && arrServerRecMsg1[1] == 0x10
                                && arrServerRecMsg1[2] == 0x00
                                && arrServerRecMsg1[3] == 0x01
                            ) //判断帧头 01 10 00 01
                            {
                                K_Vlot_Flag = false;
                            }
                            else
                            {
                                NewPower_Set_Volt(KVoltage, 1); //载入文件中的电压设定值
                                SocConnection1.Send(Send_VCmdByte1);
                                Thread.Sleep(DelayTimeNet);
                                length1 = SocConnection1.Receive(arrServerRecMsg1);

                                if (length1 == 8)
                                {
                                    if (
                                        arrServerRecMsg1[0] == 0x01
                                        && arrServerRecMsg1[1] == 0x10
                                        && arrServerRecMsg1[2] == 0x00
                                        && arrServerRecMsg1[3] == 0x01
                                    ) //判断帧头 01 10 00 01
                                    {
                                        K_Vlot_Flag = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            NewPower_Set_Volt(KVoltage, 1); //载入文件中的电压设定值
                            SocConnection1.Send(Send_VCmdByte1);
                            Thread.Sleep(DelayTimeNet);
                            length1 = SocConnection1.Receive(arrServerRecMsg1);

                            if (length1 == 8)
                            {
                                if (
                                    arrServerRecMsg1[0] == 0x01
                                    && arrServerRecMsg1[1] == 0x10
                                    && arrServerRecMsg1[2] == 0x00
                                    && arrServerRecMsg1[3] == 0x01
                                ) //判断帧头 01 10 00 01
                                {
                                    K_Vlot_Flag = false;
                                }
                            }
                        }
                        K_Vlot_Flag = false;
                        continue;
                    }
                    //关闭K电源
                    if (KoffFlag)
                    {
                        //电源值清零
                        NewPower_OF_or_ON(0, 1);
                        SocConnection1.Send(Send_OFForONByte1);
                        Thread.Sleep(DelayTimeNet);
                        length1 = SocConnection1.Receive(arrServerRecMsg1);

                        if (length1 == 8)
                        {
                            if (
                                arrServerRecMsg1[0] == 0x01
                                && arrServerRecMsg1[1] == 0x06
                                && arrServerRecMsg1[2] == 0x00
                                && arrServerRecMsg1[3] == 0x1B
                            ) //判断帧头 01 06 00 1B
                            {
                                OldOnFalg_K = NewOnFlag_K;
                            }
                            else
                            {
                                //电源值清零
                                NewPower_OF_or_ON(0, 1);
                                SocConnection1.Send(Send_OFForONByte1);
                                Thread.Sleep(DelayTimeNet);
                                length1 = SocConnection1.Receive(arrServerRecMsg1);

                                if (length1 == 8)
                                {
                                    if (
                                        arrServerRecMsg1[0] == 0x01
                                        && arrServerRecMsg1[1] == 0x06
                                        && arrServerRecMsg1[2] == 0x00
                                        && arrServerRecMsg1[3] == 0x1B
                                    ) //判断帧头 01 06 00 1B
                                    {
                                        OldOnFalg_K = NewOnFlag_K;
                                    }
                                }
                            }
                        }
                        else
                        {
                            //电源值清零
                            NewPower_OF_or_ON(0, 1);
                            SocConnection1.Send(Send_OFForONByte1);
                            Thread.Sleep(DelayTimeNet);
                            length1 = SocConnection1.Receive(arrServerRecMsg1);

                            if (length1 == 8)
                            {
                                if (
                                    arrServerRecMsg1[0] == 0x01
                                    && arrServerRecMsg1[1] == 0x06
                                    && arrServerRecMsg1[2] == 0x00
                                    && arrServerRecMsg1[3] == 0x1B
                                ) //判断帧头 01 06 00 1B
                                {
                                    OldOnFalg_K = NewOnFlag_K;
                                }
                            }
                        }
                        KoffFlag = false;
                        continue;
                    }
                }
                catch (Exception e5)
                {
                    Console.WriteLine("ServerRecMsg1\r\n");
                    Console.WriteLine(e5.Message);
                }
            }
        }

        /// <summary>
        /// Na电源控制函数
        /// </summary>
        private void ServerRecMsg2()
        {
            float TemCurVal2; //电源暂存电流值
            byte[] arrServerRecMsg2 = new byte[8];
            byte[] arrServerRecMsg3 = new byte[9];
            int length2;
            bool ErrorFlag2 = false;

            bool CurRdFlag2 = false;

            //将套接字的监听队列长度限制为20
            SocketWatch2.Listen(20);
            SocConnection2 = SocketWatch2.Accept();

            SocConnection2.ReceiveTimeout = 1;

            while (true)
            {
                try
                {
                    //断网重连
                    if (SocConnection2.Connected == false)
                    {
                        SocConnection2 = null;
                        SocketWatch2.Listen(20);
                        SocConnection2 = SocketWatch2.Accept();
                        SocConnection2.ReceiveTimeout = 1;
                    }

                    //读电流值
                    //if (TotalPauseFlag)
                    {
                        if (!CurRdFlag2)
                        {
                            ReadErrorFlag[1] = true;
                            SocConnection2.SendBufferSize = 0;
                            Thread.Sleep(DelayReconnectiuion);
                            NewPower_OF_or_ON(1, 2);
                            SocConnection2.Send(Send_OFForONByte2);
                            Thread.Sleep(DelayTimeNet);
                            length2 = SocConnection2.Receive(arrServerRecMsg2);
                            if (length2 == 8)
                            {
                                if (
                                    arrServerRecMsg2[0] == 0x01
                                    && arrServerRecMsg2[1] == 0x06
                                    && arrServerRecMsg2[2] == 0x00
                                    && arrServerRecMsg2[3] == 0x1B
                                ) //判断帧头 01 06 00 1B
                                {
                                    ReadErrorFlag[1] = false;
                                }
                            }
                        }
                        else
                        {
                            if (ReadErrorFlag[1])
                            {
                                ReadErrorFlag[1] = false;
                            }
                        }
                        //读电流值
                        NewPower_Read_Cur(2);
                        SocConnection2.Send(Send_ReadCmdByte2);
                        Thread.Sleep(DelayTimeNet);
                        CurRdFlag2 = false;
                        //将接收到的信息存入到内存缓冲区,并返回其字节数组的长度
                        length2 = SocConnection2.Receive(arrServerRecMsg3);
                        //收到数据后，命令解析
                        if (length2 == 9)
                        {
                            if (
                                arrServerRecMsg3[0] == 0x01
                                && arrServerRecMsg3[1] == 0x03
                                && arrServerRecMsg3[2] == 0x04
                            ) //判断帧头 01 10 10
                            {
                                TemCurVal2 =
                                    (
                                        BitConverter.ToSingle(
                                            BitConverter.GetBytes(
                                                (arrServerRecMsg3[3]) << 24
                                                    | (arrServerRecMsg3[4]) << 16
                                                    | (arrServerRecMsg3[5]) << 8
                                                    | arrServerRecMsg3[6]
                                            ),
                                            0
                                        )
                                    ) * 1000;
                                //txtMsg.AppendText(TemCurVal1.ToString()+ "\r\n");
                                if (TemCurVal2 <= 10)
                                {
                                    CurReadVal_Na = 0;
                                }
                                else
                                {
                                    CurReadVal_Na = (int)TemCurVal2; //读取的电流值(实测发现需+10校正)
                                }
                                CurRdFlag2 = true;
                            }
                        }
                    }
                    //打开电源
                    if (NaOnFlag)
                    {
                        NewPower_OF_or_ON(1, 2);
                        SocConnection2.Send(Send_OFForONByte2);

                        Thread.Sleep(DelayTimeNet);
                        length2 = SocConnection2.Receive(arrServerRecMsg2);

                        if (length2 == 8)
                        {
                            if (
                                arrServerRecMsg2[0] == 0x01
                                && arrServerRecMsg2[1] == 0x06
                                && arrServerRecMsg2[2] == 0x00
                                && arrServerRecMsg2[3] == 0x1B
                            ) //判断帧头 01 06 00 1B
                            { }
                            else
                            {
                                NewPower_OF_or_ON(1, 2);
                                SocConnection2.Send(Send_OFForONByte2);

                                Thread.Sleep(DelayTimeNet);
                                length2 = SocConnection2.Receive(arrServerRecMsg2);

                                if (length2 == 8)
                                {
                                    if (
                                        arrServerRecMsg2[0] == 0x01
                                        && arrServerRecMsg2[1] == 0x06
                                        && arrServerRecMsg2[2] == 0x00
                                        && arrServerRecMsg2[3] == 0x1B
                                    ) //判断帧头 01 06 00 1B
                                    { }
                                }
                            }
                        }
                        else
                        {
                            NewPower_OF_or_ON(1, 2);
                            SocConnection2.Send(Send_OFForONByte2);

                            Thread.Sleep(DelayTimeNet);
                            length2 = SocConnection2.Receive(arrServerRecMsg2);

                            if (length2 == 8)
                            {
                                if (
                                    arrServerRecMsg2[0] == 0x01
                                    && arrServerRecMsg2[1] == 0x06
                                    && arrServerRecMsg2[2] == 0x00
                                    && arrServerRecMsg2[3] == 0x1B
                                ) //判断帧头 01 06 00 1B
                                { }
                            }
                        }
                        NaOnFlag = false;
                        continue;
                    }

                    //Na电源开关
                    if (NewOnFlag_Na != OldOnFalg_Na)
                    {
                        if (NewOnFlag_Na) //打开Na电源
                        {
                            NewPower_OF_or_ON(1, 2);
                            SocConnection2.Send(Send_OFForONByte2);
                            Thread.Sleep(DelayTimeNet);
                            length2 = SocConnection2.Receive(arrServerRecMsg2);

                            if (length2 == 8)
                            {
                                if (
                                    arrServerRecMsg2[0] == 0x01
                                    && arrServerRecMsg2[1] == 0x06
                                    && arrServerRecMsg2[2] == 0x00
                                    && arrServerRecMsg2[3] == 0x1B
                                ) //判断帧头 01 06 00 1B
                                {
                                    OldOnFalg_Na = NewOnFlag_Na;
                                }
                                else
                                {
                                    NewPower_OF_or_ON(1, 2);
                                    SocConnection2.Send(Send_OFForONByte2);
                                    Thread.Sleep(DelayTimeNet);
                                    length2 = SocConnection2.Receive(arrServerRecMsg2);

                                    if (length2 == 8)
                                    {
                                        if (
                                            arrServerRecMsg2[0] == 0x01
                                            && arrServerRecMsg2[1] == 0x06
                                            && arrServerRecMsg2[2] == 0x00
                                            && arrServerRecMsg2[3] == 0x1B
                                        ) //判断帧头 01 06 00 1B
                                        {
                                            OldOnFalg_Na = NewOnFlag_Na;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                NewPower_OF_or_ON(1, 2);
                                SocConnection2.Send(Send_OFForONByte2);
                                Thread.Sleep(DelayTimeNet);
                                length2 = SocConnection2.Receive(arrServerRecMsg2);

                                if (length2 == 8)
                                {
                                    if (
                                        arrServerRecMsg2[0] == 0x01
                                        && arrServerRecMsg2[1] == 0x06
                                        && arrServerRecMsg2[2] == 0x00
                                        && arrServerRecMsg2[3] == 0x1B
                                    ) //判断帧头 01 06 00 1B
                                    {
                                        OldOnFalg_Na = NewOnFlag_Na;
                                    }
                                }
                            }
                        }
                        else
                        {
                            //电源值清零
                            NewPower_OF_or_ON(0, 2);
                            SocConnection2.Send(Send_OFForONByte2);

                            Thread.Sleep(DelayTimeNet);
                            length2 = SocConnection2.Receive(arrServerRecMsg2);

                            if (length2 == 8)
                            {
                                if (
                                    arrServerRecMsg2[0] == 0x01
                                    && arrServerRecMsg2[1] == 0x06
                                    && arrServerRecMsg2[2] == 0x00
                                    && arrServerRecMsg2[3] == 0x1B
                                ) //判断帧头 01 06 00 1B
                                {
                                    OldOnFalg_Na = NewOnFlag_Na;
                                }
                                else
                                {
                                    NewPower_OF_or_ON(0, 2);
                                    SocConnection2.Send(Send_OFForONByte2);

                                    Thread.Sleep(DelayTimeNet);
                                    length2 = SocConnection2.Receive(arrServerRecMsg2);

                                    if (length2 == 8)
                                    {
                                        if (
                                            arrServerRecMsg2[0] == 0x01
                                            && arrServerRecMsg2[1] == 0x06
                                            && arrServerRecMsg2[2] == 0x00
                                            && arrServerRecMsg2[3] == 0x1B
                                        ) //判断帧头 01 06 00 1B
                                        {
                                            OldOnFalg_Na = NewOnFlag_Na;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                NewPower_OF_or_ON(0, 2);
                                SocConnection2.Send(Send_OFForONByte2);

                                Thread.Sleep(DelayTimeNet);
                                length2 = SocConnection2.Receive(arrServerRecMsg2);

                                if (length2 == 8)
                                {
                                    if (
                                        arrServerRecMsg2[0] == 0x01
                                        && arrServerRecMsg2[1] == 0x06
                                        && arrServerRecMsg2[2] == 0x00
                                        && arrServerRecMsg2[3] == 0x1B
                                    ) //判断帧头 01 06 00 1B
                                    {
                                        OldOnFalg_Na = NewOnFlag_Na;
                                    }
                                }
                            }
                        }
                    }
                    //当电流值不等时，设置电流
                    if (CurOldVal_Na != CurSetVal_Na)
                    {
                        NewPower_Set_Cur(CurSetVal_Na, 2);
                        SocConnection2.Send(Send_ICmdByte2);
                        Thread.Sleep(DelayTimeNet);
                        length2 = SocConnection2.Receive(arrServerRecMsg2);

                        if (length2 == 8)
                        {
                            if (
                                arrServerRecMsg2[0] == 0x01
                                && arrServerRecMsg2[1] == 0x10
                                && arrServerRecMsg2[2] == 0x00
                                && arrServerRecMsg2[3] == 0x03
                            ) //判断帧头 01 10 00 03
                            {
                                CurOldVal_Na = CurSetVal_Na;
                                ErrorFlag2 = true;
                            }
                            else
                            {
                                ErrorFlag2 = false;
                            }
                        }
                        else
                        {
                            ErrorFlag2 = false;
                        }

                        if (ErrorFlag2 == false)
                        {
                            NewPower_Set_Cur(CurSetVal_Na, 2);
                            SocConnection2.Send(Send_ICmdByte2);
                            Thread.Sleep(DelayTimeNet);
                            length2 = SocConnection2.Receive(arrServerRecMsg2);
                            if (length2 == 8)
                            {
                                if (
                                    arrServerRecMsg2[0] == 0x01
                                    && arrServerRecMsg2[1] == 0x10
                                    && arrServerRecMsg2[2] == 0x00
                                    && arrServerRecMsg2[3] == 0x03
                                ) //判断帧头 01 10 00 03
                                {
                                    CurOldVal_Na = CurSetVal_Na;
                                    ErrorFlag2 = true;
                                }
                            }
                            else { }
                        }
                    }
                    //设置电压
                    if (Na_Vlot_Flag)
                    {
                        NewPower_Set_Volt(NaVoltage, 2); //载入文件中的电压设定值
                        SocConnection2.Send(Send_VCmdByte2);
                        Thread.Sleep(DelayTimeNet);
                        length2 = SocConnection2.Receive(arrServerRecMsg2);

                        if (length2 == 8)
                        {
                            if (
                                arrServerRecMsg2[0] == 0x01
                                && arrServerRecMsg2[1] == 0x10
                                && arrServerRecMsg2[2] == 0x00
                                && arrServerRecMsg2[3] == 0x01
                            ) //判断帧头 01 10 00 01
                            {
                                Na_Vlot_Flag = false;
                            }
                            else
                            {
                                NewPower_Set_Volt(NaVoltage, 2); //载入文件中的电压设定值
                                SocConnection2.Send(Send_VCmdByte2);
                                Thread.Sleep(DelayTimeNet);
                                length2 = SocConnection2.Receive(arrServerRecMsg2);

                                if (length2 == 8)
                                {
                                    if (
                                        arrServerRecMsg2[0] == 0x01
                                        && arrServerRecMsg2[1] == 0x10
                                        && arrServerRecMsg2[2] == 0x00
                                        && arrServerRecMsg2[3] == 0x01
                                    ) //判断帧头 01 10 00 01
                                    {
                                        Na_Vlot_Flag = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            NewPower_Set_Volt(NaVoltage, 2); //载入文件中的电压设定值
                            SocConnection2.Send(Send_VCmdByte2);
                            Thread.Sleep(DelayTimeNet);
                            length2 = SocConnection2.Receive(arrServerRecMsg2);

                            if (length2 == 8)
                            {
                                if (
                                    arrServerRecMsg3[0] == 0x01
                                    && arrServerRecMsg3[1] == 0x10
                                    && arrServerRecMsg3[2] == 0x00
                                    && arrServerRecMsg3[3] == 0x01
                                ) //判断帧头 01 10 00 01
                                {
                                    Na_Vlot_Flag = false;
                                }
                            }
                        }
                        Na_Vlot_Flag = false;
                        continue;
                    }

                    //关闭Na电源
                    if (NaoffFlag)
                    {
                        //电源值清零
                        NewPower_OF_or_ON(0, 2);
                        SocConnection2.Send(Send_OFForONByte2);
                        Thread.Sleep(DelayTimeNet);
                        length2 = SocConnection2.Receive(arrServerRecMsg2);

                        if (length2 == 8)
                        {
                            if (
                                arrServerRecMsg2[0] == 0x01
                                && arrServerRecMsg2[1] == 0x06
                                && arrServerRecMsg2[2] == 0x00
                                && arrServerRecMsg2[3] == 0x1B
                            ) //判断帧头 01 06 00 1B
                            {
                                OldOnFalg_Na = NewOnFlag_Na;
                            }
                            else
                            {
                                NewPower_OF_or_ON(0, 2);
                                SocConnection2.Send(Send_OFForONByte2);

                                Thread.Sleep(DelayTimeNet);
                                length2 = SocConnection2.Receive(arrServerRecMsg2);

                                if (length2 == 8)
                                {
                                    if (
                                        arrServerRecMsg2[0] == 0x01
                                        && arrServerRecMsg2[1] == 0x06
                                        && arrServerRecMsg2[2] == 0x00
                                        && arrServerRecMsg2[3] == 0x1B
                                    ) //判断帧头 01 06 00 1B
                                    {
                                        OldOnFalg_Na = NewOnFlag_Na;
                                    }
                                }
                            }
                        }
                        else
                        {
                            NewPower_OF_or_ON(0, 2);
                            SocConnection2.Send(Send_OFForONByte2);

                            Thread.Sleep(DelayTimeNet);
                            length2 = SocConnection2.Receive(arrServerRecMsg2);

                            if (length2 == 8)
                            {
                                if (
                                    arrServerRecMsg2[0] == 0x01
                                    && arrServerRecMsg2[1] == 0x06
                                    && arrServerRecMsg2[2] == 0x00
                                    && arrServerRecMsg2[3] == 0x1B
                                ) //判断帧头 01 06 00 1B
                                {
                                    OldOnFalg_Na = NewOnFlag_Na;
                                }
                            }
                        }
                        NaoffFlag = false;
                        continue;
                    }
                }
                catch (Exception e6)
                {
                    Console.WriteLine("ServerRecMsg2\r\n");
                    Console.WriteLine(e6.Message);
                }
            }
        }

        /// <summary>
        /// Cs电源控制函数
        /// </summary>
        private void ServerRecMsg3()
        {
            float TemCurVal3; //电源暂存电流值
            byte[] arrServerRecMsg3 = new byte[8];
            byte[] arrServerRecMsg4 = new byte[9];
            int length3;
            bool ErrorFlag3 = false;

            bool CurRdFlag3 = false;
            //将套接字的监听队列长度限制为20
            SocketWatch3.Listen(20);
            SocConnection3 = SocketWatch3.Accept();
            SocConnection3.ReceiveTimeout = 1;

            while (true)
            {
                try
                {
                    //断网重连
                    if (SocConnection3.Connected == false)
                    {
                        SocConnection3 = null;
                        SocketWatch3.Listen(20);
                        SocConnection3 = SocketWatch3.Accept();
                        SocConnection3.ReceiveTimeout = 1;
                    }

                    //读电流值
                    //if (TotalPauseFlag)
                    {
                        //读电流值
                        if (!CurRdFlag3)
                        {
                            ReadErrorFlag[2] = true;
                            //异常处理，先清空缓存，等待1.5s重发。
                            SocConnection3.SendBufferSize = 0;
                            Thread.Sleep(DelayReconnectiuion);
                            NewPower_OF_or_ON(1, 3);
                            SocConnection3.Send(Send_OFForONByte3);

                            Thread.Sleep(DelayTimeNet);
                            length3 = SocConnection3.Receive(arrServerRecMsg3);
                            if (length3 == 8)
                            {
                                if (
                                    arrServerRecMsg3[0] == 0x01
                                    && arrServerRecMsg3[1] == 0x06
                                    && arrServerRecMsg3[2] == 0x00
                                    && arrServerRecMsg3[3] == 0x1B
                                ) //判断帧头 01 06 00 1B
                                {
                                    ReadErrorFlag[2] = false;
                                }
                            }
                        }
                        else
                        {
                            if (ReadErrorFlag[2])
                            {
                                //label_Pow_Cs.ChangeText("正常");
                                //label_Pow_Cs.ForeColor = Color.Black;
                                ReadErrorFlag[2] = false;
                            }
                        }

                        //读电流值
                        NewPower_Read_Cur(3);
                        SocConnection3.Send(Send_ReadCmdByte3);
                        Thread.Sleep(DelayTimeNet);
                        CurRdFlag3 = false;
                        //将接收到的信息存入到内存缓冲区,并返回其字节数组的长度
                        length3 = SocConnection3.Receive(arrServerRecMsg4);

                        //收到数据后，命令解析
                        if (length3 == 9)
                        {
                            if (
                                arrServerRecMsg4[0] == 0x01
                                && arrServerRecMsg4[1] == 0x03
                                && arrServerRecMsg4[2] == 0x04
                            ) //判断帧头 01 03 04
                            {
                                TemCurVal3 =
                                    (
                                        BitConverter.ToSingle(
                                            BitConverter.GetBytes(
                                                (arrServerRecMsg4[3]) << 24
                                                    | (arrServerRecMsg4[4]) << 16
                                                    | (arrServerRecMsg4[5]) << 8
                                                    | arrServerRecMsg4[6]
                                            ),
                                            0
                                        )
                                    ) * 1000;
                                //txtMsg.AppendText(TemCurVal1.ToString()+ "\r\n");
                                if (TemCurVal3 <= 10)
                                {
                                    CurReadVal_Cs = 0;
                                }
                                else
                                {
                                    CurReadVal_Cs = (int)TemCurVal3; //读取的电流值(实测发现需+10校正)
                                }
                                CurRdFlag3 = true;
                            }
                        }
                    }
                    //打开电源
                    if (CsOnFlag)
                    {
                        NewPower_OF_or_ON(1, 3);
                        SocConnection3.Send(Send_OFForONByte3);

                        Thread.Sleep(DelayTimeNet);
                        length3 = SocConnection3.Receive(arrServerRecMsg3);

                        if (length3 == 8)
                        {
                            if (
                                arrServerRecMsg3[0] == 0x01
                                && arrServerRecMsg3[1] == 0x06
                                && arrServerRecMsg3[2] == 0x00
                                && arrServerRecMsg3[3] == 0x1B
                            ) //判断帧头 01 06 00 1B
                            { }
                            else
                            {
                                NewPower_OF_or_ON(1, 3);
                                SocConnection3.Send(Send_OFForONByte3);

                                Thread.Sleep(DelayTimeNet);
                                length3 = SocConnection3.Receive(arrServerRecMsg3);

                                if (length3 == 8)
                                {
                                    if (
                                        arrServerRecMsg3[0] == 0x01
                                        && arrServerRecMsg3[1] == 0x06
                                        && arrServerRecMsg3[2] == 0x00
                                        && arrServerRecMsg3[3] == 0x1B
                                    ) //判断帧头 01 06 00 1B
                                    { }
                                }
                            }
                        }
                        else
                        {
                            NewPower_OF_or_ON(1, 3);
                            SocConnection3.Send(Send_OFForONByte3);

                            Thread.Sleep(DelayTimeNet);
                            length3 = SocConnection3.Receive(arrServerRecMsg3);

                            if (length3 == 8)
                            {
                                if (
                                    arrServerRecMsg3[0] == 0x01
                                    && arrServerRecMsg3[1] == 0x06
                                    && arrServerRecMsg3[2] == 0x00
                                    && arrServerRecMsg3[3] == 0x1B
                                ) //判断帧头 01 06 00 1B
                                { }
                            }
                        }
                        CsOnFlag = false;
                        continue;
                    }

                    //Cs电源开关
                    if (NewOnFlag_Cs != OldOnFalg_Cs)
                    {
                        if (NewOnFlag_Cs) //打开Cs电源
                        {
                            NewPower_OF_or_ON(1, 3);
                            SocConnection3.Send(Send_OFForONByte3);
                            Thread.Sleep(DelayTimeNet);
                            length3 = SocConnection3.Receive(arrServerRecMsg3);

                            if (length3 == 8)
                            {
                                if (
                                    arrServerRecMsg3[0] == 0x01
                                    && arrServerRecMsg3[1] == 0x06
                                    && arrServerRecMsg3[2] == 0x00
                                    && arrServerRecMsg3[3] == 0x1B
                                ) //判断帧头 01 06 00 1B
                                {
                                    OldOnFalg_Cs = NewOnFlag_Cs;
                                }
                                else
                                {
                                    NewPower_OF_or_ON(1, 3);
                                    SocConnection3.Send(Send_OFForONByte3);
                                    Thread.Sleep(DelayTimeNet);
                                    length3 = SocConnection3.Receive(arrServerRecMsg3);

                                    if (length3 == 8)
                                    {
                                        if (
                                            arrServerRecMsg3[0] == 0x01
                                            && arrServerRecMsg3[1] == 0x06
                                            && arrServerRecMsg3[2] == 0x00
                                            && arrServerRecMsg3[3] == 0x1B
                                        ) //判断帧头 01 06 00 1B
                                        {
                                            OldOnFalg_Cs = NewOnFlag_Cs;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                NewPower_OF_or_ON(1, 3);
                                SocConnection3.Send(Send_OFForONByte3);
                                Thread.Sleep(DelayTimeNet);
                                length3 = SocConnection3.Receive(arrServerRecMsg3);

                                if (length3 == 8)
                                {
                                    if (
                                        arrServerRecMsg3[0] == 0x01
                                        && arrServerRecMsg3[1] == 0x06
                                        && arrServerRecMsg3[2] == 0x00
                                        && arrServerRecMsg3[3] == 0x1B
                                    ) //判断帧头 01 06 00 1B
                                    {
                                        OldOnFalg_Cs = NewOnFlag_Cs;
                                    }
                                }
                            }
                        }
                        else
                        {
                            //电源值清零
                            NewPower_OF_or_ON(0, 3);
                            SocConnection3.Send(Send_OFForONByte3);

                            Thread.Sleep(DelayTimeNet);
                            length3 = SocConnection3.Receive(arrServerRecMsg3);

                            if (length3 == 8)
                            {
                                if (
                                    arrServerRecMsg3[0] == 0x01
                                    && arrServerRecMsg3[1] == 0x06
                                    && arrServerRecMsg3[2] == 0x00
                                    && arrServerRecMsg3[3] == 0x1B
                                ) //判断帧头 01 06 00 1B
                                {
                                    OldOnFalg_Cs = NewOnFlag_Cs;
                                }
                                else
                                {
                                    NewPower_OF_or_ON(0, 3);
                                    SocConnection3.Send(Send_OFForONByte3);

                                    Thread.Sleep(DelayTimeNet);
                                    length3 = SocConnection3.Receive(arrServerRecMsg3);

                                    if (length3 == 8)
                                    {
                                        if (
                                            arrServerRecMsg3[0] == 0x01
                                            && arrServerRecMsg3[1] == 0x06
                                            && arrServerRecMsg3[2] == 0x00
                                            && arrServerRecMsg3[3] == 0x1B
                                        ) //判断帧头 01 06 00 1B
                                        {
                                            OldOnFalg_Cs = NewOnFlag_Cs;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                NewPower_OF_or_ON(0, 3);
                                SocConnection3.Send(Send_OFForONByte3);

                                Thread.Sleep(DelayTimeNet);
                                length3 = SocConnection3.Receive(arrServerRecMsg3);

                                if (length3 == 8)
                                {
                                    if (
                                        arrServerRecMsg3[0] == 0x01
                                        && arrServerRecMsg3[1] == 0x06
                                        && arrServerRecMsg3[2] == 0x00
                                        && arrServerRecMsg3[3] == 0x1B
                                    ) //判断帧头 01 06 00 1B
                                    {
                                        OldOnFalg_Cs = NewOnFlag_Cs;
                                    }
                                }
                            }
                        }
                    }
                    //当电流值不等时，设置电流
                    if (CurOldVal_Cs != CurSetVal_Cs)
                    {
                        NewPower_Set_Cur(CurSetVal_Cs, 3);
                        SocConnection3.Send(Send_ICmdByte3);
                        Thread.Sleep(DelayTimeNet);
                        length3 = SocConnection3.Receive(arrServerRecMsg3);

                        if (length3 == 8)
                        {
                            if (
                                arrServerRecMsg3[0] == 0x01
                                && arrServerRecMsg3[1] == 0x10
                                && arrServerRecMsg3[2] == 0x00
                                && arrServerRecMsg3[3] == 0x03
                            ) //判断帧头 01 06 00 03
                            {
                                CurOldVal_Cs = CurSetVal_Cs;
                                ErrorFlag3 = true;
                            }
                            else
                            {
                                ErrorFlag3 = false;
                            }
                        }
                        else
                        {
                            ErrorFlag3 = false;
                        }

                        if (ErrorFlag3 == false)
                        {
                            NewPower_Set_Cur(CurSetVal_Cs, 3);
                            SocConnection3.Send(Send_ICmdByte3);
                            Thread.Sleep(DelayTimeNet);
                            length3 = SocConnection3.Receive(arrServerRecMsg3);
                            if (length3 == 8)
                            {
                                if (
                                    arrServerRecMsg3[0] == 0x01
                                    && arrServerRecMsg3[1] == 0x10
                                    && arrServerRecMsg3[2] == 0x00
                                    && arrServerRecMsg3[3] == 0x03
                                ) //判断帧头 01 06 00 03
                                {
                                    CurOldVal_Cs = CurSetVal_Cs;
                                    ErrorFlag3 = true;
                                }
                            }
                            else { }
                        }
                    }
                    //设置电压
                    if (Cs_Vlot_Flag == true)
                    {
                        NewPower_Set_Volt(CsVoltage, 3); //载入文件中的电压设定值
                        SocConnection3.Send(Send_VCmdByte3);
                        Thread.Sleep(DelayTimeNet);
                        length3 = SocConnection3.Receive(arrServerRecMsg3);

                        if (length3 == 8)
                        {
                            if (
                                arrServerRecMsg3[0] == 0x01
                                && arrServerRecMsg3[1] == 0x10
                                && arrServerRecMsg3[2] == 0x00
                                && arrServerRecMsg3[3] == 0x01
                            ) //判断帧头 01 10 00 01
                            {
                                Cs_Vlot_Flag = false;
                            }
                            else
                            {
                                NewPower_Set_Volt(CsVoltage, 3); //载入文件中的电压设定值
                                SocConnection3.Send(Send_VCmdByte3);
                                Thread.Sleep(DelayTimeNet);
                                length3 = SocConnection3.Receive(arrServerRecMsg3);

                                if (length3 == 8)
                                {
                                    if (
                                        arrServerRecMsg3[0] == 0x01
                                        && arrServerRecMsg3[1] == 0x10
                                        && arrServerRecMsg3[2] == 0x00
                                        && arrServerRecMsg3[3] == 0x01
                                    ) //判断帧头 01 10 00 01
                                    {
                                        Cs_Vlot_Flag = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            NewPower_Set_Volt(CsVoltage, 3); //载入文件中的电压设定值
                            SocConnection3.Send(Send_VCmdByte3);
                            Thread.Sleep(DelayTimeNet);
                            length3 = SocConnection3.Receive(arrServerRecMsg3);

                            if (length3 == 8)
                            {
                                if (
                                    arrServerRecMsg3[0] == 0x01
                                    && arrServerRecMsg3[1] == 0x10
                                    && arrServerRecMsg3[2] == 0x00
                                    && arrServerRecMsg3[3] == 0x01
                                ) //判断帧头 01 10 00 01
                                {
                                    Cs_Vlot_Flag = false;
                                }
                            }
                        }
                        Cs_Vlot_Flag = false;
                        continue;
                    }
                    //关闭Cs电源
                    if (CsoffFlag)
                    {
                        //电源值清零
                        NewPower_OF_or_ON(0, 3);
                        SocConnection3.Send(Send_OFForONByte3);

                        Thread.Sleep(DelayTimeNet);
                        length3 = SocConnection3.Receive(arrServerRecMsg3);

                        if (length3 == 8)
                        {
                            if (
                                arrServerRecMsg3[0] == 0x01
                                && arrServerRecMsg3[1] == 0x06
                                && arrServerRecMsg3[2] == 0x00
                                && arrServerRecMsg3[3] == 0x1B
                            ) //判断帧头 01 06 00 1B
                            {
                                OldOnFalg_Cs = NewOnFlag_Cs;
                            }
                            else
                            {
                                NewPower_OF_or_ON(0, 3);
                                SocConnection3.Send(Send_OFForONByte3);

                                Thread.Sleep(DelayTimeNet);
                                length3 = SocConnection3.Receive(arrServerRecMsg3);

                                if (length3 == 8)
                                {
                                    if (
                                        arrServerRecMsg3[0] == 0x01
                                        && arrServerRecMsg3[1] == 0x06
                                        && arrServerRecMsg3[2] == 0x00
                                        && arrServerRecMsg3[3] == 0x1B
                                    ) //判断帧头 01 06 00 1B
                                    {
                                        OldOnFalg_Cs = NewOnFlag_Cs;
                                    }
                                }
                            }
                        }
                        else
                        {
                            NewPower_OF_or_ON(0, 3);
                            SocConnection3.Send(Send_OFForONByte3);

                            Thread.Sleep(DelayTimeNet);
                            length3 = SocConnection3.Receive(arrServerRecMsg3);

                            if (length3 == 8)
                            {
                                if (
                                    arrServerRecMsg3[0] == 0x01
                                    && arrServerRecMsg3[1] == 0x06
                                    && arrServerRecMsg3[2] == 0x00
                                    && arrServerRecMsg3[3] == 0x1B
                                ) //判断帧头 01 06 00 1B
                                {
                                    OldOnFalg_Cs = NewOnFlag_Cs;
                                }
                            }
                        }
                        CsoffFlag = false;
                        continue;
                    }
                }
                catch (Exception e7)
                {
                    Console.WriteLine("ServerRecMsg3\r\n");
                    Console.WriteLine(e7.Message);
                }
            }
        }

        /// <summary>
        /// Sb电源控制函数
        /// </summary>
        private void ServerRecMsg4()
        {
            float TemCurVal4; //电源暂存电流值
            byte[] arrServerRecMsg4 = new byte[8];
            byte[] arrServerRecMsg5 = new byte[9];
            int length4;
            bool ErrorFlag4 = false;

            bool CurRdFlag4 = false;

            //将套接字的监听队列长度限制为20
            SocketWatch4.Listen(20);
            SocConnection4 = SocketWatch4.Accept();
            SocConnection4.ReceiveTimeout = 1;

            while (true)
            {
                try
                {
                    //断网重连
                    if (SocConnection4.Connected == false)
                    {
                        SocConnection4 = null;
                        SocketWatch4.Listen(20);
                        SocConnection4 = SocketWatch4.Accept();
                        SocConnection4.ReceiveTimeout = 1;
                    }
                    //读电流值
                    //if (TotalPauseFlag)
                    {
                        //读电流值
                        if (!CurRdFlag4)
                        {
                            ReadErrorFlag[3] = true;
                            //异常处理，先清空缓存，等待1.5s重发。
                            SocConnection4.SendBufferSize = 0;
                            Thread.Sleep(DelayReconnectiuion);
                            NewPower_OF_or_ON(1, 4);
                            SocConnection4.Send(Send_OFForONByte4);
                            Thread.Sleep(DelayTimeNet);
                            length4 = SocConnection4.Receive(arrServerRecMsg4);
                            if (length4 == 8)
                            {
                                if (
                                    arrServerRecMsg4[0] == 0x01
                                    && arrServerRecMsg4[1] == 0x06
                                    && arrServerRecMsg4[2] == 0x00
                                    && arrServerRecMsg4[3] == 0x1B
                                ) //判断帧头 01 06 00 1B
                                {
                                    ReadErrorFlag[3] = false;
                                }
                            }
                        }
                        else
                        {
                            if (ReadErrorFlag[3])
                            {
                                //label_Pow_Sb.ChangeText("正常");
                                //label_Pow_Sb.ForeColor = Color.Black;
                                ReadErrorFlag[3] = false;
                            }
                        }
                        //读电流值
                        NewPower_Read_Cur(4);
                        SocConnection4.Send(Send_ReadCmdByte4);
                        Thread.Sleep(DelayTimeNet);
                        CurRdFlag4 = false;
                        //将接收到的信息存入到内存缓冲区,并返回其字节数组的长度
                        length4 = SocConnection4.Receive(arrServerRecMsg5);

                        //收到数据后，命令解析
                        if (length4 == 9)
                        {
                            if (
                                arrServerRecMsg5[0] == 0x01
                                && arrServerRecMsg5[1] == 0x03
                                && arrServerRecMsg5[2] == 0x04
                            ) //判断帧头 01 03 04
                            {
                                TemCurVal4 =
                                    (
                                        BitConverter.ToSingle(
                                            BitConverter.GetBytes(
                                                (arrServerRecMsg5[3]) << 24
                                                    | (arrServerRecMsg5[4]) << 16
                                                    | (arrServerRecMsg5[5]) << 8
                                                    | arrServerRecMsg5[6]
                                            ),
                                            0
                                        )
                                    ) * 1000;
                                //txtMsg.AppendText(TemCurVal1.ToString()+ "\r\n");
                                if (TemCurVal4 <= 10)
                                {
                                    CurReadVal_Sb = 0;
                                }
                                else
                                {
                                    CurReadVal_Sb = (int)TemCurVal4; //读取的电流值(实测发现需+10校正)
                                }
                                CurRdFlag4 = true;
                            }
                        }
                    }
                    if (SbOnFlag)
                    {
                        NewPower_OF_or_ON(1, 4);
                        SocConnection4.Send(Send_OFForONByte4);

                        Thread.Sleep(DelayTimeNet);
                        length4 = SocConnection4.Receive(arrServerRecMsg4);

                        if (length4 == 8)
                        {
                            if (
                                arrServerRecMsg4[0] == 0x01
                                && arrServerRecMsg4[1] == 0x06
                                && arrServerRecMsg4[2] == 0x00
                                && arrServerRecMsg4[3] == 0x1B
                            ) //判断帧头 01 06 00 1B
                            { }
                            else
                            {
                                NewPower_OF_or_ON(1, 4);
                                SocConnection4.Send(Send_OFForONByte4);

                                Thread.Sleep(DelayTimeNet);
                                length4 = SocConnection4.Receive(arrServerRecMsg4);

                                if (length4 == 8)
                                {
                                    if (
                                        arrServerRecMsg4[0] == 0x01
                                        && arrServerRecMsg4[1] == 0x06
                                        && arrServerRecMsg4[2] == 0x00
                                        && arrServerRecMsg4[3] == 0x1B
                                    ) //判断帧头 01 06 00 1B
                                    { }
                                }
                            }
                        }
                        else
                        {
                            NewPower_OF_or_ON(1, 4);
                            SocConnection4.Send(Send_OFForONByte4);

                            Thread.Sleep(DelayTimeNet);
                            length4 = SocConnection4.Receive(arrServerRecMsg4);

                            if (length4 == 8)
                            {
                                if (
                                    arrServerRecMsg4[0] == 0x01
                                    && arrServerRecMsg4[1] == 0x06
                                    && arrServerRecMsg4[2] == 0x00
                                    && arrServerRecMsg4[3] == 0x1B
                                ) //判断帧头 01 06 00 1B
                                { }
                            }
                        }
                        SbOnFlag = false;
                        continue;
                    }

                    //Sb电源开关
                    if (NewOnFlag_Sb != OldOnFalg_Sb)
                    {
                        if (NewOnFlag_Sb) //打开电源
                        {
                            NewPower_OF_or_ON(1, 4);
                            SocConnection4.Send(Send_OFForONByte4);
                            Thread.Sleep(DelayTimeNet);
                            length4 = SocConnection4.Receive(arrServerRecMsg4);

                            if (length4 == 8)
                            {
                                if (
                                    arrServerRecMsg4[0] == 0x01
                                    && arrServerRecMsg4[1] == 0x06
                                    && arrServerRecMsg4[2] == 0x00
                                    && arrServerRecMsg4[3] == 0x1B
                                ) //判断帧头 01 06 00 1B
                                {
                                    OldOnFalg_Sb = NewOnFlag_Sb;
                                }
                                else
                                {
                                    NewPower_OF_or_ON(1, 4);
                                    SocConnection4.Send(Send_OFForONByte4);
                                    Thread.Sleep(DelayTimeNet);
                                    length4 = SocConnection4.Receive(arrServerRecMsg4);

                                    if (length4 == 8)
                                    {
                                        if (
                                            arrServerRecMsg4[0] == 0x01
                                            && arrServerRecMsg4[1] == 0x06
                                            && arrServerRecMsg4[2] == 0x00
                                            && arrServerRecMsg4[3] == 0x1B
                                        ) //判断帧头 01 06 00 1B
                                        {
                                            OldOnFalg_Sb = NewOnFlag_Sb;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                NewPower_OF_or_ON(1, 4);
                                SocConnection4.Send(Send_OFForONByte4);
                                Thread.Sleep(DelayTimeNet);
                                length4 = SocConnection4.Receive(arrServerRecMsg4);

                                if (length4 == 8)
                                {
                                    if (
                                        arrServerRecMsg4[0] == 0x01
                                        && arrServerRecMsg4[1] == 0x06
                                        && arrServerRecMsg4[2] == 0x00
                                        && arrServerRecMsg4[3] == 0x1B
                                    ) //判断帧头 01 06 00 1B
                                    {
                                        OldOnFalg_Sb = NewOnFlag_Sb;
                                    }
                                }
                            }
                        }
                        else
                        {
                            //电源值清零
                            NewPower_OF_or_ON(0, 4);
                            SocConnection4.Send(Send_OFForONByte4);

                            Thread.Sleep(DelayTimeNet);
                            length4 = SocConnection4.Receive(arrServerRecMsg4);

                            if (length4 == 8)
                            {
                                if (
                                    arrServerRecMsg4[0] == 0x01
                                    && arrServerRecMsg4[1] == 0x06
                                    && arrServerRecMsg4[2] == 0x00
                                    && arrServerRecMsg4[3] == 0x1B
                                ) //判断帧头 01 06 00 1B
                                {
                                    OldOnFalg_Sb = NewOnFlag_Sb;
                                }
                                else
                                {
                                    NewPower_OF_or_ON(0, 4);
                                    SocConnection4.Send(Send_OFForONByte4);
                                    Thread.Sleep(DelayTimeNet);
                                    length4 = SocConnection4.Receive(arrServerRecMsg4);

                                    if (length4 == 8)
                                    {
                                        if (
                                            arrServerRecMsg4[0] == 0x01
                                            && arrServerRecMsg4[1] == 0x06
                                            && arrServerRecMsg4[2] == 0x00
                                            && arrServerRecMsg4[3] == 0x1B
                                        ) //判断帧头 01 06 00 1B
                                        {
                                            OldOnFalg_Sb = NewOnFlag_Sb;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                NewPower_OF_or_ON(0, 4);
                                SocConnection4.Send(Send_OFForONByte4);
                                Thread.Sleep(DelayTimeNet);
                                length4 = SocConnection4.Receive(arrServerRecMsg4);

                                if (length4 == 8)
                                {
                                    if (
                                        arrServerRecMsg4[0] == 0x01
                                        && arrServerRecMsg4[1] == 0x06
                                        && arrServerRecMsg4[2] == 0x00
                                        && arrServerRecMsg4[3] == 0x1B
                                    ) //判断帧头 01 06 00 1B
                                    {
                                        OldOnFalg_Sb = NewOnFlag_Sb;
                                    }
                                }
                            }
                        }
                    }

                    //当电流值不等时，设置电流
                    if (CurOldVal_Sb != CurSetVal_Sb)
                    {
                        NewPower_Set_Cur(CurSetVal_Sb, 4);
                        SocConnection4.Send(Send_ICmdByte4);
                        Thread.Sleep(DelayTimeNet);
                        length4 = SocConnection4.Receive(arrServerRecMsg4);

                        if (length4 == 8)
                        {
                            if (
                                arrServerRecMsg4[0] == 0x01
                                && arrServerRecMsg4[1] == 0x10
                                && arrServerRecMsg4[2] == 0x00
                                && arrServerRecMsg4[3] == 0x03
                            ) //判断帧头 01 10 00 03
                            {
                                CurOldVal_Sb = CurSetVal_Sb;
                                ErrorFlag4 = true;
                            }
                            else
                            {
                                ErrorFlag4 = false;
                            }
                        }
                        else
                        {
                            ErrorFlag4 = false;
                        }

                        if (ErrorFlag4 == false)
                        {
                            NewPower_Set_Cur(CurSetVal_Sb, 4);
                            SocConnection4.Send(Send_ICmdByte4);
                            Thread.Sleep(DelayTimeNet);
                            length4 = SocConnection4.Receive(arrServerRecMsg4);
                            if (length4 == 8)
                            {
                                if (
                                    arrServerRecMsg4[0] == 0x01
                                    && arrServerRecMsg4[1] == 0x10
                                    && arrServerRecMsg4[2] == 0x00
                                    && arrServerRecMsg4[3] == 0x03
                                ) //判断帧头 01 10 00 03
                                {
                                    CurOldVal_Sb = CurSetVal_Sb;
                                    ErrorFlag4 = true;
                                }
                            }
                            else { }
                        }
                    }

                    //设置电压
                    if (Sb_Vlot_Flag == true)
                    {
                        NewPower_Set_Volt(SbVoltage, 4); //载入文件中的电压设定值
                        SocConnection4.Send(Send_VCmdByte4);
                        Thread.Sleep(DelayTimeNet);
                        length4 = SocConnection4.Receive(arrServerRecMsg4);

                        if (length4 == 8)
                        {
                            if (
                                arrServerRecMsg4[0] == 0x01
                                && arrServerRecMsg4[1] == 0x10
                                && arrServerRecMsg4[2] == 0x00
                                && arrServerRecMsg4[3] == 0x01
                            ) //判断帧头 01 10 00 01
                            {
                                Sb_Vlot_Flag = false;
                            }
                            else
                            {
                                NewPower_Set_Volt(SbVoltage, 4); //载入文件中的电压设定值
                                SocConnection4.Send(Send_VCmdByte4);
                                Thread.Sleep(DelayTimeNet);
                                length4 = SocConnection4.Receive(arrServerRecMsg4);

                                if (length4 == 8)
                                {
                                    if (
                                        arrServerRecMsg4[0] == 0x01
                                        && arrServerRecMsg4[1] == 0x10
                                        && arrServerRecMsg4[2] == 0x00
                                        && arrServerRecMsg4[3] == 0x01
                                    ) //判断帧头 01 10 00 01
                                    {
                                        Sb_Vlot_Flag = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            NewPower_Set_Volt(SbVoltage, 4); //载入文件中的电压设定值
                            SocConnection4.Send(Send_VCmdByte4);
                            Thread.Sleep(DelayTimeNet);
                            length4 = SocConnection4.Receive(arrServerRecMsg4);

                            if (length4 == 8)
                            {
                                if (
                                    arrServerRecMsg4[0] == 0x01
                                    && arrServerRecMsg4[1] == 0x10
                                    && arrServerRecMsg4[2] == 0x00
                                    && arrServerRecMsg4[3] == 0x01
                                ) //判断帧头 01 10 00 01
                                {
                                    Sb_Vlot_Flag = false;
                                }
                            }
                        }
                        Sb_Vlot_Flag = false;
                        continue;
                    }

                    //关闭Sb电源
                    if (SboffFlag)
                    {
                        //电源值清零
                        NewPower_OF_or_ON(0, 4);
                        SocConnection4.Send(Send_OFForONByte4);

                        Thread.Sleep(DelayTimeNet);
                        length4 = SocConnection4.Receive(arrServerRecMsg4);

                        if (length4 == 8)
                        {
                            if (
                                arrServerRecMsg4[0] == 0x01
                                && arrServerRecMsg4[1] == 0x06
                                && arrServerRecMsg4[2] == 0x00
                                && arrServerRecMsg4[3] == 0x1B
                            ) //判断帧头 01 06 00 1B
                            {
                                OldOnFalg_Sb = NewOnFlag_Sb;
                            }
                            else
                            {
                                NewPower_OF_or_ON(0, 4);
                                SocConnection4.Send(Send_OFForONByte4);

                                Thread.Sleep(DelayTimeNet);
                                length4 = SocConnection4.Receive(arrServerRecMsg4);

                                if (length4 == 8)
                                {
                                    if (
                                        arrServerRecMsg4[0] == 0x01
                                        && arrServerRecMsg4[1] == 0x06
                                        && arrServerRecMsg4[2] == 0x00
                                        && arrServerRecMsg4[3] == 0x1B
                                    ) //判断帧头 01 06 00 1B
                                    {
                                        OldOnFalg_Sb = NewOnFlag_Sb;
                                    }
                                }
                            }
                        }
                        else
                        {
                            NewPower_OF_or_ON(0, 4);
                            SocConnection4.Send(Send_OFForONByte4);

                            Thread.Sleep(DelayTimeNet);
                            length4 = SocConnection4.Receive(arrServerRecMsg4);

                            if (length4 == 8)
                            {
                                if (
                                    arrServerRecMsg4[0] == 0x01
                                    && arrServerRecMsg4[1] == 0x06
                                    && arrServerRecMsg4[2] == 0x00
                                    && arrServerRecMsg4[3] == 0x1B
                                ) //判断帧头 01 06 00 1B
                                {
                                    OldOnFalg_Sb = NewOnFlag_Sb;
                                }
                            }
                        }
                        SboffFlag = false;
                        continue;
                    }
                }
                catch (Exception e8)
                {
                    Console.WriteLine("ServerRecMsg4\r\n");
                    Console.WriteLine(e8.Message);
                }
            }
        }

        /// <summary>
        /// 电压设置函数
        /// </summary>
        /// <param name="VoltVal">设置电压数值</param>
        /// <param name="PoNo">通过电源地址码进行设置</param>
        void NewPower_Set_Volt(double VoltVal, int PoNo)
        {
            UInt16 CRCVal = 0;
            UInt16 Vdata = 0,
                Vdata1 = 0;
            byte[] VSetValue = new byte[4];

            if (VoltVal > Vmax_HSP)
            {
                switch (PoNo)
                {
                    case 1:
                        Send_VCmdByte1 = null;
                        break;
                    case 2:
                        Send_VCmdByte2 = null;
                        break;
                    case 3:
                        Send_VCmdByte3 = null;
                        break;
                    case 4:
                        Send_VCmdByte4 = null;
                        break;
                    case 5:
                        Send_VCmdByte5 = null;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                Vdata = (UInt16)((65535 * VoltVal) / Vmax_HSP);
                Vdata1 = (UInt16)((65535 * VoltVal) / Vmax_PLD);
                VSetValue = BitConverter.GetBytes((float)VoltVal).Reverse().ToArray();
            }
            switch (PoNo)
            {
                case 1:
                {
                    //设置电源的字节参数
                    Send_VCmdByte1[0] = 0x01; //
                    Send_VCmdByte1[1] = 0x10; //功能码10表示预设寄存器
                    Send_VCmdByte1[2] = 0x00; //
                    Send_VCmdByte1[3] = 0x01; //电压地址
                    Send_VCmdByte1[4] = 0x00; //
                    Send_VCmdByte1[5] = 0x02; //2个寄存器
                    Send_VCmdByte1[6] = 0x04; //4个字节数据
                    Send_VCmdByte1[7] = VSetValue[0]; //4个数据
                    Send_VCmdByte1[8] = VSetValue[1];
                    Send_VCmdByte1[9] = VSetValue[2];
                    Send_VCmdByte1[10] = VSetValue[3];

                    CRCVal = CRC16(Send_VCmdByte1, 11); //CRC校验
                    Send_VCmdByte1[11] = (byte)(CRCVal % 256); //CRC校验码需交换高低字节
                    Send_VCmdByte1[12] = (byte)(CRCVal / 256);

                    break;
                }
                case 2:
                {
                    //设置电源的字节参数
                    Send_VCmdByte2[0] = 0x01; //
                    Send_VCmdByte2[1] = 0x10; //功能码10表示预设寄存器
                    Send_VCmdByte2[2] = 0x00; //
                    Send_VCmdByte2[3] = 0x01; //电压地址
                    Send_VCmdByte2[4] = 0x00; //
                    Send_VCmdByte2[5] = 0x02; //2个寄存器
                    Send_VCmdByte2[6] = 0x04; //4个字节数据
                    Send_VCmdByte2[7] = VSetValue[0]; //4个数据
                    Send_VCmdByte2[8] = VSetValue[1];
                    Send_VCmdByte2[9] = VSetValue[2];
                    Send_VCmdByte2[10] = VSetValue[3];

                    CRCVal = CRC16(Send_VCmdByte2, 11); //CRC校验
                    Send_VCmdByte2[11] = (byte)(CRCVal % 256); //CRC校验码需交换高低字节
                    Send_VCmdByte2[12] = (byte)(CRCVal / 256);

                    break;
                }
                case 3:
                {
                    Send_VCmdByte3[0] = 0x01; //
                    Send_VCmdByte3[1] = 0x10; //功能码10表示预设寄存器
                    Send_VCmdByte3[2] = 0x00; //
                    Send_VCmdByte3[3] = 0x01; //电压地址
                    Send_VCmdByte3[4] = 0x00; //
                    Send_VCmdByte3[5] = 0x02; //2个寄存器
                    Send_VCmdByte3[6] = 0x04; //4个字节数据
                    Send_VCmdByte3[7] = VSetValue[0]; //4个数据
                    Send_VCmdByte3[8] = VSetValue[1];
                    Send_VCmdByte3[9] = VSetValue[2];
                    Send_VCmdByte3[10] = VSetValue[3];

                    CRCVal = CRC16(Send_VCmdByte3, 11); //CRC校验
                    Send_VCmdByte3[11] = (byte)(CRCVal % 256); //CRC校验码需交换高低字节
                    Send_VCmdByte3[12] = (byte)(CRCVal / 256);

                    break;
                }
                case 4:
                {
                    Send_VCmdByte4[0] = 0x01; //
                    Send_VCmdByte4[1] = 0x10; //功能码10表示预设寄存器
                    Send_VCmdByte4[2] = 0x00; //
                    Send_VCmdByte4[3] = 0x01; //电压地址
                    Send_VCmdByte4[4] = 0x00; //
                    Send_VCmdByte4[5] = 0x02; //2个寄存器
                    Send_VCmdByte4[6] = 0x04; //4个字节数据
                    Send_VCmdByte4[7] = VSetValue[0]; //4个数据
                    Send_VCmdByte4[8] = VSetValue[1];
                    Send_VCmdByte4[9] = VSetValue[2];
                    Send_VCmdByte4[10] = VSetValue[3];

                    CRCVal = CRC16(Send_VCmdByte4, 11); //CRC校验
                    Send_VCmdByte4[11] = (byte)(CRCVal % 256); //CRC校验码需交换高低字节
                    Send_VCmdByte4[12] = (byte)(CRCVal / 256);

                    break;
                }
                case 5:
                {
                    //设置电源的字节参数
                    Send_VCmdByte5[0] = 0x01; //(byte)RD_ADDR.Value;
                    Send_VCmdByte5[1] = 0x06; //功能码06表示预设单个寄存器
                    Send_VCmdByte5[2] = 0x00; //2-3位是参数地址
                    Send_VCmdByte5[3] = 0x03; //00 03表示设定电压
                    //电源电压值数据data=(65535*Vx)/Vmax
                    Send_VCmdByte5[4] = (byte)(Vdata / 256); //电压值高8位
                    Send_VCmdByte5[5] = (byte)(Vdata % 256); //电压低8位

                    CRCVal = CRC16(Send_VCmdByte5, 6); //CRC校验
                    Send_VCmdByte5[6] = (byte)(CRCVal % 256); //CRC校验码需交换高低字节
                    Send_VCmdByte5[7] = (byte)(CRCVal / 256);

                    break;
                }
                default:
                    break;
            }
        }

        /// <summary>
        /// 电流读取函数
        ///
        /// </summary>
        /// <param name="PoNo">通过电源地址码进行读数</param>
        void NewPower_Read_Cur(int PoNo)
        {
            UInt16 CRCVal = 0;

            switch (PoNo)
            {
                case 1:
                {
                    Send_ReadCmdByte1[0] = 0x01; //(byte)RD_ADDR.Value;
                    Send_ReadCmdByte1[1] = 0x03;
                    Send_ReadCmdByte1[2] = 0x00;
                    Send_ReadCmdByte1[3] = 0x1F;

                    Send_ReadCmdByte1[4] = 0x00;
                    Send_ReadCmdByte1[5] = 0x02;

                    CRCVal = CRC16(Send_ReadCmdByte1, 6); //CRC校验
                    Send_ReadCmdByte1[6] = (byte)(CRCVal % 256); //CRC校验码需交换高低字节
                    Send_ReadCmdByte1[7] = (byte)(CRCVal / 256);

                    break;
                }
                case 2:
                {
                    Send_ReadCmdByte2[0] = 0x01; //(byte)RD_ADDR.Value;
                    Send_ReadCmdByte2[1] = 0x03;
                    Send_ReadCmdByte2[2] = 0x00;
                    Send_ReadCmdByte2[3] = 0x1F;

                    Send_ReadCmdByte2[4] = 0x00;
                    Send_ReadCmdByte2[5] = 0x02;

                    CRCVal = CRC16(Send_ReadCmdByte2, 6); //CRC校验
                    Send_ReadCmdByte2[6] = (byte)(CRCVal % 256); //CRC校验码需交换高低字节
                    Send_ReadCmdByte2[7] = (byte)(CRCVal / 256);
                    break;
                }
                case 3:
                {
                    Send_ReadCmdByte3[0] = 0x01; //(byte)RD_ADDR.Value;
                    Send_ReadCmdByte3[1] = 0x03;
                    Send_ReadCmdByte3[2] = 0x00;
                    Send_ReadCmdByte3[3] = 0x1F;

                    Send_ReadCmdByte3[4] = 0x00;
                    Send_ReadCmdByte3[5] = 0x02;

                    CRCVal = CRC16(Send_ReadCmdByte3, 6); //CRC校验
                    Send_ReadCmdByte3[6] = (byte)(CRCVal % 256); //CRC校验码需交换高低字节
                    Send_ReadCmdByte3[7] = (byte)(CRCVal / 256);
                    break;
                }
                case 4:
                {
                    Send_ReadCmdByte4[0] = 0x01; //(byte)RD_ADDR.Value;
                    Send_ReadCmdByte4[1] = 0x03;
                    Send_ReadCmdByte4[2] = 0x00;
                    Send_ReadCmdByte4[3] = 0x1F;

                    Send_ReadCmdByte4[4] = 0x00;
                    Send_ReadCmdByte4[5] = 0x02;

                    CRCVal = CRC16(Send_ReadCmdByte4, 6); //CRC校验
                    Send_ReadCmdByte4[6] = (byte)(CRCVal % 256); //CRC校验码需交换高低字节
                    Send_ReadCmdByte4[7] = (byte)(CRCVal / 256);
                    break;
                }
                case 5:
                {
                    Send_ReadCmdByte5[0] = 0x01; //(byte)RD_ADDR.Value;
                    Send_ReadCmdByte5[1] = 0x03;
                    Send_ReadCmdByte5[2] = 0x00;
                    Send_ReadCmdByte5[3] = 0x01;

                    Send_ReadCmdByte5[4] = 0x00;
                    Send_ReadCmdByte5[5] = 0x01;

                    CRCVal = CRC16(Send_ReadCmdByte5, 6); //CRC校验
                    Send_ReadCmdByte5[6] = (byte)(CRCVal % 256); //CRC校验码需交换高低字节
                    Send_ReadCmdByte5[7] = (byte)(CRCVal / 256);
                    break;
                }
                default:
                    break;
            }
        }

        /// <summary>
        /// 电源开关函数
        /// </summary>
        /// <param name="OFForON">0表示关电源，1表示开电源</param>
        /// <param name="PoNo">通过电源地址码设置</param>
        void NewPower_OF_or_ON(int OFForON, int PoNo)
        {
            UInt16 CRCVal = 0;

            switch (PoNo)
            {
                case 1:
                {
                    Send_OFForONByte1[0] = 0x01; //(byte)RD_ADDR.Value;
                    Send_OFForONByte1[1] = 0x06;
                    Send_OFForONByte1[2] = 0x00;
                    Send_OFForONByte1[3] = 0x1B;

                    Send_OFForONByte1[4] = 0x00;
                    Send_OFForONByte1[5] = (byte)OFForON;

                    CRCVal = CRC16(Send_OFForONByte1, 6); //CRC校验
                    Send_OFForONByte1[6] = (byte)(CRCVal % 256); //CRC校验码需交换高低字节
                    Send_OFForONByte1[7] = (byte)(CRCVal / 256);
                    break;
                }
                case 2:
                {
                    Send_OFForONByte2[0] = 0x01; //(byte)RD_ADDR.Value;
                    Send_OFForONByte2[1] = 0x06;
                    Send_OFForONByte2[2] = 0x00;
                    Send_OFForONByte2[3] = 0x1B;

                    Send_OFForONByte2[4] = 0x00;
                    Send_OFForONByte2[5] = (byte)OFForON;

                    CRCVal = CRC16(Send_OFForONByte2, 6); //CRC校验
                    Send_OFForONByte2[6] = (byte)(CRCVal % 256); //CRC校验码需交换高低字节
                    Send_OFForONByte2[7] = (byte)(CRCVal / 256);
                    break;
                }
                case 3:
                {
                    Send_OFForONByte3[0] = 0x01; //(byte)RD_ADDR.Value;
                    Send_OFForONByte3[1] = 0x06;
                    Send_OFForONByte3[2] = 0x00;
                    Send_OFForONByte3[3] = 0x1B;

                    Send_OFForONByte3[4] = 0x00;
                    Send_OFForONByte3[5] = (byte)OFForON;

                    CRCVal = CRC16(Send_OFForONByte3, 6); //CRC校验
                    Send_OFForONByte3[6] = (byte)(CRCVal % 256); //CRC校验码需交换高低字节
                    Send_OFForONByte3[7] = (byte)(CRCVal / 256);
                    break;
                }
                case 4:
                {
                    Send_OFForONByte4[0] = 0x01; //(byte)RD_ADDR.Value;
                    Send_OFForONByte4[1] = 0x06;
                    Send_OFForONByte4[2] = 0x00;
                    Send_OFForONByte4[3] = 0x1B;

                    Send_OFForONByte4[4] = 0x00;
                    Send_OFForONByte4[5] = (byte)OFForON;

                    CRCVal = CRC16(Send_OFForONByte4, 6); //CRC校验
                    Send_OFForONByte4[6] = (byte)(CRCVal % 256); //CRC校验码需交换高低字节
                    Send_OFForONByte4[7] = (byte)(CRCVal / 256);
                    break;
                }
                case 5:
                {
                    Send_OFForONByte5[0] = 0x01; //(byte)RD_ADDR.Value;
                    Send_OFForONByte5[1] = 0x06;
                    Send_OFForONByte5[2] = 0x00;
                    Send_OFForONByte5[3] = 0x05;

                    Send_OFForONByte5[4] = 0x00;
                    Send_OFForONByte5[5] = (byte)OFForON;

                    CRCVal = CRC16(Send_OFForONByte5, 6); //CRC校验
                    Send_OFForONByte5[6] = (byte)(CRCVal % 256); //CRC校验码需交换高低字节
                    Send_OFForONByte5[7] = (byte)(CRCVal / 256);
                    break;
                }
                default:
                    break;
            }
        }

        /// <summary>
        /// 电流设置函数
        /// </summary>
        /// <param name="CurVal">电源电流设定值</param>
        /// <param name="PoNo">通过电源地址码进行设定</param>
        void NewPower_Set_Cur(double CurVal, int PoNo)
        {
            UInt16 CRCVal = 0;
            UInt16 Idata = 0,
                Idata1 = 0;
            byte[] ISetValue = new byte[4];
            if (CurVal > Imax_IV + 1000) //加1000余量，防止Send_ICmdByte清空，导致程序错误。
            {
                switch (PoNo)
                {
                    case 1:
                        Send_ICmdByte1 = null;
                        break;
                    case 2:
                        Send_ICmdByte2 = null;
                        break;
                    case 3:
                        Send_ICmdByte3 = null;
                        break;
                    case 4:
                        Send_ICmdByte4 = null;
                        break;
                    case 5:
                        Send_ICmdByte5 = null;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                Idata = (UInt16)((65535 * CurVal) / Imax_HSP);
                Idata1 = (UInt16)((65535 * CurVal) / Imax_PLD);
                ISetValue = BitConverter.GetBytes((float)(CurVal / 1000)).Reverse().ToArray(); //将浮点数转换为字节数
            }
            switch (PoNo)
            {
                case 1:
                {
                    //设置电源的字节参数
                    Send_ICmdByte1[0] = 0x01; //功能码10表示预设寄存器
                    Send_ICmdByte1[1] = 0x10;
                    Send_ICmdByte1[2] = 0x00;
                    Send_ICmdByte1[3] = 0x03; //电流地址

                    Send_ICmdByte1[4] = 0x00; //
                    Send_ICmdByte1[5] = 0x02; //2个寄存器
                    Send_ICmdByte1[6] = 0x04; //4个字节数据
                    Send_ICmdByte1[7] = ISetValue[0]; //4个数据
                    Send_ICmdByte1[8] = ISetValue[1];
                    Send_ICmdByte1[9] = ISetValue[2];
                    Send_ICmdByte1[10] = ISetValue[3];
                    CRCVal = CRC16(Send_ICmdByte1, 11); //CRC校验
                    Send_ICmdByte1[11] = (byte)(CRCVal % 256); //CRC校验码需交换高低字节
                    Send_ICmdByte1[12] = (byte)(CRCVal / 256);
                    break;
                }
                case 2:
                {
                    //设置电源的字节参数
                    Send_ICmdByte2[0] = 0x01; //功能码10表示预设寄存器
                    Send_ICmdByte2[1] = 0x10;
                    Send_ICmdByte2[2] = 0x00;
                    Send_ICmdByte2[3] = 0x03; //电流地址

                    Send_ICmdByte2[4] = 0x00; //
                    Send_ICmdByte2[5] = 0x02; //2个寄存器
                    Send_ICmdByte2[6] = 0x04; //4个字节数据
                    Send_ICmdByte2[7] = ISetValue[0]; //4个数据
                    Send_ICmdByte2[8] = ISetValue[1];
                    Send_ICmdByte2[9] = ISetValue[2];
                    Send_ICmdByte2[10] = ISetValue[3];
                    CRCVal = CRC16(Send_ICmdByte2, 11); //CRC校验
                    Send_ICmdByte2[11] = (byte)(CRCVal % 256); //CRC校验码需交换高低字节
                    Send_ICmdByte2[12] = (byte)(CRCVal / 256);
                    break;
                }
                case 3:
                {
                    //设置电源的字节参数
                    Send_ICmdByte3[0] = 0x01; //功能码10表示预设寄存器
                    Send_ICmdByte3[1] = 0x10;
                    Send_ICmdByte3[2] = 0x00;
                    Send_ICmdByte3[3] = 0x03; //电流地址

                    Send_ICmdByte3[4] = 0x00; //
                    Send_ICmdByte3[5] = 0x02; //2个寄存器
                    Send_ICmdByte3[6] = 0x04; //4个字节数据
                    Send_ICmdByte3[7] = ISetValue[0]; //4个数据
                    Send_ICmdByte3[8] = ISetValue[1];
                    Send_ICmdByte3[9] = ISetValue[2];
                    Send_ICmdByte3[10] = ISetValue[3];
                    CRCVal = CRC16(Send_ICmdByte3, 11); //CRC校验
                    Send_ICmdByte3[11] = (byte)(CRCVal % 256); //CRC校验码需交换高低字节
                    Send_ICmdByte3[12] = (byte)(CRCVal / 256);
                    break;
                }
                case 4:
                {
                    //设置电源的字节参数
                    Send_ICmdByte4[0] = 0x01; //功能码10表示预设寄存器
                    Send_ICmdByte4[1] = 0x10;
                    Send_ICmdByte4[2] = 0x00;
                    Send_ICmdByte4[3] = 0x03; //电流地址

                    Send_ICmdByte4[4] = 0x00; //
                    Send_ICmdByte4[5] = 0x02; //2个寄存器
                    Send_ICmdByte4[6] = 0x04; //4个字节数据
                    Send_ICmdByte4[7] = ISetValue[0]; //4个数据
                    Send_ICmdByte4[8] = ISetValue[1];
                    Send_ICmdByte4[9] = ISetValue[2];
                    Send_ICmdByte4[10] = ISetValue[3];
                    CRCVal = CRC16(Send_ICmdByte4, 11); //CRC校验
                    Send_ICmdByte4[11] = (byte)(CRCVal % 256); //CRC校验码需交换高低字节
                    Send_ICmdByte4[12] = (byte)(CRCVal / 256);
                    break;
                }
                case 5:
                {
                    //设置电源的字节参数
                    Send_ICmdByte5[0] = 0x01; //(byte)RD_ADDR.Value;
                    Send_ICmdByte5[1] = 0x06; //功能码06表示预设单个寄存器
                    Send_ICmdByte5[2] = 0x00; //
                    Send_ICmdByte5[3] = 0x04; //00 04表示设定电流

                    Send_ICmdByte5[4] = (byte)(Idata / 256); //寄存器数值的高8位，取设定电压值X100后的高8位
                    Send_ICmdByte5[5] = (byte)(Idata % 256); //寄存器数值的低8位，取设定电压值X100后的低8位

                    CRCVal = CRC16(Send_ICmdByte5, 6); //CRC校验
                    Send_ICmdByte5[6] = (byte)(CRCVal % 256); //CRC校验码需交换高低字节
                    Send_ICmdByte5[7] = (byte)(CRCVal / 256);
                    break;
                }
                default:
                    break;
            }
        }

        /// <summary>
        /// CRC16校验函数
        /// </summary>
        /// <param name="InByte"></param>
        /// <param name="ByteLen"></param>
        /// <returns></returns>
        UInt16 CRC16(byte[] InByte, int ByteLen)
        {
            UInt16 CRC_Reg = 0xFFFF;
            int a,
                b;
            a = CRC_Reg;
            b = 0;
            for (int i = 0; i < ByteLen; i++)
            {
                b = InByte[i];
                a = a ^ b; //做异或运算
                //右移一位，并检查最低位
                for (int j = 0; j < 8; j++)
                {
                    if (a % 2 == 1) //如果最低位为1，则用右移（除以2）后的数值与0xA001进行异或运算
                    {
                        a /= 2;
                        a = a ^ 0xA001;
                    }
                    else
                        a /= 2; //如果最低位为0则右移1位（除以2）后结束本次移位，跳入下次移位
                }
            }
            //交换高低字节运算放在使用时进行
            CRC_Reg = (UInt16)a; //(a/256+(a%256)*256);
            return CRC_Reg;
        }

        /// <summary>
        /// 延时函数
        /// </summary>
        /// <param name="milliSecond"></param>
        public static void Delay(int milliSecond)
        {
            int start = Environment.TickCount;
            while (Math.Abs(Environment.TickCount - start) < milliSecond) //毫秒
            {
                System.Windows.Forms.Application.DoEvents(); //可执行某无聊的操作
            }
        }

        ~PowerController()
        {
            if (Thread1 != null && Thread1.IsAlive)
                Thread1.Abort();
            if (Thread2 != null && Thread2.IsAlive)
                Thread2.Abort();
            if (Thread3 != null && Thread3.IsAlive)
                Thread3.Abort();
            if (Thread4 != null && Thread4.IsAlive)
                Thread4.Abort();
        }

        /// <summary>
        /// 线程关闭函数
        /// </summary>
        public void CloseThread()
        {
            if (Thread1 != null && Thread1.IsAlive)
                Thread1.Abort();
            if (Thread2 != null && Thread2.IsAlive)
                Thread2.Abort();
            if (Thread3 != null && Thread3.IsAlive)
                Thread3.Abort();
            if (Thread4 != null && Thread4.IsAlive)
                Thread4.Abort();
        }

        /// <summary>
        /// 读电流
        /// </summary>
        /// <param name="prefix">传送K、N、C、S</param>
        /// <returns>返回K，N，C，S，实际读取值</returns>
        public double GetI(string prefix)
        {
            switch (prefix)
            {
                case "K":
                    return CurReadVal_K;
                case "N":
                    return CurReadVal_Na;
                case "C":
                    return CurReadVal_Cs;
                case "S":
                    return CurReadVal_Sb;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// 设置电流
        /// </summary>
        /// <param name="prefix">传送K、N、C、S</param>
        /// <param name="value">实际设定值</param>
        /// <returns>返回设定值</returns>
        public double SetI(string prefix, double value)
        {
            switch (prefix)
            {
                case "K":
                    CurSetVal_K = (int)value;
                    return CurSetVal_K;
                case "N":
                    CurSetVal_Na = (int)value;
                    return CurSetVal_Na;
                case "C":
                    CurSetVal_Cs = (int)value;
                    return CurSetVal_Cs;
                case "S":
                    CurSetVal_Sb = (int)value;
                    return CurSetVal_Sb;
                default:
                    return 1;
            }
        }

        /// <summary>
        /// 打开电源函数
        /// </summary>
        /// <param name="prefix">传送K、N、C、S</param>
        /// <returns>K、N、C、S返回值为1 default返回值为2</returns>
        public double OpenI(string prefix)
        {
            switch (prefix)
            {
                case "K":
                    NewOnFlag_K = true;
                    return 1;
                case "N":
                    NewOnFlag_Na = true;
                    return 1;
                case "C":
                    NewOnFlag_Cs = true;
                    return 1;
                case "S":
                    NewOnFlag_Sb = true;
                    return 1;
                default:
                    return 2;
            }
        }

        /// <summary>
        /// 关闭电源函数
        /// </summary>
        /// <param name="prefix">传送K、N、C、S</param>
        /// <returns>K、N、C、S返回值为0 default返回值为3</returns>
        public double CloseI(string prefix)
        {
            switch (prefix)
            {
                case "K":
                    KoffFlag = true;
                    return 1;
                case "N":
                    NaoffFlag = true;
                    return 1;
                case "C":
                    CsoffFlag = true;
                    return 1;
                case "S":
                    SboffFlag = true;
                    return 1;
                default:
                    return 3;
            }
        }
    }
}
