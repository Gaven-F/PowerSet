using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.ModelConfiguration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        public int TotalTime { get; set; } = 0;

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

        public CycleController Controller { get; set; }

        private bool _isFinish = false;

        /// <summary>
        /// 周期是否完成
        /// </summary>
        public bool IsFinish
        {
            get => TotalTime > (WorkTime + SleepTime) * Count | _isFinish;
            set { _isFinish = value; }
        }

        private double _value = 0;
        public double Value
        {
            get => IsWork ? _value : 0;
            set { _value = value; }
        }

        public string ProcessFlag { get; set; }

        public int GetCurrentCount()
        {
            return TotalTime / (WorkTime + SleepTime);
        }

        public event Action<CycleExecuteArg> Execute;
        public event Action<CycleExecuteArg> Executed;

        public void Start()
        {
            var startTime = DateTime.Now;
            Task.Run(() =>
            {
                while (!IsFinish)
                {
                    if ((DateTime.Now - startTime).TotalSeconds < TotalTime)
                    {
                        Task.Delay(10).Wait();
                        continue;
                    }

                    Debug.WriteLine($"{Index}{ProcessFlag}: {TotalTime} {IsFinish}");
                    Execute?.Invoke(
                        new CycleExecuteArg()
                        {
                            TotalTime = TotalTime,
                            Flag = ProcessFlag,
                            Count = Count,
                            Value = Value,
                            Index = Index,
                            IsFinish = IsFinish,
                        }
                    );

                    TotalTime++;
                }

                Executed?.Invoke(
                    new CycleExecuteArg()
                    {
                        TotalTime = TotalTime,
                        Flag = ProcessFlag,
                        Count = Count,
                        Value = Value,
                        Index = Index,
                        IsFinish = IsFinish,
                    }
                );

                if (!Controller.IsFinish)
                {
                    Controller.TryEnterNextCycle();
                }
                else
                {
                    Controller.Finish();
                }
            });
        }
    }

    internal class CycleExecuteArg
    {
        public double Value { get; set; }
        public string Flag { get; set; }

        /// <summary>
        /// 周期执行次数
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 执行总时间
        /// </summary>
        public int TotalTime { get; set; }

        /// <summary>
        /// 周期
        /// </summary>
        public int Index { get; set; }

        public bool IsFinish { get; set; }
    }
}
