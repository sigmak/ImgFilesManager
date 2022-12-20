using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ImgFilesManager
{
    /// <summary>
    /// �׷���
    /// </summary>
    public partial class Graph : PictureBox
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////// Field
        ////////////////////////////////////////////////////////////////////////////////////////// Private

        #region Field

        /// <summary>
        /// Ŀ�� �׸��� ����Ʈ
        /// </summary>
        private List<PlotCurve> plotCurveList;

        #endregion

        //////////////////////////////////////////////////////////////////////////////////////////////////// Property
        ////////////////////////////////////////////////////////////////////////////////////////// Public

        #region Ŀ�� �׸��� ����Ʈ - PlotCurveList

        /// <summary>
        /// Ŀ�� �׸��� ����Ʈ
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

        #region ������ - Graph()

        /// <summary>
        /// ������
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

        #region Ŀ�� �׸��� �߰��ϱ� - AddPlotCurve(plotColor, curveArray)

        /// <summary>
        /// Ŀ�� �׸��� �߰��ϱ�
        /// </summary>
        /// <param name="plotColor">�׸��� ����</param>
        /// <param name="curveArray">Ŀ�� �迭</param>
        public void AddPlotCurve(Color plotColor, double[] curveArray)
        {
            PlotCurveList.Add(new PlotCurve(plotColor, curveArray));
        }

        #endregion
        #region Ŀ�� �׸��� ����Ʈ ����� - ClearPlotCurveList()

        /// <summary>
        /// Ŀ�� �׸��� ����Ʈ �����
        /// </summary>
        public void ClearPlotCurveList()
        {
            PlotCurveList.Clear();    
        }

        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////// Protected

        #region ����Ʈ�� ó���ϱ� - OnPaint(e)

        /// <summary>
        /// ����Ʈ�� ó���ϱ�
        /// </summary>
        /// <param name="e">�̺�Ʈ ����</param>
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

        #region ����Ʈ �迭 ���ϱ� - GetPointArray(sourceArray, width, height)

        /// <summary>
        /// ����Ʈ �迭 ���ϱ�
        /// </summary>
        /// <param name="sourceArray">�ҽ� �迭</param>
        /// <param name="width">�ʺ�</param>
        /// <param name="height">����</param>
        /// <returns>����Ʈ �迭</returns>
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