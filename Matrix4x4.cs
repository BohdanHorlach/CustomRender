using System;


namespace LR_6
{
    public class Matrix4x4
    {
        private double[,] values;


        public Matrix4x4()
        {
            values = new double[4, 4];
            for (int i = 0; i < 4; i++)
                values[i, i] = 1;
        }


        public double this[int row, int col]
        {
            get => values[row, col];
            set => values[row, col] = value;
        }


        public Point3D MultiplyToPoint(Point3D point)
        {
            double x = values[0, 0] * point.X + values[0, 1] * point.Y + values[0, 2] * point.Z + values[0, 3];
            double y = values[1, 0] * point.X + values[1, 1] * point.Y + values[1, 2] * point.Z + values[1, 3];
            double z = values[2, 0] * point.X + values[2, 1] * point.Y + values[2, 2] * point.Z + values[2, 3];
            double w = values[3, 0] * point.X + values[3, 1] * point.Y + values[3, 2] * point.Z + values[3, 3];

            if (w != 0)
            {
                x /= w;
                y /= w;
                z /= w;
            }

            return new Point3D(x, y, z, w);
        }


        public static Matrix4x4 operator *(Matrix4x4 a, Matrix4x4 b)
        {
            var result = new Matrix4x4();
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    result[row, col] = a[row, 0] * b[0, col] +
                                       a[row, 1] * b[1, col] +
                                       a[row, 2] * b[2, col] +
                                       a[row, 3] * b[3, col];
                }
            }
            return result;
        }


        public static Matrix4x4 CreateTranslation(double x, double y, double z)
        {
            var matrix = new Matrix4x4();
            matrix[0, 0] = 1;
            matrix[1, 1] = 1;
            matrix[2, 2] = 1;
            matrix[0, 3] = x;
            matrix[1, 3] = y;
            matrix[2, 3] = z;
            return matrix;
        }


        public static Matrix4x4 RotateOnX(double angle)
        {
            var matrix = new Matrix4x4();
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);
            matrix[1, 1] = cos;
            matrix[1, 2] = -sin;
            matrix[2, 1] = sin;
            matrix[2, 2] = cos;
            return matrix;
        }


        public static Matrix4x4 RotateOnY(double angle)
        {
            var matrix = new Matrix4x4();
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);
            matrix[0, 0] = cos;
            matrix[0, 2] = sin;
            matrix[2, 0] = -sin;
            matrix[2, 2] = cos;
            return matrix;
        }


        public static Matrix4x4 RotateOnZ(double angle)
        {
            var matrix = new Matrix4x4();
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);
            matrix[0, 0] = cos;
            matrix[0, 1] = -sin;
            matrix[1, 0] = sin;
            matrix[1, 1] = cos;
            return matrix;
        }


        public static Matrix4x4 GetPerspective(double fov, double aspect, double near, double far)
        {
            var matrix = new Matrix4x4();

            matrix[0, 0] = 1 / (aspect * Math.Tan(fov / 2));
            matrix[1, 1] = 1 / Math.Tan(fov / 2);
            matrix[2, 2] = -(far + near) / (far - near);
            matrix[2, 3] = -2 * far * near / (far - near);
            matrix[3, 2] = -1;
            matrix[3, 3] = 1;
            return matrix;
        }


        public static Matrix4x4 GetOrthographic(double near, double far)
        {
            var matrix = new Matrix4x4();

            matrix[0, 0] = 1;
            matrix[1, 1] = 1;
            matrix[2, 2] = -2 / (far - near);
            matrix[2, 3] = -(far + near) / (far - near);
            matrix[3, 2] = (far * near) / (near - far);
            matrix[3, 3] = 1;

            return matrix;
        }
    }
}