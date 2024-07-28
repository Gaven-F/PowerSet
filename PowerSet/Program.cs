using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

            //var data = new List<DataSave>();
            //data.Add(new DataSave() {Data1 = "11" });
            //data.Add(new DataSave() {Data1 = "22" });
            //data.Add(new DataSave() {Data1 = "33" });

            //GF_SqlHelper.Core.DbInit();
            //GF_SqlHelper.Core.InitTable<DataSave>("星期一");
            //GF_SqlHelper.Core.AddData("星期一",data);
            //GF_SqlHelper.Core.AddData("星期一", new DataSave() { Data1 = "44" });
            //var d = GF_SqlHelper.Core.GetData<DataSave>("星期一").Result;
            //Console.WriteLine(d);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main.MainForm());
        }
    }

    class DataSave : GF_SqlHelper.BaseClass.BaseTable
    {
        public string Data1 { get; set; }
    }
}
