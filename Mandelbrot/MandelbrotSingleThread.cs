using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
//using System.Windows.Media;
using System.Text;
using System.Threading.Tasks;

namespace Mandelbrot
{
    public class MandelbrotSingleThread : MandelbrotBase
    {
        public override string Name { 
            get { return "MandelbrotSingleThread"; }
        }

        public MandelbrotSingleThread(int pixelsX, int pixelsY) : base(pixelsX, pixelsY)
        {
        }

        public override void Compute()
        {
            Compute(new Tuple<double, double>(LowerX, UpperX),
                    new Tuple<double, double>(LowerY, UpperY),
                    Image);
        }
    }
}
