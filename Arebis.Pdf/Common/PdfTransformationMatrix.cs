using System;

namespace Arebis.Pdf.Common
{
    /// <summary>
    /// A PdfTransformationMatrix.
    /// </summary>
    [Serializable]
    public class PdfTransformationMatrix
    {
        public PdfTransformationMatrix()
            : this(1.0, 0.0, 0.0, 1.0, 0.0, 0.0)
        { }

        public PdfTransformationMatrix(double a, double b, double c, double d, double e, double f)
        {
            this.A = a;
            this.B = b;
            this.C = c;
            this.D = d;
            this.E = e;
            this.F = f;
        }

        public PdfTransformationMatrix(double[] values)
        {
            this.A = values[0];
            this.B = values[1];
            this.C = values[2];
            this.D = values[3];
            this.E = values[4];
            this.F = values[5];
        }

        public double A { get; set; }

        public double B { get; set; }
        
        public double C { get; set; }
        
        public double D { get; set; }
        
        public double E { get; set; }
        
        public double F { get; set; }
    }
}
