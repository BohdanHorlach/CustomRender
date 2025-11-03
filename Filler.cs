using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;


namespace LR_6
{
    internal class Filler
    {
        internal struct Edge
        {
            public Point3D Start;
            public Point3D End;

            public Edge(Point3D start, Point3D end)
            {
                Start = start;
                End = end;
            }
        }


        private List<Edge> GetEdges(Point3D[] vertices)
        {
            List<Edge> edges = new List<Edge>();

            for (int i = 0; i < vertices.Length; i++)
            {
                int next = (i + 1) % vertices.Length;
                edges.Add(new Edge(vertices[i], vertices[next]));
            }

            return edges;
        }


        public void FillPolygon(Display display, Point3D[] vertices2D, Color color, RobertsData rd)
        {
            List<Edge> edges = GetEdges(vertices2D);

            double minY = vertices2D.Min(v => v.Y);
            double maxY = vertices2D.Max(v => v.Y);

            for (int y = (int)minY; y <= maxY; y++)
            {
                List<int> intersections = new List<int>();

                foreach (var edge in edges)
                {
                    var (p1, p2) = (edge.Start, edge.End);
                    if ((p1.Y <= y && p2.Y > y) || (p2.Y <= y && p1.Y > y))
                    {
                        double t = (y - p1.Y) / (double)(p2.Y - p1.Y);
                        int x = (int)(p1.X + t * (p2.X - p1.X));
                        intersections.Add(x);
                    }
                }

                intersections.Sort();

                for (int i = 0; i < intersections.Count; i += 2)
                {
                    if (i + 1 >= intersections.Count) break;

                    int startX = intersections[i];
                    int endX = intersections[i + 1];

                    double z = -(rd.A * startX + rd.B * y + rd.D) / rd.C;

                    for (int x = startX; x <= endX; x++)
                    {
                        display.SetPixel(x, y, z, color);
                        z += -rd.A / rd.C;
                    }
                }
            }
        }


        private int Clamp(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        private Color InterpolateColor(Color color1, Color color2, double t)
        {
            double r = Math.Round(color1.R + t * (color2.R - color1.R));
            double g = Math.Round(color1.G + t * (color2.G - color1.G));
            double b = Math.Round(color1.B + t * (color2.B - color1.B));

            return Color.FromArgb(Clamp((int)r, 0, 255), Clamp((int)g, 0, 255), Clamp((int)b, 0, 255));
        }


        public void FillPolygonGouraud(Display display, Point3D[] vertices2D, Color[] vertexColors, RobertsData rd)
        {
            if (vertices2D.Length != vertexColors.Length)
                throw new ArgumentException("Need more color");

            List<Edge> edges = GetEdges(vertices2D);

            double minY = vertices2D.Min(v => v.Y);
            double maxY = vertices2D.Max(v => v.Y);

            for (int y = (int)minY; y <= maxY; y++)
            {
                List<(int X, Color Color)> intersections = new List<(int, Color)>();

                foreach (var edge in edges)
                {
                    var (p1, p2) = (edge.Start, edge.End);

                    if ((p1.Y <= y && p2.Y > y) || (p2.Y <= y && p1.Y > y))
                    {
                        double t = (y - p1.Y) / (double)(p2.Y - p1.Y);
                        int x = (int)(p1.X + t * (p2.X - p1.X));

                        Color interpolatedColor = InterpolateColor(vertexColors[Array.IndexOf(vertices2D, p1)],
                                                                    vertexColors[Array.IndexOf(vertices2D, p2)], t);

                        intersections.Add((x, interpolatedColor));
                    }
                }

                intersections.Sort((a, b) => a.X.CompareTo(b.X));

                for (int i = 0; i < intersections.Count; i += 2)
                {
                    if (i + 1 >= intersections.Count) break;

                    int startX = intersections[i].X;
                    int endX = intersections[i + 1].X;

                    Color startColor = intersections[i].Color;
                    Color endColor = intersections[i + 1].Color;

                    double z = -(rd.A * startX + rd.B * y + rd.D) / rd.C;

                    for (int x = startX; x <= endX; x++)
                    {
                        double t = (x - startX) / (double)(endX - startX);

                        Color pixelColor = InterpolateColor(startColor, endColor, t);

                        display.SetPixel(x, y, z, pixelColor);

                        z += -rd.A / rd.C;
                    }
                }
            }
        }
    }
}
