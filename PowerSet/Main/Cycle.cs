using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace PowerSet.Main
{
    internal class Cycle
    {
        /*
         * 执行时间、休眠时间、执行次数
         */
        public int Index { get; set; }
        public int WorkTime { get; set; }
        public int SleepTime { get; set; }
        public int Count { get; set; }
        public int CurrentTime { get; set; }
        public int Value { get; set; }
        public string Flag { get; set; }
        public bool CloseFlag { get; set; } = false;
        public bool FinishFlag { get; set; } = false;
        private Timer _timer;
        private long VSecond = 0;
        private DateTime Time = DateTime.Now;

        public event Action<CycleExecuteArg> WorkExecute;
        public event Action<CycleExecuteArg> SleepExecute;
        public event Action<CycleExecuteArg> Finish;

        public void Start()
        {
            Time = DateTime.Now;
            _timer = new Timer(Execute, null, 0, 1);
        }

        private void Execute(object state)
        {
            if ((DateTime.Now - Time).TotalSeconds < VSecond)
                return;

            VSecond++;

            if (CurrentTime / (WorkTime + SleepTime) < Count && !CloseFlag)
            {
                if ((CurrentTime % (WorkTime + SleepTime) - WorkTime) < 0)
                {
                    WorkExecute?.Invoke(
                        new CycleExecuteArg()
                        {
                            Flag = Flag,
                            Value = Value,
                            Index = Index,
                            IsFinish = false,
                            TotalCount = CurrentTime / (WorkTime + SleepTime) + 1,
                            TotalTime = CurrentTime
                        }
                    );
                }
                else
                {
                    SleepExecute?.Invoke(
                        new CycleExecuteArg()
                        {
                            Flag = Flag,
                            Value = Value,
                            Index = Index,
                            IsFinish = false,
                            TotalCount = CurrentTime / (WorkTime + SleepTime) + 1,
                            TotalTime = CurrentTime
                        }
                    );
                }
                CurrentTime++;
            }
            else
            {
                if (!FinishFlag)
                {
                    FinishFlag = true;
                    Finish?.Invoke(
                        new CycleExecuteArg()
                        {
                            Flag = Flag,
                            Value = Value,
                            Index = Index,
                            IsFinish = true,
                            TotalCount = CurrentTime / (WorkTime + SleepTime) + 1,
                            TotalTime = CurrentTime
                        }
                    );
                }
                try
                {
                    _timer.Change(Timeout.Infinite, Timeout.Infinite); // 停止计时器
                    _timer.Dispose(); // 释放计时器资源
                }
                catch (Exception) { }
            }
        }

        /// <summary>
        /// 强制关闭，非正常情况请勿调用，无法触发Finish事件
        /// </summary>
        internal void Close()
        {
            CloseFlag = true;
            if (_timer != null)
            {
                try
                {
                    _timer.Change(Timeout.Infinite, Timeout.Infinite); // 停止计时器
                    _timer.Dispose(); // 释放计时器资源
                }
                catch (Exception) { }
            }
        }

        internal class CycleExecuteArg
        {
            public double Value { get; set; }
            public string Flag { get; set; }
            public int TotalTime { get; set; }
            public int TotalCount { get; set; }
            public int Index { get; set; }
            public bool IsFinish { get; set; }
        }

        ///// <summary>
        ///// 周期序号
        ///// </summary>
        //public int Index { get; set; }

        ///// <summary>
        ///// 需要执行的次数
        ///// </summary>
        //public int Count { get; set; }
        //public int CurrentCount
        //{
        //	get => (TotalTime - 1) / (WorkTime + SleepTime) + 1;
        //}

        ///// <summary>
        ///// 当前周期执行总时间
        ///// </summary>
        //public int TotalTime { get; set; } = 0;

        ///// <summary>
        ///// 周期需要的工作时间
        ///// </summary>
        //public int WorkTime { get; set; }

        ///// <summary>
        ///// 周期需要的休眠时间
        ///// </summary>
        //public int SleepTime { get; set; }

        ///// <summary>
        ///// 是否属于工作状态
        ///// </summary>
        //public bool IsWork
        //{
        //	get => TotalTime % (WorkTime + SleepTime) - WorkTime < 0;
        //}

        //public CycleController Controller { get; set; }

        //    private bool _isFinish = false;

        //    /// <summary>
        //    /// 周期是否完成
        //    /// </summary>
        //    public bool IsFinish
        //    {
        //        get => TotalTime > (WorkTime + SleepTime) * Count | _isFinish;
        //        set { _isFinish = value; }
        //    }

        //    private double _value = 0;
        //    public double Value
        //    {
        //        get => IsWork ? _value : 0;
        //        set { _value = value; }
        //    }

        //    public string ProcessFlag { get; set; }

        //    public int GetCurrentCount()
        //    {
        //        return TotalTime / (WorkTime + SleepTime);
        //    }

        //    public event Action<CycleExecuteArg> Execute;
        //    public event Action<CycleExecuteArg> Executed;

        //    public void Start()
        //    {
        //        var startTime = DateTime.Now;
        //        Task.Run(() =>
        //        {
        //            while (!IsFinish)
        //            {
        //                if ((DateTime.Now - startTime).TotalSeconds < TotalTime)
        //                {
        //                    Task.Delay(10).Wait();
        //                    continue;
        //                }

        //                Debug.WriteLine($"{Index}{ProcessFlag}: {TotalTime} {IsFinish}");
        //                Execute?.Invoke(
        //                    new CycleExecuteArg()
        //                    {
        //                        TotalTime = TotalTime,
        //                        Flag = ProcessFlag,
        //                        CurrentCount = CurrentCount,
        //                        Value = Value,
        //                        Index = Index,
        //                        IsFinish = IsFinish,
        //                    }
        //                );

        //                TotalTime++;
        //            }

        //            Executed?.Invoke(
        //                new CycleExecuteArg()
        //                {
        //                    TotalTime = TotalTime,
        //                    Flag = ProcessFlag,
        //                    CurrentCount = CurrentCount,
        //                    Value = Value,
        //                    Index = Index,
        //                    IsFinish = IsFinish,
        //                }
        //            );

        //            if (!Controller.IsFinish)
        //            {
        //                Controller.TryEnterNextCycle();
        //            }
        //            else
        //            {
        //                Controller.Finish();
        //            }
        //        });
        //    }
        //}
    }
}
