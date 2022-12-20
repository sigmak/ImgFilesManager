using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ImgFilesManager
{
    /// <summary>
    /// 그래프
    /// </summary>
    public partial class Graph : PictureBox
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////// Field
        ////////////////////////////////////////////////////////////////////////////////////////// Private

        #region Field

        /// <summary>
        /// 커브 그리기 리스트
        /// </summary>
        private List<PlotCurve> plotCurveList;

        #endregion

        //////////////////////////////////////////////////////////////////////////////////////////////////// Property
        ////////////////////////////////////////////////////////////////////////////////////////// Public

        #region 커브 그리기 리스트 - PlotCurveList

        /// <summary>
        /// 커브 그리기 리스트
        /// </summary>
        private IList<PlotCurve> PlotCurveList
        {
            get
            {
                if(this.plotCurveList == null)
                {
                    this.plotCurveList = new List<PlotCurve>();
                }

                return this.plotCurveList;             
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////////////////////////// Constructor
        ////////////////////////////////////////////////////////////////////////////////////////// Public

        #region 생성자 - Graph()

        /// <summary>
        /// 생성자
        /// </summary>
        public Graph()
        {
            InitializeComponent();

            BorderStyle = BorderStyle.FixedSingle;
            BackColor   = Color.White;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////////////////////////// Method
        ////////////////////////////////////////////////////////////////////////////////////////// Public

        #region 커브 그리기 추가하기 - AddPlotCurve(plotColor, curveArray)

        /// <summary>
        /// 커브 그리기 추가하기
        /// </summary>
        /// <param name="plotColor">그리기 색상</param>
        /// <param name="curveArray">커브 배열</param>
        public void AddPlotCurve(Color plotColor, double[] curveArray)
        {
            PlotCurveList.Add(new PlotCurve(plotColor, curveArray));
        }

        #endregion
        #region 커브 그리기 리스트 지우기 - ClearPlotCurveList()

        /// <summary>
        /// 커브 그리기 리스트 지우기
        /// </summary>
        public void ClearPlotCurveList()
        {
            PlotCurveList.Clear();    
        }

        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////// Protected

        #region 페인트시 처리하기 - OnPaint(e)

        /// <summary>
        /// 페인트시 처리하기
        /// </summary>
        /// <param name="e">이벤트 인자</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if((this.PlotCurveList == null) || (this.PlotCurveList.Count == 0))
            {
                return;
            }

            Graphics graphics = e.Graphics;

            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode     = SmoothingMode.HighQuality;

            foreach(PlotCurve plotCurve in PlotCurveList)
            {
                Point[] pointArray = GetPointArray(plotCurve.CurveArray, Width, Height);

                graphics.DrawCurve(new Pen(plotCurve.PlotColor), pointArray);
            }
        }

        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////// Private

        #region 포인트 배열 구하기 - GetPointArray(sourceArray, width, height)

        /// <summary>
        /// 포인트 배열 구하기
        /// </summary>
        /// <param name="sourceArray">소스 배열</param>
        /// <param name="width">너비</param>
        /// <param name="height">높이</param>
        /// <returns>포인트 배열</returns>
        private static Point[] GetPointArray(double[] sourceArray, int width, int height)
        {
            double      horizontalIncrement = (double)width / (double)sourceArray.Length;
            double      currentX            = 0;
            List<Point> pointList           = new List<Point>();

            height -= 4;         

            foreach(double source in sourceArray)
            {
                int y = height - ((int)Math.Round(source * height)) + 2;
                int x = (int)Math.Round(currentX);

                pointList.Add(new Point(x, y));                

                currentX += horizontalIncrement;
            }

            return pointList.ToArray();
        }

        #endregion
    }
}