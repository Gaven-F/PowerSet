using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace PowerSet.Main
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// 折线图高度占比
        /// </summary>
        private readonly double ChartHightPct = 25;

        private readonly List<string> ReadOnlyTableCol = new List<string> { "周期", "执行次数", "实际电流" };
        private readonly List<string> Prefix = new List<string> { "K", "N", "C", "S" };

        private readonly Dictionary<string, ParamSet> ParamSets = new Dictionary<string, ParamSet>();
        private readonly Dictionary<string, DataTable> DataTables = new Dictionary<string, DataTable>();

        public MainForm()
        {
            InitializeComponent();
            // 设置
            SplitLayout.SplitterDistance = (int)(SplitLayout.Size.Height * (ChartHightPct / 100));

            Prefix.ForEach(f => ParamSets.Add(f, new ParamSet()));
            InitTableData();
        }

        private void InitTableData()
        {
            var baseDataTable = new DataTable();
            var t = typeof(ParamProcessSet);
            var ps = t.GetProperties();

            for (int i = 0; i < 12; i++)
            {
                baseDataTable.Rows.Add();
            }

            foreach (var property in ps)
            {
                var colset = property.GetCustomAttributes(typeof(ColSet), false) as ColSet[];
                if (colset.Length > 0)
                {
                    baseDataTable.Columns.Add(colset[0].Name);
                    foreach (DataRow row in baseDataTable.Rows)
                    {
                        row[colset[0].Name] = colset[0].DefaultVal;
                    }
                }
            }

            Prefix.ForEach(f => DataTables.Add(f, baseDataTable.Copy()));

            var allParamSetTableName = Prefix.Select(it => it + "ParamSetTable").ToList();
            var allParamSetTable = allParamSetTableName.Select(name =>
            {
                var table = Controls.Find(name,true);
                return table.Length > 0 ? table[0] as DataGridView : null;
            }).ToList();
            Console.WriteLine(Controls);

            allParamSetTable.ForEach(table =>
            {
                if (table == null) return;
                table.DataSource = DataTables[table.Name.First().ToString()];
                foreach (DataGridViewTextBoxColumn col in table.Columns)
                {
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    if (ReadOnlyTableCol.Contains(col.Name, true)) col.ReadOnly = true;
                }

                for (int i = 0; i < table.RowCount; i++)
                {
                    KParamSetTable.Rows[i].Cells["周期"].Value = $"周期{i + 1: 000}";
                }
            });
        }

        private void CloseSysEvent(object sender, EventArgs e) => Close();

        private void HiddenChartClick(object sender, EventArgs e) =>
            SplitLayout.Panel1Collapsed = !SplitLayout.Panel1Collapsed;

        private void KAddProcessBtn_Click(object sender, EventArgs e)
        {
            (KParamSetTable.DataSource as DataTable).Rows.Add($"周期{KParamSetTable.RowCount + 1: 000}", "1000", "1000", "0", "0", "0", "0");
        }
    }
}
