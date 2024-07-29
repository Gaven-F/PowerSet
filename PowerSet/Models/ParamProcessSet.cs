using PowerSet.Attributes;

namespace PowerSet.Main
{
    class ParamProcessSet : GF_SqlHelper.BaseClass.BaseTable
    {
        [ColSet("周期")]
        public int ProcessIndex { get; set; } = -1;

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
}
