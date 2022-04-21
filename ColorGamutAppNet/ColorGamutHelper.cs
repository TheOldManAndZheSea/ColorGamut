using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
/*****************************************************

*　　功能　　　　　　求色域覆盖率

*　　作者　　　　　　张天赐

*　　时间　　　　　　2022.04.14

*　　

*　　修改说明　　　　.......


*******************************************************/
namespace ColorGamutLib
{
    /// <summary>
    /// 色域覆盖率算法类
    /// </summary>
    public class ColorGamutHelper
    {
        /// <summary>
        /// 色域覆盖率算法类
        /// </summary>
        /// <param name="sr">标准R坐标</param>
        /// <param name="sg">标准G坐标</param>
        /// <param name="sb">标准B坐标</param>
        /// <param name="tr">测试R坐标</param>
        /// <param name="tg">测试G坐标</param>
        /// <param name="tb">测试B坐标</param>
        public ColorGamutHelper(ColorGamutPoint sr, ColorGamutPoint sg, ColorGamutPoint sb, ColorGamutPoint tr, ColorGamutPoint tg, ColorGamutPoint tb)
        {
            this.Standard_R = sr;
            this.Standard_G = sg;
            this.Standard_B = sb;
            this.Test_R = tr;
            this.Test_G = tg;
            this.Test_B = tb;
            TestPoints = new List<ColorGamutPoint> { tr,tg,tb };
            StandardPoints = new List<ColorGamutPoint> { sr, sg, sb };
            GetStandardGamutArea();
            GetTestGamutCoverage();
        }
        #region 属性
        /// <summary>
        /// 标准红坐标
        /// </summary>
        public ColorGamutPoint Standard_R { get; set; }
        /// <summary>
        /// 标准绿坐标
        /// </summary>
        public ColorGamutPoint Standard_G { get; set; }
        /// <summary>
        /// 标准蓝坐标
        /// </summary>
        public ColorGamutPoint Standard_B { get; set; }

        /// <summary>
        /// 测试红坐标
        /// </summary>
        public ColorGamutPoint Test_R { get; set; }
        /// <summary>
        /// 测试绿坐标
        /// </summary>
        public ColorGamutPoint Test_G { get; set; }
        /// <summary>
        /// 测试蓝坐标
        /// </summary>
        public ColorGamutPoint Test_B { get; set; }
        /// <summary>
        /// 标准色域覆盖面积 保留四位小数
        /// </summary>
        public double StandardGamutArea { get; set; }
        /// <summary>
        /// 测试色域覆盖面积 保留四位小数
        /// </summary>
        public double TestGamutArea { get; set; }
        /// <summary>
        /// 覆盖率 保留四位小数
        /// </summary>
        public double GamutCoverage { get; set; }
        /// <summary>
        /// 测试坐标集合
        /// </summary>
        public List<ColorGamutPoint> TestPoints { get; set; }
        /// <summary>
        /// 标准坐标集合
        /// </summary>
        public List<ColorGamutPoint> StandardPoints { get; set; }
        /// <summary>
        /// 实际覆盖坐标集合
        /// </summary>
        public List<ColorGamutPoint> CoveragePoints { get; set; }

        #endregion

        #region 方法
        /// <summary>
        /// 获取标准gamma色域值
        /// </summary>
        public bool GetStandardGamutArea()
        {
            StandardGamutArea = GetPolygonArea(new List<ColorGamutPoint> { Standard_R, Standard_G, Standard_B });
            if (StandardGamutArea > 0) return true;
            return false;
        }
        /// <summary>
        /// 计算测试色域覆盖率
        /// </summary>
        public bool GetTestGamutCoverage()
        {
            if (StandardGamutArea == 0) return false;
            this.CoveragePoints = GetGamutNodals();
            TestGamutArea = GetPolygonArea(this.CoveragePoints);
            if (TestGamutArea > 0)
            {
                GamutCoverage = Math.Round(TestGamutArea / StandardGamutArea, 4);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 获取覆盖坐标交点集合
        /// </summary>
        /// <returns></returns>
        public List<ColorGamutPoint> GetGamutNodals()
        {
            ColorGamutPoint point1 = CrossPoint(Test_B, Test_G, Standard_G, Standard_R);//
            ColorGamutPoint point2 = CrossPoint(Test_R, Test_G, Standard_R, Standard_G);
            ColorGamutPoint point3 = CrossPoint(Test_R, Test_G, Standard_R, Standard_B);//

            ColorGamutPoint point4 = CrossPoint(Test_R, Test_B, Standard_R, Standard_B);//
            ColorGamutPoint point5 = CrossPoint(Test_G, Test_B, Standard_R, Standard_B);

            ColorGamutPoint point6 = CrossPoint(Test_G, Test_B, Standard_G, Standard_B);
            ColorGamutPoint point7 = CrossPoint(Test_R, Test_B, Standard_G, Standard_B);//

            //ColorGamutPoint point8 = CrossPoint(Test_G, Test_R, Standard_B, Standard_R);//
            ColorGamutPoint point8 = CrossPoint(Test_G, Test_R, Standard_G, Standard_B);//
            List<ColorGamutPoint> points = new List<ColorGamutPoint> { point1, point2, point3, point4, point5, point6, point7, point8 };
            //List<ColorGamutPoint> points = new List<ColorGamutPoint> {  point5 };
            points = points.Where(s => (s.X > 0 && s.Y > 0)).Distinct(new DistinctTest<ColorGamutPoint>()).ToList();
            //if (points.Count<3)//此时必定有一个交藏在了其中一个三角形中
            //{
            //var point0 = new ColorGamutPoint { X=0,Y=0};
            if (point1.X == 0)
            {
                //判断 测试点是否在标准三角形内 计算夹角 和为360则在标准三角形内
                var testAngle = Angle(Test_G, Standard_G, Standard_B) + Angle(Test_G, Standard_G, Standard_R) + Angle(Test_G, Standard_R, Standard_B);
                if ((int)testAngle == 360)
                {
                    points.Add(Test_G);
                }
                else
                {
                    points.Add(Standard_G);
                }
                //points.Add(Test_G);

            }
            if (point3.X == 0)
            {
                var testAngle = Angle(Test_R, Standard_G, Standard_B) + Angle(Test_R, Standard_G, Standard_R) + Angle(Test_R, Standard_R, Standard_B);
                if ((int)testAngle == 360)
                {
                    points.Add(Test_R);
                }
                else
                {
                    points.Add(Standard_R);
                }
                //points.Add(Test_R);
            }
            if (point5.X == 0)
            {
                var testAngle = Angle(Test_B, Standard_G, Standard_B) + Angle(Test_B, Standard_G, Standard_R) + Angle(Test_B, Standard_R, Standard_B);
                if ((int)testAngle == 360)
                {
                    points.Add(Test_B);
                }
                else
                {
                    points.Add(Standard_B);
                }
                //points.Add(Test_B);
            }
            //}
            return points;
        }
        /// <summary>
        /// 求三个点的夹角度数
        /// </summary>
        /// <param name="cen"></param>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public double Angle(ColorGamutPoint cen, ColorGamutPoint first, ColorGamutPoint second)
        {
            const double M_PI = 3.1415926535897;

            double ma_x = first.X - cen.X;
            double ma_y = first.Y - cen.Y;
            double mb_x = second.X - cen.X;
            double mb_y = second.Y - cen.Y;
            double v1 = (ma_x * mb_x) + (ma_y * mb_y);
            double ma_val = Math.Sqrt(ma_x * ma_x + ma_y * ma_y);
            double mb_val = Math.Sqrt(mb_x * mb_x + mb_y * mb_y);
            double cosM = v1 / (ma_val * mb_val);
            double angleAMB = Math.Acos(cosM) * 180 / M_PI;

            return angleAMB;
        }
        /// <summary>
        /// 通过多边形的所有点坐标 计算多边形的面积
        /// </summary>
        /// <param name="vectorPoints">点坐标集合</param>
        /// <returns></returns>
        public double GetPolygonArea(List<ColorGamutPoint> vectorPoints)
        {
            var NvectorPoints = SortPolyPoints(vectorPoints);//需要对传入的坐标进行顺时针排序 否则计算出来的面积不正确
            int j;
            int n = NvectorPoints.Count;
            double area = 0;
            for (int i = 0; i < n; i++)
            {
                j = (i + 1) % n;
                area += NvectorPoints[i].X * NvectorPoints[j].Y - NvectorPoints[j].X * NvectorPoints[i].Y;
            }
            if (area < 0) area = -area;
            return Math.Round(area / 2, 4);
        }
        /// <summary>
        /// 多边形点集排序
        /// </summary>
        /// <param name="vPoints"></param>
        /// <returns></returns>
        public List<ColorGamutPoint> SortPolyPoints(List<ColorGamutPoint> vPoints)
        {
            if (vPoints == null || vPoints.Count == 0) return null;
            //计算重心
            ColorGamutPoint center = new ColorGamutPoint();
            double X = 0, Y = 0;
            for (int i = 0; i < vPoints.Count; i++)
            {
                X += vPoints[i].X;
                Y += vPoints[i].Y;
            }
            center = new ColorGamutPoint { X=X / vPoints.Count, Y=Y / vPoints.Count };
            //冒泡排序
            for (int i = 0; i < vPoints.Count - 1; i++)
            {
                for (int j = 0; j < vPoints.Count - i - 1; j++)
                {
                    if (PointCmp(vPoints[j], vPoints[j + 1], center))
                    {
                        ColorGamutPoint tmp = vPoints[j];
                        vPoints[j] = vPoints[j + 1];
                        vPoints[j + 1] = tmp;
                    }
                }
            }
            return vPoints;
        }

        /// <summary>
        /// 若点a大于点b,即点a在点b顺时针方向,返回true,否则返回false
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="center"></param>
        /// <returns></returns>
        private bool PointCmp(ColorGamutPoint a, ColorGamutPoint b, ColorGamutPoint center)
        {
            if (a.X >= 0 && b.X < 0)
                return true;
            else if (a.X == 0 && b.X == 0)
                return a.Y > b.Y;
            //向量OA和向量OB的叉积
            double det = (a.X - center.X) * (b.Y - center.Y) - (b.X - center.X) * (a.Y - center.Y);
            if (det < 0)
                return true;
            if (det > 0)
                return false;
            //向量OA和向量OB共线，以距离判断大小
            double d1 = (a.X - center.X) * (a.X - center.X) + (a.Y - center.Y) * (a.Y - center.Y);
            double d2 = (b.X - center.X) * (b.X - center.X) + (b.Y - center.Y) * (b.Y - center.Y);
            return d1 > d2;
        }

        /// <summary>
        /// 计算两条直线的交点坐标
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <param name="line3"></param>
        /// <param name="line4"></param>
        /// <returns></returns>
        public ColorGamutPoint CrossPoint(ColorGamutPoint line1, ColorGamutPoint line2, ColorGamutPoint line3, ColorGamutPoint line4) //交点
        {
            //如果有两点相等 直接返回这个点
            if (line1.Equals(line3) || line1.Equals(line4))
            {
                return line1;
            }
            else if (line2.Equals(line3) || line2.Equals(line4))
            {
                return line2;
            }
            double x1, x2, x3, x4, y1, y2, y3, y4;
            x1 = line1.X;
            y1 = line1.Y;
            x2 = line2.X;
            y2 = line2.Y;
            x3 = line3.X;
            y3 = line3.Y;
            x4 = line4.X;
            y4 = line4.Y;
            // double计算容差
            double rong = 1e-8;

            //求 最大x 和最大Y
            double[] X_Arry = new double[] { x1, x2, x3, x4 };
            double maxX = GetApproachs(X_Arry)[1];
            double minX = GetApproachs(X_Arry)[0];
            //double maxX = X_Arry.Max();
            //double minX = X_Arry.Min();
            double[] Y_Arry = new double[] { y1, y2, y3, y4 };
            double maxY = GetApproachs(Y_Arry)[1];
            double minY = GetApproachs(Y_Arry)[0];
            //double maxY = Y_Arry.Max();
            //double minY = Y_Arry.Min();

            //求相邻最近的两个点 最相邻的就是最容易相交的
            double value = GetPointsLines(line1, line2);
            double value1 = GetPointsLines(line3, line4);

            if (value > value1)
            {
                if (line3.X > line4.X)
                {
                    maxX = line3.X;
                    minX = line4.X;
                }
                else
                {
                    maxX = line4.X;
                    minX = line3.X;
                }
                if (line3.Y > line4.Y)
                {
                    maxY = line3.Y;
                    minY = line4.Y;
                }
                else
                {
                    maxY = line4.Y;
                    minY = line3.Y;
                }
            }
            else
            {
                if (line1.X > line2.X)
                {
                    maxX = line1.X;
                    minX = line2.X;
                }
                else
                {
                    maxX = line2.X;
                    minX = line1.X;
                }
                if (line1.Y > line2.Y)
                {
                    maxY = line1.Y;
                    minY = line2.Y;
                }
                else
                {
                    maxY = line2.Y;
                    minY = line1.Y;
                }
            }

            //因为求斜率需要用除法,分母可能为0,所以求斜率之前,
            //需要两条线是否x轴平行或者y轴平行
            if ((Math.Abs(x2 - x1) < rong) && (Math.Abs(x4 - x3) < rong))
            {
                return new ColorGamutPoint { X = 0, Y = 0 };
            }
            if ((Math.Abs(y2 - y1) < rong) && (Math.Abs(y4 - y3) < rong))
            {
                return new ColorGamutPoint { X = 0, Y = 0 };
            }

            //求斜率,分母为0并不报错,而是赋值成 Infinity
            double a = Math.Round((y2 - y1) / (x2 - x1), 4); //需考虑分母不能为0 即x2=x1 l1垂直于x轴
            double b = Math.Round((y4 - y3) / (x4 - x3), 4); //需考虑分母不能为0 即x4=x3 l2垂直于x轴

            double _x, _y = 0;

            //L1或L2两直线可能其中一个有Y轴平行(垂直X轴)的
            if (Math.Abs(x2 - x1) < rong) //L1垂直于x轴  则x=x1=x2,(x2 - x1)是0==a分母,a=Infinity正无穷
            {
                _x = x1;
                _y = Math.Round(b * x1 - b * x3 + y3, 4);//公式变换第一种
                if (minX <= _x && _x <= maxX && _y <= maxY && minY <= _y)
                {
                    return new ColorGamutPoint { X = Math.Round(_x, 4), Y = Math.Round(_y, 4) };
                }

            }
            else if (Math.Abs(x4 - x3) < rong) //L2垂直于x轴 则x=x3=x4,(x4 - x3)是0==b分母,b=Infinity正无穷
            {
                _x = x3;
                _y = Math.Round(a * _x - a * x1 + y1, 4);//公式变换第一种
                if (minX <= _x && _x <= maxX && _y <= maxY && minY <= _y)
                {
                    return new ColorGamutPoint { X = Math.Round(_x, 4), Y = Math.Round(_y, 4) };
                }
            }

            //两条直线都是非垂直状态

            /* 知道了点和斜率,那么两条点斜式方程联立.
               因为直线方程是一条参照线,两端无限延长,除非平行,否则必有交点.
               又因为交点是两条线的共同解,所以点斜式:line1和line2的y相减是0,y=k(_x-x1)+y1
               所以未知数y就相减去掉,剩下x,来求y. 
               反之,也可以相减去掉x,来求y.
               [y=a(_x-x1)+y1] - [y=b(_x-x3)+y3] =0; 
               [a*(_x-x1)+y1] - [b*(_x-x3)+y3] =0;
               [a*_x-a*x1+y1] - [b*_x-b*x3+y3] =0;
               a*_x-a*x1+y1 - b*_x+b*x3-y3 =0; 去括号,+-互变 
               (a - b)* _x = 0 + a*x1 - y1 - b*x3 + y3; //移项,+-互变              
               _x = (a * x1 - y1 - b * x3 + y3) / (a - b);
            */
            _x = Math.Round((a * x1 - y1 - b * x3 + y3) / (a - b), 4);

            //但是上面程序代码已经算了_x了,直接套入点斜式方程,偷懒...也可以通过公式计算 
            // y-y1=k*x-k*x1
            // y-y1+k*x1=k*x
            // (y-y1+k*x1)/k=x 
            // [(y-y1-a*x1)/a] - [(y-y3-b*x3)/b] =0; //这是按照公式的方法

            _y = Math.Round(a * _x - a * x1 + y1, 4);  // 点斜式方程 y-y1=k(x-x1)
            if (minX <= _x && _x <= maxX && _y <= maxY && minY <= _y)
            {
                ColorGamutPoint colorGamut = new ColorGamutPoint { X = Math.Round(_x, 4), Y = Math.Round(_y, 4) };
                //判断交点不能超出两点长度
                if (GetPointsLines(line1, colorGamut) > value || GetPointsLines(line2, colorGamut) > value)
                {
                    return new ColorGamutPoint { X = 0, Y = 0 };
                }
                if (GetPointsLines(line3, colorGamut) > value1 || GetPointsLines(line4, colorGamut) > value1)
                {
                    return new ColorGamutPoint { X = 0, Y = 0 };
                }
                return colorGamut;
            }
            if (maxX == minX && minY == maxY)
            {
                return new ColorGamutPoint { X = minX, Y = minY };
            }
            return new ColorGamutPoint { X = 0, Y = 0 };
        }
        /// <summary>
        /// 求两坐标连线距离
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <returns></returns>
        public double GetPointsLines(ColorGamutPoint line1, ColorGamutPoint line2)
        {
            return Math.Sqrt(Math.Abs(line1.X - line2.X) * Math.Abs(line1.X - line2.X) + Math.Abs(line1.Y - line2.Y) * Math.Abs(line1.Y - line2.Y));
        }
        /// <summary>
        /// 获取最接近的两个点
        /// </summary>
        /// <param name="arry"></param>
        /// <returns></returns>
        private double[] GetApproachs(double[] arry)
        {
            arry = arry.OrderBy(i => i).ToArray();
            double[] result = new double[2];
            var a = arry[0] - arry[1];
            var b = arry[0] - arry[2];
            var c = arry[0] - arry[3];
            var d = arry[1] - arry[2];
            var e = arry[1] - arry[3];
            var f = arry[2] - arry[3];
            var tempArry = new double[] { a >= 0 ? a : -a, b >= 0 ? b : 0, c >= 0 ? c : 0, d >= 0 ? d : 0, e >= 0 ? e : 0, f >= 0 ? f : 0 };
            var min = tempArry.Min();
            if (a == min)
            {
                result[0] = arry[0];
                result[1] = arry[1];
            }
            else if (b == min)
            {
                result[0] = arry[0];
                result[1] = arry[2];
            }
            else if (c == min)
            {
                result[0] = arry[0];
                result[1] = arry[3];
            }
            else if (d == min)
            {
                result[0] = arry[1];
                result[1] = arry[2];
            }
            else if (e == min)
            {
                result[0] = arry[1];
                result[1] = arry[3];
            }
            else if (f == min)
            {
                result[0] = arry[2];
                result[1] = arry[3];
            }
            return result;
        }
        #endregion
    }

    public class ColorGamutPoint : IEquatable<ColorGamutPoint>
    {
        public double X { get; set; }

        public double Y { get; set; }

        public bool Equals(ColorGamutPoint other)
        {
            return this.X == other.X && this.Y == other.Y;
        }
    }

    class DistinctTest<TModel> : IEqualityComparer<TModel>
    {
        public bool Equals(TModel x, TModel y)
        {
            //Test
            ColorGamutPoint t = x as ColorGamutPoint;
            ColorGamutPoint tt = y as ColorGamutPoint;
            if (t != null && tt != null) return t.X == tt.X && t.Y == tt.Y;
            return false;
        }

        public int GetHashCode(TModel obj)
        {
            return obj.ToString().GetHashCode();
        }
    }

}
