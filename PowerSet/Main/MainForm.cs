﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using PowerSet.Attributes;
using C = PowerSet.Main.ConstData;

namespace PowerSet.Models
{
    public partial class MainForm : Form
    {
        private readonly List<string> ReadOnlyTableCol = new List<string> { "周期", "执行次数", "实际电流" };

        private readonly List<string> Prefix = new List<string> { C.K, C.N, C.C, C.S };

        /// <summary>
        /// 某项是否启动
        /// </summary>
        private readonly Dictionary<string, bool> ProcessStart = new Dictionary<string, bool>()
        {
            { C.K, false },
            { C.N, false },
            { C.C, false },
            { C.S, false }
        };

        private readonly Dictionary<string, ParamSet> ParamSets =
            new Dictionary<string, ParamSet>();

        private readonly Dictionary<string, DataTable> DataTables =
            new Dictionary<string, DataTable>();

        private readonly Dictionary<string, List<RealIData>> Data =
            new Dictionary<string, List<RealIData>>();

        private readonly Dictionary<string, Control> Uis = new Dictionary<string, Control>();

        private readonly DataTable ChartData = new DataTable();

        private System.Threading.Timer MainTimer;

        private readonly Random R = new Random();

        private long Cnt = 0;

        private readonly Dictionary<string, double> RealVals = new Dictionary<string, double>();

        public MainForm()
        {
            InitializeComponent();
            InitUIMap();
            InitTableData();
            InitChart();
            EventBind();
            StartProcess();
        }

        /// <summary>
        /// 启动进程/计时器
        /// </summary>
        private void StartProcess()
        {
            // Timer用于更新数据以及部分周期视图更新
            MainTimer = new System.Threading.Timer(MainTimerAction, null, 0, 1000);
        }

        /// <summary>
        /// 事件绑定
        /// </summary>
        private void EventBind()
        {
            XAxisMargin_Val.ValueChanged += ValChanged;
            YAxisMargin_Val.ValueChanged += ValChanged;

            Prefix.ForEach(p =>
            {
                if (Uis.TryGetValue(p + C.UI_I_LAB, out var c) && c is Label)
                    c.Click += LabClick;
            });
        }

        /// <summary>
        /// 标签点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// 用户需要点击标签决定打开相应的进程
        /// </remarks>
        private void LabClick(object sender, EventArgs e)
        {
            if (sender is Label s && s.Name.Contains(C.UI_I_LAB))
            {
                Prefix.ForEach(p =>
                {
                    if (s.Name.StartsWith(p))
                        ProcessStart[p] = !ProcessStart[p];

                    if (!Data.TryGetValue(p, out var data) && ProcessStart[p])
                        Data[p] = new List<RealIData>();
                });
            }
        }

        /// <summary>
        /// Timer更新函数
        /// </summary>
        /// <param name="state">可传参</param>
        private void MainTimerAction(object state)
        {
            // TODO: 实际取值逻辑
            GetRealData();

            // TODO: 缓存所有实时数据到内存中
            Prefix.ForEach(p =>
            {
                if (!Data.TryGetValue(p, out var data))
                    return;

                data.Add(new RealIData() { XVal = Cnt, YVal = RealVals[p] });
            });

            Prefix.ForEach(p =>
            {
                if (ProcessStart[p])
                {
                    var c = Uis[p + C.UI_I_VAL_LAB];
                    if (c != null && c is Label)
                    {
                        if (c.InvokeRequired)
                        {
                            c.BeginInvoke(
                                new Action(() =>
                                {
                                    c.Text = RealVals[p].ToString("0.00mA");
                                })
                            );
                        }
                        else
                        {
                            c.Text = RealVals[p].ToString("0.00mA");
                        }
                    }
                }
            });

            AddDataToChart();
            Prefix.ForEach(p => { });
            Cnt++;
        }

        /// <summary>
        /// 添加数据到图表
        /// </summary>
        private void AddDataToChart()
        {
            ChartData.Rows.Add(Cnt, RealVals[C.K], RealVals[C.N], RealVals[C.C], RealVals[C.S]);
        }

        /// <summary>
        /// 获取实时数据
        /// </summary>
        private void GetRealData()
        {
            Prefix.ForEach(p => RealVals[p] = R.NextDouble() * 50);
        }

        /// <summary>
        /// 初始化图表
        /// </summary>
        private void InitChart()
        {
            XAxisMargin_Val.Value = 5;
            YAxisMargin_Val.Value = .5M;
            #region 设置图表高度
            ChartHight_Val.Value = 25;
            SplitLayout.SplitterDistance = (int)(
                SplitLayout.Size.Height * (Convert.ToDouble(ChartHight_Val.Value) / 100.0)
            );
            #endregion

            #region 图表初始化及数据绑定
            var chartArea = new ChartArea(C.CHART_BASE_CHARTAREA);
            Prefix.ForEach(p =>
            {
                Chart.Series.Add(
                    new Series(p)
                    {
                        ChartType = SeriesChartType.StepLine,
                        Legend = C.BASE_LEGEND,
                        XValueMember = "X",
                        YValueMembers = p + "Y",
                        BorderWidth = 4,
                    }
                );
            });
            Chart.Legends.Add(new Legend(C.BASE_LEGEND));
            Chart.ChartAreas.Add(chartArea);
            chartArea.AxisX.Interval = Convert.ToDouble(XAxisMargin_Val.Value);
            chartArea.AxisY.Interval = Convert.ToDouble(YAxisMargin_Val.Value);

            chartArea.AxisX.Minimum = 1;

            chartArea.AxisX.LabelStyle.Format = "0S";
            chartArea.AxisY.LabelStyle.Format = "0A";

            #endregion

            #region 初始化图表数据表
            ChartData.Columns.Add("X");
            Prefix.ForEach(p =>
            {
                ChartData.Columns.Add($"{p}Y");
            });
            //ChartData.Rows.Add(0, 0, 0, 0, 0);
            ChartData.RowChanged += DataChanged;
            #endregion
        }

        /// <summary>
        /// 数据更新事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataChanged(object sender, DataRowChangeEventArgs e)
        {
            if (Chart.InvokeRequired)
            {
                Chart.BeginInvoke(
                    new Action(() =>
                    {
                        Chart.DataSource = ChartData;
                        Chart.DataBind();
                    })
                );
            }
            else
            {
                Chart.DataSource = ChartData;
                Chart.DataBind();
            }
        }

        /// <summary>
        /// 初始化数据表
        /// </summary>
        private void InitTableData()
        {
            Prefix.ForEach(f => ParamSets.Add(f, new ParamSet()));

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


            var allParamSetTableName = Prefix.Select(it => it + C.UI_PARAM_SET_TABLE).ToList();
            var allParamSetTable = allParamSetTableName
                .Select(name =>
                    Uis.TryGetValue(name, out var table) && table is DataGridView
                        ? table as DataGridView
                        : null
                )
                .ToList();

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

        /// <summary>
        /// 初始化UI表
        /// </summary>
        private void InitUIMap()
        {
            Prefix.ForEach(p =>
            {
                var uiNames = typeof(C)
                    .GetFields(BindingFlags.Static | BindingFlags.NonPublic)
                    .Where(it => it.Name.StartsWith("UI"))
                    .Select(it => it.GetValue(null))
                    .ToList();

                uiNames.ForEach(uiName =>
                {
                    var c = Controls.Find(p + uiName, true);
                    if (c.Length > 0)
                        Uis.Add(c[0].Name, c[0]);
                });
            });
        }

        private void CloseSysEvent(object sender, EventArgs e)
        {
            MainTimer.Dispose();
            Close();
        }

        private void HiddenChartClick(object sender, EventArgs e) =>
            SplitLayout.Panel1Collapsed = !SplitLayout.Panel1Collapsed;

        // TODO: 统一管理
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

        private void ChartHight_Val_Changed(object sender, EventArgs e)
        {
            SplitLayout.SplitterDistance = (int)(
                SplitLayout.Size.Height * (ChartHight_Val.Value / 100)
            );
        }

        private void ValChanged(object sender, EventArgs e)
        {
            if (sender is NumericUpDown s && s.Name.Contains("AxisMargin_Val"))
            {
                var val = Convert.ToDouble(s.Value);
                switch (s.Name)
                {
                    case "XAxisMargin_Val":
                        Chart.ChartAreas[C.CHART_BASE_CHARTAREA].AxisX.Interval = val;
                        break;

                    case "YAxisMargin_Val":
                        Chart.ChartAreas[C.CHART_BASE_CHARTAREA].AxisY.Interval = val;
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
