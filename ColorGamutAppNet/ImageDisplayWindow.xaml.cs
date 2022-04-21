using ColorGamutLib;
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
using System.Windows.Shapes;

namespace ColorGamutAppNet
{
    /// <summary>
    /// ImageDisplayWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ImageDisplayWindow : HandyControl.Controls.GlowWindow
    {
        public ImageDisplayWindow()
        {
            InitializeComponent();
        }
        private readonly List<ColorGamutPoint> gamutPointsStand;
        private readonly List<ColorGamutPoint> gamutPointsTest;
        private readonly List<ColorGamutPoint> gamutPointsTestCoverage;
        public ImageDisplayWindow(List<ColorGamutPoint> stand, List<ColorGamutPoint> test, List<ColorGamutPoint> cover)
        {
            InitializeComponent();
            this.gamutPointsStand = stand;
            this.gamutPointsTest = test;
            this.gamutPointsTestCoverage = cover;
            this.zoomNum.ValueChanged += ZoomNum_ValueChanged;
            RefreshThis();

        }

        private void ZoomNum_ValueChanged(object sender, HandyControl.Data.FunctionEventArgs<double> e)
        {
            RefreshThis();
        }

        /// <summary>
        /// 刷新控件
        /// </summary>
        private void RefreshThis()
        {
            plotCanvas.Children.Clear();
            plotCanvas.Children.Add(GetPolyline(gamutPointsTest,new SolidColorBrush(Colors.Red)));
            plotCanvas.Children.Add(GetPolyline(gamutPointsStand,new SolidColorBrush(Colors.Green)));
            plotCanvas.Children.Add(GetPolyline(gamutPointsTestCoverage, new SolidColorBrush(Colors.Blue)));
        }

        /// <summary>
        /// 获得画出来的图形
        /// </summary>
        /// <param name="points"></param>
        /// <param name="brush"></param>
        /// <returns></returns>
        private Polyline GetPolyline(List<ColorGamutPoint> points,Brush brush)
        {
            if (points == null) return null;
            Polyline line = new Polyline {Stroke=brush,StrokeThickness=2 };
            points.Add(points[0]);
            foreach (var item in points)
            {
                line.Points.Add(new Point {  X=item.X * zoomNum.Value,Y=item.Y * zoomNum.Value });
            }
            return line;
        }
    }
}
