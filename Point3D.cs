using System;


namespace LR_6
{
    public struct Point3D
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double W { get; set; }


        public Point3D(double x, double y, double z, double w = 1)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }


        public static Point3D CrossProduct(Point3D point1, Point3D point2)
        {
            Point3D c = new Point3D();

            c.X = point1.Y * point2.Z - point1.Z * point2.Y;
            c.Y = point1.Z * point2.X - point1.X * point2.Z;
            c.Z = point1.X * point2.Y - point1.Y * point2.X;

            return c;
        }


        public Point3D GetNormilizeVector()
        {
            Point3D point = new Point3D();

            double magnitude = Math.Sqrt((X * X) + (Y * Y) + (Z * Z));

            point.X = this.X / magnitude;
            point.Y = this.Y / magnitude;
            point.Z = this.Z / magnitude;

            return point;
        }


        public double DotProduct(Point3D point)
        {
            return this.X * point.X + this.Y * point.Y + this.Z * point.Z;
        }


        public static Point3D operator - (Point3D point1, Point3D point2)
        {
            Point3D newPoint = new Point3D();

            newPoint.X = point1.X - point2.X;
            newPoint.Y = point1.Y - point2.Y;
            newPoint.Z = point1.Z - point2.Z;

            return newPoint;
        }


        public static Point3D operator -(Point3D point1)
        {
            Point3D newPoint = new Point3D();

            newPoint.X = -point1.X;
            newPoint.Y = -point1.Y;
            newPoint.Z = -point1.Z;

            return newPoint;
        }


        public static Point3D operator + (Point3D point1, Point3D point2)
        {
            Point3D newPoint = new Point3D();

            newPoint.X = point1.X + point2.X;
            newPoint.Y = point1.Y + point2.Y;
            newPoint.Z = point1.Z + point2.Z;

            return newPoint;
        }


        public static Point3D operator * (Point3D point1, double value)
        {
            Point3D newPoint = new Point3D();

            newPoint.X = point1.X * value;
            newPoint.Y = point1.Y * value;
            newPoint.Z = point1.Z * value;

            return newPoint;
        }


        public static Point3D operator /(Point3D point, double value)
        {
            if (value == 0) throw new DivideByZeroException("Cannot divide a point by zero.");

            Point3D newPoint = new Point3D();

            newPoint.X = point.X / value;
            newPoint.Y = point.Y / value;
            newPoint.Z = point.Z / value;

            return newPoint;
        }
    }
}
