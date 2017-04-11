using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace ChartControls.Model
{
    //雷达图实体
    public class RadarChartModel
    {
        public RadarChartModel()
        {
            dataChartCollection = new List<KeyValuePair<string, double>>();
        }
        //一个雷达图的集合
        List<KeyValuePair<string, double>> dataChartCollection;

        public List<KeyValuePair<string, double>> DataChartCollection
        {
            get { return dataChartCollection; }
            set { dataChartCollection = value; }
        }

        //颜色
        private Brush stroke = new SolidColorBrush(Color.FromRgb(255, 0, 0));

        public Brush Stroke
        {
            get { return stroke; }
            set { stroke = value; }
        }

        //显示内容
        private string toolTip = "";

        public string ToolTip
        {
            get { return toolTip; }
            set { toolTip = value; }
        }

    }
}
