using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace ChartControl.Model
{
    public class FunnelChartModel
    {
        public string Key { get; set; }

        public double Value { get; set; }

        public Brush Fill { get; set; }

        public string ToolTip { get; set; }
    }
}
