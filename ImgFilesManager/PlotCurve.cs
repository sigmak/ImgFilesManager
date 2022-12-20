using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImgFilesManager
{
    /// <summary>
    /// 커브 그리기
    /// </summary>
    public class PlotCurve
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////// Field
        ////////////////////////////////////////////////////////////////////////////////////////// Private

        #region Field

        /// <summary>
        /// 그리기 색상
        /// </summary>
        private readonly Color plotColor;

        /// <summary>
        /// 커브 배열
        /// </summary>
        private readonly double[] curveArray;

        #endregion

        //////////////////////////////////////////////////////////////////////////////////////////////////// Property
        ////////////////////////////////////////////////////////////////////////////////////////// Public

        #region 그리기 색상 - PlotColor

        /// <summary>
        /// 그리기 색상
        /// </summary>
        public Color PlotColor
        {
            get
            {
                return this.plotColor;
            }
        }

        #endregion
        #region 커브 배열 - CurveArray

        /// <summary>
        /// 커브 배열
        /// </summary>
        public double[] CurveArray
        {
            get
            {
                return this.curveArray;
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////////////////////////// Constructor
        ////////////////////////////////////////////////////////////////////////////////////////// Public

        #region 생성자 - PlotCurve(plotColor, curveArray)

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="plotColor">그리기 색상</param>
        /// <param name="curveArray">커브 배열</param>
        public PlotCurve(Color plotColor, double[] curveArray)
        {
            this.plotColor = plotColor;
            this.curveArray = curveArray;
        }

        #endregion
    }

}
