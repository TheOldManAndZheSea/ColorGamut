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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ColorGamutAppNet.Properties;

namespace ColorGamutAppNet
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : HandyControl.Controls.GlowWindow
    {
        private ColorGamutHelper gamutHelper;
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            this.Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            Settings.Default.S_R_X = stand_R_X.Value;
            Settings.Default.S_R_Y = stand_R_Y.Value;
            Settings.Default.S_G_X = stand_G_X.Value;
            Settings.Default.S_G_Y = stand_G_X.Value;
            Settings.Default.S_B_X = stand_B_X.Value;
            Settings.Default.S_B_Y = stand_B_Y.Value;
            Settings.Default.T_R_X = test_R_X.Value;
            Settings.Default.T_R_Y = test_R_Y.Value;
            Settings.Default.T_G_X = test_G_X.Value;
            Settings.Default.T_G_Y = test_G_Y.Value;
            Settings.Default.T_B_X = test_B_X.Value;
            Settings.Default.T_B_Y = test_B_Y.Value;
            Settings.Default.Save();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            stand_R_X.Value = Settings.Default.S_R_X;
            stand_R_Y.Value = Settings.Default.S_R_Y;
            stand_G_X.Value = Settings.Default.S_G_X;
            stand_G_X.Value = Settings.Default.S_G_Y;
            stand_B_X.Value = Settings.Default.S_B_X;
            stand_B_Y.Value = Settings.Default.S_B_Y;
            test_R_X.Value = Settings.Default.T_R_X;
            test_R_Y.Value = Settings.Default.T_R_Y;
            test_G_X.Value = Settings.Default.T_G_X;
            test_G_Y.Value = Settings.Default.T_G_Y;
            test_B_X.Value = Settings.Default.T_B_X;
            test_B_Y.Value = Settings.Default.T_B_Y;
        }

        private void btn_compute_Click(object sender, RoutedEventArgs e)
        {
            var s_r = new ColorGamutPoint { X = stand_R_X.Value, Y = stand_R_Y.Value };
            var s_g = new ColorGamutPoint { X = stand_G_X.Value, Y = stand_G_Y.Value };
            var s_b = new ColorGamutPoint { X = stand_B_X.Value, Y = stand_B_Y.Value };
            var t_r = new ColorGamutPoint { X = test_R_X.Value, Y = test_R_Y.Value };
            var t_g = new ColorGamutPoint { X = test_G_X.Value, Y = test_G_Y.Value };
            var t_b = new ColorGamutPoint { X = test_B_X.Value, Y = test_B_Y.Value };
            gamutHelper = new ColorGamutHelper(s_r, s_g, s_b, t_r, t_g, t_b);
            txt_standArea.Text = $"标准色域面积：{gamutHelper.StandardGamutArea}";
            txt_testArea.Text = $"测试色域面积：{gamutHelper.GetPolygonArea(new List<ColorGamutPoint> { t_b, t_g, t_r })}";
            txt_GamutCoverageArea.Text = $"对标覆盖面积：{gamutHelper.TestGamutArea}";
            txt_GamutCoverage.Text = $"对标覆盖率：{(gamutHelper.GamutCoverage > 1 ? 1 : gamutHelper.GamutCoverage) * 100}%";
            btn_openImage.IsEnabled = true;
        }

        private void btn_openImage_Click(object sender, RoutedEventArgs e)
        {
            ImageDisplayWindow imageDisplay = new ImageDisplayWindow(gamutHelper.StandardPoints,gamutHelper.TestPoints,gamutHelper.CoveragePoints);
            imageDisplay.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            imageDisplay.Owner = this;
            imageDisplay.ShowDialog();
        }
    }
}
