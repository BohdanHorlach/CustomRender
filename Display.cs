using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LR_6
{
    internal class Display
    {
        private Bitmap _bitmap;
        private Filler _filler;
        private double[,] _zBuffer;
        private Size _size;

        public Bitmap Bitmap { get => _bitmap; }


        public Display(int weight, int height)
        {
            _bitmap = new Bitmap(weight, height);
            _size = new Size(weight, height);
            _filler = new Filler();

            InitializeZBuffer();
        }


        private void InitializeZBuffer()
        {
            _zBuffer = new double[_size.Width, _size.Height];
            ClearZBuffer();
        }


        private void ClearBitmap()
        {
            using (Graphics g = Graphics.FromImage(_bitmap))
            {
                g.Clear(Color.White);
            }
        }


        private void ClearZBuffer()
        {
            for (int i = 0; i < _size.Width; i++)
            {
                for (int j = 0; j < _size.Height; j++)
                {
                    _zBuffer[i, j] = double.MaxValue;
                }
            }
        }


        public void SetPixel(int x, int y, double z, Color color)
        {
            if (x < 0 || x >= _size.Width || y < 0 || y >= _size.Height)
                return;


            if (z < _zBuffer[x, y])
            {
                _bitmap.SetPixel(x, y, color);
                _zBuffer[x, y] = z;
            }
        }


        public void SetPixel(int x, int y, double z, Color color, int size)
        {
            for (int dx = -size; dx <= size; dx++)
            {
                for (int dy = -size; dy <= size; dy++)
                {
                    int newX = (int)x + dx;
                    int newY = (int)y + dy;

                    SetPixel(newX, newY, z, color);
                }
            }
        }


        public void DrawLine(Point3D firstPoint, Point3D secondPoint, Color color, RobertsData rd, int size)
        {
            int hx = secondPoint.X > firstPoint.X ? 1 : -1;
            int hy = secondPoint.Y > firstPoint.Y ? 1 : -1;

            int dx = (int)Math.Abs(firstPoint.X - secondPoint.X);
            int dy = (int)Math.Abs(firstPoint.Y - secondPoint.Y);

            if (dx > dy)
            {
                int e = 2 * dy - dx;
                int y = (int)firstPoint.Y;

                double z = -(rd.A * firstPoint.X + rd.B * y + rd.D) / rd.C;

                SetPixel((int)firstPoint.X, y, z, color, size);

                for (int x = (int)firstPoint.X; x != (int)secondPoint.X; x += hx)
                {
                    z = -(rd.A * x + rd.B * y + rd.D) / rd.C;

                    SetPixel(x, y, z, color, size);
                    e += 2 * dy;

                    if (e > 0)
                    {
                        e -= 2 * dx;
                        y += hy;
                    }
                }
            }
            else
            {
                int e = 2 * dx - dy;
                int x = (int)firstPoint.X;
                double z = -(rd.A * x + rd.B * firstPoint.Y + rd.D) / rd.C;

                SetPixel(x, (int)firstPoint.Y, z, color, size);


                for (int y = (int)firstPoint.Y; y != (int)secondPoint.Y; y += hy)
                {
                    z = -(rd.A * x + rd.B * y + rd.D) / rd.C;

                    SetPixel(x, y, z, color, size);
                    e += 2 * dx;

                    if (e > 0)
                    {
                        e -= 2 * dy;
                        x += hx;
                    }
                }
            }
        }


        public void DrawPolygon(Point3D[] polygonPoints, Color color, RobertsData rd, int edgeSize = 1)
        {
            int numVertices = polygonPoints.Length;
            edgeSize--;

            for (int i = 0; i < numVertices; i++)
            {
                int nextI = (i + 1) % numVertices;

                Point3D start = polygonPoints[i];
                Point3D end = polygonPoints[nextI];

                DrawLine(start, end, color, rd, edgeSize);
            }
        }


        public void DrawNormal(Point3D[] vertices, RobertsData rd, Color color, double normalLength = 5)
        {
            Point3D center = new Point3D(
                vertices.Average(v => v.X),
                vertices.Average(v => v.Y),
                vertices.Average(v => v.Z)
            );

            Point3D normalEnd = center + -rd.normal * normalLength;

            DrawLine(center, normalEnd, color, rd, 1);
        }


        public void FillPolygon(Point3D[] vertices2D, Color[] colors, RobertsData rd)
        {
            _filler.FillPolygonGouraud(this, vertices2D, colors, rd);
        }


        public void Clear()
        {
            ClearBitmap();
            ClearZBuffer();
        }

    }
}
