using System;
using System.Collections.Generic;
using System.Data;

namespace PowerSet.Main
{
    internal class CycleController
    {
        private readonly char[] split = new char[] { ' ' };

        /// <summary>
        /// 执行总次数
        /// </summary>
        public int TotalCnt { get; set; }
        public int Current { get; set; }
        public string Flag { get; set; }
        public List<Cycle> Cycles { get; set; } = new List<Cycle>();

        public event Action Finish;

		public void Start()
        {
            Cycles[Current].Start();
        }

        public CycleController(DataTable table, string flag)
        {
            Flag = flag;
            if (table.Columns.Count != 6 || table.Columns[0].ColumnName != "周期")
            {
                throw new Exception("传入的数据表不为周期表！");
            }

            TotalCnt = table.Rows.Count;

            for (int i = 0; i < table.Rows.Count; i++)
            {
                var row = table.Rows[i];
                var index =
                    Convert.ToInt32(
                        table
                            .Rows[i][0]
                            .ToString()
                            .Split(split, StringSplitOptions.RemoveEmptyEntries)[1]
                    ) - 1;

                var c = new Cycle()
                {
                    Index = index,
                    Value = Convert.ToInt32(row[1]),
                    WorkTime = Convert.ToInt32(row[2]),
                    SleepTime = Convert.ToInt32(row[3]),
                    Count = Convert.ToInt32(row[4]),
                    Flag = flag
                };

                c.Finish += C_Finish;

                Cycles.Add(c);
            }
        }

		public void FinishAll()
		{
            Cycles[Current].Close();
            TotalCnt = -1;
		}

		private void C_Finish(Cycle.CycleExecuteArg arg)
        {
            if (Current < TotalCnt)
            {
                Current++;
                Cycles[Current].Start();
            }
            else
            {
                Finish?.Invoke();
            }
        }
    }
}
