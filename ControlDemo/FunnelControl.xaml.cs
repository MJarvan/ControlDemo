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
using ChartControl.Model;
using ChartControls.Logic;
using System.Windows.Media.Animation;


namespace ControlDemo
{
    /// <summary>
    /// FunnelControl.xaml 的交互逻辑
    /// </summary>
    public partial class FunnelControl : UserControl
    {
        public FunnelControl()
        {
            InitializeComponent();

            checkMax.IsChecked = false;
        }

        //重写ArrangeOverride获得grid实际高度，宽度
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var size = base.ArrangeOverride(arrangeBounds);

            // 自定义代码        
            return size;
        }

        #region 全局变量

        Canvas ca = new Canvas();
        Popup popupMessage = new Popup() { StaysOpen = true, AllowsTransparency = true };
        TextBlock txtMessage = new TextBlock() { FontSize = 15, Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)), TextWrapping = TextWrapping.Wrap, Margin = new Thickness(5) };

        //绑定数据源
        List<FunnelChartModel> FunnelDataContext = new List<FunnelChartModel>();

        public double WindowsHeight { get; set; }
        public double WindowsWidth { get; set; }

        //最大宽度
        private double FunnelMaxWidth = 0;
        //最大高度
        private double FunnelMaxHeight = 0;

        //宽度比  实际宽度/漏斗图数值
        private double FunnelWithRatio = 0;
        //第一个漏斗图中心点
        private Point FirstCenterPoint = new Point();
        //颜色
        private Brush FunnelBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));

        private bool IsDraw = false;

        private double controlWidth = 600;
        private double controlHeight = 400;
        #endregion

        #region  布局画图

        protected override void OnRender(System.Windows.Media.DrawingContext dc)
        {

            base.OnRender(dc);

            if (FunnelDataContext.Count == 0)
            {
                return;
            }
            //根据窗体的大小中心点
            FirstCenterPoint = new Point(this.ActualWidth / 2, 30);
            //获取最大半径
            FunnelMaxWidth = this.ActualWidth - 60;

            FunnelMaxHeight = (this.ActualHeight - 60) / FunnelDataContext.Count;

            FunnelWithRatio = FunnelMaxWidth / FunnelDataContext[0].Value;

            //加载布局
            if (IsDraw == false)
            {
                LoadLayout();
                IsDraw = true;
            }
            //加载雷达图表
            LoadRadarChart();

        }

        //加载布局
        public void LoadLayout()
        {
            GridControl.Children.Add(ca);
            popupMessage.Placement = PlacementMode.Mouse;
            Border border = new Border() { Background = new SolidColorBrush(Color.FromRgb(0, 0, 0)), Opacity = 0.8, CornerRadius = new CornerRadius(5) };
            border.Child = txtMessage;
            popupMessage.Child = border;
            GridControl.Children.Add(popupMessage);
        }

        //加载漏斗图
        private void LoadRadarChart()
        {
            ca.Children.Clear();
            if (FunnelDataContext.Count == 0)
            {
                return;
            }

            Point center = FirstCenterPoint;
            //画漏斗图
            for (int i = 0; i < FunnelDataContext.Count; i++)
            {
                PointCollection pCollection = new PointCollection();
                if (i == FunnelDataContext.Count - 1)
                {

                    pCollection.Add(new Point(center.X - FunnelDataContext[i].Value * FunnelWithRatio / 2, center.Y));
                    pCollection.Add(new Point(center.X + FunnelDataContext[i].Value * FunnelWithRatio / 2, center.Y));
                    center = new Point(center.X, center.Y + FunnelMaxHeight);
                    pCollection.Add(new Point(center.X, center.Y));
                }
                else
                {
                    pCollection.Add(new Point(center.X - FunnelDataContext[i].Value * FunnelWithRatio / 2, center.Y));
                    pCollection.Add(new Point(center.X + FunnelDataContext[i].Value * FunnelWithRatio / 2, center.Y));
                    center = new Point(center.X, center.Y + FunnelMaxHeight);

                    pCollection.Add(new Point(center.X + FunnelDataContext[i + 1].Value * FunnelWithRatio / 2, center.Y));
                    pCollection.Add(new Point(center.X - FunnelDataContext[i + 1].Value * FunnelWithRatio / 2, center.Y));
                }
                Polygon plg = new Polygon();
                plg.Points = pCollection;
                plg.StrokeThickness = 1.5;
                plg.Stroke = FunnelDataContext[i].Fill;
                plg.Fill = FunnelDataContext[i].Fill;
                plg.FillRule = FillRule.Nonzero;

                //索引
                plg.Tag = i;
                plg.PreviewMouseMove += new MouseEventHandler(plg_PreviewMouseMove);
                plg.MouseLeave += new MouseEventHandler(plg_MouseLeave);
                ca.Children.Add(plg);
                //Grid txtGrid = new Grid();
                //txtGrid.VerticalAlignment = VerticalAlignment.Center;
                //txtGrid.HorizontalAlignment = HorizontalAlignment.Center;
                //txtGrid.Loaded += new RoutedEventHandler(txtGrid_Loaded);
                //txtGrid.Height = FunnelMaxHeight;
                //TextBlock txtFunnelMessage = new TextBlock() { FontSize = 13, Foreground = new SolidColorBrush(Colors.White)	 };
                //txtFunnelMessage.Text = FunnelDataContext[i].Key;
                //txtGrid.Children.Add(txtFunnelMessage);
                //txtGrid.Tag = center;
                //ca.Children.Add(txtGrid);
            }
        }

        #endregion

        #region  方法
        //添加数据源
        public void AddData(List<KeyValuePair<string, double>> keyValuePairList)
        {
            if (keyValuePairList == null)
                return;
            List<System.Drawing.Color> Colors = ColorLogic.GetColors(keyValuePairList.Count);

            for (int i = 0; i < keyValuePairList.Count; i++)
            {
                FunnelChartModel Fmodel = new FunnelChartModel();
                Fmodel.Key = keyValuePairList[i].Key;
                Fmodel.Value = keyValuePairList[i].Value;
                Fmodel.Fill = new SolidColorBrush(Color.FromRgb(Colors[i].R, Colors[i].G, Colors[i].B));
                Fmodel.ToolTip = "漏斗图" + Environment.NewLine.ToString() + keyValuePairList[i].Key.ToString() + "：" + keyValuePairList[i].Value.ToString();
                FunnelDataContext.Add(Fmodel);
            }

            FunnelDataContext = FunnelDataContext.OrderByDescending(m => m.Value).ToList();
        }

        //清空绑定数据
        public void ClearData()
        {
            if (FunnelDataContext != null)
            {
                FunnelDataContext.Clear();
            }
        }
        #endregion

        #region 事件
        //文本呈现高度后触发事件
        private void txtFunnelMessage_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void txtGrid_Loaded(object sender, RoutedEventArgs e)
        {
            Grid txtGrid_Loaded = sender as Grid;
            if (txtGrid_Loaded != null)
            {
                Point point = (Point)txtGrid_Loaded.Tag;

                if (point != null)
                {
                    point = new Point(point.X - txtGrid_Loaded.ActualWidth / 2, point.Y - FunnelMaxHeight);

                    txtGrid_Loaded.SetValue(Canvas.LeftProperty, point.X);
                    txtGrid_Loaded.SetValue(Canvas.TopProperty, point.Y);
                }
            }
        }





        private void plg_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!popupMessage.IsOpen)
                popupMessage.IsOpen = true;
            //Point p = e.GetPosition((IInputElement)sender);
            //popupMessage.HorizontalOffset = p.X;
            //popupMessage.VerticalOffset = p.Y;

            Polygon path = sender as Polygon;
            if (path != null)
            {
                if (Int32.Parse(path.Tag.ToString()) != index)
                {
                    index = Int32.Parse(path.Tag.ToString());
                    txtMessage.Text = FunnelDataContext[index].ToolTip;
                }
            }

        }
        int index = -1;

        private void plg_MouseLeave(object sender, MouseEventArgs e)
        {
            if (popupMessage.IsOpen)
                popupMessage.IsOpen = false;
        }
        #endregion



        /// <summary>
        /// 放大控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkMax_Checked(object sender, RoutedEventArgs e)
        {
            this.RenderTransformOrigin = new Point(0.5, 0.5);
            this.BeginAnimation(WidthProperty, GetAnimation(controlWidth, WindowsWidth - 100));
            this.BeginAnimation(HeightProperty, GetAnimation(controlHeight, WindowsHeight - 120));
        }

        //实现缓动动画
        private AnimationTimeline GetAnimation(double oldValue, double newValue)
        {
            var sizeAnimation = new DoubleAnimation()
            {
                From = oldValue,
                To = newValue,
                Duration = TimeSpan.FromSeconds(1),
                EasingFunction = new BackEase()
                {
                    Amplitude = 0.5,
                    EasingMode = EasingMode.EaseInOut,
                },
            };

            return sizeAnimation;
        }

        /// <summary>
        /// 缩小控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkMax_Unchecked(object sender, RoutedEventArgs e)
        {
            this.RenderTransformOrigin = new Point(0.5, 0.5);
            this.BeginAnimation(WidthProperty, GetAnimation(this.ActualWidth, controlWidth));
            this.BeginAnimation(HeightProperty, GetAnimation(this.ActualHeight, controlHeight));
        }
    }
}
