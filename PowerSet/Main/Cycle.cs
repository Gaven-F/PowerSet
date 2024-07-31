using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSet.Main
{

    internal class Cycle
    {
        /// <summary>
        /// 周期序号
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 需要执行的次数
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 当前周期执行总时间
        /// </summary>
        public int TotalTime { get; set; }

        /// <summary>
        /// 周期需要的工作时间
        /// </summary>
        public int WorkTime { get; set; }

        /// <summary>
        /// 周期需要的休眠时间
        /// </summary>
        public int SleepTime { get; set; }

        /// <summary>
        /// 是否属于工作状态
        /// </summary>
        public bool IsWork
        {
            get => TotalTime % (WorkTime + SleepTime) - WorkTime < 0;
        }

        /// <summary>
        /// 周期是否完成
        /// </summary>
        public bool IsFinish
        {
            get => TotalTime >= (WorkTime + SleepTime) * Count;
        }

        public double Value { get; set; }

        public int GetCurrentCount()
        {
            return TotalTime / (WorkTime + SleepTime);
        }
    }
}
