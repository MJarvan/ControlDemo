using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections;
using System.Data;

namespace ControlDemo2
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            AddTableData();
            AddRadarControl();
        }

        //模拟数据源table
        DataTable dt = new DataTable("Table");

        /// <summary>
        /// 向dt添加数据
        /// </summary>
        private void AddTableData()
        {
            //加两列
            dt.Columns.Add("Name", System.Type.GetType("System.String"));
            dt.Columns.Add("Max", System.Type.GetType("System.Int32"));
            dt.Columns.Add("Num", System.Type.GetType("System.Double"));

            //加数据啦
            DataRow dr1 = dt.NewRow();
            dr1["Name"] = "吴荷叶";
            dr1["Max"] = 20;
            dr1["Num"] = 10;
            dt.Rows.Add(dr1);

            DataRow dr2 = dt.NewRow();
            dr2["Name"] = "刚哥";
            dr2["Max"] = 20;
            dr2["Num"] = 18;
            dt.Rows.Add(dr2);

            DataRow dr3 = dt.NewRow();
            dr3["Name"] = "涛哥";
            dr3["Max"] = 20;
            dr3["Num"] = 8;
            dt.Rows.Add(dr3);

            DataRow dr4 = dt.NewRow();
            dr4["Name"] = "哇哥";
            dr4["Max"] = 20;
            dr4["Num"] = 12;
            dt.Rows.Add(dr4);

            DataRow dr5 = dt.NewRow();
            dr5["Name"] = "哦哥";
            dr5["Max"] = 20;
            dr5["Num"] = 2;
            dt.Rows.Add(dr5);

            DataRow dr6 = dt.NewRow();
            dr6["Name"] = "系哥";
            dr6["Max"] = 20;
            dr6["Num"] = 4;
            dt.Rows.Add(dr6);

            DataRow dr7 = dt.NewRow();
            dr7["Name"] = "狗哥";
            dr7["Max"] = 20;
            dr7["Num"] = 1;
            dt.Rows.Add(dr7);

            DataRow dr8 = dt.NewRow();
            dr8["Name"] = "滴哥";
            dr8["Max"] = 20;
            dr8["Num"] = 1;
            dt.Rows.Add(dr8);
        }

        //加控件啦
        private void AddRadarControl()
        {
            RadarControl radar = new RadarControl();
            List<KeyValuePair<string, double>> keyValuePairList = new List<KeyValuePair<string, double>>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                KeyValuePair<string, double> keyPair = new KeyValuePair<string, double>(dt.Rows[i]["Name"].ToString().Trim(), double.Parse(dt.Rows[i]["Num"].ToString()));
                keyValuePairList.Add(keyPair);
                radar.MaxValue = Int32.Parse(dt.Rows[i]["max"].ToString());

            }

            radar.AddData(keyValuePairList);
            ChartGrid.Children.Add(radar);
        }

        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;

                if (child == null)
                    child = GetVisualChild<T>(v);
                else
                    break;
            }
            return child;
        }
    }
}
