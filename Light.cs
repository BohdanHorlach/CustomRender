using LR_6;
using System.Drawing;

public class Light
{
    public Point3D Position { get; set; }
    public Color Color { get; set; }

    public Light(Point3D position, Color color)
    {
        Position = position;
        Color = color;
    }
}