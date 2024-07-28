using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PowerSet.Main
{

    class ParamSet : GF_SqlHelper.BaseClass.BaseTable
    {
        /// <summary>
        /// 管号
        /// </summary>
        public string TubeNum { get; set; }
        /// <summary>
        /// 操作者
        /// </summary>
        public string Worker { get; set; }


        public ObservableCollection<ParamProcessSet> Process { get; set; } = new ObservableCollection<ParamProcessSet>();
    }

    class ParamProcessSet : GF_SqlHelper.BaseClass.BaseTable
    {
        [ColSet("周期")]
        public int ProcessIndex { get; set; } = -1;

        /// <summary>
        /// 电流 Current
        /// </summary>
        [ColSet("电流设置", DefaultVal = "1000")]
        public int I { get; set; } = -1;
        [ColSet("实际电流", false, DefaultVal = "1000")]
        public int RealI { get; set; } = -1;
        [ColSet("执行时间", DefaultVal = "0")]
        public int ProcessTime { get; set; } = -1;
        [ColSet("关闭时间", DefaultVal = "0")]
        public int CloseTime { get; set; } = -1;
        [ColSet("设置次数", DefaultVal = "0")]
        public int ProcessCnt { get; set; } = -1;
        [ColSet("执行次数", DefaultVal = "0")]
        public int CurrentCnt { get; set; } = -1;
    }

    class ColSet : System.Attribute
    {
        public string Name { get; set; }
        public bool Show { get; set; }

        public string DefaultVal { get; set; } = "";
        public ColSet(string name = null, bool show = true)
        {
            Name = name; Show = show;
        }
    }
}
