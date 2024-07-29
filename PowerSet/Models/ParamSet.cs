using System.Collections.Generic;

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

        [SqlSugar.SugarColumn(IsIgnore = true)]
        public List<ParamProcessSet> Process { get; set; } = new List<ParamProcessSet>();
    }
}
