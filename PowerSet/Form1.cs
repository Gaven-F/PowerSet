using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;

namespace PowerSet
{
    public partial class Form1 : Form
    {

        // 电源相关参数
        //定义电源电压最大值、电流最大值(在IV电源的最大电压和最大电流分别为20V和10A)
        public static double Vmax_IV = 20.0;
        public static double Imax_IV = 10000;//10.5;
        //定义电源电压最大值、电流最大值(在HSP-3010电源的最大电压和最大电流分别为31V和10.5A)
        public static double Vmax_HSP = 31.0;
        public static double Imax_HSP = 10500;//10.5;
        //定义电源输出控制参数，OVPmax和VCPmax(在HSP-3010型O.V.P 34.0V，O.C.P 12.0A）
        public static double OVPmax_HSP = 34.0;
        public static double OCPmax_HSP = 12000;//12A
        //定义电源电压最大值、电流最大值(在PLD-3010型电源的最大电压和最大电流分别为32V和11A)
        public static double Vmax_PLD = 32.0;
        public static double Imax_PLD = 11000;//11
        //定义电源输出控制参数，OVPmax和VCPmax(在PLD-3010型电的O.V.P 36.0V，O.C.P 12.0A）
        public static double OVPmax_PLD = 36.0;
        public static double OCPmax_PLD = 12000;//12A

        Byte[] Send_OFForONByte1 = new byte[8];//发送电源开关命令字节缓冲区
        Byte[] Send_OFForONByte2 = new byte[8];//发送电源开关命令字节缓冲区
        Byte[] Send_OFForONByte3 = new byte[8];//发送电源开关命令字节缓冲区
        Byte[] Send_OFForONByte4 = new byte[8];//发送电源开关命令字节缓冲区
        Byte[] Send_OFForONByte5 = new byte[8];//发送电源开关命令字节缓冲区

        Byte[] Send_VCmdByte1 = new byte[13];   //发送电源电压设定命令字节缓冲区
        Byte[] Send_VCmdByte2 = new byte[13];   //发送电源电压设定命令字节缓冲区
        Byte[] Send_VCmdByte3 = new byte[13];   //发送电源电压设定命令字节缓冲区
        Byte[] Send_VCmdByte4 = new byte[13];   //发送电源电压设定命令字节缓冲区
        Byte[] Send_VCmdByte5 = new byte[8];   //发送电源电压设定命令字节缓冲区

        Byte[] Send_ICmdByte1 = new byte[13];   //发送电源电流命令字节缓冲区
        Byte[] Send_ICmdByte2 = new byte[13];   //发送电源电流命令字节缓冲区
        Byte[] Send_ICmdByte3 = new byte[13];   //发送电源电流命令字节缓冲区
        Byte[] Send_ICmdByte4 = new byte[13];   //发送电源电流命令字节缓冲区
        Byte[] Send_ICmdByte5 = new byte[8];   //发送电源电流命令字节缓冲区

        Byte[] Send_ReadCmdByte1 = new byte[13];//发送电源电流读取命令字节缓冲区
        Byte[] Send_ReadCmdByte2 = new byte[13];//发送电源电流读取命令字节缓冲区
        Byte[] Send_ReadCmdByte3 = new byte[13];//发送电源电流读取命令字节缓冲区
        Byte[] Send_ReadCmdByte4 = new byte[13];//发送电源电流读取命令字节缓冲区
        Byte[] Send_ReadCmdByte5 = new byte[8];//发送电源电流读取命令字节缓冲区

        //电源电流上次设定值
        public static int CurOldVal_K;//K电源
        public static int CurOldVal_Na;//Na电源
        public static int CurOldVal_Cs;//Cs电源
        public static int CurOldVal_Sb;//Sb电源
        public static int CurOldVal_DY;//灯源电源

        //电源电流当前设定值
        public static int CurSetVal_K;//K电源
        public static int CurSetVal_Na;//Na电源
        public static int CurSetVal_Cs;//Cs电源
        public static int CurSetVal_Sb;//Sb电源
        public static int CurSetVal_DY;//灯源电源

        //电源电流返回值（读取值）
        public static int CurReadVal_K;//K电源
        public static int CurReadVal_Na;//Na电源
        public static int CurReadVal_Cs;//Cs电源
        public static int CurReadVal_Sb;//Sb电源
        public static int CurReadVal_DY;//灯源电源

        //电源开关标志，old表示上次的状态，new表示当前状态,false表示关，true表示开
        public static bool NewOnFlag_K;//K电源
        public static bool NewOnFlag_Na;//Na电源
        public static bool NewOnFlag_Cs;//Cs电源
        public static bool NewOnFlag_Sb;//Sb电源
        public static bool NewOnFlag_DY;//灯源电源

        public static bool OldOnFalg_K;//K电源
        public static bool OldOnFalg_Na;//Na电源
        public static bool OldOnFalg_Cs;//Cs电源
        public static bool OldOnFalg_Sb;//Sb电源
        public static bool OldOnFalg_DY;//灯源电源

        public static bool K_Vlot_Flag;//K电源电压设定标志
        public static bool KoffFlag;//叉掉弹窗时，设置电流值为零的标志
        public static bool Na_Vlot_Flag;//Na电源电压设定标志
        public static bool NaoffFlag;//叉掉弹窗时，设置电流值为零的标志
        public static bool Cs_Vlot_Flag;//Cs电源电压设定标志
        public static bool CsoffFlag;//叉掉弹窗时，设置电流值为零的标志
        public static bool Sb_Vlot_Flag;//Sb电源电压设定标志
        public static bool SboffFlag;//叉掉弹窗时，设置电流值为零的标志
        public static bool DY_Vlot_Flag;//灯源电源电压设定标志
        public static bool DYoffFlag;//叉掉弹窗时，设置电流值为零的标志

        public static bool KOnFlag;//K电源开标志。
        public static bool NaOnFlag;//K电源开标志。
        public static bool CsOnFlag;//K电源开标志。
        public static bool SbOnFlag;//K电源开标志。
        public static bool DYOnFlag;//K电源开标志。 

        public static int KPauseFlag; //暂停标志位，点击暂停按钮时，关闭所有电源操作。
        public static int KContinueFlag;//继续标志位，点击继续按钮时，恢复电源原有操作。
        public static int NaPauseFlag; //暂停标志位，点击暂停按钮时，关闭所有电源操作。
        public static int NaContinueFlag;//继续标志位，点击继续按钮时，恢复电源原有操作。
        public static int CsPauseFlag; //暂停标志位，点击暂停按钮时，关闭所有电源操作。
        public static int CsContinueFlag;//继续标志位，点击继续按钮时，恢复电源原有操作。
        public static int SbPauseFlag; //暂停标志位，点击暂停按钮时，关闭所有电源操作。
        public static int SbContinueFlag;//继续标志位，点击继续按钮时，恢复电源原有操作。
        public static int DYPauseFlag; //暂停标志位，点击暂停按钮时，关闭所有电源操作。
        public static int DYContinueFlag;//继续标志位，点击继续按钮时，恢复电源原有操作。
        public static long GYTime;//总工艺时间
        public static long TimeCount;
        public static int Sb_Num = 500;//Sb底数，关闭Sb时，Sb的底数
                                       //读数异常标志
        public static bool[] ReadErrorFlag = new bool[5];
        public static int DelayTimeNet = 100;//网络端口收发信息延时
        public static int DelayTimeNet280 = 280;//2022-9-14 由于电源开关失控，故将延时提升到110ms,110ms不够，于是将延时改为280
        public static int DelayTimeSerial = 100;//串口收发信息延时
        public static int DelayReconnectiuion = 1500;//断线重连延时
        //网络定义
        Socket SocketWatch1 = null; //负责监听客户端的套接字
        Socket SocketWatch2 = null;
        Socket SocketWatch3 = null;
        Socket SocketWatch4 = null;
        //创建一个负责和客户端通信的套接字 
        Socket SocConnection1, SocConnection2, SocConnection3, SocConnection4;
        Thread Thread1, Thread2, Thread3, Thread4;
        public static int KCount, NaCount, CsCount, SbCount;
        public static int Xcount;
        public static int Ycount;

        public FileInfo ParameterSave = new FileInfo("./Parameter.dat");   //参数保存文件

        //参数界面参数定义
        public static int KInitCurrent, NaInitCurrent, CsInitCurrent, SbInitCurrent;//电源启动电流
        public static int KAddCurrent, NaAddCurrent, CsAddCurrent, SbAddCurrent;//电源增加电流
        public static int KIntervalTime=10, NaIntervalTime = 10, CsIntervalTime = 10, SbIntervalTime = 10;//电源间隔时间
        public static int KPeriod = 10, NaPeriod = 10, CsPeriod = 10, SbPeriod = 10;//电源周期
        public static int KVoltage=25, NaVoltage = 25, CsVoltage = 25, SbVoltage = 25;//电源电压
        public static int KMaxTime, NaMaxTime, CsMaxTime, SbMaxTime;//电源关闭时间
        public static long KTimePeriodCount, NaTimePeriodCount, CsTimePeriodCount, SbTimePeriodCount;//周期计时
        public static int KPeriodCount, NaPeriodCount, CsPeriodCount, SbPeriodCount;//周期执行数
        public static int XIntervalTime = 10, XMaxTime = 40;
        public static double YIntervalAmpere = 0.5, YMaxAmpere = 4;






        public static long TimeCnt = 0;

        public static bool TotalPauseFlag;//暂停标志
        public const int KIndex = 12, NaIndex = 12, CsIndex = 12, SbIndex = 12;
        public static int[] KT1Cur = new int[KIndex] { 1000, 1500, 2000, 2500, 3000, 3500, 4000, 4500, 5000, 5500, 6000, 6500};
        public static int[] KT1Execution = new int[KIndex] { 10, 10, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30 };
        public static int[] KT1TurnOff = new int[KIndex] { 0, 0, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30};
        public static int[] KT1Nsb = new int[KIndex] { 1, 1, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3};

        public static int[] NaT1Cur = new int[NaIndex] { 1000, 1500, 2000, 2500, 3000, 3500, 4000, 4500, 5000, 5500, 6000, 6500 };
        public static int[] NaT1Execution = new int[NaIndex] { 10, 10, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30 };
        public static int[] NaT1TurnOff = new int[NaIndex] { 0, 0, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30 };
        public static int[] NaT1Nsb = new int[NaIndex] { 1, 1, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 };

        public static int[] CsT1Cur = new int[CsIndex] { 1000, 1500, 2000, 2500, 3000, 3500, 4000, 4500, 5000, 5500, 6000, 6500 };
        public static int[] CsT1Execution = new int[CsIndex] { 10, 10, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30 };
        public static int[] CsT1TurnOff = new int[CsIndex] { 0, 0, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30 };
        public static int[] CsT1Nsb = new int[CsIndex] { 1, 1, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 };
        public static int[] SbT1Cur = new int[SbIndex] { 1000, 1500, 2000, 2500, 3000, 3500, 4000, 4500, 5000, 5500, 6000, 6500 };
        public static int[] SbT1Execution = new int[SbIndex] { 10, 10, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30 };
        public static int[] SbT1TurnOff = new int[SbIndex] { 0, 0, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30 };
        public static int[] SbT1Nsb = new int[SbIndex] { 1, 1, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 };
        public static int KCurIndex = 0, NaCurIndex = 0, CsCurIndex = 0, SbCurIndex = 0;
        public static long KOpenTimeCnt = 0, NaOpenTimeCnt = 0, CsOpenTimeCnt = 0, SbOpenTimeCnt = 0,
                           KCloseTimeCnt = 0, NaCloseTimeCnt = 0, CsCloseTimeCnt = 0, SbCloseTimeCnt = 0,
                           KNsb = 0, NaNsb = 0, CsNsb = 0, SbNsb = 0;

        public static bool KStartFlag, NaStartFlag, CsStartFlag, SbStartFlag;

        public static bool InitDraw;








        /// <summary>
        /// Form初始化函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {

            DrawCoord();
            InitPower();
            InitParameter();
            NetConnections();
            
            //chart1.Series["K电源"].Points.AddXY(0, 1);
            //chart1.Series["K电源"].Points.AddXY(10, 1);
            //chart1.Series["K电源"].Points.AddXY(10, 2);
            //chart1.Series["K电源"].Points.AddXY(20, 2);
            //chart1.Series["K电源"].Points.AddXY(20, 3);
            //chart1.Series["K电源"].Points.AddXY(30, 3);
            //chart1.Series["K电源"].Points.AddXY(30, 4);
            //chart1.Series["K电源"].Points.AddXY(40, 4);
            //chart1.Invalidate();

            timer1.Start();// 启动定时器
                           //WindowState = FormWindowState.Maximized;    //最大化窗体

            //解决tableLayoutPannel闪烁问题
            tableLayoutPanel1.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(tableLayoutPanel1, true, null);
            tableLayoutPanel2.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(tableLayoutPanel2, true, null);
            tableLayoutPanel3.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(tableLayoutPanel3, true, null);
            tableLayoutPanel4.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(tableLayoutPanel4, true, null);
            tableLayoutPanel5.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(tableLayoutPanel5, true, null);

            // 刷新图表显示  
            //chart1.Invalidate(); // 或者使用 chart1.Refresh();
        }


        private void T1_Cur_ValueChanged(object sender, EventArgs e)
        {
            KT1Cur[0] = Int32.Parse(T1_Cur.Value.ToString());
        }
        private void T2_Cur_ValueChanged(object sender, EventArgs e)
        {
            KT1Cur[1] = Int32.Parse(T2_Cur.Value.ToString());
        }

        private void T3_Cur_ValueChanged(object sender, EventArgs e)
        {
            KT1Cur[2] = Int32.Parse(T3_Cur.Value.ToString());
        }
        private void T4_Cur_ValueChanged(object sender, EventArgs e)
        {
            KT1Cur[3] = Int32.Parse(T4_Cur.Value.ToString());
        }

        private void T5_Cur_ValueChanged(object sender, EventArgs e)
        {
            KT1Cur[4] = Int32.Parse(T5_Cur.Value.ToString());
        }

        private void T6_Cur_ValueChanged(object sender, EventArgs e)
        {
            KT1Cur[5] = Int32.Parse(T6_Cur.Value.ToString());
        }
        private void T7_Cur_ValueChanged(object sender, EventArgs e)
        {
            KT1Cur[6] = Int32.Parse(T7_Cur.Value.ToString());
        }
        private void T8_Cur_ValueChanged(object sender, EventArgs e)
        {
            KT1Cur[7] = Int32.Parse(T8_Cur.Value.ToString());
        }

        private void T9_Cur_ValueChanged(object sender, EventArgs e)
        {
            KT1Cur[8] = Int32.Parse(T9_Cur.Value.ToString());
        }

        private void T10_Cur_ValueChanged(object sender, EventArgs e)
        {
            KT1Cur[9] = Int32.Parse(T10_Cur.Value.ToString());
        }

        private void T11_Cur_ValueChanged(object sender, EventArgs e)
        {
            KT1Cur[10] = Int32.Parse(T11_Cur.Value.ToString());
        }

        private void T12_Cur_ValueChanged(object sender, EventArgs e)
        {
            KT1Cur[11] = Int32.Parse(T12_Cur.Value.ToString());
        }



       
        private void T1_Execution_ValueChanged(object sender, EventArgs e)
        {
            KT1Execution[0] = Int32.Parse(T1_Execution.Value.ToString());
        }
        private void T2_Execution_ValueChanged(object sender, EventArgs e)
        {
            KT1Execution[1] = Int32.Parse(T2_Execution.Value.ToString());
        }

        private void T3_Execution_ValueChanged(object sender, EventArgs e)
        {
            KT1Execution[2] = Int32.Parse(T3_Execution.Value.ToString());
        }

        private void T4_Execution_ValueChanged(object sender, EventArgs e)
        {
            KT1Execution[3] = Int32.Parse(T4_Execution.Value.ToString());
        }

        private void T5_Execution_ValueChanged(object sender, EventArgs e)
        {
            KT1Execution[4] = Int32.Parse(T5_Execution.Value.ToString());
        }

        private void T6_Execution_ValueChanged(object sender, EventArgs e)
        {
            KT1Execution[5] = Int32.Parse(T6_Execution.Value.ToString());
        }

        private void T7_Execution_ValueChanged(object sender, EventArgs e)
        {
            KT1Execution[6] = Int32.Parse(T7_Execution.Value.ToString());
        }

        private void T8_Execution_ValueChanged(object sender, EventArgs e)
        {
            KT1Execution[7] = Int32.Parse(T8_Execution.Value.ToString());
        }

        private void T9_Execution_ValueChanged(object sender, EventArgs e)
        {
            KT1Execution[8] = Int32.Parse(T9_Execution.Value.ToString());
        }

        private void T10_Execution_ValueChanged(object sender, EventArgs e)
        {
            KT1Execution[9] = Int32.Parse(T10_Execution.Value.ToString());
        }

        private void T11_Execution_ValueChanged(object sender, EventArgs e)
        {
            KT1Execution[10] = Int32.Parse(T11_Execution.Value.ToString());
        }

        private void T12_Execution_ValueChanged(object sender, EventArgs e)
        {
            KT1Execution[11] = Int32.Parse(T12_Execution.Value.ToString());
        }

       
        private void T1_TurnOff_ValueChanged(object sender, EventArgs e)
        {
            KT1TurnOff[0] = Int32.Parse(T1_TurnOff.Value.ToString());
        }

        private void T2_TurnOff_ValueChanged(object sender, EventArgs e)
        {
            KT1TurnOff[1] = Int32.Parse(T2_TurnOff.Value.ToString());
        }

        private void T3_TurnOff_ValueChanged(object sender, EventArgs e)
        {
            KT1TurnOff[2] = Int32.Parse(T3_TurnOff.Value.ToString());
        }

        private void T4_TurnOff_ValueChanged(object sender, EventArgs e)
        {
            KT1TurnOff[3] = Int32.Parse(T4_TurnOff.Value.ToString());
        }

        private void T5_TurnOff_ValueChanged(object sender, EventArgs e)
        {
            KT1TurnOff[4] = Int32.Parse(T5_TurnOff.Value.ToString());
        }

        private void T6_TurnOff_ValueChanged(object sender, EventArgs e)
        {
            KT1TurnOff[5] = Int32.Parse(T6_TurnOff.Value.ToString());
        }

        private void T7_TurnOff_ValueChanged(object sender, EventArgs e)
        {
            KT1TurnOff[6] = Int32.Parse(T7_TurnOff.Value.ToString());
        }

        private void T8_TurnOff_ValueChanged(object sender, EventArgs e)
        {
            KT1TurnOff[7] = Int32.Parse(T8_TurnOff.Value.ToString());
        }

        private void T9_TurnOff_ValueChanged(object sender, EventArgs e)
        {
            KT1TurnOff[8] = Int32.Parse(T9_TurnOff.Value.ToString());
        }

        private void T10_TurnOff_ValueChanged(object sender, EventArgs e)
        {
            KT1TurnOff[9] = Int32.Parse(T10_TurnOff.Value.ToString());
        }

        private void T11_TurnOff_ValueChanged(object sender, EventArgs e)
        {
            KT1TurnOff[10] = Int32.Parse(T11_TurnOff.Value.ToString());
        }

        private void T12_TurnOff_ValueChanged(object sender, EventArgs e)
        {
            KT1TurnOff[11] = Int32.Parse(T12_TurnOff.Value.ToString());
        }

        private void Na_T1_Cur_ValueChanged(object sender, EventArgs e)
        {
            NaT1Cur[0] = Int32.Parse(Na_T1_Cur.Value.ToString());
        }

        private void Na_T2_Cur_ValueChanged(object sender, EventArgs e)
        {
            NaT1Cur[1] = Int32.Parse(Na_T2_Cur.Value.ToString());
        }

        private void Na_T3_Cur_ValueChanged(object sender, EventArgs e)
        {
            NaT1Cur[2] = Int32.Parse(Na_T3_Cur.Value.ToString());
        }

        private void Na_T4_Cur_ValueChanged(object sender, EventArgs e)
        {
            NaT1Cur[3] = Int32.Parse(Na_T4_Cur.Value.ToString());
        }

        private void Na_T5_Cur_ValueChanged(object sender, EventArgs e)
        {
            NaT1Cur[4] = Int32.Parse(Na_T5_Cur.Value.ToString());
        }

        private void Na_T6_Cur_ValueChanged(object sender, EventArgs e)
        {
            NaT1Cur[5] = Int32.Parse(Na_T6_Cur.Value.ToString());
        }

        private void Na_T7_Cur_ValueChanged(object sender, EventArgs e)
        {
            NaT1Cur[6] = Int32.Parse(Na_T7_Cur.Value.ToString());
        }

        private void Na_T8_Cur_ValueChanged(object sender, EventArgs e)
        {
            NaT1Cur[7] = Int32.Parse(Na_T8_Cur.Value.ToString());
        }

        private void Na_T9_Cur_ValueChanged(object sender, EventArgs e)
        {
            NaT1Cur[8] = Int32.Parse(Na_T9_Cur.Value.ToString());
        }

        private void Na_T10_Cur_ValueChanged(object sender, EventArgs e)
        {
            NaT1Cur[9] = Int32.Parse(Na_T10_Cur.Value.ToString());
        }

        private void Na_T11_Cur_ValueChanged(object sender, EventArgs e)
        {
            NaT1Cur[10] = Int32.Parse(Na_T11_Cur.Value.ToString());
        }

        private void Na_T12_Cur_ValueChanged(object sender, EventArgs e)
        {
            NaT1Cur[11] = Int32.Parse(Na_T12_Cur.Value.ToString());
        }

        private void Na_T1_Execution_ValueChanged(object sender, EventArgs e)
        {
            NaT1Execution[0] = Int32.Parse(Na_T1_Execution.Value.ToString());
        }

        private void Na_T2_Execution_ValueChanged(object sender, EventArgs e)
        {
            NaT1Execution[1] = Int32.Parse(Na_T2_Execution.Value.ToString());
        }

        private void Na_T3_Execution_ValueChanged(object sender, EventArgs e)
        {
            NaT1Execution[2] = Int32.Parse(Na_T3_Execution.Value.ToString());
        }

        private void Na_T4_Execution_ValueChanged(object sender, EventArgs e)
        {
            NaT1Execution[3] = Int32.Parse(Na_T4_Execution.Value.ToString());
        }

        private void Na_T5_Execution_ValueChanged(object sender, EventArgs e)
        {
            NaT1Execution[4] = Int32.Parse(Na_T5_Execution.Value.ToString());
        }

        private void Na_T6_Execution_ValueChanged(object sender, EventArgs e)
        {
            NaT1Execution[5] = Int32.Parse(Na_T6_Execution.Value.ToString());
        }

        private void Na_T7_Execution_ValueChanged(object sender, EventArgs e)
        {
            NaT1Execution[6] = Int32.Parse(Na_T7_Execution.Value.ToString());
        }

        private void Na_T8_Execution_ValueChanged(object sender, EventArgs e)
        {
            NaT1Execution[7] = Int32.Parse(Na_T8_Execution.Value.ToString());
        }

        private void Na_T9_Execution_ValueChanged(object sender, EventArgs e)
        {
            NaT1Execution[8] = Int32.Parse(Na_T9_Execution.Value.ToString());
        }

        private void Na_T10_Execution_ValueChanged(object sender, EventArgs e)
        {
            NaT1Execution[9] = Int32.Parse(Na_T10_Execution.Value.ToString());
        }

        private void Na_T11_Execution_ValueChanged(object sender, EventArgs e)
        {
            NaT1Execution[10] = Int32.Parse(Na_T11_Execution.Value.ToString());
        }

        private void Na_T12_Execution_ValueChanged(object sender, EventArgs e)
        {
            NaT1Execution[11] = Int32.Parse(Na_T12_Execution.Value.ToString());
        }

        private void Na_T1_TurnOff_ValueChanged(object sender, EventArgs e)
        {
            NaT1TurnOff[0] = Int32.Parse(Na_T1_TurnOff.Value.ToString());
        }

        private void Na_T2_TurnOff_ValueChanged(object sender, EventArgs e)
        {
            NaT1TurnOff[1] = Int32.Parse(Na_T2_TurnOff.Value.ToString());
        }

        private void Na_T3_TurnOff_ValueChanged(object sender, EventArgs e)
        {
            NaT1TurnOff[2] = Int32.Parse(Na_T3_TurnOff.Value.ToString());
        }

        private void Na_T4_TurnOff_ValueChanged(object sender, EventArgs e)
        {
            NaT1TurnOff[3] = Int32.Parse(Na_T4_TurnOff.Value.ToString());
        }

        private void Na_T5_TurnOff_ValueChanged(object sender, EventArgs e)
        {
            NaT1TurnOff[4] = Int32.Parse(Na_T5_TurnOff.Value.ToString());
        }

        private void Na_T6_TurnOff_ValueChanged(object sender, EventArgs e)
        {
            NaT1TurnOff[5] = Int32.Parse(Na_T6_TurnOff.Value.ToString());
        }

        private void Na_T7_TurnOff_ValueChanged(object sender, EventArgs e)
        {
            NaT1TurnOff[6] = Int32.Parse(Na_T7_TurnOff.Value.ToString());
        }

        private void Na_T8_TurnOff_ValueChanged(object sender, EventArgs e)
        {
            NaT1TurnOff[7] = Int32.Parse(Na_T8_TurnOff.Value.ToString());
        }

        private void Na_T9_TurnOff_ValueChanged(object sender, EventArgs e)
        {
            NaT1TurnOff[8] = Int32.Parse(Na_T9_TurnOff.Value.ToString());
        }

        private void Na_T10_TurnOff_ValueChanged(object sender, EventArgs e)
        {
            NaT1TurnOff[9] = Int32.Parse(Na_T10_TurnOff.Value.ToString());
        }

        private void Na_T11_TurnOff_ValueChanged(object sender, EventArgs e)
        {
            NaT1TurnOff[10] = Int32.Parse(Na_T11_TurnOff.Value.ToString());
        }

        private void Na_T12_TurnOff_ValueChanged(object sender, EventArgs e)
        {
            NaT1TurnOff[11] = Int32.Parse(Na_T12_TurnOff.Value.ToString());
        }

        private void Cs_T1_Cur_ValueChanged(object sender, EventArgs e)
        {
            CsT1Cur[0] = Int32.Parse(Cs_T1_Cur.Value.ToString());
        }

        private void Cs_T2_Cur_ValueChanged(object sender, EventArgs e)
        {
            CsT1Cur[1] = Int32.Parse(Cs_T2_Cur.Value.ToString());
        }

        private void Cs_T3_Cur_ValueChanged(object sender, EventArgs e)
        {
            CsT1Cur[2] = Int32.Parse(Cs_T3_Cur.Value.ToString());
        }

        private void Cs_T4_Cur_ValueChanged(object sender, EventArgs e)
        {
            CsT1Cur[3] = Int32.Parse(Cs_T4_Cur.Value.ToString());
        }

        private void Cs_T5_Cur_ValueChanged(object sender, EventArgs e)
        {
            CsT1Cur[4] = Int32.Parse(Cs_T5_Cur.Value.ToString());
        }

        private void Cs_T6_Cur_ValueChanged(object sender, EventArgs e)
        {
            CsT1Cur[5] = Int32.Parse(Cs_T6_Cur.Value.ToString());
        }

        private void Cs_T7_Cur_ValueChanged(object sender, EventArgs e)
        {
            CsT1Cur[6] = Int32.Parse(Cs_T7_Cur.Value.ToString());
        }

        private void Cs_T8_Cur_ValueChanged(object sender, EventArgs e)
        {
            CsT1Cur[7] = Int32.Parse(Cs_T8_Cur.Value.ToString());
        }

        private void Cs_T9_Cur_ValueChanged(object sender, EventArgs e)
        {
            CsT1Cur[8] = Int32.Parse(Cs_T9_Cur.Value.ToString());
        }

        private void Cs_T10_Cur_ValueChanged(object sender, EventArgs e)
        {
            CsT1Cur[9] = Int32.Parse(Cs_T10_Cur.Value.ToString());
        }

        private void Cs_T11_Cur_ValueChanged(object sender, EventArgs e)
        {
            CsT1Cur[10] = Int32.Parse(Cs_T11_Cur.Value.ToString());
        }

        private void Cs_T12_Cur_ValueChanged(object sender, EventArgs e)
        {
            CsT1Cur[11] = Int32.Parse(Cs_T12_Cur.Value.ToString());
        }

        private void Cs_T1_Execution_ValueChanged(object sender, EventArgs e)
        {
            CsT1Execution[0] = Int32.Parse(Cs_T1_Execution.Value.ToString());
        }

        private void Cs_T2_Execution_ValueChanged(object sender, EventArgs e)
        {
            CsT1Execution[0] = Int32.Parse(Cs_T1_Execution.Value.ToString());
        }

        private void Cs_T3_Execution_ValueChanged(object sender, EventArgs e)
        {
            CsT1Execution[2] = Int32.Parse(Cs_T3_Execution.Value.ToString());
        }

        private void Cs_T4_Execution_ValueChanged(object sender, EventArgs e)
        {
            CsT1Execution[3] = Int32.Parse(Cs_T4_Execution.Value.ToString());
        }

        private void Cs_T5_Execution_ValueChanged(object sender, EventArgs e)
        {
            CsT1Execution[4] = Int32.Parse(Cs_T5_Execution.Value.ToString());
        }

        private void Cs_T6_Execution_ValueChanged(object sender, EventArgs e)
        {
            CsT1Execution[5] = Int32.Parse(Cs_T6_Execution.Value.ToString());
        }

        private void Cs_T7_Execution_ValueChanged(object sender, EventArgs e)
        {
            CsT1Execution[6] = Int32.Parse(Cs_T7_Execution.Value.ToString());
        }

        private void Cs_T8_Execution_ValueChanged(object sender, EventArgs e)
        {
            CsT1Execution[7] = Int32.Parse(Cs_T8_Execution.Value.ToString());
        }

        private void Cs_T9_Execution_ValueChanged(object sender, EventArgs e)
        {
            CsT1Execution[8] = Int32.Parse(Cs_T9_Execution.Value.ToString());
        }

        private void Cs_T10_Execution_ValueChanged(object sender, EventArgs e)
        {
            CsT1Execution[9] = Int32.Parse(Cs_T10_Execution.Value.ToString());
        }

        private void Cs_T11_Execution_ValueChanged(object sender, EventArgs e)
        {
            CsT1Execution[10] = Int32.Parse(Cs_T11_Execution.Value.ToString());
        }

        private void Cs_T12_Execution_ValueChanged(object sender, EventArgs e)
        {
            CsT1Execution[11] = Int32.Parse(Cs_T12_Execution.Value.ToString());
        }

        private void Cs_T1_TurnOff_ValueChanged(object sender, EventArgs e)
        {
            CsT1TurnOff[0] = Int32.Parse(Cs_T1_TurnOff.Value.ToString());
        }

        private void Cs_T2_TurnOff_ValueChanged(object sender, EventArgs e)
        {
            CsT1TurnOff[1] = Int32.Parse(Cs_T2_TurnOff.Value.ToString());
        }

        private void Cs_T3_TurnOff_ValueChanged(object sender, EventArgs e)
        {
            CsT1TurnOff[2] = Int32.Parse(Cs_T3_TurnOff.Value.ToString());
        }

        private void Cs_T4_TurnOff_ValueChanged(object sender, EventArgs e)
        {
            CsT1TurnOff[3] = Int32.Parse(Cs_T4_TurnOff.Value.ToString());
        }

        private void Cs_T5_TurnOff_ValueChanged(object sender, EventArgs e)
        {
            CsT1TurnOff[4] = Int32.Parse(Cs_T5_TurnOff.Value.ToString());
        }

        private void Cs_T6_TurnOff_ValueChanged(object sender, EventArgs e)
        {
            CsT1TurnOff[5] = Int32.Parse(Cs_T6_TurnOff.Value.ToString());
        }

        private void Cs_T7_TurnOff_ValueChanged(object sender, EventArgs e)
        {
            CsT1TurnOff[6] = Int32.Parse(Cs_T7_TurnOff.Value.ToString());
        }

        private void Cs_T8_TurnOff_ValueChanged(object sender, EventArgs e)
        {
            CsT1TurnOff[7] = Int32.Parse(Cs_T8_TurnOff.Value.ToString());
        }

        private void Cs_T9_TurnOff_ValueChanged(object sender, EventArgs e)
        {
            CsT1TurnOff[8] = Int32.Parse(Cs_T9_TurnOff.Value.ToString());
        }

        private void Cs_T10_TurnOff_ValueChanged(object sender, EventArgs e)
        {
            CsT1TurnOff[9] = Int32.Parse(Cs_T10_TurnOff.Value.ToString());
        }

        private void Cs_T11_TurnOff_ValueChanged(object sender, EventArgs e)
        {
            CsT1TurnOff[10] = Int32.Parse(Cs_T11_TurnOff.Value.ToString());
        }

        private void Cs_T12_TurnOff_ValueChanged(object sender, EventArgs e)
        {
            CsT1TurnOff[11] = Int32.Parse(Cs_T12_TurnOff.Value.ToString());
        }

        private void Cs_T1_Nsb_ValueChanged(object sender, EventArgs e)
        {
            CsT1Nsb[0] = Int32.Parse(Cs_T1_Nsb.Value.ToString());
        }

        private void Cs_T2_Nsb_ValueChanged(object sender, EventArgs e)
        {
            CsT1Nsb[1] = Int32.Parse(Cs_T2_Nsb.Value.ToString());
        }

        private void Cs_T3_Nsb_ValueChanged(object sender, EventArgs e)
        {
            CsT1Nsb[2] = Int32.Parse(Cs_T3_Nsb.Value.ToString());
        }

        private void Cs_T4_Nsb_ValueChanged(object sender, EventArgs e)
        {
            CsT1Nsb[3] = Int32.Parse(Cs_T4_Nsb.Value.ToString());
        }

        private void Cs_T5_Nsb_ValueChanged(object sender, EventArgs e)
        {
            CsT1Nsb[4] = Int32.Parse(Cs_T5_Nsb.Value.ToString());
        }

        private void Cs_T6_Nsb_ValueChanged(object sender, EventArgs e)
        {
            CsT1Nsb[5] = Int32.Parse(Cs_T6_Nsb.Value.ToString());
        }

        private void Cs_T7_Nsb_ValueChanged(object sender, EventArgs e)
        {
            CsT1Nsb[6] = Int32.Parse(Cs_T7_Nsb.Value.ToString());
        }

        private void Cs_T8_Nsb_ValueChanged(object sender, EventArgs e)
        {
            CsT1Nsb[7] = Int32.Parse(Cs_T8_Nsb.Value.ToString());
        }

        private void Cs_T9_Nsb_ValueChanged(object sender, EventArgs e)
        {
            CsT1Nsb[8] = Int32.Parse(Cs_T9_Nsb.Value.ToString());
        }

        private void Cs_T10_Nsb_ValueChanged(object sender, EventArgs e)
        {
            CsT1Nsb[9] = Int32.Parse(Cs_T10_Nsb.Value.ToString());
        }

        private void Cs_T11_Nsb_ValueChanged(object sender, EventArgs e)
        {
            CsT1Nsb[10] = Int32.Parse(Cs_T11_Nsb.Value.ToString());
        }

        private void Cs_T12_Nsb_ValueChanged(object sender, EventArgs e)
        {
            CsT1Nsb[11] = Int32.Parse(Cs_T12_Nsb.Value.ToString());
        }

        private void Sb_T1_Cur_ValueChanged(object sender, EventArgs e)
        {
            SbT1Cur[0] = Int32.Parse(Sb_T1_Cur.Value.ToString());
        }

        private void Sb_T2_Cur_ValueChanged(object sender, EventArgs e)
        {
            SbT1Cur[1] = Int32.Parse(Sb_T2_Cur.Value.ToString());
        }

        private void Sb_T3_Cur_ValueChanged(object sender, EventArgs e)
        {
            SbT1Cur[2] = Int32.Parse(Sb_T3_Cur.Value.ToString());
        }

        private void Sb_T4_Cur_ValueChanged(object sender, EventArgs e)
        {
            SbT1Cur[3] = Int32.Parse(Sb_T4_Cur.Value.ToString());
        }

        private void Sb_T5_Cur_ValueChanged(object sender, EventArgs e)
        {
            SbT1Cur[4] = Int32.Parse(Sb_T5_Cur.Value.ToString());
        }

        private void Sb_T6_Cur_ValueChanged(object sender, EventArgs e)
        {
            SbT1Cur[5] = Int32.Parse(Sb_T6_Cur.Value.ToString());
        }

        private void Sb_T7_Cur_ValueChanged(object sender, EventArgs e)
        {
            SbT1Cur[6] = Int32.Parse(Sb_T7_Cur.Value.ToString());
        }

        private void Sb_T8_Cur_ValueChanged(object sender, EventArgs e)
        {
            SbT1Cur[7] = Int32.Parse(Sb_T8_Cur.Value.ToString());
        }

        private void Sb_T9_Cur_ValueChanged(object sender, EventArgs e)
        {
            SbT1Cur[8] = Int32.Parse(Sb_T9_Cur.Value.ToString());
        }

        private void Sb_T10_Cur_ValueChanged(object sender, EventArgs e)
        {
            SbT1Cur[9] = Int32.Parse(Sb_T10_Cur.Value.ToString());
        }

        private void Sb_T11_Cur_ValueChanged(object sender, EventArgs e)
        {
            SbT1Cur[10] = Int32.Parse(Sb_T11_Cur.Value.ToString());
        }

        private void Sb_T12_Cur_ValueChanged(object sender, EventArgs e)
        {
            SbT1Cur[11] = Int32.Parse(Sb_T12_Cur.Value.ToString());
        }

        private void Sb_T1_Execution_ValueChanged(object sender, EventArgs e)
        {
            SbT1Execution[0] = Int32.Parse(Sb_T1_Execution.Value.ToString());
        }

        private void Sb_T2_Execution_ValueChanged(object sender, EventArgs e)
        {
            SbT1Execution[1] = Int32.Parse(Sb_T2_Execution.Value.ToString());
        }

        private void Sb_T3_Execution_ValueChanged(object sender, EventArgs e)
        {
            SbT1Execution[2] = Int32.Parse(Sb_T3_Execution.Value.ToString());
        }

        private void Sb_T4_Execution_ValueChanged(object sender, EventArgs e)
        {
            SbT1Execution[3] = Int32.Parse(Sb_T4_Execution.Value.ToString());
        }

        private void Sb_T5_Execution_ValueChanged(object sender, EventArgs e)
        {
            SbT1Execution[4] = Int32.Parse(Sb_T5_Execution.Value.ToString());
        }

        private void Sb_T6_Execution_ValueChanged(object sender, EventArgs e)
        {
            SbT1Execution[5] = Int32.Parse(Sb_T6_Execution.Value.ToString());
        }

        private void Sb_T7_Execution_ValueChanged(object sender, EventArgs e)
        {
            SbT1Execution[6] = Int32.Parse(Sb_T7_Execution.Value.ToString());
        }

        private void Sb_T8_Execution_ValueChanged(object sender, EventArgs e)
        {
            SbT1Execution[7] = Int32.Parse(Sb_T8_Execution.Value.ToString());
        }

        private void Sb_T9_Execution_ValueChanged(object sender, EventArgs e)
        {
            SbT1Execution[8] = Int32.Parse(Sb_T9_Execution.Value.ToString());
        }

        private void Sb_T10_Execution_ValueChanged(object sender, EventArgs e)
        {
            SbT1Execution[9] = Int32.Parse(Sb_T10_Execution.Value.ToString());
        }

        private void Sb_T11_Execution_ValueChanged(object sender, EventArgs e)
        {
            SbT1Execution[10] = Int32.Parse(Sb_T11_Execution.Value.ToString());
        }

        private void Sb_T12_Execution_ValueChanged(object sender, EventArgs e)
        {
            SbT1Execution[11] = Int32.Parse(Sb_T12_Execution.Value.ToString());
        }

        private void Sb_T1_TurnOff_ValueChanged(object sender, EventArgs e)
        {
            SbT1TurnOff[0] = Int32.Parse(Sb_T1_TurnOff.Value.ToString());
        }

        private void Sb_T2_TurnOff_ValueChanged(object sender, EventArgs e)
        {
            SbT1TurnOff[1] = Int32.Parse(Sb_T2_TurnOff.Value.ToString());
        }

        private void Sb_T3_TurnOff_ValueChanged(object sender, EventArgs e)
        {
            SbT1TurnOff[2] = Int32.Parse(Sb_T3_TurnOff.Value.ToString());
        }

        private void Sb_T4_TurnOff_ValueChanged(object sender, EventArgs e)
        {
            SbT1TurnOff[3] = Int32.Parse(Sb_T4_TurnOff.Value.ToString());
        }

        private void Sb_T5_TurnOff_ValueChanged(object sender, EventArgs e)
        {
            SbT1TurnOff[4] = Int32.Parse(Sb_T5_TurnOff.Value.ToString());
        }

        private void Sb_T6_TurnOff_ValueChanged(object sender, EventArgs e)
        {
            SbT1TurnOff[5] = Int32.Parse(Sb_T6_TurnOff.Value.ToString());
        }

        private void Sb_T7_TurnOff_ValueChanged(object sender, EventArgs e)
        {
            SbT1TurnOff[6] = Int32.Parse(Sb_T7_TurnOff.Value.ToString());
        }

        private void Sb_T8_TurnOff_ValueChanged(object sender, EventArgs e)
        {
            SbT1TurnOff[7] = Int32.Parse(Sb_T8_TurnOff.Value.ToString());
        }

        private void Sb_T9_TurnOff_ValueChanged(object sender, EventArgs e)
        {
            SbT1TurnOff[8] = Int32.Parse(Sb_T9_TurnOff.Value.ToString());
        }

        private void Sb_T10_TurnOff_ValueChanged(object sender, EventArgs e)
        {
            SbT1TurnOff[9] = Int32.Parse(Sb_T10_TurnOff.Value.ToString());
        }

        private void Sb_T11_TurnOff_ValueChanged(object sender, EventArgs e)
        {
            SbT1TurnOff[10] = Int32.Parse(Sb_T11_TurnOff.Value.ToString());
        }

        private void Sb_T12_TurnOff_ValueChanged(object sender, EventArgs e)
        {
            SbT1TurnOff[11] = Int32.Parse(Sb_T12_TurnOff.Value.ToString());
        }

        private void Sb_T1_Nsb_ValueChanged(object sender, EventArgs e)
        {
            SbT1Nsb[0] = Int32.Parse(Sb_T1_Nsb.Value.ToString());
        }

        private void Sb_T2_Nsb_ValueChanged(object sender, EventArgs e)
        {
            SbT1Nsb[1] = Int32.Parse(Sb_T2_Nsb.Value.ToString());
        }

        private void Sb_T3_Nsb_ValueChanged(object sender, EventArgs e)
        {
            SbT1Nsb[2] = Int32.Parse(Sb_T3_Nsb.Value.ToString());
        }

        private void Sb_T4_Nsb_ValueChanged(object sender, EventArgs e)
        {
            SbT1Nsb[3] = Int32.Parse(Sb_T4_Nsb.Value.ToString());
        }

        private void Sb_T5_Nsb_ValueChanged(object sender, EventArgs e)
        {
            SbT1Nsb[4] = Int32.Parse(Sb_T5_Nsb.Value.ToString());
        }

        private void Sb_T6_Nsb_ValueChanged(object sender, EventArgs e)
        {
            SbT1Nsb[5] = Int32.Parse(Sb_T6_Nsb.Value.ToString());
        }

        private void Sb_T7_Nsb_ValueChanged(object sender, EventArgs e)
        {
            SbT1Nsb[6] = Int32.Parse(Sb_T7_Nsb.Value.ToString());
        }

        private void Sb_T8_Nsb_ValueChanged(object sender, EventArgs e)
        {
            SbT1Nsb[7] = Int32.Parse(Sb_T8_Nsb.Value.ToString());
        }

        private void Sb_T9_Nsb_ValueChanged(object sender, EventArgs e)
        {
            SbT1Nsb[8] = Int32.Parse(Sb_T9_Nsb.Value.ToString());
        }

        private void Sb_T10_Nsb_ValueChanged(object sender, EventArgs e)
        {
            SbT1Nsb[9] = Int32.Parse(Sb_T10_Nsb.Value.ToString());
        }

        private void tableLayoutPanel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Sb_T11_Nsb_ValueChanged(object sender, EventArgs e)
        {
            SbT1Nsb[10] = Int32.Parse(Sb_T11_Nsb.Value.ToString());
        }

        private void Sb_T12_Nsb_ValueChanged(object sender, EventArgs e)
        {
            SbT1Nsb[11] = Int32.Parse(Sb_T12_Nsb.Value.ToString());
        }

        private void AddRow_Click(object sender, EventArgs e)
        {
            tableLayoutPanel4.RowCount += 1;
            //KIndex += 1;
            Label newLabel1 = new Label();
            newLabel1.Dock = DockStyle.Fill;
            newLabel1.Font = new Font("Arial", 14.25f); // 注意这里的 14.25f，使用 float 类型  
            newLabel1.TextAlign = ContentAlignment.MiddleCenter;
            tableLayoutPanel4.Controls.Add(newLabel1, 0, tableLayoutPanel4.RowCount - 1);
            newLabel1.Text ="周期"+ (KT1Cur.Length + 1).ToString();
            NumericUpDown newNumericUpDown0 = new NumericUpDown();
            tableLayoutPanel4.Controls.Add(newNumericUpDown0, 1, tableLayoutPanel4.RowCount - 1);
            NumericUpDown newNumericUpDown1 = new NumericUpDown();
            tableLayoutPanel4.Controls.Add(newNumericUpDown1, 2, tableLayoutPanel4.RowCount - 1);
            NumericUpDown newNumericUpDown2 = new NumericUpDown();
            tableLayoutPanel4.Controls.Add(newNumericUpDown2, 3, tableLayoutPanel4.RowCount - 1);
        }

        private void Na_T1_Nsb_ValueChanged(object sender, EventArgs e)
        {
            NaT1Nsb[0] = Int32.Parse(Na_T1_Nsb.Value.ToString());
        }

        private void Na_T2_Nsb_ValueChanged(object sender, EventArgs e)
        {
            NaT1Nsb[1] = Int32.Parse(Na_T2_Nsb.Value.ToString());
        }

        private void Na_T3_Nsb_ValueChanged(object sender, EventArgs e)
        {
            NaT1Nsb[2] = Int32.Parse(Na_T3_Nsb.Value.ToString());
        }

        private void Na_T4_Nsb_ValueChanged(object sender, EventArgs e)
        {
            NaT1Nsb[3] = Int32.Parse(Na_T4_Nsb.Value.ToString());
        }

        private void Na_T5_Nsb_ValueChanged(object sender, EventArgs e)
        {
            NaT1Nsb[4] = Int32.Parse(Na_T5_Nsb.Value.ToString());
        }

        private void Na_T6_Nsb_ValueChanged(object sender, EventArgs e)
        {
            NaT1Nsb[5] = Int32.Parse(Na_T6_Nsb.Value.ToString());
        }

        private void Na_T7_Nsb_ValueChanged(object sender, EventArgs e)
        {
            NaT1Nsb[6] = Int32.Parse(Na_T7_Nsb.Value.ToString());
        }

        private void Na_T8_Nsb_ValueChanged(object sender, EventArgs e)
        {
            NaT1Nsb[7] = Int32.Parse(Na_T8_Nsb.Value.ToString());
        }

        private void Na_T9_Nsb_ValueChanged(object sender, EventArgs e)
        {
            NaT1Nsb[8] = Int32.Parse(Na_T9_Nsb.Value.ToString());
        }

        private void Na_T10_Nsb_ValueChanged(object sender, EventArgs e)
        {
            NaT1Nsb[9] = Int32.Parse(Na_T10_Nsb.Value.ToString());
        }

        private void Na_T11_Nsb_ValueChanged(object sender, EventArgs e)
        {
            NaT1Nsb[10] = Int32.Parse(Na_T11_Nsb.Value.ToString());
        }

        private void Na_T12_Nsb_ValueChanged(object sender, EventArgs e)
        {
            NaT1Nsb[11] = Int32.Parse(Na_T12_Nsb.Value.ToString());
        }

        private void T1_Nsb_ValueChanged(object sender, EventArgs e)
        {
            KT1Nsb[0] = Int32.Parse(T1_Nsb.Value.ToString());
        }

        private void T2_Nsb_ValueChanged(object sender, EventArgs e)
        {
            KT1Nsb[1] = Int32.Parse(T2_Nsb.Value.ToString());
        }

        private void T3_Nsb_ValueChanged(object sender, EventArgs e)
        {
            KT1Nsb[2] = Int32.Parse(T3_Nsb.Value.ToString());
        }

        private void T4_Nsb_ValueChanged(object sender, EventArgs e)
        {
            KT1Nsb[3] = Int32.Parse(T4_Nsb.Value.ToString());
        }

        private void T5_Nsb_ValueChanged(object sender, EventArgs e)
        {
            KT1Nsb[4] = Int32.Parse(T5_Nsb.Value.ToString());
        }

        private void T6_Nsb_ValueChanged(object sender, EventArgs e)
        {
            KT1Nsb[5] = Int32.Parse(T6_Nsb.Value.ToString());
        }

        private void T7_Nsb_ValueChanged(object sender, EventArgs e)
        {
            KT1Nsb[6] = Int32.Parse(T7_Nsb.Value.ToString());
        }

        private void T8_Nsb_ValueChanged(object sender, EventArgs e)
        {
            KT1Nsb[7] = Int32.Parse(T8_Nsb.Value.ToString());
        }

        private void T9_Nsb_ValueChanged(object sender, EventArgs e)
        {
            KT1Nsb[8] = Int32.Parse(T9_Nsb.Value.ToString());
        }

        private void T10_Nsb_ValueChanged(object sender, EventArgs e)
        {
            KT1Nsb[9] = Int32.Parse(T10_Nsb.Value.ToString());
        }

        private void T11_Nsb_ValueChanged(object sender, EventArgs e)
        {
            KT1Nsb[10] = Int32.Parse(T11_Nsb.Value.ToString());
        }

        private void T12_Nsb_ValueChanged(object sender, EventArgs e)
        {
            KT1Nsb[11] = Int32.Parse(T12_Nsb.Value.ToString());
        }


        /// <summary>
        /// 坐标显示函数
        /// </summary>
        private void DrawCoord() 
        {
            //chart1.Series["K电源"].ChartType = SeriesChartType.Line;
            //chart1.Series["K电源"].ChartType = SeriesChartType.Line;
            //chart1.Series["K电源"].ChartType = SeriesChartType.Line;
            //chart1.Series["K电源"].ChartType = SeriesChartType.Line;
            // 假设chart1已经在你的窗体上  
            ChartArea chartArea = chart1.ChartAreas[0]; // 获取第一个ChartArea  

            // 设置X轴的范围和间隔  
            chartArea.AxisX.Minimum = 0;
            chartArea.AxisX.Maximum = XMaxTime;
            chartArea.AxisX.Interval = XIntervalTime;
            chartArea.AxisX.LabelStyle.Format = "0秒";

            // 设置Y轴的范围和间隔
            chartArea.AxisY.Minimum = 0; // 
            chartArea.AxisY.Maximum = YMaxAmpere; //
            chartArea.AxisY.Interval = YIntervalAmpere; // 
            chartArea.AxisY.LabelStyle.Format = "0.0A";

            // 如果需要，你还可以设置轴的其他属性，如标题、标签样式等  
            chartArea.AxisX.Title = "时间 (秒)";
            chartArea.AxisY.Title = "电流 (A)";
            if (!InitDraw)
            {
                chart1.Series["K电源"].Points.AddXY(0, 0);
                chart1.Series["Na电源"].Points.AddXY(0, 0);
                chart1.Series["Cs电源"].Points.AddXY(0, 0);
                chart1.Series["Sb电源"].Points.AddXY(0, 0);
            }
           

        }

        /// <summary>
        /// 参数初始化函数
        /// </summary>
        private void InitParameter() 
        {
            XIntervalTime = Int32.Parse(IntervalTime.Value.ToString());
            XMaxTime = Int32.Parse(MaxTime.Value.ToString());
            YIntervalAmpere =Double.Parse(IntervalAmpere.Value.ToString());
            YMaxAmpere = Double.Parse(MaxAmpere.Value.ToString());

        }
        private void But_Stare_Click(object sender, EventArgs e)
        {
            if (But_Stare.Text == "开 始")
            {
                TotalPauseFlag = true;//暂停标致，设置为true，启动程序
                But_Stare.Text = "暂 停";
                But_End.Enabled = false;
                textBox_InFo.AppendText(DateTime.Now.ToString("yyyy年MM月dd日HH:mm:ss") + " :" + "工艺开始" + "\r\n");
            }
            else//暂停
            {
                if (But_Stare.Text == "暂 停")
                {
                   TotalPauseFlag = false;
                   But_Stare.Text = "继 续";
                   But_End.Enabled = true;
                   KPauseFlag = 1;//暂停标志位
                   NaPauseFlag = 1;//暂停标志位
                   CsPauseFlag = 1;//暂停标志位
                   SbPauseFlag = 1;//暂停标志位    
                   textBox_InFo.AppendText(DateTime.Now.ToString("yyyy年MM月dd日HH:mm:ss") + " :" + "工艺暂停" + "\r\n");
                }
                else//继续
                {
                    TotalPauseFlag = true;
                    But_Stare.Text = "暂 停";
                    But_End.Enabled = false;
                    KContinueFlag = 1;
                    NaContinueFlag = 1;
                    CsContinueFlag = 1;
                    SbContinueFlag = 1;
                    textBox_InFo.AppendText(DateTime.Now.ToString("yyyy年MM月dd日HH:mm:ss") + " :" + "工艺继续" + "\r\n");

                }
            }

        }
        private void But_End_Click(object sender, EventArgs e)
        {
            NewOnFlag_K = false;
            NewOnFlag_Na = false;
            NewOnFlag_Cs = false;
            NewOnFlag_Sb = false;
            NewOnFlag_DY = false;

            //给电源值零值
            DYoffFlag = true;
            KoffFlag = true;
            NaoffFlag = true;
            CsoffFlag = true;
            SboffFlag = true;

            TotalPauseFlag = false;
            But_End.Enabled = false;
            But_Stare.Text = "开 始";
            textBox_InFo.AppendText(DateTime.Now.ToString("yyyy年MM月dd日HH:mm:ss") + " :" + "工艺结束" + "\r\n");


            for (int i = 0; i < 12; i++)
            {
                //K
                string KLabelTime = $"T{i + 1}"; // 构建Label的名称  
                Control KLabelT = this.Controls.Find(KLabelTime, true).FirstOrDefault(); // 查找Label控件  
                if (KLabelT is Label KLabelTDisplay) // 检查找到的控件是否是Label  
                {
                    KLabelTDisplay.BackColor = SystemColors.Control;
                }
                string TCur = $"T{i + 1}_Cur";
                Control Cur = this.Controls.Find(TCur, true).FirstOrDefault() as NumericUpDown;
                if (Cur != null)
                {
                    Cur.BackColor = SystemColors.Control;
                }
                string TExecution = $"T{i + 1}_Execution";
                Control Execution = this.Controls.Find(TExecution, true).FirstOrDefault() as NumericUpDown;
                if (Execution != null)
                {
                    Execution.BackColor = SystemColors.Control;
                }

                string TTurnOff = $"T{i + 1}_TurnOff";
                Control TurnOff = this.Controls.Find(TTurnOff, true).FirstOrDefault() as NumericUpDown;
                if (TurnOff != null)
                {
                    TurnOff.BackColor = SystemColors.Control;
                }

                string TNsb = $"T{i + 1}_Nsb";
                Control Nsb = this.Controls.Find(TNsb, true).FirstOrDefault() as NumericUpDown;
                if (Nsb != null)
                {
                    Nsb.BackColor = SystemColors.Control;
                }

                string KLabelName = $"T{i + 1}_Nsb_Display"; // 构建Label的名称  
                Control KLabelControl = this.Controls.Find(KLabelName, true).FirstOrDefault(); // 查找Label控件  
                if (KLabelControl is Label Klabel) // 检查找到的控件是否是Label  
                {
                    Klabel.Text = 0.ToString(); // 设置Label的Text属性  
                }
                //Na
                string NaLabelTime = $"Na_T{i + 1}"; // 构建Label的名称  
                Control NaLabelT = this.Controls.Find(NaLabelTime, true).FirstOrDefault(); // 查找Label控件  
                if (NaLabelT is Label NaLabelTDisplay) // 检查找到的控件是否是Label  
                {
                    NaLabelTDisplay.BackColor = SystemColors.Control;
                }
                string NaTCur = $"Na_T{i + 1}_Cur";
                Control NaCur = this.Controls.Find(NaTCur, true).FirstOrDefault() as NumericUpDown;
                if (NaCur != null)
                {
                    NaCur.BackColor = SystemColors.Control;
                }
                string NaTExecution = $"Na_T{i + 1}_Execution";
                Control NaExecution = this.Controls.Find(NaTExecution, true).FirstOrDefault() as NumericUpDown;
                if (NaExecution != null)
                {
                    NaExecution.BackColor = SystemColors.Control;
                }

                string NaTTurnOff = $"Na_T{i + 1}_TurnOff";
                Control NaTurnOff = this.Controls.Find(NaTTurnOff, true).FirstOrDefault() as NumericUpDown;
                if (NaTurnOff != null)
                {
                    NaTurnOff.BackColor = SystemColors.Control;
                }

                string NaTNsb = $"Na_T{i + 1}_Nsb";
                Control NaNsb = this.Controls.Find(NaTNsb, true).FirstOrDefault() as NumericUpDown;
                if (NaNsb != null)
                {
                    NaNsb.BackColor = SystemColors.Control;
                }

                string NaLabelName = $"Na_T{i + 1}_Nsb_Display"; // 构建Label的名称  
                Control NaLabelControl = this.Controls.Find(NaLabelName, true).FirstOrDefault(); // 查找Label控件  
                if (NaLabelControl is Label Nalabel) // 检查找到的控件是否是Label  
                {
                    Nalabel.Text = 0.ToString(); // 设置Label的Text属性  
                }

                //Cs
                string CsLabelTime = $"Cs_T{i + 1}"; // 构建Label的名称  
                Control CsLabelT = this.Controls.Find(CsLabelTime, true).FirstOrDefault(); // 查找Label控件  
                if (CsLabelT is Label CsLabelTDisplay) // 检查找到的控件是否是Label  
                {
                    CsLabelTDisplay.BackColor = SystemColors.Control;
                }
                string CsTCur = $"Cs_T{i + 1}_Cur";
                Control CsCur = this.Controls.Find(CsTCur, true).FirstOrDefault() as NumericUpDown;
                if (CsCur != null)
                {
                    CsCur.BackColor = SystemColors.Control;
                }
                string CsTExecution = $"Cs_T{i + 1}_Execution";
                Control CsExecution = this.Controls.Find(CsTExecution, true).FirstOrDefault() as NumericUpDown;
                if (CsExecution != null)
                {
                    CsExecution.BackColor = SystemColors.Control;
                }

                string CsTTurnOff = $"Cs_T{i + 1}_TurnOff";
                Control CsTurnOff = this.Controls.Find(CsTTurnOff, true).FirstOrDefault() as NumericUpDown;
                if (CsTurnOff != null)
                {
                    CsTurnOff.BackColor = SystemColors.Control;
                }

                string CsTNsb = $"Cs_T{i + 1}_Nsb";
                Control CsNsb = this.Controls.Find(CsTNsb, true).FirstOrDefault() as NumericUpDown;
                if (CsNsb != null)
                {
                    CsNsb.BackColor = SystemColors.Control;
                }

                string CsLabelCsme = $"Cs_T{i + 1}_Nsb_Display"; // 构建Label的名称  
                Control CsLabelControl = this.Controls.Find(CsLabelCsme, true).FirstOrDefault(); // 查找Label控件  
                if (CsLabelControl is Label Cslabel) // 检查找到的控件是否是Label  
                {
                    Cslabel.Text = 0.ToString(); // 设置Label的Text属性  
                }


                //Sb
                string SbLabelTime = $"Sb_T{i + 1}"; // 构建Label的名称  
                Control SbLabelT = this.Controls.Find(SbLabelTime, true).FirstOrDefault(); // 查找Label控件  
                if (SbLabelT is Label SbLabelTDisplay) // 检查找到的控件是否是Label  
                {
                    SbLabelTDisplay.BackColor = SystemColors.Control;
                }
                string SbTCur = $"Sb_T{i + 1}_Cur";
                Control SbCur = this.Controls.Find(SbTCur, true).FirstOrDefault() as NumericUpDown;
                if (SbCur != null)
                {
                    SbCur.BackColor = SystemColors.Control;
                }
                string SbTExecution = $"Sb_T{i + 1}_Execution";
                Control SbExecution = this.Controls.Find(SbTExecution, true).FirstOrDefault() as NumericUpDown;
                if (SbExecution != null)
                {
                    SbExecution.BackColor = SystemColors.Control;
                }

                string SbTTurnOff = $"Sb_T{i + 1}_TurnOff";
                Control SbTurnOff = this.Controls.Find(SbTTurnOff, true).FirstOrDefault() as NumericUpDown;
                if (SbTurnOff != null)
                {
                    SbTurnOff.BackColor = SystemColors.Control;
                }

                string SbTNsb = $"Sb_T{i + 1}_Nsb";
                Control SbNsb = this.Controls.Find(SbTNsb, true).FirstOrDefault() as NumericUpDown;
                if (SbNsb != null)
                {
                    SbNsb.BackColor = SystemColors.Control;
                }

                string SbLabelSbme = $"Sb_T{i + 1}_Nsb_Display"; // 构建Label的名称  
                Control SbLabelControl = this.Controls.Find(SbLabelSbme, true).FirstOrDefault(); // 查找Label控件  
                if (SbLabelControl is Label Sblabel) // 检查找到的控件是否是Label  
                {
                    Sblabel.Text = 0.ToString(); // 设置Label的Text属性  
                }




            }
            //清零操作
            TimeCount = 0;

            KStartFlag = false;
            NaStartFlag = false;
            CsStartFlag = false;
            SbStartFlag = false;

            KT1Cur = new int[12] { 1000, 1500, 2000, 2500, 3000, 3500, 4000, 4500, 5000, 5500, 6000, 6500 };
            KT1Execution = new int[12] { 10, 10, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30 };
            KT1TurnOff = new int[12] { 0, 0, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30 };
            KT1Nsb = new int[12] { 1, 1, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 };

            NaT1Cur = new int[12] { 1000, 1500, 2000, 2500, 3000, 3500, 4000, 4500, 5000, 5500, 6000, 6500 };
            NaT1Execution = new int[12] { 10, 10, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30 };
            NaT1TurnOff = new int[12] { 0, 0, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30 };
            NaT1Nsb = new int[12] { 1, 1, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 };

            CsT1Cur = new int[12] { 1000, 1500, 2000, 2500, 3000, 3500, 4000, 4500, 5000, 5500, 6000, 6500 };
            CsT1Execution = new int[12] { 10, 10, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30 };
            CsT1TurnOff = new int[12] { 0, 0, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30 };
            CsT1Nsb = new int[12] { 1, 1, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 };
            SbT1Cur = new int[12] { 1000, 1500, 2000, 2500, 3000, 3500, 4000, 4500, 5000, 5500, 6000, 6500 };
            SbT1Execution = new int[12] { 10, 10, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30 };
            SbT1TurnOff = new int[12] { 0, 0, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30 };
            SbT1Nsb = new int[12] { 1, 1, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 };

            KCurIndex = 0; 
            NaCurIndex = 0; 
            CsCurIndex = 0; 
            SbCurIndex = 0;

            KOpenTimeCnt = 0; 
            NaOpenTimeCnt = 0; 
            CsOpenTimeCnt = 0; 
            SbOpenTimeCnt = 0;

            KCloseTimeCnt = 0; 
            NaCloseTimeCnt = 0; 
            CsCloseTimeCnt = 0; 
            SbCloseTimeCnt = 0;

            KNsb = 0; 
            NaNsb = 0; 
            CsNsb = 0; 
            SbNsb = 0;

            CurSetVal_K = 0;
            CurSetVal_Na = 0;
            CurSetVal_Cs = 0;
            CurSetVal_Sb = 0;
            CurSetVal_DY = 0;

            KCount = 0;
            NaCount = 0;
            CsCount = 0;
            SbCount = 0;

            InitDraw = true;//表示结束后，重新开始不画(0, 0)线。

            chart1.Series["K电源"].Points.Clear();
            chart1.Series["Na电源"].Points.Clear();
            chart1.Series["Cs电源"].Points.Clear();
            chart1.Series["Sb电源"].Points.Clear();

            chart1.Series["K电源"].Points.AddXY(0, 0);
            chart1.Series["Na电源"].Points.AddXY(0, 0);
            chart1.Series["Cs电源"].Points.AddXY(0, 0);
            chart1.Series["Sb电源"].Points.AddXY(0, 0);





        }
        private void label_K_Click(object sender, EventArgs e)
        {
            if (TotalPauseFlag) 
            {
                NewOnFlag_K = !NewOnFlag_K;
                KStartFlag = true;
            }
                
        }
        private void label_Na_Click(object sender, EventArgs e)
        {
            if (TotalPauseFlag) 
            {
                NewOnFlag_Na = !NewOnFlag_Na;
                NaStartFlag = true;
            }

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                //给电源值零值
                DYoffFlag = true;
                DYoffFlag = true;
                KoffFlag = true;
                NaoffFlag = true;
                CsoffFlag = true;
                SboffFlag = true;

                Delay(60);
                Thread1.Abort();
                Thread2.Abort();
                Thread3.Abort();
                Thread4.Abort();
                //if (Thread1.ThreadState == ThreadState.Running)
                    
                //if (Thread2.ThreadState == ThreadState.Running)
                    
                //if (Thread3.ThreadState == ThreadState.Running)
                  
                //if (Thread4.ThreadState == ThreadState.Running)
                  
                timer1.Stop();
                //断开连接
                SocketWatch1.Close();//关闭监听         
                SocConnection1.Close();
                SocConnection1 = null;
                SocketWatch1 = null;

                SocketWatch2.Close();//关闭监听         
                SocConnection2.Close();
                SocConnection2 = null;
                SocketWatch2 = null;

                SocketWatch3.Close();//关闭监听         
                SocConnection3.Close();
                SocConnection3 = null;
                SocketWatch3 = null;

                SocketWatch4.Close();//关闭监听         
                SocConnection4.Close();
                SocConnection4 = null;
                SocketWatch4 = null;
                Environment.Exit(0);

            }
            catch (Exception ePort)
            {
                Console.WriteLine("ePort error\r\n");
                Console.WriteLine(ePort.Message);
            }
        }

        private void label_Cs_Click(object sender, EventArgs e)
        {
            if (TotalPauseFlag) 
            {
                NewOnFlag_Cs = !NewOnFlag_Cs;
                CsStartFlag = true;
            }
               
        }
        private void IntervalTime_ValueChanged(object sender, EventArgs e)
        {
            XIntervalTime = Int32.Parse(IntervalTime.Value.ToString());
        }

        private void MaxTime_ValueChanged(object sender, EventArgs e)
        {
            XMaxTime = Int32.Parse(MaxTime.Value.ToString());
        }

        private void IntervalAmpere_ValueChanged(object sender, EventArgs e)
        {
            YIntervalAmpere = Int32.Parse(IntervalAmpere.Value.ToString());
        }

        private void MaxAmpere_ValueChanged(object sender, EventArgs e)
        {
            YMaxAmpere = Int32.Parse(MaxAmpere.Value.ToString());
        }

        private void label_Sb_Click(object sender, EventArgs e)
        {
            if (TotalPauseFlag) 
            {
                NewOnFlag_Sb = !NewOnFlag_Sb;
                SbStartFlag = true;
            }
                
        }

        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }


        /// <summary>
        /// 电源初始化函数
        /// </summary>
        void InitPower() 
        {
            if (!ParameterSave.Exists)
            {
                // 如果文件不存在，则创建它（通过写入一些内容）  
                using (FileStream fileStream = ParameterSave.Create())
                {
                }

                textBox_InFo.AppendText("文件已创建" + "\r\n");
            }
            else
            {
                //textBox_InFo.AppendText("文件已存在" + "\r\n");
                //System.IO.StreamReader objReader = new StreamReader("./Parameter.dat");
                //string sLine = "";
                //sLine = objReader.ReadLine();
                //K_Current.Value = Convert.ToInt32(sLine);
                //sLine = objReader.ReadLine();
                //Na_Current.Value = Convert.ToInt32(sLine);
                //sLine = objReader.ReadLine();
                //Cs_Current.Value = Convert.ToInt32(sLine);
                //sLine = objReader.ReadLine();
                //Sb_Current.Value = Convert.ToInt32(sLine);

                //objReader.Close();
                //objReader = null;
            }
        }
        /// <summary>
        /// 网络连接函数
        /// </summary>
        void NetConnections()
        {
            textBox_InFo.AppendText("进入程序，开始连接网络" + "\r\n");
            //定义一个套接字用于监听客户端发来的信息  包含3个参数(IP4寻址协议,流式连接,TCP协议)
            SocketWatch1 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipaddress1 = IPAddress.Parse("192.168.0.201"); //获取文本框输入的IP地址
            //将IP地址和端口号绑定到网络节点endpoint上 
            IPEndPoint endpoint1 = new IPEndPoint(ipaddress1, 23); //获取文本框上输入的端口号
            //监听绑定的网络节点
            SocketWatch1.Bind(endpoint1);
            Thread1 = new Thread(ServerRecMsg1);
            Thread1.Start();

            SocketWatch2 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipaddress2 = IPAddress.Parse("192.168.0.201");
            IPEndPoint endpoint2 = new IPEndPoint(ipaddress2, 26);
            SocketWatch2.Bind(endpoint2);
            Thread2 = new Thread(ServerRecMsg2);
            Thread2.Start();

            SocketWatch3 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipaddress3 = IPAddress.Parse("192.168.0.201");
            IPEndPoint endpoint3 = new IPEndPoint(ipaddress2, 29);
            SocketWatch3.Bind(endpoint3);
            Thread3 = new Thread(ServerRecMsg3);
            Thread3.Start();

            SocketWatch4 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipaddress4 = IPAddress.Parse("192.168.0.201");
            IPEndPoint endpoint4 = new IPEndPoint(ipaddress4, 32);
            SocketWatch4.Bind(endpoint4);
            Thread4 = new Thread(ServerRecMsg4);
            Thread4.Start();
            textBox_InFo.AppendText("已找到IP地址，网络连接成功" + "\r\n");

        }


        /// <summary>
        /// K电源控制函数
        /// </summary>
        private void ServerRecMsg1()
        {
            textBox_InFo.AppendText( "K电源线程开始工作" + "\r\n");
            float TemCurVal1;//K电源暂存电流值
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
                {    //断网重连
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
                            K_Display.ChangeText("----");
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
                                if (arrServerRecMsg1[0] == 0x01 && arrServerRecMsg1[1] == 0x06 && arrServerRecMsg1[2] == 0x00 && arrServerRecMsg1[3] == 0x1B)//判断帧头 01 06 00 1B
                                {
                                    //label_Pow_K.ChangeText("正常");
                                    //label_Pow_K.ForeColor = Color.Black;
                                    ReadErrorFlag[0] = false;
                                    CurOldVal_K = 0;//保证掉电重启后，能恢复电源值。
                                    //h恢复正常后的处理
                                    if (label_K.BackColor == Color.Red)
                                    {
                                        NewPower_Set_Cur( CurSetVal_K, 1);
                                        SocConnection1.Send(Send_ICmdByte1);
                                        Thread.Sleep(DelayTimeNet);
                                        length1 = SocConnection1.Receive(arrServerRecMsg1);
                                    }
                                    else
                                    {
                                        NewPower_Set_Cur(0, 1);
                                        CurOldVal_K = 0;
                                        SocConnection1.Send(Send_ICmdByte1);
                                        Thread.Sleep(DelayTimeNet);
                                        length1 = SocConnection1.Receive(arrServerRecMsg1);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (ReadErrorFlag[0])
                            {
                                //label_Pow_K.ChangeText("正常");
                                //label_Pow_K.ForeColor = Color.Black;
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
                            if (arrServerRecMsg2[0] == 0x01 && arrServerRecMsg2[1] == 0x03 && arrServerRecMsg2[2] == 0x04)//判断帧头 01 03 04
                            {

                                TemCurVal1 = (BitConverter.ToSingle(BitConverter.GetBytes((arrServerRecMsg2[3]) << 24 | (arrServerRecMsg2[4]) << 16 | (arrServerRecMsg2[5]) << 8 | arrServerRecMsg2[6]), 0)) * 1000;
                                if (TemCurVal1 <= 10)
                                {
                                     CurReadVal_K = 0;
                                }
                                else
                                {
                                     CurReadVal_K = (int)TemCurVal1;
                                    // CurReadVal_K = (int)(OCPmax_PLD * TemCurVal1 / 65535f); ;  //读取的电流值(实测发现需+10校正)
                                }
                                CurRdFlag = true;
                            }
                            K_Display.ChangeText(( CurReadVal_K).ToString());
                            //Display_K.SuspendLayout();
                        }
                    }
                    //程序暂停
                    if ( KPauseFlag == 1)
                    {
                        //电源值清零
                        //NewPower_Set_Cur(0, 1);
                        //SocConnection1.Send(Send_ICmdByte1);
                        NewPower_OF_or_ON(0, 1);
                        SocConnection1.Send(Send_OFForONByte1);
                        Thread.Sleep(DelayTimeNet);
                        length1 = SocConnection1.Receive(arrServerRecMsg1);
                        if (length1 == 8)
                        {
                            if (arrServerRecMsg1[0] == 0x01 && arrServerRecMsg1[1] == 0x06 && arrServerRecMsg1[2] == 0x00 && arrServerRecMsg1[3] == 0x1B)//判断帧头 01 06 00 1B
                            {
                                label_K.BackColor = Color.Gray;
                                K_Display.ChangeText(0.ToString());
                                textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "K程序已经暂停" + "\r\n");
                            }
                            else
                            {
                                //电源值清零
                                //NewPower_Set_Cur(0, 1);
                                //SocConnection1.Send(Send_ICmdByte1);
                                NewPower_OF_or_ON(0, 1);
                                SocConnection1.Send(Send_OFForONByte1);
                                Thread.Sleep(DelayTimeNet);
                                length1 = SocConnection1.Receive(arrServerRecMsg1);
                                if (length1 == 8)
                                {
                                    if (arrServerRecMsg1[0] == 0x01 && arrServerRecMsg1[1] == 0x06 && arrServerRecMsg1[2] == 0x00 && arrServerRecMsg1[3] == 0x1B)//判断帧头 01 06 00 1B
                                    {
                                        label_K.BackColor = Color.Gray;
                                        K_Display.ChangeText(0.ToString());
                                        textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "K程序已经暂停" + "\r\n");
                                    }
                                }
                            }
                        }
                        else
                        {
                            //电源值清零
                            //NewPower_Set_Cur(0, 1);
                            //SocConnection1.Send(Send_ICmdByte1);
                            NewPower_OF_or_ON(0, 1);
                            SocConnection1.Send(Send_OFForONByte1);
                            Thread.Sleep(DelayTimeNet);
                            length1 = SocConnection1.Receive(arrServerRecMsg1);
                            if (length1 == 8)
                            {
                                if (arrServerRecMsg1[0] == 0x01 && arrServerRecMsg1[1] == 0x06 && arrServerRecMsg1[2] == 0x00 && arrServerRecMsg1[3] == 0x1B)//判断帧头 01 06 00 1B
                                {
                                    label_K.BackColor = Color.Gray;
                                    K_Display.ChangeText(0.ToString());
                                    textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "K程序已经暂停" + "\r\n");
                                }
                            }
                        }
                         KPauseFlag = 0;
                        continue;
                    }
                    //程序继续
                    if ( KContinueFlag == 1)
                    {
                        if ( NewOnFlag_K)
                        {
                             OldOnFalg_K = false;
                        }
                         KContinueFlag = 0;
                        continue;
                    }
                    //K电源开关


                    if ( NewOnFlag_K !=  OldOnFalg_K)
                    {

                        if ( NewOnFlag_K)//打开K电源
                        {
                            NewPower_OF_or_ON(1, 1);
                            SocConnection1.Send(Send_OFForONByte1);
                            Thread.Sleep(DelayTimeNet);
                            length1 = SocConnection1.Receive(arrServerRecMsg1);
                            if (length1 == 8)
                            {

                                if (arrServerRecMsg1[0] == 0x01 && arrServerRecMsg1[1] == 0x06 && arrServerRecMsg1[2] == 0x00 && arrServerRecMsg1[3] == 0x1B)//判断帧头 01 06 00 1B
                                {
                                    OldOnFalg_K =  NewOnFlag_K;
                                    label_K.BackColor = Color.Red;
                                    textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "K电源打开成功1" + "\r\n");
                                }
                                else
                                {
                                    NewPower_OF_or_ON(1, 1);
                                    SocConnection1.Send(Send_OFForONByte1);
                                    Thread.Sleep(DelayTimeNet);
                                    length1 = SocConnection1.Receive(arrServerRecMsg1);

                                    if (length1 == 8)
                                    {
                                        if (arrServerRecMsg1[0] == 0x01 && arrServerRecMsg1[1] == 0x06 && arrServerRecMsg1[2] == 0x00 && arrServerRecMsg1[3] == 0x1B)//判断帧头 01 06 00 1B
                                        {
                                             OldOnFalg_K =  NewOnFlag_K;
                                            label_K.BackColor = Color.Red;
                                            textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "K电源打开成功2" + "\r\n");
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
                                    if (arrServerRecMsg1[0] == 0x01 && arrServerRecMsg1[1] == 0x06 && arrServerRecMsg1[2] == 0x00 && arrServerRecMsg1[3] == 0x1B)//判断帧头 01 06 00 1B
                                    {
                                         OldOnFalg_K =  NewOnFlag_K;
                                        label_K.BackColor = Color.Red;
                                        textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "K电源打开成功3" + "\r\n");
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
                                if (arrServerRecMsg1[0] == 0x01 && arrServerRecMsg1[1] == 0x06 && arrServerRecMsg1[2] == 0x00 && arrServerRecMsg1[3] == 0x1B)//判断帧头 01 06 00 1B
                                {
                                     OldOnFalg_K =  NewOnFlag_K;
                                    label_K.BackColor = Color.Gray;
                                    textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "K电源第一次关闭成功" + "\r\n");
                                }
                                else
                                {
                                    //NewPower_Set_Cur(0, 1);
                                    //SocConnection1.Send(Send_ICmdByte1);
                                    NewPower_OF_or_ON(0, 1);
                                    SocConnection1.Send(Send_OFForONByte1);
                                    Thread.Sleep(DelayTimeNet);
                                    length1 = SocConnection1.Receive(arrServerRecMsg1);

                                    if (length1 == 8)
                                    {
                                        if (arrServerRecMsg1[0] == 0x01 && arrServerRecMsg1[1] == 0x06 && arrServerRecMsg1[2] == 0x00 && arrServerRecMsg1[3] == 0x1B)//判断帧头 01 06 00 1B
                                        {
                                            OldOnFalg_K =  NewOnFlag_K;
                                            label_K.BackColor = Color.Gray;
                                            textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "K电源第二次关闭成功" + "\r\n");
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
                                    if (arrServerRecMsg1[0] == 0x01 && arrServerRecMsg1[1] == 0x06 && arrServerRecMsg1[2] == 0x00 && arrServerRecMsg1[3] == 0x1B)//判断帧头 01 06 00 1B
                                    {
                                         OldOnFalg_K =  NewOnFlag_K;
                                        label_K.BackColor = Color.Gray;
                                        textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "K电源第三次关闭成功" + "\r\n");
                                    }
                                }

                            }
                        }

                    }
                    //当电流值不等时，设置电流
                    if ( CurOldVal_K !=  CurSetVal_K && (label_K.BackColor == Color.Red))
                    {
                        NewPower_Set_Cur( CurSetVal_K, 1);
                        SocConnection1.Send(Send_ICmdByte1);
                        Thread.Sleep(DelayTimeNet);
                        length1 = SocConnection1.Receive(arrServerRecMsg1);
                        if (length1 == 8)
                        {
                            if (arrServerRecMsg1[0] == 0x01 && arrServerRecMsg1[1] == 0x10 && arrServerRecMsg1[2] == 0x00 && arrServerRecMsg1[3] == 0x03)//判断帧头 01 10 00 03
                            {

                                 CurOldVal_K =  CurSetVal_K;
                                ErrorFlag1 = true;
                                textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "K电源电流值已设定" + "\r\n");
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
                            NewPower_Set_Cur( CurSetVal_K, 1);
                            SocConnection1.Send(Send_ICmdByte1);
                            textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "K电源电流值设定失败，重新设定" + "\r\n");
                            Thread.Sleep(DelayTimeNet);
                            length1 = SocConnection1.Receive(arrServerRecMsg1);
                            if (length1 == 8)
                            {
                                if (arrServerRecMsg1[0] == 0x01 && arrServerRecMsg1[1] == 0x10 && arrServerRecMsg1[2] == 0x00 && arrServerRecMsg1[3] == 0x03)//判断帧头 01 10 00 03
                                {
                                     CurOldVal_K =  CurSetVal_K;
                                    ErrorFlag1 = true;
                                    textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "K电源电流值第二次设定成功" + "\r\n");
                                }
                            }
                            else
                            {
                                textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "两次重发均未收到电源返回命令" + "\r\n");
                            }

                        }
                    }
                    //设置电压
                    if (K_Vlot_Flag)
                    {
                        NewPower_Set_Volt(KVoltage, 1);//载入文件中的电压设定值
                        SocConnection1.Send(Send_VCmdByte1);
                        Thread.Sleep(DelayTimeNet);
                        length1 = SocConnection1.Receive(arrServerRecMsg1);

                        if (length1 == 8)
                        {
                            if (arrServerRecMsg1[0] == 0x01 && arrServerRecMsg1[1] == 0x10 && arrServerRecMsg1[2] == 0x00 && arrServerRecMsg1[3] == 0x01)//判断帧头 01 10 00 01
                            {
                                K_Vlot_Flag = false;
                                textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "K电源电压值第一次设定成功" + "\r\n");
                            }
                            else
                            {
                                NewPower_Set_Volt(KVoltage, 1);//载入文件中的电压设定值
                                SocConnection1.Send(Send_VCmdByte1);
                                Thread.Sleep(DelayTimeNet);
                                length1 = SocConnection1.Receive(arrServerRecMsg1);

                                if (length1 == 8)
                                {
                                    if (arrServerRecMsg1[0] == 0x01 && arrServerRecMsg1[1] == 0x10 && arrServerRecMsg1[2] == 0x00 && arrServerRecMsg1[3] == 0x01)//判断帧头 01 10 00 01
                                    {
                                        K_Vlot_Flag = false;
                                        textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "K电源电压值第二次设定成功" + "\r\n");
                                    }
                                }
                            }
                        }
                        else
                        {
                            NewPower_Set_Volt(KVoltage, 1);//载入文件中的电压设定值
                            SocConnection1.Send(Send_VCmdByte1);
                            Thread.Sleep(DelayTimeNet);
                            length1 = SocConnection1.Receive(arrServerRecMsg1);

                            if (length1 == 8)
                            {
                                if (arrServerRecMsg1[0] == 0x01 && arrServerRecMsg1[1] == 0x10 && arrServerRecMsg1[2] == 0x00 && arrServerRecMsg1[3] == 0x01)//判断帧头 01 10 00 01
                                {
                                    K_Vlot_Flag = false;
                                    textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "K电源电压值第二次设定成功" + "\r\n");
                                }
                            }
                        }
                        K_Vlot_Flag = false;
                        continue;
                    }
                    //关闭K电源
                    if (KoffFlag == true)
                    {
                        //电源值清零
                        //NewPower_Set_Cur(0, 1);
                        //SocConnection1.Send(Send_ICmdByte1);
                        NewPower_OF_or_ON(0, 1);
                        SocConnection1.Send(Send_OFForONByte1);
                        Thread.Sleep(DelayTimeNet);
                        length1 = SocConnection1.Receive(arrServerRecMsg1);

                        if (length1 == 8)
                        {
                            if (arrServerRecMsg1[0] == 0x01 && arrServerRecMsg1[1] == 0x06 && arrServerRecMsg1[2] == 0x00 && arrServerRecMsg1[3] == 0x1B)//判断帧头 01 06 00 1B
                            {
                                 OldOnFalg_K =  NewOnFlag_K;

                                textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "K电源第一次关闭成功" + "\r\n");
                            }
                            else
                            {
                                //电源值清零
                                //NewPower_Set_Cur(0, 1);
                                //SocConnection1.Send(Send_ICmdByte1);
                                NewPower_OF_or_ON(0, 1);
                                SocConnection1.Send(Send_OFForONByte1);
                                Thread.Sleep(DelayTimeNet);
                                length1 = SocConnection1.Receive(arrServerRecMsg1);

                                if (length1 == 8)
                                {
                                    if (arrServerRecMsg1[0] == 0x01 && arrServerRecMsg1[1] == 0x06 && arrServerRecMsg1[2] == 0x00 && arrServerRecMsg1[3] == 0x1B)//判断帧头 01 06 00 1B
                                    {
                                         OldOnFalg_K =  NewOnFlag_K;

                                        textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "K电源第一次关闭成功" + "\r\n");
                                    }
                                }
                            }
                        }
                        else
                        {
                            //电源值清零
                            //NewPower_Set_Cur(0, 1);
                            //SocConnection1.Send(Send_ICmdByte1);
                            NewPower_OF_or_ON(0, 1);
                            SocConnection1.Send(Send_OFForONByte1);
                            Thread.Sleep(DelayTimeNet);
                            length1 = SocConnection1.Receive(arrServerRecMsg1);

                            if (length1 == 8)
                            {
                                if (arrServerRecMsg1[0] == 0x01 && arrServerRecMsg1[1] == 0x06 && arrServerRecMsg1[2] == 0x00 && arrServerRecMsg1[3] == 0x1B)//判断帧头 01 06 00 1B
                                {
                                     OldOnFalg_K =  NewOnFlag_K;

                                    textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "K电源第一次关闭成功" + "\r\n");
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
                    //Thr1.Abort();
                }
            }
        }
        /// <summary>
        /// Na电源控制函数
        /// </summary>
        private void ServerRecMsg2()
        {
            textBox_InFo.AppendText("Na电源线程开始工作" + "\r\n");
            float TemCurVal2;//电源暂存电流值
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
                            Na_Display.ChangeText("----");
                            ReadErrorFlag[1] = true;
                            //异常处理，先清空缓存，等待1.5s重发。
                            SocConnection2.SendBufferSize = 0;
                            Thread.Sleep(DelayReconnectiuion);
                            NewPower_OF_or_ON(1, 2);
                            SocConnection2.Send(Send_OFForONByte2);
                            Thread.Sleep(DelayTimeNet);
                            length2 = SocConnection2.Receive(arrServerRecMsg2);
                            if (length2 == 8)
                            {
                                if (arrServerRecMsg2[0] == 0x01 && arrServerRecMsg2[1] == 0x06 && arrServerRecMsg2[2] == 0x00 && arrServerRecMsg2[3] == 0x1B)//判断帧头 01 06 00 1B
                                {
                                    //label_Pow_Na.ChangeText("正常");
                                    //label_Pow_Na.ForeColor = Color.Black;
                                    ReadErrorFlag[1] = false;
                                    //恢复正常后的处理
                                    if (label_Na.BackColor == Color.Red)
                                    {
                                        //NewPower_Set_Cur( CurSetVal_Na, 2);
                                        SocConnection2.Send(Send_ICmdByte2);
                                        Thread.Sleep(DelayTimeNet);
                                        length2 = SocConnection2.Receive(arrServerRecMsg2);
                                    }
                                    else
                                    {
                                        NewPower_Set_Cur(0, 2);
                                        // CurOldVal_Na = 0;
                                        SocConnection2.Send(Send_ICmdByte2);
                                        Thread.Sleep(DelayTimeNet);
                                        length2 = SocConnection2.Receive(arrServerRecMsg2);
                                    }

                                }
                            }
                        }
                        else
                        {
                            if (ReadErrorFlag[1])
                            {
                                //恢复正常
                                //label_Pow_Na.ChangeText("正常");
                                //label_Pow_Na.ForeColor = Color.Black;
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
                            if (arrServerRecMsg3[0] == 0x01 && arrServerRecMsg3[1] == 0x03 && arrServerRecMsg3[2] == 0x04)//判断帧头 01 10 10
                            {
                                TemCurVal2 = (BitConverter.ToSingle(BitConverter.GetBytes((arrServerRecMsg3[3]) << 24 | (arrServerRecMsg3[4]) << 16 | (arrServerRecMsg3[5]) << 8 | arrServerRecMsg3[6]), 0)) * 1000;
                                //txtMsg.AppendText(TemCurVal1.ToString()+ "\r\n");
                                if (TemCurVal2 <= 10)
                                {
                                    CurReadVal_Na = 0;
                                }
                                else
                                {
                                    CurReadVal_Na = (int)TemCurVal2;  //读取的电流值(实测发现需+10校正)
                                }
                                CurRdFlag2 = true;
                            }
                            Na_Display.ChangeText((CurReadVal_Na).ToString());
                        }
                    }

                    //程序暂停
                    if ( NaPauseFlag == 1)
                    {
                        //电源值清零
                        NewPower_OF_or_ON(0, 2);
                        SocConnection2.Send(Send_OFForONByte2);

                        Thread.Sleep(DelayTimeNet);
                        length2 = SocConnection2.Receive(arrServerRecMsg2);

                        if (length2 == 8)
                        {
                            if (arrServerRecMsg2[0] == 0x01 && arrServerRecMsg2[1] == 0x06 && arrServerRecMsg2[2] == 0x00 && arrServerRecMsg2[3] == 0x1B)//判断帧头 01 06 00 1B
                            {
                                label_Na.BackColor = Color.Gray;
                                Na_Display.ChangeText(0.ToString());
                                textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Na程序已经暂停" + "\r\n");
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
                                    if (arrServerRecMsg2[0] == 0x01 && arrServerRecMsg2[1] == 0x06 && arrServerRecMsg2[2] == 0x00 && arrServerRecMsg2[3] == 0x1B)//判断帧头 01 06 00 1B
                                    {
                                        label_Na.BackColor = Color.Gray;
                                        Na_Display.ChangeText(0.ToString());
                                        textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Na程序已经暂停" + "\r\n");
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
                                if (arrServerRecMsg2[0] == 0x01 && arrServerRecMsg2[1] == 0x06 && arrServerRecMsg2[2] == 0x00 && arrServerRecMsg2[3] == 0x1B)//判断帧头 01 06 00 1B
                                {
                                    label_Na.BackColor = Color.Gray;
                                    Na_Display.ChangeText(0.ToString());
                                    textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Na程序已经暂停" + "\r\n");
                                }

                            }

                        }
                         NaPauseFlag = 0;
                        continue;
                    }
                    //程序继续
                    if ( NaContinueFlag == 1)
                    {
                        if ( NewOnFlag_Na)
                        {
                             OldOnFalg_Na = false;
                        }
                         NaContinueFlag = 0;
                        continue;
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
                            if (arrServerRecMsg2[0] == 0x01 && arrServerRecMsg2[1] == 0x06 && arrServerRecMsg2[2] == 0x00 && arrServerRecMsg2[3] == 0x1B)//判断帧头 01 06 00 1B
                            {
                                textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Na电源开启成功1" + "\r\n");
                            }
                            else
                            {
                                NewPower_OF_or_ON(1, 2);
                                SocConnection2.Send(Send_OFForONByte2);

                                Thread.Sleep(DelayTimeNet);
                                length2 = SocConnection2.Receive(arrServerRecMsg2);

                                if (length2 == 8)
                                {
                                    if (arrServerRecMsg2[0] == 0x01 && arrServerRecMsg2[1] == 0x06 && arrServerRecMsg2[2] == 0x00 && arrServerRecMsg2[3] == 0x1B)//判断帧头 01 06 00 1B
                                    {
                                        textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Na电源开启成功2" + "\r\n");
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
                                if (arrServerRecMsg2[0] == 0x01 && arrServerRecMsg2[1] == 0x06 && arrServerRecMsg2[2] == 0x00 && arrServerRecMsg2[3] == 0x1B)//判断帧头 01 06 00 1B
                                {
                                    textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Na电源开启成功3" + "\r\n");
                                }
                            }

                        }
                        NaOnFlag = false;
                        continue;
                    }


                    //Na电源开关
                    if ( NewOnFlag_Na !=  OldOnFalg_Na)
                    {
                        if ( NewOnFlag_Na)//打开Na电源
                        {
                            NewPower_OF_or_ON(1, 2);
                            SocConnection2.Send(Send_OFForONByte2);
                            Thread.Sleep(DelayTimeNet);
                            length2 = SocConnection2.Receive(arrServerRecMsg2);

                            if (length2 == 8)
                            {
                                if (arrServerRecMsg2[0] == 0x01 && arrServerRecMsg2[1] == 0x06 && arrServerRecMsg2[2] == 0x00 && arrServerRecMsg2[3] == 0x1B)//判断帧头 01 06 00 1B
                                {
                                     OldOnFalg_Na =  NewOnFlag_Na;
                                    label_Na.BackColor = Color.Red;
                                    textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Na电源打开成功1" + "\r\n");
                                }
                                else
                                {
                                    NewPower_OF_or_ON(1, 2);
                                    SocConnection2.Send(Send_OFForONByte2);
                                    Thread.Sleep(DelayTimeNet);
                                    length2 = SocConnection2.Receive(arrServerRecMsg2);

                                    if (length2 == 8)
                                    {
                                        if (arrServerRecMsg2[0] == 0x01 && arrServerRecMsg2[1] == 0x06 && arrServerRecMsg2[2] == 0x00 && arrServerRecMsg2[3] == 0x1B)//判断帧头 01 06 00 1B
                                        {
                                             OldOnFalg_Na =  NewOnFlag_Na;
                                            label_Na.BackColor = Color.Red;
                                            textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Na电源打开成功2" + "\r\n");
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
                                    if (arrServerRecMsg2[0] == 0x01 && arrServerRecMsg2[1] == 0x06 && arrServerRecMsg2[2] == 0x00 && arrServerRecMsg2[3] == 0x1B)//判断帧头 01 06 00 1B
                                    {
                                         OldOnFalg_Na =  NewOnFlag_Na;
                                        label_Na.BackColor = Color.Red;
                                        textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Na电源打开成功3" + "\r\n");
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
                                if (arrServerRecMsg2[0] == 0x01 && arrServerRecMsg2[1] == 0x06 && arrServerRecMsg2[2] == 0x00 && arrServerRecMsg2[3] == 0x1B)//判断帧头 01 06 00 1B
                                {
                                     OldOnFalg_Na =  NewOnFlag_Na;
                                    label_Na.BackColor = Color.Gray;
                                    textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Na电源第一次关闭成功" + "\r\n");
                                }
                                else
                                {
                                    NewPower_OF_or_ON(0, 2);
                                    SocConnection2.Send(Send_OFForONByte2);

                                    Thread.Sleep(DelayTimeNet);
                                    length2 = SocConnection2.Receive(arrServerRecMsg2);

                                    if (length2 == 8)
                                    {
                                        if (arrServerRecMsg2[0] == 0x01 && arrServerRecMsg2[1] == 0x06 && arrServerRecMsg2[2] == 0x00 && arrServerRecMsg2[3] == 0x1B)//判断帧头 01 06 00 1B
                                        {
                                             OldOnFalg_Na =  NewOnFlag_Na;
                                            label_Na.BackColor = Color.Gray;
                                            textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Na电源第一次关闭成功" + "\r\n");
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
                                    if (arrServerRecMsg2[0] == 0x01 && arrServerRecMsg2[1] == 0x06 && arrServerRecMsg2[2] == 0x00 && arrServerRecMsg2[3] == 0x1B)//判断帧头 01 06 00 1B
                                    {
                                         OldOnFalg_Na =  NewOnFlag_Na;
                                        label_Na.BackColor = Color.Gray;
                                        textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Na电源第一次关闭成功" + "\r\n");
                                    }
                                }
                            }
                        }

                    }
                    //当电流值不等时，设置电流
                    if ( CurOldVal_Na !=  CurSetVal_Na && (label_Na.BackColor == Color.Red))
                    {
                        NewPower_Set_Cur( CurSetVal_Na, 2);
                        SocConnection2.Send(Send_ICmdByte2);
                        Thread.Sleep(DelayTimeNet);
                        length2 = SocConnection2.Receive(arrServerRecMsg2);

                        if (length2 == 8)
                        {
                            if (arrServerRecMsg2[0] == 0x01 && arrServerRecMsg2[1] == 0x10 && arrServerRecMsg2[2] == 0x00 && arrServerRecMsg2[3] == 0x03)//判断帧头 01 10 00 03
                            {
                                 CurOldVal_Na =  CurSetVal_Na;
                                ErrorFlag2 = true;
                                textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Na电源电流值已设定" + "\r\n");
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
                            NewPower_Set_Cur( CurSetVal_Na, 2);
                            SocConnection2.Send(Send_ICmdByte2);
                            textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Na电源电流值设定失败，重新设定" + "\r\n");
                            Thread.Sleep(DelayTimeNet);
                            length2 = SocConnection2.Receive(arrServerRecMsg2);
                            if (length2 == 8)
                            {
                                if (arrServerRecMsg2[0] == 0x01 && arrServerRecMsg2[1] == 0x10 && arrServerRecMsg2[2] == 0x00 && arrServerRecMsg2[3] == 0x03)//判断帧头 01 10 00 03
                                {
                                     CurOldVal_Na =  CurSetVal_Na;
                                    ErrorFlag2 = true;
                                    textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Na电源电流值第二次设定成功" + "\r\n");
                                }
                            }
                            else
                            {
                                textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "两次重发均未收到电源返回命令" + "\r\n");
                            }

                        }
                    }
                    //设置电压
                    if (Na_Vlot_Flag == true)
                    {
                        NewPower_Set_Volt(NaVoltage, 2);//载入文件中的电压设定值
                        SocConnection2.Send(Send_VCmdByte2);
                        Thread.Sleep(DelayTimeNet);
                        length2 = SocConnection2.Receive(arrServerRecMsg2);

                        if (length2 == 8)
                        {
                            if (arrServerRecMsg2[0] == 0x01 && arrServerRecMsg2[1] == 0x10 && arrServerRecMsg2[2] == 0x00 && arrServerRecMsg2[3] == 0x01)//判断帧头 01 10 00 01
                            {

                                Na_Vlot_Flag = false;
                                textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Na电源电压值第一次设定成功" + "\r\n");
                            }
                            else
                            {
                                NewPower_Set_Volt(NaVoltage, 2);//载入文件中的电压设定值
                                SocConnection2.Send(Send_VCmdByte2);
                                Thread.Sleep(DelayTimeNet);
                                length2 = SocConnection2.Receive(arrServerRecMsg2);

                                if (length2 == 8)
                                {
                                    if (arrServerRecMsg2[0] == 0x01 && arrServerRecMsg2[1] == 0x10 && arrServerRecMsg2[2] == 0x00 && arrServerRecMsg2[3] == 0x01)//判断帧头 01 10 00 01
                                    {
                                        Na_Vlot_Flag = false;
                                        textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Na电源电压值第一次设定成功" + "\r\n");
                                    }
                                }
                            }
                        }
                        else
                        {
                            NewPower_Set_Volt(NaVoltage, 2);//载入文件中的电压设定值
                            SocConnection2.Send(Send_VCmdByte2);
                            Thread.Sleep(DelayTimeNet);
                            length2 = SocConnection2.Receive(arrServerRecMsg2);

                            if (length2 == 8)
                            {
                                if (arrServerRecMsg3[0] == 0x01 && arrServerRecMsg3[1] == 0x10 && arrServerRecMsg3[2] == 0x00 && arrServerRecMsg3[3] == 0x01)//判断帧头 01 10 00 01
                                {
                                    Na_Vlot_Flag = false;
                                    textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Na电源电压值第一次设定成功" + "\r\n");
                                }
                            }
                        }
                        Na_Vlot_Flag = false;
                        continue;
                    }

                    //关闭Na电源
                    if (NaoffFlag == true)
                    {
                        //电源值清零
                        NewPower_OF_or_ON(0, 2);
                        SocConnection2.Send(Send_OFForONByte2);
                        Thread.Sleep(DelayTimeNet);
                        length2 = SocConnection2.Receive(arrServerRecMsg2);

                        if (length2 == 8)
                        {
                            if (arrServerRecMsg2[0] == 0x01 && arrServerRecMsg2[1] == 0x06 && arrServerRecMsg2[2] == 0x00 && arrServerRecMsg2[3] == 0x1B)//判断帧头 01 06 00 1B
                            {
                                 OldOnFalg_Na =  NewOnFlag_Na;

                                textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Na电源第一次关闭成功" + "\r\n");
                            }
                            else
                            {
                                NewPower_OF_or_ON(0, 2);
                                SocConnection2.Send(Send_OFForONByte2);

                                Thread.Sleep(DelayTimeNet);
                                length2 = SocConnection2.Receive(arrServerRecMsg2);

                                if (length2 == 8)
                                {
                                    if (arrServerRecMsg2[0] == 0x01 && arrServerRecMsg2[1] == 0x06 && arrServerRecMsg2[2] == 0x00 && arrServerRecMsg2[3] == 0x1B)//判断帧头 01 06 00 1B
                                    {
                                         OldOnFalg_Na =  NewOnFlag_Na;

                                        textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Na电源第一次关闭成功" + "\r\n");
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
                                if (arrServerRecMsg2[0] == 0x01 && arrServerRecMsg2[1] == 0x06 && arrServerRecMsg2[2] == 0x00 && arrServerRecMsg2[3] == 0x1B)//判断帧头 01 06 00 1B
                                {
                                     OldOnFalg_Na =  NewOnFlag_Na;

                                    textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Na电源第一次关闭成功" + "\r\n");
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
            textBox_InFo.AppendText("Cs电源线程开始工作" + "\r\n");
            float TemCurVal3;//电源暂存电流值
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
                            
                            Cs_Display.ChangeText("----");
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
                                if (arrServerRecMsg3[0] == 0x01 && arrServerRecMsg3[1] == 0x06 && arrServerRecMsg3[2] == 0x00 && arrServerRecMsg3[3] == 0x1B)//判断帧头 01 06 00 1B
                                {
                                    //label_Pow_Cs.ChangeText("正常");
                                    //label_Pow_Cs.ForeColor = Color.Black;
                                    ReadErrorFlag[2] = false;
                                    //h恢复正常后的处理
                                    if (label_Cs.BackColor == Color.Red)
                                    {
                                        NewPower_Set_Cur( CurSetVal_Cs, 1);
                                        SocConnection3.Send(Send_ICmdByte3);
                                        Thread.Sleep(DelayTimeNet);
                                        length3 = SocConnection3.Receive(arrServerRecMsg3);
                                    }
                                    else
                                    {
                                        NewPower_Set_Cur(0, 3);
                                         CurOldVal_Cs = 0;
                                        SocConnection3.Send(Send_ICmdByte3);
                                        Thread.Sleep(DelayTimeNet);
                                        length3 = SocConnection3.Receive(arrServerRecMsg3);
                                    }
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
                            if (arrServerRecMsg4[0] == 0x01 && arrServerRecMsg4[1] == 0x03 && arrServerRecMsg4[2] == 0x04)//判断帧头 01 03 04
                            {
                                TemCurVal3 = (BitConverter.ToSingle(BitConverter.GetBytes((arrServerRecMsg4[3]) << 24 | (arrServerRecMsg4[4]) << 16 | (arrServerRecMsg4[5]) << 8 | arrServerRecMsg4[6]), 0)) * 1000;
                                //txtMsg.AppendText(TemCurVal1.ToString()+ "\r\n");
                                if (TemCurVal3 <= 10)
                                {
                                     CurReadVal_Cs = 0;

                                }
                                else
                                {
                                     CurReadVal_Cs = (int)TemCurVal3;  //读取的电流值(实测发现需+10校正)

                                }
                                CurRdFlag3 = true;
                            }
                            Cs_Display.ChangeText(( CurReadVal_Cs).ToString());

                            //Display_K.SuspendLayout();
                        }
                    }

                    //程序暂停
                    if ( CsPauseFlag == 1)
                    {
                        //电源值清零
                        NewPower_OF_or_ON(0, 3);
                        SocConnection3.Send(Send_OFForONByte3);

                        Thread.Sleep(DelayTimeNet);
                        length3 = SocConnection3.Receive(arrServerRecMsg3);

                        if (length3 == 8)
                        {
                            if (arrServerRecMsg3[0] == 0x01 && arrServerRecMsg3[1] == 0x06 && arrServerRecMsg3[2] == 0x00 && arrServerRecMsg3[3] == 0x1B)//判断帧头 01 06 00 1B
                            {
                                label_Cs.BackColor = Color.Gray;
                                Cs_Display.ChangeText(0.ToString());
                                textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Cs程序已经暂停" + "\r\n");
                            }
                            else
                            {
                                NewPower_OF_or_ON(0, 3);
                                SocConnection3.Send(Send_OFForONByte3);

                                Thread.Sleep(DelayTimeNet);
                                length3 = SocConnection3.Receive(arrServerRecMsg3);

                                if (length3 == 8)
                                {
                                    if (arrServerRecMsg3[0] == 0x01 && arrServerRecMsg3[1] == 0x06 && arrServerRecMsg3[2] == 0x00 && arrServerRecMsg3[3] == 0x1B)//判断帧头 01 06 00 1B
                                    {
                                        label_Cs.BackColor = Color.Gray;
                                        Cs_Display.ChangeText(0.ToString());
                                        textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Cs程序已经暂停" + "\r\n");
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
                                if (arrServerRecMsg3[0] == 0x01 && arrServerRecMsg3[1] == 0x06 && arrServerRecMsg3[2] == 0x00 && arrServerRecMsg3[3] == 0x1B)//判断帧头 01 06 00 1B
                                {
                                    label_Cs.BackColor = Color.Gray;
                                    Cs_Display.ChangeText(0.ToString());
                                    textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Cs程序已经暂停" + "\r\n");
                                }
                            }
                        }
                         CsPauseFlag = 0;
                        continue;
                    }
                    //程序继续
                    if ( CsContinueFlag == 1)
                    {
                        if ( NewOnFlag_Cs)
                        {
                             OldOnFalg_Cs = false;
                        }
                         CsContinueFlag = 0;
                        continue;
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
                            if (arrServerRecMsg3[0] == 0x01 && arrServerRecMsg3[1] == 0x06 && arrServerRecMsg3[2] == 0x00 && arrServerRecMsg3[3] == 0x1B)//判断帧头 01 06 00 1B
                            {
                                textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Cs电源开启成功1" + "\r\n");
                            }
                            else
                            {
                                NewPower_OF_or_ON(1, 3);
                                SocConnection3.Send(Send_OFForONByte3);

                                Thread.Sleep(DelayTimeNet);
                                length3 = SocConnection3.Receive(arrServerRecMsg3);

                                if (length3 == 8)
                                {
                                    if (arrServerRecMsg3[0] == 0x01 && arrServerRecMsg3[1] == 0x06 && arrServerRecMsg3[2] == 0x00 && arrServerRecMsg3[3] == 0x1B)//判断帧头 01 06 00 1B
                                    {
                                        textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Cs电源开启成功2" + "\r\n");
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
                                if (arrServerRecMsg3[0] == 0x01 && arrServerRecMsg3[1] == 0x06 && arrServerRecMsg3[2] == 0x00 && arrServerRecMsg3[3] == 0x1B)//判断帧头 01 06 00 1B
                                {
                                    textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Cs电源开启成功3" + "\r\n");
                                }
                            }
                        }
                        CsOnFlag = false;
                        continue;
                    }

                    //Cs电源开关
                    if ( NewOnFlag_Cs !=  OldOnFalg_Cs)
                    {
                        if ( NewOnFlag_Cs)//打开Cs电源
                        {
                            NewPower_OF_or_ON(1, 3);
                            SocConnection3.Send(Send_OFForONByte3);
                            Thread.Sleep(DelayTimeNet);
                            length3 = SocConnection3.Receive(arrServerRecMsg3);

                            if (length3 == 8)
                            {
                                if (arrServerRecMsg3[0] == 0x01 && arrServerRecMsg3[1] == 0x06 && arrServerRecMsg3[2] == 0x00 && arrServerRecMsg3[3] == 0x1B)//判断帧头 01 06 00 1B
                                {
                                     OldOnFalg_Cs =  NewOnFlag_Cs;
                                    label_Cs.BackColor = Color.Red;
                                    textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Cs电源打开成功1" + "\r\n");
                                    //textBox_InFo.AppendText(" TimeCount: " +  TimeCount + " 开Cs" + "\r\n");
                                }
                                else
                                {
                                    NewPower_OF_or_ON(1, 3);
                                    SocConnection3.Send(Send_OFForONByte3);
                                    Thread.Sleep(DelayTimeNet);
                                    length3 = SocConnection3.Receive(arrServerRecMsg3);

                                    if (length3 == 8)
                                    {
                                        if (arrServerRecMsg3[0] == 0x01 && arrServerRecMsg3[1] == 0x06 && arrServerRecMsg3[2] == 0x00 && arrServerRecMsg3[3] == 0x1B)//判断帧头 01 06 00 1B
                                        {
                                             OldOnFalg_Cs =  NewOnFlag_Cs;
                                            label_Cs.BackColor = Color.Red;
                                            textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Cs电源打开成功2" + "\r\n");
                                            //textBox_InFo.AppendText(" TimeCount: " +  TimeCount + " 开Cs" + "\r\n");
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
                                    if (arrServerRecMsg3[0] == 0x01 && arrServerRecMsg3[1] == 0x06 && arrServerRecMsg3[2] == 0x00 && arrServerRecMsg3[3] == 0x1B)//判断帧头 01 06 00 1B
                                    {
                                         OldOnFalg_Cs =  NewOnFlag_Cs;
                                        label_Cs.BackColor = Color.Red;
                                        textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Cs电源打开成功3" + "\r\n");
                                        //textBox_InFo.AppendText(" TimeCount: " +  TimeCount + " 开Cs" + "\r\n");
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
                                if (arrServerRecMsg3[0] == 0x01 && arrServerRecMsg3[1] == 0x06 && arrServerRecMsg3[2] == 0x00 && arrServerRecMsg3[3] == 0x1B)//判断帧头 01 06 00 1B
                                {
                                     OldOnFalg_Cs =  NewOnFlag_Cs;
                                    label_Cs.BackColor = Color.Gray;
                                    textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Cs电源第一次关闭成功" + "\r\n");
                                    //textBox_InFo.AppendText(" TimeCount: " +  TimeCount + " 关Cs" + "\r\n");
                                }
                                else
                                {
                                    NewPower_OF_or_ON(0, 3);
                                    SocConnection3.Send(Send_OFForONByte3);

                                    Thread.Sleep(DelayTimeNet);
                                    length3 = SocConnection3.Receive(arrServerRecMsg3);

                                    if (length3 == 8)
                                    {
                                        if (arrServerRecMsg3[0] == 0x01 && arrServerRecMsg3[1] == 0x06 && arrServerRecMsg3[2] == 0x00 && arrServerRecMsg3[3] == 0x1B)//判断帧头 01 06 00 1B
                                        {
                                             OldOnFalg_Cs =  NewOnFlag_Cs;
                                            label_Cs.BackColor = Color.Gray;
                                            textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Cs电源第一次关闭成功" + "\r\n");
                                            //textBox_InFo.AppendText(" TimeCount: " +  TimeCount + " 关Cs" + "\r\n");
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
                                    if (arrServerRecMsg3[0] == 0x01 && arrServerRecMsg3[1] == 0x06 && arrServerRecMsg3[2] == 0x00 && arrServerRecMsg3[3] == 0x1B)//判断帧头 01 06 00 1B
                                    {
                                         OldOnFalg_Cs =  NewOnFlag_Cs;
                                        label_Cs.BackColor = Color.Gray;
                                        textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Cs电源第一次关闭成功" + "\r\n");
                                        //textBox_InFo.AppendText(" TimeCount: " +  TimeCount + " 关Cs" + "\r\n");
                                    }
                                }
                            }
                        }

                    }
                    //当电流值不等时，设置电流
                    if ( CurOldVal_Cs !=  CurSetVal_Cs && (label_Cs.BackColor == Color.Red))
                    {
                        NewPower_Set_Cur( CurSetVal_Cs, 3);
                        SocConnection3.Send(Send_ICmdByte3);
                        Thread.Sleep(DelayTimeNet);
                        length3 = SocConnection3.Receive(arrServerRecMsg3);

                        if (length3 == 8)
                        {
                            if (arrServerRecMsg3[0] == 0x01 && arrServerRecMsg3[1] == 0x10 && arrServerRecMsg3[2] == 0x00 && arrServerRecMsg3[3] == 0x03)//判断帧头 01 06 00 03
                            {
                                 CurOldVal_Cs =  CurSetVal_Cs;
                                ErrorFlag3 = true;
                                textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Cs电源电流值已设定" + "\r\n");
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
                            NewPower_Set_Cur( CurSetVal_Cs, 3);
                            SocConnection3.Send(Send_ICmdByte3);
                            textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Cs电源电流值设定失败，重新设定" + "\r\n");
                            Thread.Sleep(DelayTimeNet);
                            length3 = SocConnection3.Receive(arrServerRecMsg3);
                            if (length3 == 8)
                            {
                                if (arrServerRecMsg3[0] == 0x01 && arrServerRecMsg3[1] == 0x10 && arrServerRecMsg3[2] == 0x00 && arrServerRecMsg3[3] == 0x03)//判断帧头 01 06 00 03
                                {
                                     CurOldVal_Cs =  CurSetVal_Cs;
                                    ErrorFlag3 = true;
                                    textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Cs电源电流值第二次设定成功" + "\r\n");
                                }
                            }
                            else
                            {
                                textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "两次重发均未收到电源返回命令" + "\r\n");
                            }

                        }
                    }
                    //设置电压
                    if (Cs_Vlot_Flag == true)
                    {
                        NewPower_Set_Volt(CsVoltage, 3);//载入文件中的电压设定值
                        SocConnection3.Send(Send_VCmdByte3);
                        Thread.Sleep(DelayTimeNet);
                        length3 = SocConnection3.Receive(arrServerRecMsg3);

                        if (length3 == 8)
                        {
                            if (arrServerRecMsg3[0] == 0x01 && arrServerRecMsg3[1] == 0x10 && arrServerRecMsg3[2] == 0x00 && arrServerRecMsg3[3] == 0x01)//判断帧头 01 10 00 01
                            {
                                Cs_Vlot_Flag = false;
                                textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Cs电源电压值第一次设定成功" + "\r\n");
                            }
                            else
                            {
                                NewPower_Set_Volt(CsVoltage, 3);//载入文件中的电压设定值
                                SocConnection3.Send(Send_VCmdByte3);
                                Thread.Sleep(DelayTimeNet);
                                length3 = SocConnection3.Receive(arrServerRecMsg3);

                                if (length3 == 8)
                                {
                                    if (arrServerRecMsg3[0] == 0x01 && arrServerRecMsg3[1] == 0x10 && arrServerRecMsg3[2] == 0x00 && arrServerRecMsg3[3] == 0x01)//判断帧头 01 10 00 01
                                    {
                                        Cs_Vlot_Flag = false;
                                        textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Cs电源电压值第一次设定成功" + "\r\n");
                                    }
                                }
                            }
                        }
                        else
                        {
                            NewPower_Set_Volt(CsVoltage, 3);//载入文件中的电压设定值
                            SocConnection3.Send(Send_VCmdByte3);
                            Thread.Sleep(DelayTimeNet);
                            length3 = SocConnection3.Receive(arrServerRecMsg3);

                            if (length3 == 8)
                            {
                                if (arrServerRecMsg3[0] == 0x01 && arrServerRecMsg3[1] == 0x10 && arrServerRecMsg3[2] == 0x00 && arrServerRecMsg3[3] == 0x01)//判断帧头 01 10 00 01
                                {
                                    Cs_Vlot_Flag = false;
                                    textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Cs电源电压值第一次设定成功" + "\r\n");
                                }
                            }
                        }
                        Cs_Vlot_Flag = false;
                        continue;
                    }
                    //关闭Cs电源
                    if (CsoffFlag == true)
                    {
                        //电源值清零
                        NewPower_OF_or_ON(0, 3);
                        SocConnection3.Send(Send_OFForONByte3);

                        Thread.Sleep(DelayTimeNet);
                        length3 = SocConnection3.Receive(arrServerRecMsg3);

                        if (length3 == 8)
                        {
                            if (arrServerRecMsg3[0] == 0x01 && arrServerRecMsg3[1] == 0x06 && arrServerRecMsg3[2] == 0x00 && arrServerRecMsg3[3] == 0x1B)//判断帧头 01 06 00 1B
                            {
                                 OldOnFalg_Cs =  NewOnFlag_Cs;
                                textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Cs电源第一次关闭成功" + "\r\n");
                            }
                            else
                            {
                                NewPower_OF_or_ON(0, 3);
                                SocConnection3.Send(Send_OFForONByte3);

                                Thread.Sleep(DelayTimeNet);
                                length3 = SocConnection3.Receive(arrServerRecMsg3);

                                if (length3 == 8)
                                {
                                    if (arrServerRecMsg3[0] == 0x01 && arrServerRecMsg3[1] == 0x06 && arrServerRecMsg3[2] == 0x00 && arrServerRecMsg3[3] == 0x1B)//判断帧头 01 06 00 1B
                                    {
                                         OldOnFalg_Cs =  NewOnFlag_Cs;
                                        textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Cs电源第一次关闭成功" + "\r\n");
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
                                if (arrServerRecMsg3[0] == 0x01 && arrServerRecMsg3[1] == 0x06 && arrServerRecMsg3[2] == 0x00 && arrServerRecMsg3[3] == 0x1B)//判断帧头 01 06 00 1B
                                {
                                     OldOnFalg_Cs =  NewOnFlag_Cs;

                                    textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Cs电源第一次关闭成功" + "\r\n");
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
            textBox_InFo.AppendText("Sb电源线程开始工作" + "\r\n");
            float TemCurVal4;//电源暂存电流值
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
                            Sb_Display.ChangeText("----");
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
                                if (arrServerRecMsg4[0] == 0x01 && arrServerRecMsg4[1] == 0x06 && arrServerRecMsg4[2] == 0x00 && arrServerRecMsg4[3] == 0x1B)//判断帧头 01 06 00 1B
                                {
                                    //label_Pow_Sb.ChangeText("正常");
                                    //label_Pow_Sb.ForeColor = Color.Black;
                                    ReadErrorFlag[3] = false;
                                    //textBox_InFo.AppendText("1");
                                    //h恢复正常后的处理
                                    if (label_Sb.BackColor == Color.Red)
                                    {
                                        //textBox_InFo.AppendText("SetVal_Sb " + CurSetVal_Sb + "\r\n");
                                        NewPower_Set_Cur( CurSetVal_Sb, 4);
                                        SocConnection4.Send(Send_ICmdByte4);
                                        Thread.Sleep(DelayTimeNet);
                                        length4 = SocConnection4.Receive(arrServerRecMsg4);
                                    }
                                    else
                                    {
                                        //textBox_InFo.AppendText("OldVal_Sb " +  CurOldVal_Sb + "  SetVal_Sb " +  CurSetVal_Sb + "\r\n");
                                        NewPower_Set_Cur( 0, 4);
                                        CurOldVal_Sb =  Sb_Num;
                                        SocConnection4.Send(Send_ICmdByte4);
                                        Thread.Sleep(DelayTimeNet);
                                        length4 = SocConnection4.Receive(arrServerRecMsg4);
                                    }
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
                            if (arrServerRecMsg5[0] == 0x01 && arrServerRecMsg5[1] == 0x03 && arrServerRecMsg5[2] == 0x04)//判断帧头 01 03 04
                            {
                                TemCurVal4 = (BitConverter.ToSingle(BitConverter.GetBytes((arrServerRecMsg5[3]) << 24 | (arrServerRecMsg5[4]) << 16 | (arrServerRecMsg5[5]) << 8 | arrServerRecMsg5[6]), 0)) * 1000;
                                //txtMsg.AppendText(TemCurVal1.ToString()+ "\r\n");
                                if (TemCurVal4 <= 10)
                                {
                                     CurReadVal_Sb = 0;
                                }
                                else
                                {
                                     CurReadVal_Sb = (int)TemCurVal4;  //读取的电流值(实测发现需+10校正)
                                }
                                CurRdFlag4 = true;
                            }
                            Sb_Display.ChangeText(( CurReadVal_Sb).ToString());
                            //Display_K.SuspendLayout();
                        }
                    }

                    //程序暂停
                    if ( SbPauseFlag == 1)
                    {
                        //电源值清零
                        NewPower_OF_or_ON(0, 4);
                        SocConnection4.Send(Send_OFForONByte4);

                        Thread.Sleep(DelayTimeNet);
                        length4 = SocConnection4.Receive(arrServerRecMsg4);

                        if (length4 == 8)
                        {
                            if (arrServerRecMsg4[0] == 0x01 && arrServerRecMsg4[1] == 0x06 && arrServerRecMsg4[2] == 0x00 && arrServerRecMsg4[3] == 0x1B)//判断帧头 01 06 00 1B
                            {
                                label_Sb.BackColor = Color.Gray;
                                Sb_Display.ChangeText(0.ToString());
                                textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Sb程序已经暂停" + "\r\n");
                            }
                            else
                            {
                                NewPower_OF_or_ON(0, 4);
                                SocConnection4.Send(Send_OFForONByte4);

                                Thread.Sleep(DelayTimeNet);
                                length4 = SocConnection4.Receive(arrServerRecMsg4);

                                if (length4 == 8)
                                {
                                    if (arrServerRecMsg4[0] == 0x01 && arrServerRecMsg4[1] == 0x06 && arrServerRecMsg4[2] == 0x00 && arrServerRecMsg4[3] == 0x1B)//判断帧头 01 06 00 1B
                                    {
                                        label_Sb.BackColor = Color.Gray;
                                        Sb_Display.ChangeText(0.ToString());
                                        textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Sb程序已经暂停" + "\r\n");
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
                                if (arrServerRecMsg4[0] == 0x01 && arrServerRecMsg4[1] == 0x06 && arrServerRecMsg4[2] == 0x00 && arrServerRecMsg4[3] == 0x1B)//判断帧头 01 06 00 1B
                                {
                                    label_Sb.BackColor = Color.Gray;
                                    Sb_Display.ChangeText(0.ToString());
                                    textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Sb程序已经暂停" + "\r\n");
                                }
                            }
                        }
                         SbPauseFlag = 0;
                        continue;
                    }
                    //程序继续
                    if ( SbContinueFlag == 1)
                    {
                        if ( NewOnFlag_Sb)
                        {
                             OldOnFalg_Sb = false;
                        }
                         SbContinueFlag = 0;
                        continue;
                    }

                    if (SbOnFlag)
                    {
                        NewPower_OF_or_ON(1, 4);
                        SocConnection4.Send(Send_OFForONByte4);

                        Thread.Sleep(DelayTimeNet);
                        length4 = SocConnection4.Receive(arrServerRecMsg4);

                        if (length4 == 8)
                        {
                            if (arrServerRecMsg4[0] == 0x01 && arrServerRecMsg4[1] == 0x06 && arrServerRecMsg4[2] == 0x00 && arrServerRecMsg4[3] == 0x1B)//判断帧头 01 06 00 1B
                            {
                                textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Sb电源开启成功" + "\r\n");
                            }
                            else
                            {
                                NewPower_OF_or_ON(1, 4);
                                SocConnection4.Send(Send_OFForONByte4);

                                Thread.Sleep(DelayTimeNet);
                                length4 = SocConnection4.Receive(arrServerRecMsg4);

                                if (length4 == 8)
                                {
                                    if (arrServerRecMsg4[0] == 0x01 && arrServerRecMsg4[1] == 0x06 && arrServerRecMsg4[2] == 0x00 && arrServerRecMsg4[3] == 0x1B)//判断帧头 01 06 00 1B
                                    {
                                        textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Sb电源开启成功" + "\r\n");
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
                                if (arrServerRecMsg4[0] == 0x01 && arrServerRecMsg4[1] == 0x06 && arrServerRecMsg4[2] == 0x00 && arrServerRecMsg4[3] == 0x1B)//判断帧头 01 06 00 1B
                                {
                                    textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Sb电源开启成功" + "\r\n");
                                }
                            }
                        }
                        SbOnFlag = false;
                        continue;
                    }

                    //Sb电源开关
                    if ( NewOnFlag_Sb !=  OldOnFalg_Sb)
                    {
                        if ( NewOnFlag_Sb)//打开电源
                        {
                            NewPower_OF_or_ON(1, 4);
                            SocConnection4.Send(Send_OFForONByte4);
                            Thread.Sleep(DelayTimeNet);
                            length4 = SocConnection4.Receive(arrServerRecMsg4);

                            if (length4 == 8)
                            {
                                if (arrServerRecMsg4[0] == 0x01 && arrServerRecMsg4[1] == 0x06 && arrServerRecMsg4[2] == 0x00 && arrServerRecMsg4[3] == 0x1B)//判断帧头 01 06 00 1B
                                {
                                     OldOnFalg_Sb =  NewOnFlag_Sb;
                                    label_Sb.BackColor = Color.Red;
                                    textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Sb电源打开成功1" + "\r\n");
                                    //textBox_InFo.AppendText(" TimeCount: " +  TimeCount + " 开Sb" + "\r\n");
                                }
                                else
                                {
                                    NewPower_OF_or_ON(1, 4);
                                    SocConnection4.Send(Send_OFForONByte4);
                                    Thread.Sleep(DelayTimeNet);
                                    length4 = SocConnection4.Receive(arrServerRecMsg4);

                                    if (length4 == 8)
                                    {
                                        if (arrServerRecMsg4[0] == 0x01 && arrServerRecMsg4[1] == 0x06 && arrServerRecMsg4[2] == 0x00 && arrServerRecMsg4[3] == 0x1B)//判断帧头 01 06 00 1B
                                        {
                                             OldOnFalg_Sb =  NewOnFlag_Sb;
                                            label_Sb.BackColor = Color.Red;
                                            textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Sb电源打开成功2" + "\r\n");
                                            //textBox_InFo.AppendText(" TimeCount: " +  TimeCount + " 开Sb" + "\r\n");
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
                                    if (arrServerRecMsg4[0] == 0x01 && arrServerRecMsg4[1] == 0x06 && arrServerRecMsg4[2] == 0x00 && arrServerRecMsg4[3] == 0x1B)//判断帧头 01 06 00 1B
                                    {
                                         OldOnFalg_Sb =  NewOnFlag_Sb;
                                        label_Sb.BackColor = Color.Red;
                                        textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Sb电源打开成功3" + "\r\n");
                                        //textBox_InFo.AppendText(" TimeCount: " +  TimeCount + " 开Sb" + "\r\n");
                                    }
                                }
                            }
                        }
                        else
                        {
                            //电源值清零
                            //NewPower_Set_Cur( Sb_Num, 4);//根据客户要求，将Sb关闭时设置为 Sb_Num mA;
                            //SocConnection4.Send(Send_ICmdByte4);
                            NewPower_OF_or_ON(0, 4);
                            SocConnection4.Send(Send_OFForONByte4);


                            Thread.Sleep(DelayTimeNet);
                            length4 = SocConnection4.Receive(arrServerRecMsg4);

                            if (length4 == 8)
                            {
                                if (arrServerRecMsg4[0] == 0x01 && arrServerRecMsg4[1] == 0x06 && arrServerRecMsg4[2] == 0x00 && arrServerRecMsg4[3] == 0x1B)//判断帧头 01 06 00 1B
                                {
                                     OldOnFalg_Sb =  NewOnFlag_Sb;
                                    label_Sb.BackColor = Color.Gray;
                                    textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Sb电源第一次关闭成功" + "\r\n");
                                    //textBox_InFo.AppendText(" TimeCount: " +  TimeCount + " 关Sb" + "\r\n");
                                }
                                else
                                {
                                    //NewPower_Set_Cur( Sb_Num, 4);
                                    //SocConnection4.Send(Send_ICmdByte4);
                                    NewPower_OF_or_ON(0, 4);
                                    SocConnection4.Send(Send_OFForONByte4);
                                    Thread.Sleep(DelayTimeNet);
                                    length4 = SocConnection4.Receive(arrServerRecMsg4);

                                    if (length4 == 8)
                                    {
                                        if (arrServerRecMsg4[0] == 0x01 && arrServerRecMsg4[1] == 0x06 && arrServerRecMsg4[2] == 0x00 && arrServerRecMsg4[3] == 0x1B)//判断帧头 01 06 00 1B
                                        {
                                             OldOnFalg_Sb =  NewOnFlag_Sb;
                                            label_Sb.BackColor = Color.Gray;
                                            textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Sb电源第一次关闭成功" + "\r\n");
                                            //textBox_InFo.AppendText(" TimeCount: " +  TimeCount + " 关Sb" + "\r\n");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //NewPower_Set_Cur( Sb_Num, 4);
                                //SocConnection4.Send(Send_ICmdByte4);
                                NewPower_OF_or_ON(0, 4);
                                SocConnection4.Send(Send_OFForONByte4);
                                Thread.Sleep(DelayTimeNet);
                                length4 = SocConnection4.Receive(arrServerRecMsg4);

                                if (length4 == 8)
                                {
                                    if (arrServerRecMsg4[0] == 0x01 && arrServerRecMsg4[1] == 0x06 && arrServerRecMsg4[2] == 0x00 && arrServerRecMsg4[3] == 0x1B)//判断帧头 01 06 00 1B
                                    {
                                         OldOnFalg_Sb =  NewOnFlag_Sb;
                                        label_Sb.BackColor = Color.Gray;
                                        textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Sb电源第一次关闭成功" + "\r\n");
                                        //textBox_InFo.AppendText(" TimeCount: " +  TimeCount + " 关Sb" + "\r\n");
                                    }
                                }
                            }
                        }


                    }

                    //当电流值不等时，设置电流
                    if ( CurOldVal_Sb !=  CurSetVal_Sb && (label_Sb.BackColor == Color.Red))
                    {
                        NewPower_Set_Cur( CurSetVal_Sb, 4);
                        SocConnection4.Send(Send_ICmdByte4);
                        Thread.Sleep(DelayTimeNet);
                        length4 = SocConnection4.Receive(arrServerRecMsg4);

                        if (length4 == 8)
                        {
                            if (arrServerRecMsg4[0] == 0x01 && arrServerRecMsg4[1] == 0x10 && arrServerRecMsg4[2] == 0x00 && arrServerRecMsg4[3] == 0x03)//判断帧头 01 10 00 03
                            {
                                 CurOldVal_Sb =  CurSetVal_Sb;
                                ErrorFlag4 = true;
                                textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Sb电源电流值已设定" + "\r\n");
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
                            NewPower_Set_Cur( CurSetVal_Sb, 4);
                            SocConnection4.Send(Send_ICmdByte4);
                            textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Sb电源电流值设定失败，重新设定" + "\r\n");
                            Thread.Sleep(DelayTimeNet);
                            length4 = SocConnection4.Receive(arrServerRecMsg4);
                            if (length4 == 8)
                            {
                                if (arrServerRecMsg4[0] == 0x01 && arrServerRecMsg4[1] == 0x10 && arrServerRecMsg4[2] == 0x00 && arrServerRecMsg4[3] == 0x03)//判断帧头 01 10 00 03
                                {
                                     CurOldVal_Sb =  CurSetVal_Sb;
                                    ErrorFlag4 = true;
                                    textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Sb电源电流值第二次设定成功" + "\r\n");
                                }
                            }
                            else
                            {
                                textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "两次重发均未收到电源返回命令" + "\r\n");
                            }

                        }
                    }

                    //设置电压
                    if (Sb_Vlot_Flag == true)
                    {
                        NewPower_Set_Volt(SbVoltage, 4);//载入文件中的电压设定值
                        SocConnection4.Send(Send_VCmdByte4);
                        Thread.Sleep(DelayTimeNet);
                        length4 = SocConnection4.Receive(arrServerRecMsg4);

                        if (length4 == 8)
                        {
                            if (arrServerRecMsg4[0] == 0x01 && arrServerRecMsg4[1] == 0x10 && arrServerRecMsg4[2] == 0x00 && arrServerRecMsg4[3] == 0x01)//判断帧头 01 10 00 01
                            {
                                Sb_Vlot_Flag = false;
                                textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Sb电源电压值第一次设定成功" + "\r\n");
                            }
                            else
                            {
                                NewPower_Set_Volt(SbVoltage, 4);//载入文件中的电压设定值
                                SocConnection4.Send(Send_VCmdByte4);
                                Thread.Sleep(DelayTimeNet);
                                length4 = SocConnection4.Receive(arrServerRecMsg4);

                                if (length4 == 8)
                                {
                                    if (arrServerRecMsg4[0] == 0x01 && arrServerRecMsg4[1] == 0x10 && arrServerRecMsg4[2] == 0x00 && arrServerRecMsg4[3] == 0x01)//判断帧头 01 10 00 01
                                    {
                                        Sb_Vlot_Flag = false;
                                        textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Sb电源电压值第一次设定成功" + "\r\n");
                                    }
                                }
                            }
                        }
                        else
                        {
                            NewPower_Set_Volt(SbVoltage, 4);//载入文件中的电压设定值
                            SocConnection4.Send(Send_VCmdByte4);
                            Thread.Sleep(DelayTimeNet);
                            length4 = SocConnection4.Receive(arrServerRecMsg4);

                            if (length4 == 8)
                            {
                                if (arrServerRecMsg4[0] == 0x01 && arrServerRecMsg4[1] == 0x10 && arrServerRecMsg4[2] == 0x00 && arrServerRecMsg4[3] == 0x01)//判断帧头 01 10 00 01
                                {
                                    Sb_Vlot_Flag = false;
                                    textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Sb电源电压值第一次设定成功" + "\r\n");
                                }
                            }
                        }
                        Sb_Vlot_Flag = false;
                        continue;
                    }

                    //关闭Sb电源
                    if (SboffFlag == true)
                    {
                        //电源值清零
                        NewPower_OF_or_ON(0, 4);
                        SocConnection4.Send(Send_OFForONByte4);

                        Thread.Sleep(DelayTimeNet);
                        length4 = SocConnection4.Receive(arrServerRecMsg4);

                        if (length4 == 8)
                        {
                            if (arrServerRecMsg4[0] == 0x01 && arrServerRecMsg4[1] == 0x06 && arrServerRecMsg4[2] == 0x00 && arrServerRecMsg4[3] == 0x1B)//判断帧头 01 06 00 1B
                            {
                                 OldOnFalg_Sb =  NewOnFlag_Sb;

                                textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Sb电源第一次关闭成功" + "\r\n");
                            }
                            else
                            {
                                NewPower_OF_or_ON(0, 4);
                                SocConnection4.Send(Send_OFForONByte4);

                                Thread.Sleep(DelayTimeNet);
                                length4 = SocConnection4.Receive(arrServerRecMsg4);

                                if (length4 == 8)
                                {
                                    if (arrServerRecMsg4[0] == 0x01 && arrServerRecMsg4[1] == 0x06 && arrServerRecMsg4[2] == 0x00 && arrServerRecMsg4[3] == 0x1B)//判断帧头 01 06 00 1B
                                    {
                                         OldOnFalg_Sb =  NewOnFlag_Sb;

                                        textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Sb电源第一次关闭成功" + "\r\n");
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
                                if (arrServerRecMsg4[0] == 0x01 && arrServerRecMsg4[1] == 0x06 && arrServerRecMsg4[2] == 0x00 && arrServerRecMsg4[3] == 0x1B)//判断帧头 01 06 00 1B
                                {
                                     OldOnFalg_Sb =  NewOnFlag_Sb;

                                    textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "Sb电源第一次关闭成功" + "\r\n");
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

        //void DYRS485()
        //{
        //    int Recnum;
        //    int TemCurVal5;//K电源暂存电流值
        //    bool ErrorFlag5 = false;
        //    while (true)
        //    {
        //        try
        //        {
        //            //读电流值
        //            if (WorkFlag)
        //            {
        //                NewPower_Read_Cur(5);
        //                Port2.DiscardInBuffer();//清空串口接收缓冲区
        //                Port2.Write(Send_ReadCmdByte5, 0, 8);

        //                Thread.Sleep(DelayTimeSerial);
        //                Recnum = Port2.BytesToRead;
        //                byte[] DLrec_buf3 = new byte[Recnum];    //声明一个大小为num的字节数据用于存放读出的byte型数据
        //                Port2.Read(DLrec_buf3, 0, Recnum);   //直接从串口接收缓冲区中读7个字节
        //                if (Recnum == 7)
        //                {
        //                    if (DLrec_buf3[0] == 0x01 && DLrec_buf3[1] == 0x03 && DLrec_buf3[2] == 0x02)//判断帧头 01 03 02
        //                    {
        //                        TemCurVal5 = DLrec_buf3[3] * 256 + DLrec_buf3[4];
        //                        if (TemCurVal5 <= 10)
        //                        {
        //                             CurReadVal_DY = 0;
        //                        }
        //                        else
        //                        {
        //                             CurReadVal_DY = (int)OCPmax_HSP * TemCurVal5 / 65535 + 1;  //读取的电流值(实测发现需+1校正)
        //                        }
        //                    }
        //                    label_DY_R.ChangeText( CurReadVal_DY.ToString());
        //                    //Display_K.SuspendLayout();
        //                }
        //                else
        //                {
        //                    label_DY_R.ChangeText("----");
        //                    ReadErrorFlag[4] = true;
        //                }
        //                if (ReadErrorFlag[4])
        //                {
        //                    //异常处理
        //                    NewPower_OF_or_ON(1, 5);
        //                    Port2.DiscardInBuffer();//清空串口接收缓冲区
        //                    Port2.Write(Send_OFForONByte5, 0, 8);//发送开启电源输出命令字符串
        //                    Thread.Sleep(DelayTimeSerial);
        //                    Recnum = Port2.BytesToRead;
        //                    byte[] DLrec_buf = new byte[Recnum];    //声明一个大小为num的字节数据用于存放读出的byte型数据
        //                    Port2.Read(DLrec_buf, 0, Recnum);   //直接从串口接收缓冲区中读8个字节
        //                    if (Recnum == 8) //正确读到了8个字节
        //                    {
        //                        if (DLrec_buf[0] == 0x01 && DLrec_buf[1] == 0x06 && DLrec_buf[2] == 0x00 && DLrec_buf[3] == 0x05) //判断帧头 01 06 00 1B
        //                        {
        //                            label_Pow_DY.ChangeText("正常");
        //                            label_Pow_DY.ForeColor = Color.Black;
        //                            ReadErrorFlag[4] = false;
        //                            if (label_DY.BackColor == Color.Red)
        //                            {
        //                                NewPower_Set_Cur( CurSetVal_DY, 5);
        //                                Port2.DiscardInBuffer();//清空串口接收缓冲区
        //                                Port2.Write(Send_ICmdByte5, 0, 8);//发送开启电源输出命令字符串
        //                            }
        //                            else
        //                            {
        //                                NewPower_Set_Cur(0, 5);
        //                                 CurOldVal_DY = 0;
        //                                Port2.DiscardInBuffer();//清空串口接收缓冲区
        //                                Port2.Write(Send_ICmdByte5, 0, 8);//发送开启电源输出命令字符串
        //                            }
        //                        }
        //                    }
        //                    DLrec_buf = null;
        //                }
        //                else
        //                {
        //                    if (ReadErrorFlag[4])
        //                    {
        //                        label_Pow_DY.ChangeText("正常");
        //                        label_Pow_DY.ForeColor = Color.Black;
        //                        ReadErrorFlag[4] = false;
        //                    }
        //                }
        //                DLrec_buf3 = null;

        //            }
        //            //根据客户要求，每分钟开关灯源一次
        //            if ( DYOnOff1)
        //            {
        //                //关灯源
        //                NewPower_Set_Cur(0, 5);
        //                Port2.DiscardInBuffer();//清空串口接收缓冲区
        //                Port2.Write(Send_ICmdByte5, 0, 8);//发送开启电源输出命令字符串

        //                Thread.Sleep(DelayTimeSerial);
        //                Recnum = Port2.BytesToRead;

        //                byte[] DLrec_buf = new byte[Recnum];    //声明一个大小为num的字节数据用于存放读出的byte型数据
        //                Port2.Read(DLrec_buf, 0, Recnum);   //直接从串口接收缓冲区中读8个字节

        //                if (Recnum == 8) //正确读到了8个字节
        //                {
        //                    if (DLrec_buf[0] == 0x01 && DLrec_buf[1] == 0x06 && DLrec_buf[2] == 0x00 && DLrec_buf[3] == 0x04) //判断帧头 01 06 00 1B
        //                    {
        //                         OldOnFalg_DY =  NewOnFlag_DY;
        //                        label_DY.BackColor = Color.Gray;
        //                    }
        //                }

        //                Thread.Sleep( DYDelay);
        //                //开灯源
        //                 DYOncnt = 15;
        //                 DYOnOff2 = true;

        //                NewPower_Set_Cur( CurSetVal_DY, 5);
        //                Port2.DiscardInBuffer();//清空串口接收缓冲区
        //                Port2.Write(Send_ICmdByte5, 0, 8);//发送开启电源输出命令字符串
        //                Thread.Sleep(DelayTimeSerial);
        //                Recnum = Port2.BytesToRead;
        //                byte[] DLrec_buf1 = new byte[Recnum];    //声明一个大小为num的字节数据用于存放读出的byte型数据
        //                Port2.Read(DLrec_buf1, 0, Recnum);   //直接从串口接收缓冲区中读8个字节

        //                if (Recnum == 8)
        //                {
        //                    if (DLrec_buf1[0] == 0x01 && DLrec_buf1[1] == 0x06 && DLrec_buf1[2] == 0x00 && DLrec_buf1[3] == 0x04)//判断帧头 01 06 10 04
        //                    {
        //                         OldOnFalg_DY =  NewOnFlag_DY;
        //                        label_DY.BackColor = Color.Red;
        //                    }
        //                }
        //                 DYOnOff1 = false;


        //            }

        //            //程序暂停
        //            if ( DYPauseFlag == 1)
        //            {
        //                NewPower_Set_Cur(0, 5);

        //                Port2.DiscardInBuffer();//清空串口接收缓冲区
        //                Port2.Write(Send_ICmdByte5, 0, 8);//发送开启电源输出命令字符串

        //                Thread.Sleep(DelayTimeSerial);
        //                Recnum = Port2.BytesToRead;

        //                byte[] DLrec_buf = new byte[Recnum];    //声明一个大小为num的字节数据用于存放读出的byte型数据
        //                Port2.Read(DLrec_buf, 0, Recnum);   //直接从串口接收缓冲区中读8个字节

        //                if (Recnum == 8) //正确读到了8个字节
        //                {
        //                    if (DLrec_buf[0] == 0x01 && DLrec_buf[1] == 0x06 && DLrec_buf[2] == 0x00 && DLrec_buf[3] == 0x04) //判断帧头 01 06 00 1B
        //                    {
        //                        label_DY.BackColor = Color.Gray;
        //                        label_DY_R.ChangeText(0.ToString());
        //                        textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "灯源程序已经暂停" + "\r\n");
        //                    }
        //                    else//帧头不对重发一次
        //                    {
        //                        textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "灯源程序暂停失败，重新暂停" + "\r\n");
        //                        NewPower_Set_Cur(0, 5);

        //                        Port2.DiscardInBuffer();//清空串口接收缓冲区
        //                        Port2.Write(Send_ICmdByte5, 0, 8);//发送开启电源输出命令字符串

        //                        Thread.Sleep(DelayTimeSerial);
        //                        Recnum = Port2.BytesToRead;

        //                        DLrec_buf = new byte[Recnum];    //声明一个大小为num的字节数据用于存放读出的byte型数据
        //                        Port2.Read(DLrec_buf, 0, Recnum);   //直接从串口接收缓冲区中读8个字节

        //                        if (Recnum == 8) //正确读到了8个字节
        //                        {
        //                            if (DLrec_buf[0] == 0x01 && DLrec_buf[1] == 0x06 && DLrec_buf[2] == 0x00 && DLrec_buf[3] == 0x04) //判断帧头 01 06 00 1B
        //                            {
        //                                label_DY.BackColor = Color.Gray;
        //                                label_DY_R.ChangeText(0.ToString());
        //                                textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "灯源程序已经暂停" + "\r\n");
        //                            }
        //                        }
        //                    }
        //                }
        //                else//未收到8个字节重发一次
        //                {
        //                    NewPower_Set_Cur(0, 5);
        //                    Port2.DiscardInBuffer();//清空串口接收缓冲区
        //                    Port2.Write(Send_ICmdByte5, 0, 8);//发送开启电源输出命令字符串

        //                    Thread.Sleep(DelayTimeSerial);
        //                    Recnum = Port2.BytesToRead;

        //                    DLrec_buf = new byte[Recnum];    //声明一个大小为num的字节数据用于存放读出的byte型数据
        //                    Port2.Read(DLrec_buf, 0, Recnum);   //直接从串口接收缓冲区中读8个字节

        //                    if (Recnum == 8) //正确读到了8个字节
        //                    {
        //                        if (DLrec_buf[0] == 0x01 && DLrec_buf[1] == 0x06 && DLrec_buf[2] == 0x00 && DLrec_buf[3] == 0x04) //判断帧头 01 06 00 1B
        //                        {
        //                            label_DY.BackColor = Color.Gray;
        //                            label_DY_R.ChangeText(0.ToString());
        //                            textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "灯源程序已经暂停" + "\r\n");
        //                        }
        //                    }
        //                }
        //                 DYPauseFlag = 0;
        //                continue;
        //            }
        //            //程序继续
        //            if ( DYContinueFlag == 1)
        //            {
        //                if ( NewOnFlag_DY)
        //                {
        //                     OldOnFalg_DY = false;
        //                }

        //                 DYContinueFlag = 0;
        //                continue;
        //            }
        //            if (DYOnFlag)
        //            {
        //                NewPower_OF_or_ON(1, 5);

        //                Port2.DiscardInBuffer();//清空串口接收缓冲区
        //                Port2.Write(Send_OFForONByte5, 0, 8);//发送开启电源输出命令字符串

        //                Thread.Sleep(DelayTimeNet280);
        //                Recnum = Port2.BytesToRead;

        //                byte[] DLrec_buf = new byte[Recnum];    //声明一个大小为num的字节数据用于存放读出的byte型数据
        //                Port2.Read(DLrec_buf, 0, Recnum);   //直接从串口接收缓冲区中读8个字节

        //                if (Recnum == 8) //正确读到了8个字节
        //                {
        //                    if (DLrec_buf[0] == 0x01 && DLrec_buf[1] == 0x06 && DLrec_buf[2] == 0x00 && DLrec_buf[3] == 0x05) //判断帧头 01 06 00 1B
        //                    {
        //                        textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "灯源电源开启成功1" + "\r\n");
        //                    }
        //                    else//帧头不对重发一次
        //                    {
        //                        NewPower_OF_or_ON(1, 5);

        //                        Port2.DiscardInBuffer();//清空串口接收缓冲区
        //                        Port2.Write(Send_OFForONByte5, 0, 8);//发送开启电源输出命令字符串

        //                        Thread.Sleep(DelayTimeSerial);
        //                        Recnum = Port2.BytesToRead;

        //                        DLrec_buf = new byte[Recnum];    //声明一个大小为num的字节数据用于存放读出的byte型数据
        //                        Port2.Read(DLrec_buf, 0, Recnum);   //直接从串口接收缓冲区中读8个字节

        //                        if (Recnum == 8) //正确读到了8个字节
        //                        {
        //                            if (DLrec_buf[0] == 0x01 && DLrec_buf[1] == 0x06 && DLrec_buf[2] == 0x00 && DLrec_buf[3] == 0x05) //判断帧头 01 06 00 1B
        //                            {
        //                                textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "灯源电源开启成功1" + "\r\n");
        //                            }
        //                        }
        //                    }
        //                }
        //                else//未收到8个字节重发一次
        //                {
        //                    NewPower_OF_or_ON(1, 5);
        //                    Port2.DiscardInBuffer();//清空串口接收缓冲区
        //                    Port2.Write(Send_OFForONByte5, 0, 8);//发送开启电源输出命令字符串

        //                    Thread.Sleep(DelayTimeNet280);
        //                    Recnum = Port2.BytesToRead;

        //                    DLrec_buf = new byte[Recnum];    //声明一个大小为num的字节数据用于存放读出的byte型数据
        //                    Port2.Read(DLrec_buf, 0, Recnum);   //直接从串口接收缓冲区中读8个字节

        //                    if (Recnum == 8) //正确读到了8个字节
        //                    {
        //                        if (DLrec_buf[0] == 0x01 && DLrec_buf[1] == 0x06 && DLrec_buf[2] == 0x00 && DLrec_buf[3] == 0x05) //判断帧头 01 06 00 1B
        //                        {
        //                            textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "灯源电源开启成功2" + "\r\n");
        //                        }
        //                    }

        //                }
        //                //载入上一次灯源电流值
        //                DLrec_buf = null;
        //                DYOnFlag = false;
        //                continue;

        //            }

        //            //灯源开关
        //            if ( NewOnFlag_DY !=  OldOnFalg_DY)
        //            {
        //                if ( NewOnFlag_DY)
        //                {
        //                    NewPower_Set_Cur( CurSetVal_DY, 5);
        //                    Port2.DiscardInBuffer();//清空串口接收缓冲区
        //                    Port2.Write(Send_ICmdByte5, 0, 8);//发送开启电源输出命令字符串
        //                    Thread.Sleep(DelayTimeSerial);
        //                    Recnum = Port2.BytesToRead;
        //                    byte[] DLrec_buf1 = new byte[Recnum];    //声明一个大小为num的字节数据用于存放读出的byte型数据
        //                    Port2.Read(DLrec_buf1, 0, Recnum);   //直接从串口接收缓冲区中读8个字节

        //                    if (Recnum == 8)
        //                    {
        //                        if (DLrec_buf1[0] == 0x01 && DLrec_buf1[1] == 0x06 && DLrec_buf1[2] == 0x00 && DLrec_buf1[3] == 0x04)//判断帧头 01 06 10 04
        //                        {
        //                             OldOnFalg_DY =  NewOnFlag_DY;
        //                            label_DY.BackColor = Color.Red;
        //                            textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "灯源电源打开成功1" + "\r\n");
        //                        }
        //                        else
        //                        {
        //                            NewPower_Set_Cur( CurSetVal_DY, 5);
        //                            Port2.DiscardInBuffer();//清空串口接收缓冲区
        //                            Port2.Write(Send_ICmdByte5, 0, 8);//发送开启电源输出命令字符串
        //                            Thread.Sleep(DelayTimeSerial);
        //                            Recnum = Port2.BytesToRead;
        //                            DLrec_buf1 = new byte[Recnum];    //声明一个大小为num的字节数据用于存放读出的byte型数据
        //                            Port2.Read(DLrec_buf1, 0, Recnum);   //直接从串口接收缓冲区中读8个字节

        //                            if (Recnum == 8)
        //                            {
        //                                if (DLrec_buf1[0] == 0x01 && DLrec_buf1[1] == 0x06 && DLrec_buf1[2] == 0x00 && DLrec_buf1[3] == 0x04)//判断帧头 01 06 10 04
        //                                {
        //                                     OldOnFalg_DY =  NewOnFlag_DY;
        //                                    label_DY.BackColor = Color.Red;
        //                                }
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        NewPower_Set_Cur( CurSetVal_DY, 5);
        //                        Port2.DiscardInBuffer();//清空串口接收缓冲区
        //                        Port2.Write(Send_ICmdByte5, 0, 8);//发送开启电源输出命令字符串
        //                        Thread.Sleep(DelayTimeSerial);
        //                        Recnum = Port2.BytesToRead;
        //                        DLrec_buf1 = new byte[Recnum];    //声明一个大小为num的字节数据用于存放读出的byte型数据
        //                        Port2.Read(DLrec_buf1, 0, Recnum);   //直接从串口接收缓冲区中读8个字节

        //                        if (Recnum == 8)
        //                        {
        //                            if (DLrec_buf1[0] == 0x01 && DLrec_buf1[1] == 0x06 && DLrec_buf1[2] == 0x00 && DLrec_buf1[3] == 0x04)//判断帧头 01 06 10 04
        //                            {
        //                                 OldOnFalg_DY =  NewOnFlag_DY;
        //                                label_DY.BackColor = Color.Red;
        //                            }
        //                        }
        //                    }
        //                    DLrec_buf1 = null;
        //                }
        //                else
        //                {
        //                    NewPower_Set_Cur(0, 5);

        //                    Port2.DiscardInBuffer();//清空串口接收缓冲区
        //                    Port2.Write(Send_ICmdByte5, 0, 8);//发送开启电源输出命令字符串

        //                    Thread.Sleep(DelayTimeSerial);
        //                    Recnum = Port2.BytesToRead;

        //                    byte[] DLrec_buf = new byte[Recnum];    //声明一个大小为num的字节数据用于存放读出的byte型数据
        //                    Port2.Read(DLrec_buf, 0, Recnum);   //直接从串口接收缓冲区中读8个字节

        //                    if (Recnum == 8) //正确读到了8个字节
        //                    {
        //                        if (DLrec_buf[0] == 0x01 && DLrec_buf[1] == 0x06 && DLrec_buf[2] == 0x00 && DLrec_buf[3] == 0x04) //判断帧头 01 06 00 1B
        //                        {
        //                             OldOnFalg_DY =  NewOnFlag_DY;
        //                            label_DY.BackColor = Color.Gray;
        //                            textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "灯源电源第一次关闭成功" + "\r\n");
        //                        }
        //                        else
        //                        {
        //                            textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "灯源电源第二次关闭" + "\r\n");
        //                            NewPower_Set_Cur(0, 5);

        //                            Port2.DiscardInBuffer();//清空串口接收缓冲区
        //                            Port2.Write(Send_ICmdByte5, 0, 8);//发送开启电源输出命令字符串

        //                            Thread.Sleep(DelayTimeSerial);
        //                            Recnum = Port2.BytesToRead;

        //                            DLrec_buf = new byte[Recnum];    //声明一个大小为num的字节数据用于存放读出的byte型数据
        //                            Port2.Read(DLrec_buf, 0, Recnum);   //直接从串口接收缓冲区中读8个字节

        //                            if (Recnum == 8) //正确读到了8个字节
        //                            {
        //                                if (DLrec_buf[0] == 0x01 && DLrec_buf[1] == 0x06 && DLrec_buf[2] == 0x00 && DLrec_buf[3] == 0x04) //判断帧头 01 06 00 1B
        //                                {
        //                                     OldOnFalg_DY =  NewOnFlag_DY;
        //                                    label_DY.BackColor = Color.Gray;
        //                                    textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "灯源电源第一次关闭成功" + "\r\n");
        //                                }
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        NewPower_Set_Cur(0, 5);

        //                        Port2.DiscardInBuffer();//清空串口接收缓冲区
        //                        Port2.Write(Send_ICmdByte5, 0, 8);//发送开启电源输出命令字符串

        //                        Thread.Sleep(DelayTimeSerial);
        //                        Recnum = Port2.BytesToRead;

        //                        DLrec_buf = new byte[Recnum];    //声明一个大小为num的字节数据用于存放读出的byte型数据
        //                        Port2.Read(DLrec_buf, 0, Recnum);   //直接从串口接收缓冲区中读8个字节

        //                        if (Recnum == 8) //正确读到了8个字节
        //                        {
        //                            if (DLrec_buf[0] == 0x01 && DLrec_buf[1] == 0x06 && DLrec_buf[2] == 0x00 && DLrec_buf[3] == 0x04) //判断帧头 01 06 00 1B
        //                            {
        //                                 OldOnFalg_DY =  NewOnFlag_DY;
        //                                label_DY.BackColor = Color.Gray;
        //                            }
        //                        }
        //                    }
        //                    DLrec_buf = null;
        //                }


        //            }
        //            //关闭灯源
        //            if (DYoffFlag == true)
        //            {
        //                NewPower_Set_Cur(0, 5);

        //                Port2.DiscardInBuffer();//清空串口接收缓冲区
        //                Port2.Write(Send_ICmdByte5, 0, 8);//发送开启电源输出命令字符串

        //                Thread.Sleep(DelayTimeSerial);
        //                Recnum = Port2.BytesToRead;

        //                byte[] DLrec_buf = new byte[Recnum];    //声明一个大小为num的字节数据用于存放读出的byte型数据
        //                Port2.Read(DLrec_buf, 0, Recnum);   //直接从串口接收缓冲区中读8个字节

        //                if (Recnum == 8) //正确读到了8个字节
        //                {
        //                    if (DLrec_buf[0] == 0x01 && DLrec_buf[1] == 0x06 && DLrec_buf[2] == 0x00 && DLrec_buf[3] == 0x04) //判断帧头 01 06 00 1B
        //                    {
        //                         OldOnFalg_DY =  NewOnFlag_DY;
        //                        textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "灯源电源第一次关闭成功" + "\r\n");
        //                    }
        //                    else
        //                    {
        //                        NewPower_Set_Cur(0, 5);
        //                        Port2.DiscardInBuffer();//清空串口接收缓冲区
        //                        Port2.Write(Send_ICmdByte5, 0, 8);//发送开启电源输出命令字符串

        //                        Thread.Sleep(DelayTimeSerial);
        //                        Recnum = Port2.BytesToRead;

        //                        DLrec_buf = new byte[Recnum];    //声明一个大小为num的字节数据用于存放读出的byte型数据
        //                        Port2.Read(DLrec_buf, 0, Recnum);   //直接从串口接收缓冲区中读8个字节

        //                        if (Recnum == 8) //正确读到了8个字节
        //                        {
        //                            if (DLrec_buf[0] == 0x01 && DLrec_buf[1] == 0x06 && DLrec_buf[2] == 0x00 && DLrec_buf[3] == 0x04) //判断帧头 01 06 00 1B
        //                            {
        //                                 OldOnFalg_DY =  NewOnFlag_DY;
        //                                textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "灯源电源第一次关闭成功" + "\r\n");
        //                            }
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    NewPower_Set_Cur(0, 5);
        //                    Port2.DiscardInBuffer();//清空串口接收缓冲区
        //                    Port2.Write(Send_ICmdByte5, 0, 8);//发送开启电源输出命令字符串

        //                    Thread.Sleep(DelayTimeSerial);
        //                    Recnum = Port2.BytesToRead;

        //                    DLrec_buf = new byte[Recnum];    //声明一个大小为num的字节数据用于存放读出的byte型数据
        //                    Port2.Read(DLrec_buf, 0, Recnum);   //直接从串口接收缓冲区中读8个字节

        //                    if (Recnum == 8) //正确读到了8个字节
        //                    {
        //                        if (DLrec_buf[0] == 0x01 && DLrec_buf[1] == 0x06 && DLrec_buf[2] == 0x00 && DLrec_buf[3] == 0x04) //判断帧头 01 06 00 1B
        //                        {
        //                             OldOnFalg_DY =  NewOnFlag_DY;
        //                            textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "灯源电源第一次关闭成功" + "\r\n");
        //                        }
        //                    }
        //                }
        //                DLrec_buf = null;
        //                DYoffFlag = false;
        //            }
        //            //电压设置
        //            if (DY_Vlot_Flag == true)
        //            {
        //                NewPower_Set_Volt(FileStr.MaxVolt[4], 5);
        //                Port2.DiscardInBuffer();//清空串口接收缓冲区
        //                Port2.Write(Send_VCmdByte5, 0, 8);//发送开启电源输出命令字符串

        //                Thread.Sleep(DelayTimeSerial);
        //                Recnum = Port2.BytesToRead;

        //                byte[] DLrec_buf1 = new byte[Recnum];    //声明一个大小为num的字节数据用于存放读出的byte型数据
        //                Port2.Read(DLrec_buf1, 0, Recnum);   //直接从串口接收缓冲区中读8个字节

        //                if (Recnum == 8)
        //                {
        //                    if (DLrec_buf1[0] == 0x01 && DLrec_buf1[1] == 0x06 && DLrec_buf1[2] == 0x00 && DLrec_buf1[3] == 0x03)//判断帧头 01 10 00 01
        //                    {
        //                        DY_Vlot_Flag = false;
        //                        textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "灯源电源电压值第一次设定成功" + "\r\n");
        //                    }
        //                    else
        //                    {
        //                        NewPower_Set_Volt(FileStr.MaxVolt[4], 5);
        //                        Port2.DiscardInBuffer();//清空串口接收缓冲区
        //                        Port2.Write(Send_VCmdByte5, 0, 8);//发送开启电源输出命令字符串

        //                        Thread.Sleep(DelayTimeSerial);
        //                        Recnum = Port2.BytesToRead;

        //                        DLrec_buf1 = new byte[Recnum];    //声明一个大小为num的字节数据用于存放读出的byte型数据
        //                        Port2.Read(DLrec_buf1, 0, Recnum);   //直接从串口接收缓冲区中读8个字节

        //                        if (Recnum == 8)
        //                        {
        //                            if (DLrec_buf1[0] == 0x01 && DLrec_buf1[1] == 0x06 && DLrec_buf1[2] == 0x00 && DLrec_buf1[3] == 0x03)//判断帧头 01 10 00 01
        //                            {
        //                                DY_Vlot_Flag = false;
        //                                textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "灯源电源电压值第一次设定成功" + "\r\n");
        //                            }
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    NewPower_Set_Volt(FileStr.MaxVolt[4], 5);
        //                    Port2.DiscardInBuffer();//清空串口接收缓冲区
        //                    Port2.Write(Send_VCmdByte5, 0, 8);//发送开启电源输出命令字符串

        //                    Thread.Sleep(DelayTimeSerial);
        //                    Recnum = Port2.BytesToRead;

        //                    DLrec_buf1 = new byte[Recnum];    //声明一个大小为num的字节数据用于存放读出的byte型数据
        //                    Port2.Read(DLrec_buf1, 0, Recnum);   //直接从串口接收缓冲区中读8个字节

        //                    if (Recnum == 8)
        //                    {
        //                        if (DLrec_buf1[0] == 0x01 && DLrec_buf1[1] == 0x06 && DLrec_buf1[2] == 0x00 && DLrec_buf1[3] == 0x03)//判断帧头 01 10 00 01
        //                        {
        //                            DY_Vlot_Flag = false;
        //                            textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "灯源电源电压值第一次设定成功" + "\r\n");
        //                        }
        //                    }
        //                }
        //                DY_Vlot_Flag = false;
        //            }



        //            //设置灯源电流值
        //            if (( CurOldVal_DY !=  CurSetVal_DY) && (label_DY.BackColor == Color.Red))
        //            {
        //                NewPower_Set_Cur( CurSetVal_DY, 5);

        //                Port2.DiscardInBuffer();//清空串口接收缓冲区
        //                Port2.Write(Send_ICmdByte5, 0, 8);//发送开启电源输出命令字符串

        //                Thread.Sleep(DelayTimeSerial);
        //                Recnum = Port2.BytesToRead;

        //                byte[] DLrec_buf1 = new byte[Recnum];    //声明一个大小为num的字节数据用于存放读出的byte型数据
        //                Port2.Read(DLrec_buf1, 0, Recnum);   //直接从串口接收缓冲区中读8个字节

        //                if (Recnum == 8)
        //                {
        //                    if (DLrec_buf1[0] == 0x01 && DLrec_buf1[1] == 0x06 && DLrec_buf1[2] == 0x00 && DLrec_buf1[3] == 0x04)//判断帧头 01 06 10 04
        //                    {
        //                         CurOldVal_DY =  CurSetVal_DY;
        //                        ErrorFlag5 = true;
        //                        textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "灯源电源电流值已设定" + "\r\n");
        //                    }
        //                    else
        //                    {
        //                        ErrorFlag5 = false;
        //                    }

        //                    DLrec_buf1 = null;
        //                }
        //                else
        //                {
        //                    ErrorFlag5 = false;
        //                }
        //                if (ErrorFlag5 == false)
        //                {
        //                    NewPower_Set_Cur( CurSetVal_DY, 5);
        //                    Port2.DiscardInBuffer();//清空串口接收缓冲区
        //                    Port2.Write(Send_ICmdByte5, 0, 8);//发送开启电源输出命令字符串
        //                    textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "灯源电源电流值设定失败，重新设定" + "\r\n");
        //                    Thread.Sleep(DelayTimeSerial);

        //                    Recnum = Port2.BytesToRead;
        //                    byte[] DLrec_buf2 = new byte[Recnum];
        //                    Port2.Read(DLrec_buf2, 0, Recnum);   //直接从串口接收缓冲区中读8个字节
        //                    if (Recnum == 8)
        //                    {
        //                        if (DLrec_buf2[0] == 0x01 && DLrec_buf2[1] == 0x06 && DLrec_buf2[2] == 0x00 && DLrec_buf2[3] == 0x04)//判断帧头 01 06 00 01
        //                        {
        //                             CurOldVal_DY =  CurSetVal_DY;
        //                            ErrorFlag5 = true;
        //                            textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "灯源电源电流值第二次设定成功" + "\r\n");
        //                        }
        //                        DLrec_buf2 = null;
        //                    }
        //                    else
        //                    {
        //                        textBox_InFo.AppendText((GYTime / 3600).ToString("00") + ":" + ((GYTime % 3600) / 60).ToString("00") + ":" + ((GYTime % 3600) % 60).ToString("00") + " " + "两次重发均未收到电源返回命令" + "\r\n");
        //                    }
        //                }
        //                Thread.Sleep(DelayTimeSerial);
        //            }

        //            Thread.Sleep(20);
        //        }
        //        catch (Exception e485)
        //        {
        //            Console.WriteLine("DYRS485\r\n");
        //            Console.WriteLine(e485.Message);
        //        }
        //    }
        //}
        /// <summary>
        /// Timer定时器函数
        /// </summary>
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (TotalPauseFlag)
            {
                GYTime++;
                if (TimeCount == 0)
                {
                    DrawCoord();
                }
                TimeCount++;

                if (KCurIndex < KT1Cur.Length)//不能超过数组最大值
                {
                    if (NewOnFlag_K && KStartFlag)
                    {
                        //textBox_InFo.AppendText(" KT1TurnOff[0]  " + KT1TurnOff[0] + " KCurIndex  " + KCurIndex + "\r\n");
                        //if (KT1TurnOff[0] == 0 && KCurIndex == 0)
                        //{
                        //    KOpenTimeCnt++;
                        //    if (KOpenTimeCnt >= KT1Execution[KCurIndex])
                        //    {
                        //        T1_Execution
                        //        T1_Execution.BackColor = Color.Red;
                        //        T1_TurnOff.BackColor = Color.Red;
                        //        T1_Nsb.BackColor = Color.Red;
                        //        T1_Nsb_Display.Text = 1.ToString();
                        //        KCurIndex++;
                        //        KOpenTimeCnt = 0;
                        //    }
                        //}
                        //else if (KT1TurnOff[1] == 0 && KCurIndex == 1)
                        //{
                        //    KOpenTimeCnt++;
                        //    if (KOpenTimeCnt >= KT1Execution[KCurIndex])
                        //    {
                        //        T1_Execution
                        //        T2_Execution.BackColor = Color.Red;
                        //        T2_TurnOff.BackColor = Color.Red;
                        //        T2_Nsb.BackColor = Color.Red;
                        //        T2_Nsb_Display.Text = 1.ToString();
                        //        KCurIndex++;
                        //        KOpenTimeCnt = 0;
                        //    }
                        //}
                        //label
                        string KLabelTime = $"T{KCurIndex + 1}"; // 构建Label的名称  
                        Control KLabelT = this.Controls.Find(KLabelTime, true).FirstOrDefault(); // 查找Label控件  
                        if (KLabelT is Label KLabelTDisplay) // 检查找到的控件是否是Label  
                        {
                            if (TimeCount % 2 == 0)
                            {
                                KLabelTDisplay.BackColor = Color.Green; // 设置Label的Text属性  
                            }
                            else
                            {
                                KLabelTDisplay.BackColor = Color.Gray;
                            }
                        }
                        else
                        {
                            textBox_InFo.AppendText("没找到LABEL  " + "\r\n");
                        }


                        if (KOpenTimeCnt == 0)
                        {
                            CurSetVal_K = KT1Cur[KCurIndex];
                            chart1.Series["K电源"].Points.AddXY(KCount, CurSetVal_K / 1000.0);
                        }
                        KOpenTimeCnt++;
                        if (KOpenTimeCnt >= KT1Execution[KCurIndex])
                        {
                            //T1_Execution
                            string TExecution = $"T{KCurIndex + 1}_Execution";
                            Control Execution = this.Controls.Find(TExecution, true).FirstOrDefault() as NumericUpDown;
                            if (Execution != null)
                            {
                                Execution.BackColor = Color.Red;
                            }
                            NewOnFlag_K = false;
                        }

                        KCount++;
                        double KEnd = KCount;
                        if (KEnd >= chart1.ChartAreas[0].AxisX.Maximum)
                        {
                            chart1.ChartAreas[0].AxisX.Maximum = (int)Math.Ceiling(KEnd) + 10;
                        }
                        if (CurSetVal_K / 1000.0 >= chart1.ChartAreas[0].AxisY.Maximum)
                        {
                            chart1.ChartAreas[0].AxisY.Maximum = (int)Math.Ceiling(CurSetVal_K / 1000.0) + 1;
                        }

                        chart1.Series["K电源"].Points.AddXY(KEnd, CurSetVal_K / 1000.0);

                        //T1_Cur
                        string TCur = $"T{KCurIndex + 1}_Cur";
                        Control Cur = this.Controls.Find(TCur, true).FirstOrDefault() as NumericUpDown;
                        if (Cur != null)
                        {
                            Cur.BackColor = Color.Red;
                        }

                        //textBox_InFo.AppendText(" KOpenTimeCnt  " + KOpenTimeCnt + " CurSetVal_K  " + CurSetVal_K + " NewOnFlag_K " + NewOnFlag_K + "\r\n");
                    }
                    if (!NewOnFlag_K && KStartFlag)
                    {
                        string KLabelTime = $"T{KCurIndex + 1}"; // 构建Label的名称  
                        Control KLabelT = this.Controls.Find(KLabelTime, true).FirstOrDefault(); // 查找Label控件  
                        if (KLabelT is Label KLabelTDisplay) // 检查找到的控件是否是Label  
                        {
                            KLabelTDisplay.BackColor = Color.Gray;
                        }
                        else
                        {
                            textBox_InFo.AppendText("没找到LABEL  " + "\r\n");
                        }

                        if (KCloseTimeCnt == 0)
                        {
                            chart1.Series["K电源"].Points.AddXY(KCount, 0);
                        }
                        KCloseTimeCnt++;
                        if (KCloseTimeCnt >= KT1TurnOff[KCurIndex])
                        {
                            //T1_TurnOff
                           string TTurnOff = $"T{KCurIndex + 1}_TurnOff";
                            Control TurnOff = this.Controls.Find(TTurnOff, true).FirstOrDefault() as NumericUpDown;
                            if (TurnOff != null)
                            {
                                TurnOff.BackColor = Color.Red;
                            }

                            NewOnFlag_K = true;
                            KOpenTimeCnt = 0;
                            KCloseTimeCnt = 0;
                            KNsb++;
                            string KLabelName = $"T{KCurIndex + 1}_Nsb_Display"; // 构建Label的名称  
                            Control KLabelControl = this.Controls.Find(KLabelName, true).FirstOrDefault(); // 查找Label控件  
                            if (KLabelControl is Label Klabel) // 检查找到的控件是否是Label  
                            {
                                Klabel.Text = KNsb.ToString(); // 设置Label的Text属性  
                            }
                            else
                            {
                                textBox_InFo.AppendText("没找到LABEL  " + "\r\n");
                                // 如果找不到Label，您可能想要记录一个错误或执行其他操作  
                                Console.WriteLine($"Label '{KLabelName}' not found.");
                            }
                            if (KNsb == KT1Nsb[KCurIndex])
                            {
                                //T1_Nsb
                                string TNsb = $"T{KCurIndex + 1}_Nsb";
                                Control Nsb = this.Controls.Find(TNsb, true).FirstOrDefault() as NumericUpDown;
                                if (Nsb != null)
                                {
                                    Nsb.BackColor = Color.Red;
                                }
                                KCurIndex++;
                                KNsb = 0;
                            }
                            
                        }
                        KCount++;
                        double KEnd = KCount;
                        if (KEnd >= chart1.ChartAreas[0].AxisX.Maximum)
                        {
                            chart1.ChartAreas[0].AxisX.Maximum = (int)Math.Ceiling(KEnd) + 10;
                        }
                        if (CurSetVal_K / 1000.0 >= chart1.ChartAreas[0].AxisY.Maximum)
                        {
                            chart1.ChartAreas[0].AxisY.Maximum = (int)Math.Ceiling(CurSetVal_K / 1000.0) + 1;
                        }
                        
                        chart1.Series["K电源"].Points.AddXY(KEnd, 0);


                        //textBox_InFo.AppendText(" KCloseTimeCnt  " + KCloseTimeCnt + " KT1TurnOff[KCurIndex]  " + KT1TurnOff[KCurIndex - 1] + " KNsb  " + KNsb + " NewOnFlag_K " + NewOnFlag_K + "\r\n");
                    }
                    if (CurSetVal_K < 10)//没有电流，关闭电源
                    {
                        KStartFlag = false;
                        NewOnFlag_K = false;
                        //KCurIndex = 0;
                        //KOpenTimeCnt = 0;
                        //KCloseTimeCnt = 0;
                        //KNsb = 0;
                        //KCount = 0;
                    }
                }
                else
                {
                    CurSetVal_K = 0;
                    KStartFlag = false;
                    NewOnFlag_K = false;
                }


                if (NaCurIndex < NaT1Cur.Length)//不能超过数组最大值
                {
                    if (NewOnFlag_Na && NaStartFlag)
                    {
                        //label
                        string NaLabelTime = $"Na_T{NaCurIndex + 1}"; // 构建Label的名称  
                        Control NaLabelT = this.Controls.Find(NaLabelTime, true).FirstOrDefault(); // 查找Label控件  
                        if (NaLabelT is Label NaLabelTDisplay) // 检查找到的控件是否是Label  
                        {
                            if (TimeCount % 2 == 0)
                            {
                                NaLabelTDisplay.BackColor = Color.Green; // 设置Label的Text属性  
                            }
                            else
                            {
                                NaLabelTDisplay.BackColor = Color.Gray;
                            }
                        }
                        else
                        {
                            textBox_InFo.AppendText("没找到LABEL-Na  " + "\r\n");
                        }


                        if (NaOpenTimeCnt == 0)
                        {
                            CurSetVal_Na = NaT1Cur[NaCurIndex];
                            chart1.Series["Na电源"].Points.AddXY(NaCount, CurSetVal_Na / 1000.0);
                        }
                        NaOpenTimeCnt++;
                        if (NaOpenTimeCnt >= NaT1Execution[NaCurIndex])
                        {
                            //T1_Execution
                            string NaTExecution = $"Na_T{NaCurIndex + 1}_Execution";
                            Control NaExecution = this.Controls.Find(NaTExecution, true).FirstOrDefault() as NumericUpDown;
                            if (NaExecution != null)
                            {
                                NaExecution.BackColor = Color.Red;
                            }
                            NewOnFlag_Na = false;
                        }

                        NaCount++;
                        double NaEnd = NaCount;
                        if (NaEnd >= chart1.ChartAreas[0].AxisX.Maximum)
                        {
                            chart1.ChartAreas[0].AxisX.Maximum = (int)Math.Ceiling(NaEnd) + 10;
                        }
                        if (CurSetVal_Na / 1000.0 >= chart1.ChartAreas[0].AxisY.Maximum)
                        {
                            chart1.ChartAreas[0].AxisY.Maximum = (int)Math.Ceiling(CurSetVal_Na / 1000.0) + 1;
                        }

                        chart1.Series["Na电源"].Points.AddXY(NaEnd, CurSetVal_Na / 1000.0);

                        //T1_Cur
                        string TCur = $"Na_T{NaCurIndex + 1}_Cur";
                        Control Cur = this.Controls.Find(TCur, true).FirstOrDefault() as NumericUpDown;
                        if (Cur != null)
                        {
                            Cur.BackColor = Color.Red;
                        }

                        //textBox_InFo.AppendText(" NaOpenTimeCnt  " + NaOpenTimeCnt + " CurSetVal_Na  " + CurSetVal_Na + " NewOnFlag_Na " + NewOnFlag_Na + "\r\n");


                    }
                    if (!NewOnFlag_Na && NaStartFlag)
                    {
                        string NaLabelTime = $"Na_T{NaCurIndex + 1}"; // 构建Label的名称  
                        Control NaLabelT = this.Controls.Find(NaLabelTime, true).FirstOrDefault(); // 查找Label控件  
                        if (NaLabelT is Label NaLabelTDisplay) // 检查找到的控件是否是Label  
                        {
                            NaLabelTDisplay.BackColor = Color.Gray;
                        }
                        else
                        {
                            textBox_InFo.AppendText("没找到LABEL  " + "\r\n");
                        }

                        if (NaCloseTimeCnt == 0)
                        {
                            chart1.Series["Na电源"].Points.AddXY(NaCount, 0);
                        }
                        NaCloseTimeCnt++;
                        if (NaCloseTimeCnt >= NaT1TurnOff[NaCurIndex])
                        {
                            //T1_TurnOff
                            string TTurnOff = $"Na_T{NaCurIndex + 1}_TurnOff";
                            Control TurnOff = this.Controls.Find(TTurnOff, true).FirstOrDefault() as NumericUpDown;
                            if (TurnOff != null)
                            {
                                TurnOff.BackColor = Color.Red;
                            }

                            NewOnFlag_Na = true;
                            NaOpenTimeCnt = 0;
                            NaCloseTimeCnt = 0;
                            NaNsb++;
                            string NaLabelName = $"Na_T{NaCurIndex + 1}_Nsb_Display"; // 构建Label的名称  
                            Control NaLabelControl = this.Controls.Find(NaLabelName, true).FirstOrDefault(); // 查找Label控件  
                            if (NaLabelControl is Label Nalabel) // 检查找到的控件是否是Label  
                            {
                                Nalabel.Text = NaNsb.ToString(); // 设置Label的Text属性  
                            }
                            else
                            {
                                textBox_InFo.AppendText("没找到LABEL  " + "\r\n");
                                // 如果找不到Label，您可能想要记录一个错误或执行其他操作  
                                Console.WriteLine($"Label '{NaLabelName}' not found.");
                            }
                            if (NaNsb == NaT1Nsb[NaCurIndex])
                            {
                                //T1_Nsb
                                string NaTNsb = $"Na_T{NaCurIndex + 1}_Nsb";
                                Control Nsb = this.Controls.Find(NaTNsb, true).FirstOrDefault() as NumericUpDown;
                                if (Nsb != null)
                                {
                                    Nsb.BackColor = Color.Red;
                                }
                                NaCurIndex++;
                                NaNsb = 0;
                            }

                        }
                        NaCount++;
                        double NaEnd = NaCount;
                        if (NaEnd >= chart1.ChartAreas[0].AxisX.Maximum)
                        {
                            chart1.ChartAreas[0].AxisX.Maximum = (int)Math.Ceiling(NaEnd) + 10;
                        }
                        if (CurSetVal_Na / 1000.0 >= chart1.ChartAreas[0].AxisY.Maximum)
                        {
                            chart1.ChartAreas[0].AxisY.Maximum = (int)Math.Ceiling(CurSetVal_Na / 1000.0) + 1;
                        }

                        chart1.Series["Na电源"].Points.AddXY(NaEnd, 0);


                        //textBox_InFo.AppendText(" NaCloseTimeCnt  " + NaCloseTimeCnt + " NaT1TurnOff[NaCurIndex]  " + NaT1TurnOff[NaCurIndex] + " NaNsb  " + NaNsb + " NewOnFlag_Na " + NewOnFlag_Na + "\r\n");
                    }
                    if (CurSetVal_Na < 10)//没有电流，关闭电源
                    {
                        NaStartFlag = false;
                        NewOnFlag_Na = false;
                    }
                }
                else
                {
                    CurSetVal_Na = 0;
                    NaStartFlag = false;
                    NewOnFlag_Na = false;
                }
                if (CsCurIndex < CsT1Cur.Length)//不能超过数组最大值
                {
                    if (NewOnFlag_Cs && CsStartFlag)
                    {
                        //label
                        string CsLabelTime = $"Cs_T{CsCurIndex + 1}"; // 构建Label的名称  
                        Control CsLabelT = this.Controls.Find(CsLabelTime, true).FirstOrDefault(); // 查找Label控件  
                        if (CsLabelT is Label CsLabelTDisplay) // 检查找到的控件是否是Label  
                        {
                            if (TimeCount % 2 == 0)
                            {
                                CsLabelTDisplay.BackColor = Color.Green; // 设置Label的Text属性  
                            }
                            else
                            {
                                CsLabelTDisplay.BackColor = Color.Gray;
                            }
                        }
                        else
                        {
                            textBox_InFo.AppendText("没找到LABEL-Cs  " + "\r\n");
                        }


                        if (CsOpenTimeCnt == 0)
                        {
                            CurSetVal_Cs = CsT1Cur[CsCurIndex];
                            chart1.Series["Cs电源"].Points.AddXY(CsCount, CurSetVal_Cs / 1000.0);
                        }
                        CsOpenTimeCnt++;
                        if (CsOpenTimeCnt >= CsT1Execution[CsCurIndex])
                        {
                            //T1_Execution
                            string CsTExecution = $"Cs_T{CsCurIndex + 1}_Execution";
                            Control CsExecution = this.Controls.Find(CsTExecution, true).FirstOrDefault() as NumericUpDown;
                            if (CsExecution != null)
                            {
                                CsExecution.BackColor = Color.Red;
                            }
                            NewOnFlag_Cs = false;
                        }

                        CsCount++;
                        double CsEnd = CsCount;
                        if (CsEnd >= chart1.ChartAreas[0].AxisX.Maximum)
                        {
                            chart1.ChartAreas[0].AxisX.Maximum = (int)Math.Ceiling(CsEnd) + 10;
                        }
                        if (CurSetVal_Cs / 1000.0 >= chart1.ChartAreas[0].AxisY.Maximum)
                        {
                            chart1.ChartAreas[0].AxisY.Maximum = (int)Math.Ceiling(CurSetVal_Cs / 1000.0) + 1;
                        }

                        chart1.Series["Cs电源"].Points.AddXY(CsEnd, CurSetVal_Cs / 1000.0);

                        //T1_Cur
                        string TCur = $"Cs_T{CsCurIndex + 1}_Cur";
                        Control Cur = this.Controls.Find(TCur, true).FirstOrDefault() as NumericUpDown;
                        if (Cur != null)
                        {
                            Cur.BackColor = Color.Red;
                        }

                        //textBox_InFo.AppendText(" CsOpenTimeCnt  " + CsOpenTimeCnt + " CurSetVal_Cs  " + CurSetVal_Cs + " NewOnFlag_Cs " + NewOnFlag_Cs + "\r\n");


                    }
                    if (!NewOnFlag_Cs && CsStartFlag)
                    {
                        string CsLabelTime = $"Cs_T{CsCurIndex + 1}"; // 构建Label的名称  
                        Control CsLabelT = this.Controls.Find(CsLabelTime, true).FirstOrDefault(); // 查找Label控件  
                        if (CsLabelT is Label CsLabelTDisplay) // 检查找到的控件是否是Label  
                        {
                            CsLabelTDisplay.BackColor = Color.Gray;
                        }
                        else
                        {
                            textBox_InFo.AppendText("没找到LABEL  " + "\r\n");
                        }

                        if (CsCloseTimeCnt == 0)
                        {
                            chart1.Series["Cs电源"].Points.AddXY(CsCount, 0);
                        }
                        CsCloseTimeCnt++;
                        if (CsCloseTimeCnt >= CsT1TurnOff[CsCurIndex])
                        {
                            //T1_TurnOff
                            string TTurnOff = $"Cs_T{CsCurIndex + 1}_TurnOff";
                            Control TurnOff = this.Controls.Find(TTurnOff, true).FirstOrDefault() as NumericUpDown;
                            if (TurnOff != null)
                            {
                                TurnOff.BackColor = Color.Red;
                            }

                            NewOnFlag_Cs = true;
                            CsOpenTimeCnt = 0;
                            CsCloseTimeCnt = 0;
                            CsNsb++;
                            string CsLabelCsme = $"Cs_T{CsCurIndex + 1}_Nsb_Display"; // 构建Label的名称  
                            Control CsLabelControl = this.Controls.Find(CsLabelCsme, true).FirstOrDefault(); // 查找Label控件  
                            if (CsLabelControl is Label Cslabel) // 检查找到的控件是否是Label  
                            {
                                Cslabel.Text = CsNsb.ToString(); // 设置Label的Text属性  
                            }
                            else
                            {
                                textBox_InFo.AppendText("没找到LABEL  " + "\r\n");
                                // 如果找不到Label，您可能想要记录一个错误或执行其他操作  
                                Console.WriteLine($"Label '{CsLabelCsme}' not found.");
                            }
                            if (CsNsb == CsT1Nsb[CsCurIndex])
                            {
                                //T1_Nsb
                                string CsTNsb = $"Cs_T{CsCurIndex + 1}_Nsb";
                                Control Nsb = this.Controls.Find(CsTNsb, true).FirstOrDefault() as NumericUpDown;
                                if (Nsb != null)
                                {
                                    Nsb.BackColor = Color.Red;
                                }
                                CsCurIndex++;
                                CsNsb = 0;
                            }

                        }
                        CsCount++;
                        double CsEnd = CsCount;
                        if (CsEnd >= chart1.ChartAreas[0].AxisX.Maximum)
                        {
                            chart1.ChartAreas[0].AxisX.Maximum = (int)Math.Ceiling(CsEnd) + 10;
                        }
                        if (CurSetVal_Cs / 1000.0 >= chart1.ChartAreas[0].AxisY.Maximum)
                        {
                            chart1.ChartAreas[0].AxisY.Maximum = (int)Math.Ceiling(CurSetVal_Cs / 1000.0) + 1;
                        }

                        chart1.Series["Cs电源"].Points.AddXY(CsEnd, 0);


                        //textBox_InFo.AppendText(" CsCloseTimeCnt  " + CsCloseTimeCnt + " CsT1TurnOff[CsCurIndex]  " + CsT1TurnOff[CsCurIndex] + " CsNsb  " + CsNsb + " NewOnFlag_Cs " + NewOnFlag_Cs + "\r\n");
                    }
                    if (CurSetVal_Cs < 10)//没有电流，关闭电源
                    {
                        CsStartFlag = false;
                        NewOnFlag_Cs = false;
                    }
                }
                else
                {
                    CurSetVal_Cs = 0;
                    CsStartFlag = false;
                    NewOnFlag_Cs = false;
                }

                if (SbCurIndex < SbT1Cur.Length)//不能超过数组最大值
                {
                    if (NewOnFlag_Sb && SbStartFlag)
                    {
                        //label
                        string SbLabelTime = $"Sb_T{SbCurIndex + 1}"; // 构建Label的名称  
                        Control SbLabelT = this.Controls.Find(SbLabelTime, true).FirstOrDefault(); // 查找Label控件  
                        if (SbLabelT is Label SbLabelTDisplay) // 检查找到的控件是否是Label  
                        {
                            if (TimeCount % 2 == 0)
                            {
                                SbLabelTDisplay.BackColor = Color.Green; // 设置Label的Text属性  
                            }
                            else
                            {
                                SbLabelTDisplay.BackColor = Color.Gray;
                            }
                        }
                        else
                        {
                            textBox_InFo.AppendText("没找到LABEL-Sb  " + "\r\n");
                        }


                        if (SbOpenTimeCnt == 0)
                        {
                            CurSetVal_Sb = SbT1Cur[SbCurIndex];
                            chart1.Series["Sb电源"].Points.AddXY(SbCount, CurSetVal_Sb / 1000.0);
                        }
                        SbOpenTimeCnt++;
                        if (SbOpenTimeCnt >= SbT1Execution[SbCurIndex])
                        {
                            //T1_Execution
                            string SbTExecution = $"Sb_T{SbCurIndex + 1}_Execution";
                            Control SbExecution = this.Controls.Find(SbTExecution, true).FirstOrDefault() as NumericUpDown;
                            if (SbExecution != null)
                            {
                                SbExecution.BackColor = Color.Red;
                            }
                            NewOnFlag_Sb = false;
                        }

                        SbCount++;
                        double SbEnd = SbCount;
                        if (SbEnd >= chart1.ChartAreas[0].AxisX.Maximum)
                        {
                            chart1.ChartAreas[0].AxisX.Maximum = (int)Math.Ceiling(SbEnd) + 10;
                        }
                        if (CurSetVal_Sb / 1000.0 >= chart1.ChartAreas[0].AxisY.Maximum)
                        {
                            chart1.ChartAreas[0].AxisY.Maximum = (int)Math.Ceiling(CurSetVal_Sb / 1000.0) + 1;
                        }

                        chart1.Series["Sb电源"].Points.AddXY(SbEnd, CurSetVal_Sb / 1000.0);

                        //T1_Cur
                        string TCur = $"Sb_T{SbCurIndex + 1}_Cur";
                        Control Cur = this.Controls.Find(TCur, true).FirstOrDefault() as NumericUpDown;
                        if (Cur != null)
                        {
                            Cur.BackColor = Color.Red;
                        }

                        //textBox_InFo.AppendText(" SbOpenTimeCnt  " + SbOpenTimeCnt + " CurSetVal_Sb  " + CurSetVal_Sb + " NewOnFlag_Sb " + NewOnFlag_Sb + "\r\n");


                    }
                    if (!NewOnFlag_Sb && SbStartFlag)
                    {
                        string SbLabelTime = $"Sb_T{SbCurIndex + 1}"; // 构建Label的名称  
                        Control SbLabelT = this.Controls.Find(SbLabelTime, true).FirstOrDefault(); // 查找Label控件  
                        if (SbLabelT is Label SbLabelTDisplay) // 检查找到的控件是否是Label  
                        {
                            SbLabelTDisplay.BackColor = Color.Gray;
                        }
                        else
                        {
                            textBox_InFo.AppendText("没找到LABEL  " + "\r\n");
                        }

                        if (SbCloseTimeCnt == 0)
                        {
                            chart1.Series["Sb电源"].Points.AddXY(SbCount, 0);
                        }
                        SbCloseTimeCnt++;
                        if (SbCloseTimeCnt >= SbT1TurnOff[SbCurIndex])
                        {
                            //T1_TurnOff
                            string TTurnOff = $"Sb_T{SbCurIndex + 1}_TurnOff";
                            Control TurnOff = this.Controls.Find(TTurnOff, true).FirstOrDefault() as NumericUpDown;
                            if (TurnOff != null)
                            {
                                TurnOff.BackColor = Color.Red;
                            }

                            NewOnFlag_Sb = true;
                            SbOpenTimeCnt = 0;
                            SbCloseTimeCnt = 0;
                            SbNsb++;
                            string SbLabelSbme = $"Sb_T{SbCurIndex + 1}_Nsb_Display"; // 构建Label的名称  
                            Control SbLabelControl = this.Controls.Find(SbLabelSbme, true).FirstOrDefault(); // 查找Label控件  
                            if (SbLabelControl is Label Sblabel) // 检查找到的控件是否是Label  
                            {
                                Sblabel.Text = SbNsb.ToString(); // 设置Label的Text属性  
                            }
                            else
                            {
                                textBox_InFo.AppendText("没找到LABEL  " + "\r\n");
                                // 如果找不到Label，您可能想要记录一个错误或执行其他操作  
                                Console.WriteLine($"Label '{SbLabelSbme}' not found.");
                            }
                            if (SbNsb == SbT1Nsb[SbCurIndex])
                            {
                                //T1_Nsb
                                string SbTNsb = $"Sb_T{SbCurIndex + 1}_Nsb";
                                Control Nsb = this.Controls.Find(SbTNsb, true).FirstOrDefault() as NumericUpDown;
                                if (Nsb != null)
                                {
                                    Nsb.BackColor = Color.Red;
                                }
                                SbCurIndex++;
                                SbNsb = 0;
                            }

                        }
                        SbCount++;
                        double SbEnd = SbCount;
                        if (SbEnd >= chart1.ChartAreas[0].AxisX.Maximum)
                        {
                            chart1.ChartAreas[0].AxisX.Maximum = (int)Math.Ceiling(SbEnd) + 10;
                        }
                        if (CurSetVal_Sb / 1000.0 >= chart1.ChartAreas[0].AxisY.Maximum)
                        {
                            chart1.ChartAreas[0].AxisY.Maximum = (int)Math.Ceiling(CurSetVal_Sb / 1000.0) + 1;
                        }

                        chart1.Series["Sb电源"].Points.AddXY(SbEnd, 0);


                        //textBox_InFo.AppendText(" SbCloseTimeCnt  " + SbCloseTimeCnt + " SbT1TurnOff[SbCurIndex]  " + SbT1TurnOff[SbCurIndex] + " SbNsb  " + SbNsb + " NewOnFlag_Sb " + NewOnFlag_Sb + "\r\n");
                    }
                    if (CurSetVal_Sb < 10)//没有电流，关闭电源
                    {
                        SbStartFlag = false;
                        NewOnFlag_Sb = false;
                    }
                }
                else
                {
                    CurSetVal_Sb = 0;
                    SbStartFlag = false;
                    NewOnFlag_Sb = false;
                }

                TimeCnt++;
                //if (TimeCnt % 2 == 0)
                //{
                //    if (ReadErrorFlag[0])
                //    {
                //        label_Pow_K.ChangeText("异常");
                //        label_Pow_K.ForeColor = Color.Red;
                //    }
                //    if (ReadErrorFlag[1])
                //    {
                //        label_Pow_Na.ChangeText("异常");
                //        label_Pow_Na.ForeColor = Color.Red;
                //    }
                //    if (ReadErrorFlag[2])
                //    {
                //        label_Pow_Cs.ChangeText("异常");
                //        label_Pow_Cs.ForeColor = Color.Red;
                //    }
                //    if (ReadErrorFlag[3])
                //    {
                //        label_Pow_Sb.ChangeText("异常");
                //        label_Pow_Sb.ForeColor = Color.Red;
                //    }
                //    //if (ReadErrorFlag[4])
                //    //{
                //    //    label_Pow_DY.ChangeText("异常");
                //    //    label_Pow_DY.ForeColor = Color.Red;
                //    //}

                //}
                //else
                //{
                //    if (ReadErrorFlag[0])
                //        label_Pow_K.ChangeText("");
                //    if (ReadErrorFlag[1])
                //        label_Pow_Na.ChangeText("");
                //    if (ReadErrorFlag[2])
                //        label_Pow_Cs.ChangeText("");
                //    if (ReadErrorFlag[3])
                //        label_Pow_Sb.ChangeText("");
                //    if (ReadErrorFlag[4])
                //        label_Pow_DY.ChangeText("");
                //}
            }
            //if (Xcount == 0)
            //{

            //}
            //switch (Xcount)
            //{
            //    case 1: chart1.Series["K电源"].Points.AddXY(0, 0.0); break;
            //    case 2: chart1.Series["K电源"].Points.AddXY(0, 1); break;
            //    case 3: chart1.Series["K电源"].Points.AddXY(10, 1); break;
            //    case 4: chart1.Series["K电源"].Points.AddXY(10, 2); break;
            //    case 5: chart1.Series["K电源"].Points.AddXY(20, 2); break;
            //    case 6: chart1.Series["K电源"].Points.AddXY(20, 3); break;
            //    case 7: chart1.Series["K电源"].Points.AddXY(30, 3); break;
            //    case 8: chart1.Series["K电源"].Points.AddXY(30, 4); break;
            //    case 9: chart1.Series["K电源"].Points.AddXY(40, 4); break;
            //    default:
            //        break;
            //}
            

            //if (Xcount==0)
            //{
            //    chart1.Series["K电源"].Points.Clear(); ; // 这将移除chart1中的所有序列  
            //}
            //else
            //{
            //    if (Xcount >= 11)
            //    {
            //        Xcount = 1;
            //        Ycount = 1;
            //    }
            //    chart1.Series["K电源"].Points.AddXY(Xcount, Ycount);
            //}
            //Xcount++;
            //Ycount++;


            // 移除旧的数据点以保持图表上的点数在一定范围内（可选）  
            //if (chart1.Series["K电源"].Points.Count > 10)
            //{
            //    chart1.Series["K电源"].Points.RemoveAt(0);
            //}

            // 刷新图表以显示更新  
            //chart1.Invalidate();



        }

        private void But_Save_Click(object sender, EventArgs e)
        {
            string[] ParameterValue = new string[4];
            //ParameterValue[0] = K_Current.Text;
            //ParameterValue[1] = Na_Current.Text;
            //ParameterValue[2] = Cs_Current.Text;
            //ParameterValue[3] = Sb_Current.Text;
            StreamWriter Value = new StreamWriter("./Parameter.dat", false, Encoding.Default);
            for (int i = 0; i < ParameterValue.Length; i++)
            {
                Value.WriteLine(ParameterValue[i]);
                Value.Flush();
            }
            Value.Close();
            Value = null;
            ParameterValue = null;
        }

        /// <summary>
        /// 电压设置函数
        /// </summary>
        /// <param name="VoltVal">设置电压数值</param>
        /// <param name="PoNo">通过电源地址码进行设置</param>
        void NewPower_Set_Volt(double VoltVal, int PoNo)
        {

            UInt16 CRCVal = 0;
            UInt16 Vdata = 0, Vdata1 = 0;
            byte[] VSetValue = new byte[4];

            if (VoltVal > Vmax_HSP)
            {
                switch (PoNo)
                {
                    case 1: Send_VCmdByte1 = null; break;
                    case 2: Send_VCmdByte2 = null; break;
                    case 3: Send_VCmdByte3 = null; break;
                    case 4: Send_VCmdByte4 = null; break;
                    case 5: Send_VCmdByte5 = null; break;
                    default: break;
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
                        Send_VCmdByte1[0] = 0x01;  //
                        Send_VCmdByte1[1] = 0x10; //功能码10表示预设寄存器
                        Send_VCmdByte1[2] = 0x00;//
                        Send_VCmdByte1[3] = 0x01;//电压地址
                        Send_VCmdByte1[4] = 0x00;//
                        Send_VCmdByte1[5] = 0x02;//2个寄存器
                        Send_VCmdByte1[6] = 0x04;//4个字节数据 
                        Send_VCmdByte1[7] = VSetValue[0];//4个数据
                        Send_VCmdByte1[8] = VSetValue[1];
                        Send_VCmdByte1[9] = VSetValue[2];
                        Send_VCmdByte1[10] = VSetValue[3];

                        CRCVal = CRC16(Send_VCmdByte1, 11);//CRC校验
                        Send_VCmdByte1[11] = (byte)(CRCVal % 256);//CRC校验码需交换高低字节
                        Send_VCmdByte1[12] = (byte)(CRCVal / 256);

                        break;
                    }
                case 2:
                    {
                        //设置电源的字节参数
                        Send_VCmdByte2[0] = 0x01;  //
                        Send_VCmdByte2[1] = 0x10; //功能码10表示预设寄存器
                        Send_VCmdByte2[2] = 0x00;//
                        Send_VCmdByte2[3] = 0x01;//电压地址
                        Send_VCmdByte2[4] = 0x00;//
                        Send_VCmdByte2[5] = 0x02;//2个寄存器
                        Send_VCmdByte2[6] = 0x04;//4个字节数据 
                        Send_VCmdByte2[7] = VSetValue[0];//4个数据
                        Send_VCmdByte2[8] = VSetValue[1];
                        Send_VCmdByte2[9] = VSetValue[2];
                        Send_VCmdByte2[10] = VSetValue[3];

                        CRCVal = CRC16(Send_VCmdByte2, 11);//CRC校验
                        Send_VCmdByte2[11] = (byte)(CRCVal % 256);//CRC校验码需交换高低字节
                        Send_VCmdByte2[12] = (byte)(CRCVal / 256);

                        break;
                    }
                case 3:
                    {
                        Send_VCmdByte3[0] = 0x01;  //
                        Send_VCmdByte3[1] = 0x10; //功能码10表示预设寄存器
                        Send_VCmdByte3[2] = 0x00;//
                        Send_VCmdByte3[3] = 0x01;//电压地址
                        Send_VCmdByte3[4] = 0x00;//
                        Send_VCmdByte3[5] = 0x02;//2个寄存器
                        Send_VCmdByte3[6] = 0x04;//4个字节数据 
                        Send_VCmdByte3[7] = VSetValue[0];//4个数据
                        Send_VCmdByte3[8] = VSetValue[1];
                        Send_VCmdByte3[9] = VSetValue[2];
                        Send_VCmdByte3[10] = VSetValue[3];

                        CRCVal = CRC16(Send_VCmdByte3, 11);//CRC校验
                        Send_VCmdByte3[11] = (byte)(CRCVal % 256);//CRC校验码需交换高低字节
                        Send_VCmdByte3[12] = (byte)(CRCVal / 256);

                        break;
                    }
                case 4:
                    {
                        Send_VCmdByte4[0] = 0x01;  //
                        Send_VCmdByte4[1] = 0x10; //功能码10表示预设寄存器
                        Send_VCmdByte4[2] = 0x00;//
                        Send_VCmdByte4[3] = 0x01;//电压地址
                        Send_VCmdByte4[4] = 0x00;//
                        Send_VCmdByte4[5] = 0x02;//2个寄存器
                        Send_VCmdByte4[6] = 0x04;//4个字节数据 
                        Send_VCmdByte4[7] = VSetValue[0];//4个数据
                        Send_VCmdByte4[8] = VSetValue[1];
                        Send_VCmdByte4[9] = VSetValue[2];
                        Send_VCmdByte4[10] = VSetValue[3];

                        CRCVal = CRC16(Send_VCmdByte4, 11);//CRC校验
                        Send_VCmdByte4[11] = (byte)(CRCVal % 256);//CRC校验码需交换高低字节
                        Send_VCmdByte4[12] = (byte)(CRCVal / 256);


                        break;
                    }
                case 5:
                    {
                        //设置电源的字节参数
                        Send_VCmdByte5[0] = 0x01;  //(byte)RD_ADDR.Value;
                        Send_VCmdByte5[1] = 0x06; //功能码06表示预设单个寄存器
                        Send_VCmdByte5[2] = 0x00;//2-3位是参数地址
                        Send_VCmdByte5[3] = 0x03;//00 03表示设定电压
                                                 //电源电压值数据data=(65535*Vx)/Vmax
                        Send_VCmdByte5[4] = (byte)(Vdata / 256);//电压值高8位 
                        Send_VCmdByte5[5] = (byte)(Vdata % 256);//电压低8位 

                        CRCVal = CRC16(Send_VCmdByte5, 6);//CRC校验
                        Send_VCmdByte5[6] = (byte)(CRCVal % 256);//CRC校验码需交换高低字节
                        Send_VCmdByte5[7] = (byte)(CRCVal / 256);

                        break;
                    }
                default: break;
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
                        Send_ReadCmdByte1[0] = 0x01;  //(byte)RD_ADDR.Value;
                        Send_ReadCmdByte1[1] = 0x03;
                        Send_ReadCmdByte1[2] = 0x00;
                        Send_ReadCmdByte1[3] = 0x1F;

                        Send_ReadCmdByte1[4] = 0x00;
                        Send_ReadCmdByte1[5] = 0x02;

                        CRCVal = CRC16(Send_ReadCmdByte1, 6);//CRC校验
                        Send_ReadCmdByte1[6] = (byte)(CRCVal % 256);//CRC校验码需交换高低字节
                        Send_ReadCmdByte1[7] = (byte)(CRCVal / 256);

                        break;
                    }
                case 2:
                    {
                        Send_ReadCmdByte2[0] = 0x01;  //(byte)RD_ADDR.Value;
                        Send_ReadCmdByte2[1] = 0x03;
                        Send_ReadCmdByte2[2] = 0x00;
                        Send_ReadCmdByte2[3] = 0x1F;

                        Send_ReadCmdByte2[4] = 0x00;
                        Send_ReadCmdByte2[5] = 0x02;

                        CRCVal = CRC16(Send_ReadCmdByte2, 6);//CRC校验
                        Send_ReadCmdByte2[6] = (byte)(CRCVal % 256);//CRC校验码需交换高低字节
                        Send_ReadCmdByte2[7] = (byte)(CRCVal / 256);
                        break;
                    }
                case 3:
                    {
                        Send_ReadCmdByte3[0] = 0x01;  //(byte)RD_ADDR.Value;
                        Send_ReadCmdByte3[1] = 0x03;
                        Send_ReadCmdByte3[2] = 0x00;
                        Send_ReadCmdByte3[3] = 0x1F;

                        Send_ReadCmdByte3[4] = 0x00;
                        Send_ReadCmdByte3[5] = 0x02;

                        CRCVal = CRC16(Send_ReadCmdByte3, 6);//CRC校验
                        Send_ReadCmdByte3[6] = (byte)(CRCVal % 256);//CRC校验码需交换高低字节
                        Send_ReadCmdByte3[7] = (byte)(CRCVal / 256);
                        break;
                    }
                case 4:
                    {
                        Send_ReadCmdByte4[0] = 0x01;  //(byte)RD_ADDR.Value;
                        Send_ReadCmdByte4[1] = 0x03;
                        Send_ReadCmdByte4[2] = 0x00;
                        Send_ReadCmdByte4[3] = 0x1F;

                        Send_ReadCmdByte4[4] = 0x00;
                        Send_ReadCmdByte4[5] = 0x02;

                        CRCVal = CRC16(Send_ReadCmdByte4, 6);//CRC校验
                        Send_ReadCmdByte4[6] = (byte)(CRCVal % 256);//CRC校验码需交换高低字节
                        Send_ReadCmdByte4[7] = (byte)(CRCVal / 256);
                        break;
                    }
                case 5:
                    {
                        Send_ReadCmdByte5[0] = 0x01;  //(byte)RD_ADDR.Value;
                        Send_ReadCmdByte5[1] = 0x03;
                        Send_ReadCmdByte5[2] = 0x00;
                        Send_ReadCmdByte5[3] = 0x01;

                        Send_ReadCmdByte5[4] = 0x00;
                        Send_ReadCmdByte5[5] = 0x01;

                        CRCVal = CRC16(Send_ReadCmdByte5, 6);//CRC校验
                        Send_ReadCmdByte5[6] = (byte)(CRCVal % 256);//CRC校验码需交换高低字节
                        Send_ReadCmdByte5[7] = (byte)(CRCVal / 256);
                        break;
                    }
                default: break;
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
                        Send_OFForONByte1[0] = 0x01;  //(byte)RD_ADDR.Value;
                        Send_OFForONByte1[1] = 0x06;
                        Send_OFForONByte1[2] = 0x00;
                        Send_OFForONByte1[3] = 0x1B;

                        Send_OFForONByte1[4] = 0x00;
                        Send_OFForONByte1[5] = (byte)OFForON;

                        CRCVal = CRC16(Send_OFForONByte1, 6);//CRC校验
                        Send_OFForONByte1[6] = (byte)(CRCVal % 256);//CRC校验码需交换高低字节
                        Send_OFForONByte1[7] = (byte)(CRCVal / 256);
                        break;
                    }
                case 2:
                    {
                        Send_OFForONByte2[0] = 0x01;  //(byte)RD_ADDR.Value;
                        Send_OFForONByte2[1] = 0x06;
                        Send_OFForONByte2[2] = 0x00;
                        Send_OFForONByte2[3] = 0x1B;

                        Send_OFForONByte2[4] = 0x00;
                        Send_OFForONByte2[5] = (byte)OFForON;

                        CRCVal = CRC16(Send_OFForONByte2, 6);//CRC校验
                        Send_OFForONByte2[6] = (byte)(CRCVal % 256);//CRC校验码需交换高低字节
                        Send_OFForONByte2[7] = (byte)(CRCVal / 256);
                        break;
                    }
                case 3:
                    {
                        Send_OFForONByte3[0] = 0x01;  //(byte)RD_ADDR.Value;
                        Send_OFForONByte3[1] = 0x06;
                        Send_OFForONByte3[2] = 0x00;
                        Send_OFForONByte3[3] = 0x1B;

                        Send_OFForONByte3[4] = 0x00;
                        Send_OFForONByte3[5] = (byte)OFForON;

                        CRCVal = CRC16(Send_OFForONByte3, 6);//CRC校验
                        Send_OFForONByte3[6] = (byte)(CRCVal % 256);//CRC校验码需交换高低字节
                        Send_OFForONByte3[7] = (byte)(CRCVal / 256);
                        break;
                    }
                case 4:
                    {
                        Send_OFForONByte4[0] = 0x01;  //(byte)RD_ADDR.Value;
                        Send_OFForONByte4[1] = 0x06;
                        Send_OFForONByte4[2] = 0x00;
                        Send_OFForONByte4[3] = 0x1B;

                        Send_OFForONByte4[4] = 0x00;
                        Send_OFForONByte4[5] = (byte)OFForON;

                        CRCVal = CRC16(Send_OFForONByte4, 6);//CRC校验
                        Send_OFForONByte4[6] = (byte)(CRCVal % 256);//CRC校验码需交换高低字节
                        Send_OFForONByte4[7] = (byte)(CRCVal / 256);
                        break;

                    }
                case 5:
                    {
                        Send_OFForONByte5[0] = 0x01;  //(byte)RD_ADDR.Value;
                        Send_OFForONByte5[1] = 0x06;
                        Send_OFForONByte5[2] = 0x00;
                        Send_OFForONByte5[3] = 0x05;

                        Send_OFForONByte5[4] = 0x00;
                        Send_OFForONByte5[5] = (byte)OFForON;

                        CRCVal = CRC16(Send_OFForONByte5, 6);//CRC校验
                        Send_OFForONByte5[6] = (byte)(CRCVal % 256);//CRC校验码需交换高低字节
                        Send_OFForONByte5[7] = (byte)(CRCVal / 256);
                        break;

                    }
                default: break;
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
            UInt16 Idata = 0, Idata1 = 0;
            byte[] ISetValue = new byte[4];
            if (CurVal > Imax_IV + 1000)//加1000余量，防止Send_ICmdByte清空，导致程序错误。
            {
                switch (PoNo)
                {
                    case 1: Send_ICmdByte1 = null; break;
                    case 2: Send_ICmdByte2 = null; break;
                    case 3: Send_ICmdByte3 = null; break;
                    case 4: Send_ICmdByte4 = null; break;
                    case 5: Send_ICmdByte5 = null; break;
                    default: break;
                }
            }
            else
            {
                Idata = (UInt16)((65535 * CurVal) / Imax_HSP);
                Idata1 = (UInt16)((65535 * CurVal) / Imax_PLD);
                ISetValue = BitConverter.GetBytes((float)(CurVal / 1000)).Reverse().ToArray();//将浮点数转换为字节数
            }
            switch (PoNo)
            {
                case 1:
                    {
                        //设置电源的字节参数
                        Send_ICmdByte1[0] = 0x01; //功能码10表示预设寄存器
                        Send_ICmdByte1[1] = 0x10;
                        Send_ICmdByte1[2] = 0x00;
                        Send_ICmdByte1[3] = 0x03;//电流地址

                        Send_ICmdByte1[4] = 0x00;//
                        Send_ICmdByte1[5] = 0x02;//2个寄存器
                        Send_ICmdByte1[6] = 0x04;//4个字节数据 
                        Send_ICmdByte1[7] = ISetValue[0];//4个数据
                        Send_ICmdByte1[8] = ISetValue[1];
                        Send_ICmdByte1[9] = ISetValue[2];
                        Send_ICmdByte1[10] = ISetValue[3];
                        CRCVal = CRC16(Send_ICmdByte1, 11);//CRC校验
                        Send_ICmdByte1[11] = (byte)(CRCVal % 256);//CRC校验码需交换高低字节
                        Send_ICmdByte1[12] = (byte)(CRCVal / 256);
                        break;
                    }
                case 2:
                    {
                        //设置电源的字节参数
                        Send_ICmdByte2[0] = 0x01; //功能码10表示预设寄存器
                        Send_ICmdByte2[1] = 0x10;
                        Send_ICmdByte2[2] = 0x00;
                        Send_ICmdByte2[3] = 0x03;//电流地址

                        Send_ICmdByte2[4] = 0x00;//
                        Send_ICmdByte2[5] = 0x02;//2个寄存器
                        Send_ICmdByte2[6] = 0x04;//4个字节数据 
                        Send_ICmdByte2[7] = ISetValue[0];//4个数据
                        Send_ICmdByte2[8] = ISetValue[1];
                        Send_ICmdByte2[9] = ISetValue[2];
                        Send_ICmdByte2[10] = ISetValue[3];
                        CRCVal = CRC16(Send_ICmdByte2, 11);//CRC校验
                        Send_ICmdByte2[11] = (byte)(CRCVal % 256);//CRC校验码需交换高低字节
                        Send_ICmdByte2[12] = (byte)(CRCVal / 256);
                        break;
                    }
                case 3:
                    {
                        //设置电源的字节参数
                        Send_ICmdByte3[0] = 0x01; //功能码10表示预设寄存器
                        Send_ICmdByte3[1] = 0x10;
                        Send_ICmdByte3[2] = 0x00;
                        Send_ICmdByte3[3] = 0x03;//电流地址

                        Send_ICmdByte3[4] = 0x00;//
                        Send_ICmdByte3[5] = 0x02;//2个寄存器
                        Send_ICmdByte3[6] = 0x04;//4个字节数据 
                        Send_ICmdByte3[7] = ISetValue[0];//4个数据
                        Send_ICmdByte3[8] = ISetValue[1];
                        Send_ICmdByte3[9] = ISetValue[2];
                        Send_ICmdByte3[10] = ISetValue[3];
                        CRCVal = CRC16(Send_ICmdByte3, 11);//CRC校验
                        Send_ICmdByte3[11] = (byte)(CRCVal % 256);//CRC校验码需交换高低字节
                        Send_ICmdByte3[12] = (byte)(CRCVal / 256);
                        break;
                    }
                case 4:
                    {
                        //设置电源的字节参数
                        Send_ICmdByte4[0] = 0x01; //功能码10表示预设寄存器
                        Send_ICmdByte4[1] = 0x10;
                        Send_ICmdByte4[2] = 0x00;
                        Send_ICmdByte4[3] = 0x03;//电流地址

                        Send_ICmdByte4[4] = 0x00;//
                        Send_ICmdByte4[5] = 0x02;//2个寄存器
                        Send_ICmdByte4[6] = 0x04;//4个字节数据 
                        Send_ICmdByte4[7] = ISetValue[0];//4个数据
                        Send_ICmdByte4[8] = ISetValue[1];
                        Send_ICmdByte4[9] = ISetValue[2];
                        Send_ICmdByte4[10] = ISetValue[3];
                        CRCVal = CRC16(Send_ICmdByte4, 11);//CRC校验
                        Send_ICmdByte4[11] = (byte)(CRCVal % 256);//CRC校验码需交换高低字节
                        Send_ICmdByte4[12] = (byte)(CRCVal / 256);
                        break;
                    }
                case 5:
                    {
                        //设置电源的字节参数
                        Send_ICmdByte5[0] = 0x01;  //(byte)RD_ADDR.Value;
                        Send_ICmdByte5[1] = 0x06; //功能码06表示预设单个寄存器
                        Send_ICmdByte5[2] = 0x00;// 
                        Send_ICmdByte5[3] = 0x04;//00 04表示设定电流

                        Send_ICmdByte5[4] = (byte)(Idata / 256);//寄存器数值的高8位，取设定电压值X100后的高8位 
                        Send_ICmdByte5[5] = (byte)(Idata % 256);//寄存器数值的低8位，取设定电压值X100后的低8位 

                        CRCVal = CRC16(Send_ICmdByte5, 6);//CRC校验
                        Send_ICmdByte5[6] = (byte)(CRCVal % 256);//CRC校验码需交换高低字节
                        Send_ICmdByte5[7] = (byte)(CRCVal / 256);
                        break;
                    }
                default: break;
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
            int a, b;
            a = CRC_Reg;
            b = 0;
            for (int i = 0; i < ByteLen; i++)
            {
                b = InByte[i];
                a = a ^ b;   //做异或运算
                             //右移一位，并检查最低位
                for (int j = 0; j < 8; j++)
                {
                    if (a % 2 == 1)   //如果最低位为1，则用右移（除以2）后的数值与0xA001进行异或运算
                    {
                        a /= 2;
                        a = a ^ 0xA001;
                    }
                    else
                        a /= 2;  //如果最低位为0则右移1位（除以2）后结束本次移位，跳入下次移位                          
                }
            }
            //交换高低字节运算放在使用时进行
            CRC_Reg = (UInt16)a;//(a/256+(a%256)*256);
            return CRC_Reg;
        }
        /// <summary>
        /// 延时函数
        /// </summary>
        /// <param name="milliSecond"></param>
        public static void Delay(int milliSecond)
        {
            int start = Environment.TickCount;
            while (Math.Abs(Environment.TickCount - start) < milliSecond)//毫秒
            {
                Application.DoEvents();//可执行某无聊的操作
            }
        }



    }
    static class F_Tool
    {
        /// <summary>
        /// 以委托的方式更改控件文本
        /// </summary>
        /// <param name="c">需要更改的控件</param>
        /// <param name="val">更改的文本内容</param>
        public static void ChangeText(this Control c, string val)
        {
            if (c.InvokeRequired)
            {
                Action<string> d = (v) => c.Text = v;
                c.Invoke(d, val);
            }
            else c.Text = val;
        }
    }
}
