using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace PowerSet.Main
{
    internal class CycleController
    {
        public CycleController() { }

        public CycleController(DataTable table, string flag)
        {
            ProcessFlag = flag;
            if (table.Columns.Count != 6 || table.Columns[0].ColumnName != "周期")
                throw new Exception("传入的数据表不为周期表！");

            EndCycle = table.Rows.Count;
            CurrentCycle = Math.Max(1, StartCycle);

            for (int i = 0; i < table.Rows.Count; i++)
            {
                var row = table.Rows[i];
                var c = new Cycle()
                {
                    Index = i,
                    Value = Convert.ToInt32(row[1]),
                    WorkTime = Convert.ToInt32(row[2]),
                    SleepTime = Convert.ToInt32(row[3]),
                    Count = Convert.ToInt32(row[4]),
                    Controller = this,
                    ProcessFlag = flag
                };

                Cycles.Add(c);
            }
        }

        public List<Cycle> Cycles { get; set; } = new List<Cycle>();

        public int StartCycle { get; set; }
        public int EndCycle { get; set; }

        public int CurrentCycle { get; set; }

        public string ProcessFlag { get; set; }

        private bool _isFinish = false;
        public bool IsFinish
        {
            get => CurrentCycle + 1 >= EndCycle | false;
            set { _isFinish = value; }
        }

        public event Action CycleStart;
        public event Action EnterNextCycle;
        public event Action AllCycleFinish;

        public Cycle GetCurrentCycle()
        {
            return Cycles[CurrentCycle - 1];
        }

        public bool TryEnterNextCycle()
        {
            if (GetCurrentCycle().IsFinish)
            {
                CurrentCycle++;
                GetCurrentCycle().Start();
                EnterNextCycle?.Invoke();
                Debug.WriteLine(CurrentCycle);
                return true;
            }
            return false;
        }

        public void Start()
        {
            if (IsFinish)
                return;
            GetCurrentCycle().Start();
            CycleStart?.Invoke();
        }

        public void Finish()
        {
            AllCycleFinish?.Invoke();
        }

        public void FinishAll()
        {
            foreach (var cycle in Cycles)
            {
                cycle.IsFinish = true;
            }
            IsFinish = true;
        }
    }
}
