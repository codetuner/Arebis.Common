using Arebis.Pdf.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Pdf.Model
{
    public class Coordinates
    {
        public Coordinates(double px1, double py1, double px2, double py2)
            : this(new double[] { px1, py1, px2, py2 })
        { }

        public Coordinates(double[] physical)
            : this(physical, new double[] { 0, 0, physical[2] - physical[0], physical[3] - physical[1] })
        { }

        public Coordinates(double[] physical, double lx1, double ly1, double? lx2, double? ly2)
            : this(physical, new double[] { lx1, ly1, lx2 ?? physical[2], ly2 ?? ((physical[1] - physical[3]) / (physical[2] - physical[0]) * ((lx2 ?? physical[2]) - lx1) + ly1) })
        { }

        public Coordinates(double[] physical, double[] logical)
        {
            this.Physical = physical;
            this.Logical = logical;
        }

        public double[] Physical { get; set; }

        public double[] Logical { get; set; }

        public double LogicalWidth
        {
            get
            {
                return Math.Abs(this.Logical[2] - this.Logical[0]);
            }
        }

        public double LogicalHeight
        {
            get
            {
                return Math.Abs(this.Logical[3] - this.Logical[1]);
            }
        }

        public double PhysicalWidth
        {
            get
            {
                return Math.Abs(this.Physical[2] - this.Physical[0]);
            }
        }

        public double PhysicalHeight
        {
            get
            {
                return Math.Abs(this.Physical[3] - this.Physical[1]);
            }
        }
        
        public double[] Translate(params double[] logicalPointXY)
        {
            return new double[]
            {
                (logicalPointXY[0] - this.Logical[0]) * (this.Physical[2] - this.Physical[0]) / (this.Logical[2] - this.Logical[0]) + this.Physical[0],
                (logicalPointXY[1] - this.Logical[1]) * (this.Physical[3] - this.Physical[1]) / (this.Logical[3] - this.Logical[1]) + this.Physical[1]
            };
        }
    }
}
