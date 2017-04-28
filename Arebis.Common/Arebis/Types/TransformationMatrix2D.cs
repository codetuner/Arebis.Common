using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Types
{
    /// <summary>
    /// Represents a transformation matrix to handle transformations in a 2D coordinate space.
    /// Use the static methods to create translating, scaling and rotating matrices.
    /// </summary>
    public class TransformationMatrix2D
    {
        private double[,] values = new double[3, 3];

        /// <summary>
        /// Creates a new blank 2D transformation matrix.
        /// </summary>
        public TransformationMatrix2D()
        { }

        /// <summary>
        /// Values of the 3x3 2D transformation matrix.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public double this[int row, int column]
        {
            get
            {
                return this.values[row, column];
            }
            set
            {
                this.values[row, column] = value;
            }
        }

        /// <summary>
        /// Returns an identity 2D transformation matrix.
        /// </summary>
        public static TransformationMatrix2D Identity()
        {
            var result = new TransformationMatrix2D();
            result[0, 0] = 1;
            result[0, 1] = 0;
            result[0, 2] = 0;
            result[1, 0] = 0;
            result[1, 1] = 1;
            result[1, 2] = 0;
            result[2, 0] = 0;
            result[2, 1] = 0;
            result[2, 2] = 1;
            return result;
        }

        /// <summary>
        /// Returns a 2D transformation matrix for translating (displacing).
        /// </summary>
        public static TransformationMatrix2D Translating(double x, double y)
        {
            var result = new TransformationMatrix2D();
            result[0, 0] = 1;
            result[0, 1] = 0;
            result[0, 2] = x;
            result[1, 0] = 0;
            result[1, 1] = 1;
            result[1, 2] = y;
            result[2, 0] = 0;
            result[2, 1] = 0;
            result[2, 2] = 1;
            return result;
        }

        /// <summary>
        /// Returns a 2D transformation matrix for scaling.
        /// </summary>
        public static TransformationMatrix2D Scaling(double sxy)
        {
            return Scaling(sxy, sxy);
        }

        /// <summary>
        /// Returns a 2D transformation matrix for scaling.
        /// </summary>
        public static TransformationMatrix2D Scaling(double sx, double sy)
        {
            var result = new TransformationMatrix2D();
            result[0, 0] = sx;
            result[0, 1] = 0;
            result[0, 2] = 0;
            result[1, 0] = 0;
            result[1, 1] = sy;
            result[1, 2] = 0;
            result[2, 0] = 0;
            result[2, 1] = 0;
            result[2, 2] = 1;
            return result;
        }

        /// <summary>
        /// Returns a 2D transformation matrix for rotation given the angle in degrees (0-360°).
        /// </summary>
        public static TransformationMatrix2D RotatingDegrees(double degrees)
        {
            return Rotating(degrees / 360.0 * 2 * Math.PI);
        }

        /// <summary>
        /// Returns a 2D transformation matrix for rotation given the rotation fraction (where 1/4th is a quarter turn or 90% or PI/2 rad).
        /// </summary>
        public static TransformationMatrix2D RotatingFraction(double fraction)
        {
            return Rotating(fraction * 2 * Math.PI);
        }

        /// <summary>
        /// Returns a 2D transformation matrix for rotation given the angle in radians.
        /// </summary>
        public static TransformationMatrix2D Rotating(double rad)
        {
            var result = new TransformationMatrix2D();
            result[0, 0] = Math.Cos(rad);
            result[0, 1] = -Math.Sin(rad);
            result[0, 2] = 0;
            result[1, 0] = Math.Sin(rad);
            result[1, 1] = Math.Cos(rad);
            result[1, 2] = 0;
            result[2, 0] = 0;
            result[2, 1] = 0;
            result[2, 2] = 1;
            return result;
        }

        /// <summary>
        /// Applies this transformation matrix to the given x and y coordinates and returns new x and y coordinates.
        /// </summary>
        public double[] Apply(params double[] coordinates)
        {
            var x = coordinates[0];
            var y = coordinates[1];
            var w = (coordinates.Length >= 3) ? coordinates[2] : 1;

            var result = new double[3];
            result[0] = this[0, 0] * x + this[0, 1] * y + this[0, 2] * w;
            result[1] = this[1, 0] * x + this[1, 1] * y + this[1, 2] * w;
            result[2] = this[2, 0] * x + this[2, 1] * y + this[2, 2] * w;

            return result;
        }

        /// <summary>
        /// Multiplies the matrix with a given factor.
        /// </summary>
        public static TransformationMatrix2D operator *(double factor, TransformationMatrix2D matrix)
        {
            var result = new TransformationMatrix2D();
            for (int r = 0; r < 3; r++)
                for (int c = 0; c < 3; c++)
                    result[r, c] = factor * matrix[r, c];

            return result;
        }

        /// <summary>
        /// Combines matrices by multiplication.
        /// </summary>
        public static TransformationMatrix2D operator *(TransformationMatrix2D matrix1, TransformationMatrix2D matrix2)
        {
            var result = new TransformationMatrix2D();
            for (int r = 0; r < 3; r++)
                for (int c = 0; c < 3; c++)
                    for(int m=0; m<3; m++)
                        result[r, c] += matrix1[r, m] * matrix2[m, c];

            return result;
        }

        /// <summary>
        /// Combines matrices by addition.
        /// </summary>
        public static TransformationMatrix2D operator +(TransformationMatrix2D matrix1, TransformationMatrix2D matrix2)
        {
            var result = new TransformationMatrix2D();
            for (int r = 0; r < 3; r++)
                for (int c = 0; c < 3; c++)
                    result[r, c] = matrix1[r, c] + matrix2[r, c];

            return result;
        }
    }
}
