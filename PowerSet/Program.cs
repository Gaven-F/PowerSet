using System;
using System.Windows.Forms;

namespace PowerSet
{
	internal static class Program
	{
		/// <summary>
		/// 应用程序的主入口点。
		/// </summary>
		[STAThread]
		static void Main()
		{
			GF_SqlHelper.Core.DbInit();

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Main.MainForm());
		}
	}
}
