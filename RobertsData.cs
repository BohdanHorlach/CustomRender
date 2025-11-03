using System;

namespace LR_6
{
    internal struct RobertsData
    {
        public Point3D normal;
        public double A;
        public double B;
        public double C;
        public double D;


        public RobertsData(Point3D[] vertices)
        {
            normal = Point3D.CrossProduct(vertices[2] - vertices[1], vertices[1] - vertices[0]).GetNormilizeVector();
            A = normal.X;
            B = normal.Y;
            C = normal.Z;
            D = -normal.DotProduct(vertices[0]);
        }
    }

}
