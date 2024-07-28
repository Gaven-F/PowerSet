using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace PowerSet.Attribute
{
    public partial class MainForm : Form
    {
        private readonly List<string> ReadOnlyTableCol = new List<string> { "周期", "执行次数", "实际电流" };
        private readonly List<string> Prefix = new List<string> { "K", "N", "C", "S" };

        private readonly Dictionary<string, ParamSet> ParamSets =
            new Dictionary<string, ParamSet>();
        private readonly Dictionary<string, DataTable> DataTables =
            new Dictionary<string, DataTable>();

        public MainForm()
        {
            InitializeComponent();
            ChartHight_Val.Value = 25;
            // 设置
            SplitLayout.SplitterDistance = (int)(
                SplitLayout.Size.Height * (Convert.ToDouble(ChartHight_Val.Value) / 100.0)
            );

            Prefix.ForEach(f => ParamSets.Add(f, new ParamSet()));
            InitTableData();
        }

        private void InitTableData()
        {
            var baseDataTable = new DataTable();
            var t = typeof(ParamProcessSet);
            var ps = t.GetProperties();

            #region 默认数据表列创建
            for (int i = 0; i < 12; i++)
            {
                baseDataTable.Rows.Add();
            }

            foreach (var property in ps)
            {
                var colset =
                    property.GetCustomAttributes(typeof(ColSetAttribute), false)
                    as ColSetAttribute[];
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
            #endregion


            var allParamSetTableName = Prefix.Select(it => it + "ParamSetTable").ToList();
            var allParamSetTable = allParamSetTableName
                .Select(name =>
                {
                    var table = Controls.Find(name, true);
                    return table.Length > 0 ? table[0] as DataGridView : null;
                })
                .ToList();
            Console.WriteLine(Controls);

            allParamSetTable.ForEach(table =>
            {
                if (table == null)
                    return;
                table.DataSource = DataTables[table.Name.First().ToString()];
                table.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                table.AllowUserToResizeRows = false;
                foreach (DataGridViewTextBoxColumn col in table.Columns)
                {
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    if (ReadOnlyTableCol.Contains(col.Name, true))
                        col.ReadOnly = true;
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
            (KParamSetTable.DataSource as DataTable).Rows.Add(
                $"周期{KParamSetTable.RowCount + 1: 000}",
                "1000",
                "1000",
                "0",
                "0",
                "0",
                "0"
            );
        }

        private void ChartHight_Val_ValueChanged(object sender, EventArgs e)
        {
            SplitLayout.SplitterDistance = (int)(
                SplitLayout.Size.Height * (ChartHight_Val.Value / 100)
            );
        }
    }
}
