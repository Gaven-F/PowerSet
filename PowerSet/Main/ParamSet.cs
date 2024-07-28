using System.Collections.Generic;

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

		public List<ParamProcessSet> Process { get; set; } = new List<ParamProcessSet>();
	}
}
