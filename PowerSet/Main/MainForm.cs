using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using PowerSet.Attributes;
using PowerSet.Utils;
using SqlSugar;
using SqlSugar.Extensions;
using C = PowerSet.Main.ConstData;

namespace PowerSet.Main
{
    public partial class MainForm : Form
    {
        #region Fileds
        private readonly List<string> ReadOnlyTableCol = new List<string> { "周期", "执行次数", "实际电流" };
        private readonly List<string> HiddenTableCol = new List<string> { "实际电流" };

        private readonly List<string> Prefix = new List<string> { C.K, C.N, C.C, C.S };

        /// <summary>
        /// 某项是否启动
        /// </summary>
        private readonly Dictionary<string, bool> ProcessStart = new Dictionary<string, bool>()
        {
            { C.K, false },
            { C.N, false },
            { C.C, false },
            { C.S, false },
        };

        private readonly Dictionary<string, CycleController> CycleControllers =
            new Dictionary<string, CycleController>();

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

        private readonly PowerController PowerController = new PowerController();

        private readonly Dictionary<string, double> RealVals = new Dictionary<string, double>();

        /// <summary>
        /// 项目实际运行时间
        /// </summary>
        private long RSecond = 1;

        /// <summary>
        /// 虚拟运行时间
        /// </summary>
        private long VSecond = 1;

        private readonly ObservableChanged<bool> Start = new ObservableChanged<bool>(false);

        private DateTime Time = DateTime.Now;

        readonly Stopwatch Stopwatch = new Stopwatch();
        #endregion

        public MainForm()
        {
            InitializeComponent();
            InitUIMap();
            InitCycleTableData();
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
            MainTimer = new System.Threading.Timer(MainTimerAction, null, 0, 1);
        }

        /// <summary>
        /// 事件绑定
        /// </summary>
        private void EventBind()
        {
            XAxisMargin_Val.ValueChanged += ValChanged;
            YAxisMargin_Val.ValueChanged += ValChanged;

            Start.PropertyChanged += StartChanged;

            Start_Btn.Click += StartBtn_Click;
            End_Btn.Click += StartBtn_Click;

            Prefix.ForEach(p =>
            {
                if (Uis.TryGetValue(p + C.UI_I_LAB, out var uiLab) && uiLab is Label)
                {
                    uiLab.Click += LabClick;
                }
                if (Uis.TryGetValue(p + C.UI_ADD_PROCESS_BTN, out var addbtn) && addbtn is Button)
                {
                    addbtn.Click += AddProcessBtn_Click;
                }
            });
        }

        private void StartBtn_Click(object sender, EventArgs e)
        {
            Start.Val = !Start.Val;
            if (Start.Val)
            {
                ChartData.Clear();
                Chart.DataBind();
                CycleControllers.Clear();
                Time = DateTime.Now;
                RSecond = 1;
                VSecond = 1;
                ChartData.Rows.Add("0秒", 0.0, 0.0, 0.0, 0.0);


                Prefix.ForEach(p =>
                {
                    CycleControllers.Add(p, new CycleController(DataTables[p], p));

                    var cycles = CycleControllers[p].Cycles;
                    cycles.ForEach(c =>
                    {
                        c.Execute += Cycle_Execute;
                        c.Executed += Cycle_Executed;
                    });

                    CycleControllers[p].Start();
                });
            }
            else
            {
                Stopwatch.Stop();
            }
        }

        /// <summary>
        /// 开始状态改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is ObservableChanged<bool> s)
            {
                UpdateControlProperty(End_Btn, new Action(() => End_Btn.Enabled = s.Val));
                UpdateControlProperty(Start_Btn, new Action(() => Start_Btn.Enabled = !s.Val));
                UpdateControlProperty(Start_Btn, new Action(() => Start_Btn.Enabled = !s.Val));
            }
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
                    if (!s.Name.StartsWith(p))
                        return;

                    ProcessStart[p] = !ProcessStart[p];

                    if (ProcessStart[p])
                    {
                        PowerController.OpenI(p);
                        PowerController.SetI(p, 0);
                    }
                    else
                    {
                        PowerController.CloseI(p);
                    }

                    if (!Data.TryGetValue(p, out var data) && ProcessStart[p])
                        Data[p] = new List<RealIData>();

                    UpdateControlProperty(
                        s,
                        new Action(() =>
                        {
                            s.BackColor = ProcessStart[p]
                                ? System.Drawing.Color.ForestGreen
                                : System.Drawing.Color.LightGray;
                        })
                    );

                    UpdateControlProperty(
                        Start_Btn,
                        new Action(
                            () => Start_Btn.Enabled = ProcessStart.Any(it => it.Value) && !Start.Val
                        )
                    );

                    Debug.WriteLine("Click");
                });
            }
        }

        /// <summary>
        /// Timer更新函数
        /// </summary>
        /// <param name="state">可传参</param>
        private void MainTimerAction(object state)
        {
            if ((DateTime.Now - Time).TotalSeconds < VSecond)
                return;

            VSecond++;

            if (!Start.Val)
                return;

            GetRealData();

            // TODO: 缓存所有实时数据到内存中
            Prefix.ForEach(p =>
            {
                if (!Data.TryGetValue(p, out var data))
                    return;

                data.Add(new RealIData() { XVal = RSecond, YVal = RealVals[p] });
                Debug.WriteLine(RealVals[p]);
            });

            Prefix.ForEach(p =>
            {
                if (ProcessStart[p])
                {
                    if (Uis.TryGetValue(p + C.UI_I_VAL_LAB, out var c) && c is Label)
                    {
                        UpdateControlProperty(
                            c,
                            new Action(() =>
                            {
                                c.Text = RealVals[p].ToString("0.00mA");
                            })
                        );
                    }
                }
            });

            AddDataToChart();
            Prefix.ForEach(p => { });
            RSecond++;
        }

        /// <summary>
        /// 添加数据到图表
        /// </summary>
        private void AddDataToChart()
        {
            ChartData.Rows.Add(
                RSecond + "秒",
                RealVals[C.K],
                RealVals[C.N],
                RealVals[C.C],
                RealVals[C.S]
            );
        }

        /// <summary>
        /// 获取实时数据
        /// </summary>
        private void GetRealData()
        {
            Prefix.ForEach(p =>
            {
                var val = .0;
                if (ProcessStart[p])
#if DEBUG
                    val = R.NextDouble() * 4;
#else
                    val = PowerController.GetI(p) / 1000.0;
#endif
                else
                    val = 0;
                RealVals[p] = val;
            });
            //Prefix.ForEach(p => RealVals[p] = PowerController.GetI(p) / 1000.0);
            //PowerController.OpenI(C.K);
            //PowerController.OpenI(C.N);
            //PowerController.OpenI(C.C);
            //PowerController.OpenI(C.S);
            //PowerController.SetI("K", 0);
            //PowerController.SetI("N", 0);
        }

        /// <summary>
        /// 初始化图表
        /// </summary>
        private void InitChart()
        {
            XAxisMargin_Val.Value = 5;
            YAxisMargin_Val.Value = .5M;

            #region 设置图表高度
            ChartHight_Val.Value = 50;
            SplitLayout.SplitterDistance = (int)(
                SplitLayout.Size.Height * (Convert.ToDouble(ChartHight_Val.Value) / 100.0)
            );
            #endregion

            #region 图表初始化及数据绑定
            var chartArea = new ChartArea(C.CHART_BASE_CHARTAREA);
            Prefix.ForEach(p =>
                Chart.Series.Add(
                    new Series(p)
                    {
                        ChartType = SeriesChartType.Line,
                        Legend = C.BASE_LEGEND,
                        XValueMember = "X",
                        YValueMembers = p + "Y",
                        BorderWidth = 4,
                    }
                )
            );
            Chart.Legends.Add(new Legend(C.BASE_LEGEND));
            Chart.ChartAreas.Add(chartArea);
            chartArea.AxisX.Interval = Convert.ToDouble(XAxisMargin_Val.Value);

            chartArea.AxisX.MajorGrid.Interval = 1;
            chartArea.AxisX.MajorGrid.LineColor = System.Drawing.Color.LightGray;

            chartArea.AxisY.Interval = Convert.ToDouble(YAxisMargin_Val.Value);

            chartArea.AxisX.Minimum = 1;

            chartArea.AxisX.Maximum = 60;
            chartArea.AxisY.Maximum = 5;

            chartArea.AxisY.LabelStyle.Format = "0.0A";
            chartArea.AxisX.LabelStyle.Format = "0S";

            #endregion

            #region 初始化图表数据表
            ChartData.Columns.Add("X");
            Prefix.ForEach(p =>
            {
                ChartData.Columns.Add($"{p}Y");
            });
            ChartData.RowChanged += DataChanged;
            ChartData.Rows.Add("0秒", 0.0, 0.0, 0.0, 0.0);
            //ChartData.Rows.Add("1秒", 0.0, 0.0, 0.0, 0.0);
            //ChartData.Rows.Add("2秒", 0.0, 0.0, 0.0, 0.0);
            //ChartData.Rows.Add("3秒", 0.0, 0.0, 0.0, 0.0);
            //ChartData.Rows.Add("4秒", 0.0, 0.0, 0.0, 0.0);
            //ChartData.Rows.Add("5秒", 0.0, 0.0, 0.0, 0.0);

            Chart.DataSource = ChartData;
            Chart.DataBind();
            #endregion
        }

        /// <summary>
        /// 数据更新事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataChanged(object sender, DataRowChangeEventArgs e)
        {
            var x = Math.Max(
                Chart.ChartAreas[C.CHART_BASE_CHARTAREA].AxisX.Maximum,
                ChartData.Rows.Count + 5
            );
            var maxVal = 0.0;
            foreach (DataRow row in ChartData.Rows)
            {
                Prefix.ForEach(p =>
                {
                    maxVal = Math.Max(Convert.ToDouble(row[p + "Y"]), maxVal);
                });
            }
            var y = Math.Max(Chart.ChartAreas[C.CHART_BASE_CHARTAREA].AxisY.Maximum, maxVal + .5);

            UpdateControlProperty(
                Chart,
                new Action(() =>
                {
                    //Chart.DataSource = ChartData;
                    Chart.DataBind();
                    Chart.ChartAreas[C.CHART_BASE_CHARTAREA].AxisX.Maximum = x;
                    Chart.ChartAreas[C.CHART_BASE_CHARTAREA].AxisY.Maximum = y;
                })
            );
        }

        /// <summary>
        /// 初始化数据表
        /// </summary>
        private void InitCycleTableData()
        {
            Prefix.ForEach(f => ParamSets.Add(f, new ParamSet()));

            var baseDataTable = new DataTable();
            var t = typeof(ParamProcessSet);
            var ps = t.GetProperties();

            #region Default Table Data Create
            for (int i = 0; i < 12; i++)
            {
                baseDataTable.Rows.Add();
            }

            foreach (var property in ps)
            {
                var colset =
                    property.GetCustomAttributes(typeof(ColSetAttribute), false)
                    as ColSetAttribute[];
                if (colset.Length > 0 && !HiddenTableCol.Contains(colset[0].Name))
                {
                    baseDataTable.Columns.Add(colset[0].Name);

                    foreach (DataRow row in baseDataTable.Rows)
                    {
                        row[colset[0].Name] = colset[0].DefaultVal;
                    }
                }
            }

            #endregion


            #region Table Set Property & Add Data
            Prefix.ForEach(p =>
            {
                DataTables.Add(p, baseDataTable.Copy());

                if (
                    Uis.TryGetValue(p + C.UI_PARAM_SET_TABLE, out var control)
                    && control is DataGridView table
                )
                {
                    table.EditingControlShowing += ParamSetTable_EditingControlShowing;

                    table.DataSource = DataTables[table.Name.First().ToString()];
                    table.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

                    foreach (DataGridViewTextBoxColumn col in table.Columns)
                    {
                        col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        col.SortMode = DataGridViewColumnSortMode.NotSortable;

                        if (ReadOnlyTableCol.Contains(col.Name, true))
                            col.ReadOnly = true;
                    }

                    for (int i = 0; i < table.RowCount; i++)
                    {
                        table.Rows[i].Cells["周期"].Value = $"周期{i + 1: 000}";
                    }
                }
            });
            #endregion
        }

        private void Cycle_Execute(CycleExecuteArg arg)
        {
            PowerController.SetI(arg.Flag, arg.Value);
            // TODO: 表格颜色闪烁
        }

        private void Cycle_Executed(CycleExecuteArg arg)
        {
            // TODO: 表格颜色变灰
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
            Prefix.ForEach(p =>
            {
                PowerController.CloseI(p);
                Task.Delay(100).Wait();
            });
            PowerController.CloseThread();
            MainTimer.Dispose();
            Close();
        }

        private void HiddenChartClick(object sender, EventArgs e) =>
            SplitLayout.Panel1Collapsed = !SplitLayout.Panel1Collapsed;

        // TODO: 统一管理
        private void AddProcessBtn_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            Prefix.ForEach(p =>
            {
                if (!btn.Name.StartsWith(p))
                    return;
                if (Uis.TryGetValue(p + C.UI_PARAM_SET_TABLE, out var c) && c is DataGridView table)
                {
                    (table.DataSource as DataTable).Rows.Add(
                        $"周期{table.RowCount + 1: 000}",
                        1000,
                        30,
                        30,
                        10,
                        0
                    );
                }
            });
        }

        private void ChartHight_Val_Changed(object sender, EventArgs e)
        {
            SplitLayout.SplitterDistance = (int)(
                SplitLayout.Size.Height * (ChartHight_Val.Value / 100)
            );
        }

        /// <summary>
        /// 数据变更后触发的更新操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        #region TableDataUtils
        #region Fileds
        private readonly Dictionary<string, int> TableColMap = new Dictionary<string, int>
        {
            { "周期", 0 },
            { "电流设置", 1 },
            { "执行时间", 2 },
            { "关闭时间", 3 },
            { "设置次数", 4 },
            { "执行次数", 5 },
        };

        #endregion
        private string GetTableDataVal(string flag, int rowIndex, string colName)
        {
            return Convert.ToString(DataTables[flag].Rows[rowIndex][colName]);
        }

        private string GetTableDataVal(string flag, int rowIndex, int colIndex)
        {
            return Convert.ToString(DataTables[flag].Rows[rowIndex][colIndex]);
        }

        /// <summary>
        /// 获取周期
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        private int GetTableCycle(string flag, int rowIndex)
        {
            return Convert.ToInt32(
                GetTableDataVal(flag, rowIndex, 0)
                    .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1]
            );
        }
        #endregion

        #region ControlUtils
        void UpdateControlProperty(Control control, Action updateAction)
        {
            if (control.InvokeRequired)
            {
                control.BeginInvoke(new Action(() => updateAction()));
            }
            else
            {
                updateAction();
            }
        }
        #endregion


        private void ParamSetTable_EditingControlShowing(
            object sender,
            DataGridViewEditingControlShowingEventArgs e
        )
        {
            var table = (DataGridView)sender;
            var numColNum = new int[] { 1, 2, 3, 4, };
            if (numColNum.Contains(table.CurrentCell.ColumnIndex))
            {
                e.Control.KeyPress -= TextBox_KeyPress;
                e.Control.KeyPress += TextBox_KeyPress;
            }
        }

        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // 检查输入是否为数字或控制字符（如退格）
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}
