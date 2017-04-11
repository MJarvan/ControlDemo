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
using System.Windows.Controls.Primitives;
using ChartControls.Model;

namespace ControlDemo2
{
    /// <summary>
    /// RadarControl.xaml 的交互逻辑
    /// </summary>
    public partial class RadarControl : UserControl
    {
        public RadarControl()
        {
            InitializeComponent();
        }

        //重写ArrangeOverride获得grid实际高度，宽度
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var size = base.ArrangeOverride(arrangeBounds);

            // 自定义代码        
            return size;
        }

        #region 依赖属性

        //多边形数量
        public int PolygonCount
        {
            get
            {
                return (int)GetValue(PolygonCountProperty);
            }
            set
            {
                SetValue(PolygonCountProperty, value);
            }
        }
        public static readonly DependencyProperty PolygonCountProperty =
            DependencyProperty.Register("PolygonCount", typeof(int), typeof(RadarControl), new UIPropertyMetadata(5));

        //标题
        public string DecTitle
        {
            get
            {
                return (string)GetValue(DecTitleProperty);
            }
            set
            {
                SetValue(DecTitleProperty, value);
            }
        }
        public static readonly DependencyProperty DecTitleProperty =
            DependencyProperty.Register("DecTitle", typeof(string), typeof(RadarControl), new UIPropertyMetadata(null));

        //尺码最大值
        public double MaxValue
        {
            get
            {
                return (double)GetValue(MaxValueProperty);
            }
            set
            {
                SetValue(MaxValueProperty, value);
            }
        }
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(double), typeof(RadarControl), new UIPropertyMetadata(0.0));
#endregion

        #region 全局变量

        Canvas ca = new Canvas();
        Popup popupMessage = new Popup() { StaysOpen = true, AllowsTransparency = true };
        TextBlock txtMessage = new TextBlock() { FontSize = 15, Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)), TextWrapping = TextWrapping.Wrap, Margin = new Thickness(5) };

        //绑定数据源
        private List<RadarChartModel> radarChartModelList = new List<RadarChartModel>();

        public List<RadarChartModel> RadarChartModelList
        {
            get { return radarChartModelList; }
            set { radarChartModelList = value; }
        }

        //背景色
        private Brush selectBackground = new SolidColorBrush(Color.FromRgb(0xEB, 0x42, 0x00));
        //半径
        private double radius = 100;
        //最大半径
        private double Maxradius = 100;
        //多边形园心
        private Point CanvesCenter = new Point();

        private bool IsDraw = false;

        #endregion

        #region  布局画图
        //加载布局
        public void LoadLayout()
        {
            GridControl.Children.Add(ca);
            popupMessage.Placement = PlacementMode.MousePoint;
            Border border = new Border() { Background = new SolidColorBrush(Color.FromRgb(0, 0, 0)), Opacity = 0.8, CornerRadius = new CornerRadius(5) };
            border.Child = txtMessage;
            popupMessage.Child = border;
            GridControl.Children.Add(popupMessage);
        }
        //加载雷达图
        private void LoadRadarChart()
        {
            ca.Children.Clear();
            if (radarChartModelList.Count == 0)
            {
                return;
            }
            //半径递减值
            double reducevalue = radius / PolygonCount;

            PointCollection firstPoints = new PointCollection();

            //画多形
            for (int i = 0; i < PolygonCount; i++)
            {
                Polygon plg = new Polygon();


                PointCollection Points = GetPolygonPoint(CanvesCenter, radius, radarChartModelList[0].DataChartCollection.Count);
                plg.Points = Points;
                plg.StrokeThickness = 1.5;
                plg.Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                if (i == 0)
                {
                    firstPoints = Points;
                }
                if (i % 2 == 0)
                {
                    plg.Fill = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                }
                else
                {
                    plg.Fill = new SolidColorBrush(Color.FromRgb(239, 239, 239));
                }

                plg.FillRule = FillRule.Nonzero;
                ca.Children.Add(plg);
                radius = radius - reducevalue;
            }

            //画到圆心的线
            for (int i = 0; i < firstPoints.Count; i++)
            {
                Line line = new Line();
                line.X1 = CanvesCenter.X;
                line.Y1 = CanvesCenter.Y;
                line.X2 = firstPoints[i].X;
                line.Y2 = firstPoints[i].Y;
                line.StrokeThickness = 1.5;
                line.Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                ca.Children.Add(line);
                TextBlock txtContent = new TextBlock();
                txtContent.Text = radarChartModelList[0].DataChartCollection[i].Key;
                txtContent.Tag = firstPoints[i];
                txtContent.Loaded += new RoutedEventHandler(txtContent_Loaded);
                ca.Children.Add(txtContent);
            }

            //画雷达线
            for (int r = 0; r < radarChartModelList.Count; r++)
            {
                Path radarPaht = new Path();
                radarPaht.PreviewMouseMove += new MouseEventHandler(plgValue_PreviewMouseMove);
                radarPaht.MouseLeave += new MouseEventHandler(plgValue_MouseLeave);
                radarPaht.Fill = radarChartModelList[r].Stroke;
                radarPaht.Stroke = radarChartModelList[r].Stroke;
                radarPaht.StrokeThickness = 2;
                //标识
                radarPaht.Tag = r;

                GeometryGroup geometryGroup = new GeometryGroup();
                PointCollection pCollection = GetValuePoint(CanvesCenter, radarChartModelList[r].DataChartCollection);
                for (int i = 0; i < pCollection.Count; i++)
                {
                    int startIndex = i;
                    int endIndex = i + 1;
                    if (i == pCollection.Count - 1)
                        endIndex = 0;
                    geometryGroup.Children.Add(new LineGeometry(pCollection[startIndex], pCollection[endIndex]));
                }
                for (int i = 0; i < pCollection.Count; i++)
                {
                    geometryGroup.Children.Add(new EllipseGeometry(pCollection[i], 3, 3));
                }
                radarPaht.Data = geometryGroup;
                ca.Children.Add(radarPaht);
            }


            //画雷达图
            //Polygon plgValue = new Polygon();
            //plgValue.Points = GetValuePoint(CanvesCenter, RadarDataContext);
            //plgValue.StrokeThickness = 2;
            //plgValue.Stroke = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            //plgValue.Fill = Brushes.Transparent;
            //plgValue.FillRule = FillRule.Nonzero;
            //plgValue.PreviewMouseMove += new MouseEventHandler(plgValue_PreviewMouseMove);
            //plgValue.MouseLeave += new MouseEventHandler(plgValue_MouseLeave);

            //ca.Children.Add(plgValue);




        }


        protected override void OnRender(System.Windows.Media.DrawingContext dc)
        {
            base.OnRender(dc);
            if (radarChartModelList.Count == 0)
            {
                return;
            }

            //根据窗体的大小获取图形的半径

            if (this.ActualWidth > this.ActualHeight)
            {
                radius = this.ActualHeight / 2 - 40;
            }
            else
            {
                radius = this.ActualWidth / 2 - 40;
            }
            //获取最大半径
            Maxradius = radius;
            //获取圆心位置
            CanvesCenter = new Point(this.ActualWidth / 2, this.ActualHeight / 2);


            //加载布局
            if (IsDraw == false)
            {
                LoadLayout();
                IsDraw = true;
            }
            //加载雷达图表
            LoadRadarChart();

        }
        #endregion


        #region  方法
        //添加数据源
        public void AddData(List<KeyValuePair<string, double>> keyValuePairList)
        {
            if (keyValuePairList == null)
                return;
            if (keyValuePairList.Count > 0)
            {
                RadarChartModel model = new RadarChartModel();
                model.DataChartCollection = keyValuePairList;
                model.Stroke = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                model.ToolTip = DecTitle;

                for (int i = 0; i < keyValuePairList.Count; i++)
                {
                    model.ToolTip = model.ToolTip + System.Environment.NewLine + keyValuePairList[i].Key + ":" + keyValuePairList[i].Value;
                    if (keyValuePairList[i].Value > MaxValue)
                    {
                        MaxValue = keyValuePairList[i].Value;
                    }
                }
                radarChartModelList.Add(model);
            }
        }

        //清空绑定数据
        public void ClearData()
        {
            if (radarChartModelList != null)
            {
                radarChartModelList.Clear();
            }
        }
        /// <summary>
        /// 根据半径和圆心确定N个点  
        /// </summary>
        /// <param name="center">center 表示坐标原点</param>
        /// <param name="r">r 表示半径</param>
        /// <param name="polygonBound">polygonBound 表示边数</param>
        /// <returns></returns>
        private PointCollection GetPolygonPoint(Point center, double r, int polygonBound)
        {
            double g = 0;
            double perangle = 360 / polygonBound;
            double pi = Math.PI;

            List<Point> values = new List<Point>();
            for (int i = 0; i < (int)polygonBound; i++)
            {
                Point p2 = new Point(r * Math.Cos(g * pi / 180), r * Math.Sin(g * pi / 180));
                //实际坐标
                Point ActualPoint = new Point(p2.X + center.X, p2.Y + center.Y);
                values.Add(ActualPoint);
                g -= perangle;
            }
            PointCollection pcollect = new PointCollection(values);
            return pcollect;
        }

        private PointCollection GetValuePoint(Point center, List<KeyValuePair<string, double>> keyList)
        {
            double g = 0;
            double perangle = 360 / keyList.Count;
            double pi = Math.PI;
            List<Point> values = new List<Point>();
            for (int i = 0; i < keyList.Count; i++)
            {
                double r = Maxradius / MaxValue * keyList[i].Value;
                Point p2 = new Point(r * Math.Cos(g * pi / 180), r * Math.Sin(g * pi / 180));
                //实际坐标
                Point ActualPoint = new Point(p2.X + center.X, p2.Y + center.Y);
                values.Add(ActualPoint);
                g -= perangle;
            }

            PointCollection pcollect = new PointCollection(values);

            return pcollect;

        }
        /// <summary>
        /// 根据文本框宽度，高度，和最外面的原点确定textbox的位置
        /// </summary>
        /// <param name="p"></param>
        /// <param name="with">文本框宽度</param>
        /// <param name="height">文本框高度</param>
        /// <returns></returns>
        private Point GetTxtPoint(Point p, double with, double height)
        {
            double pi = Math.PI;
            double r = 0;
            if (with > height)
            {
                r = Maxradius + with / 2 + 5;
            }
            else
            {
                r = Maxradius + height / 2 + 5;
            }
            Point p2 = new Point((p.X - CanvesCenter.X) / Maxradius * r + CanvesCenter.X, (p.Y - CanvesCenter.Y) / Maxradius * r + CanvesCenter.Y);
            Point Actualpoint = new Point(p2.X - with / 2, p2.Y - height / 2);
            //根据x轴值变化，文本宽度调整文位置
            double ChangeY = (1 - Math.Abs(p2.X - CanvesCenter.X) / r) * (with / 2) - height / 2;
            if (ChangeY > 0)
            {
                if (Actualpoint.Y > CanvesCenter.Y)
                {
                    return new Point(Actualpoint.X, Actualpoint.Y - ChangeY);
                }
                else
                {
                    return new Point(Actualpoint.X, Actualpoint.Y + ChangeY);
                }
            }
            return Actualpoint;


        }

        #endregion

        #region 事件
        //文本呈现高度后触发事件
        private void txtContent_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock txtContent = sender as TextBlock;
            if (txtContent != null)
            {
                double with = txtContent.ActualWidth;
                Point point = (Point)txtContent.Tag;
                if (point != null)
                {
                    point = GetTxtPoint(point, txtContent.ActualWidth, txtContent.ActualHeight);
                    txtContent.SetValue(Canvas.LeftProperty, point.X);
                    txtContent.SetValue(Canvas.TopProperty, point.Y);
                }
            }
        }


        private void plgValue_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!popupMessage.IsOpen)
                popupMessage.IsOpen = true;
            //Point p = e.GetPosition((IInputElement)sender);
            //popupMessage.HorizontalOffset = p.X;
            //popupMessage.VerticalOffset = p.Y;
            Path path = sender as Path;
            if (path != null)
            {
                if (Int32.Parse(path.Tag.ToString()) != index)
                {
                    index = Int32.Parse(path.Tag.ToString());
                    txtMessage.Text = radarChartModelList[index].ToolTip;
                }
            }

        }
        int index = -1;

        private void plgValue_MouseLeave(object sender, MouseEventArgs e)
        {
            if (popupMessage.IsOpen)
                popupMessage.IsOpen = false;
        }
        #endregion

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
