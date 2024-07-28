using System.Collections.ObjectModel;

namespace PowerSet.Attribute
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

        public ObservableCollection<ParamProcessSet> Process { get; set; } =
            new ObservableCollection<ParamProcessSet>();
    }
}
