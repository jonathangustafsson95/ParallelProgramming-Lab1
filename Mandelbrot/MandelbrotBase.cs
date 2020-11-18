using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mandelbrot
{
    public abstract class MandelbrotBase
    {
        public abstract string Name { get; }

        public double LowerX { get; set; }
        public double UpperX { get; set; }
        public double LowerY { get; set; }
        public double UpperY { get; set; }

        public int[,] Image { get; protected set; }

        protected MandelbrotBase(int pixelsX, int pixelsY)
        {
            width = pixelsX;
            height = pixelsY;
            Image = new int[pixelsX, pixelsY];
            LowerX = -1.0;
            UpperX = 1.0;
            LowerY = -1.0;
            UpperY = 1.0;
        }

        public abstract void Compute();

        protected void Compute(Tuple<double, double> xRange, Tuple<double, double> yRange, int[,] image)
        {
            int widthPixels = image.GetLength(0);
            int heightPixels = image.GetLength(1);
            double stepx = (xRange.Item2 - xRange.Item1) / widthPixels;
            double stepy = (yRange.Item2 - yRange.Item1) / heightPixels;

            for (int i = 0; i < widthPixels; i++) {
                for (int j = 0; j < heightPixels; j++) {
                    double tempx = xRange.Item1 + i * stepx;
                    double tempy = yRange.Item1 + j * stepy;
                    int color = Diverge(tempx, tempy);
                    image[i, j] = MAX_ITERATIONS - color;
                }
            }
        }

        protected int Diverge(double cx, double cy)
        {
            int iter = 0;
            double vx = cx, vy = cy;
            while (iter < MAX_ITERATIONS && (vx*vx + vy*vy) < 4) {
                double tx = vx * vx - vy * vy + cx;
                double ty = 2 * vx * vy + cy;
                vx = tx;
                vy = ty;
                iter++;
            }
            return iter;
        }

        protected const int MAX_ITERATIONS = 255;
        protected readonly int width;
        protected readonly int height;
    }
}
