using LR_6;
using System.Drawing;
using System.Windows.Forms;
using System;
using System.Collections.Generic;


public class MainForm : Form
{
    private Point3D[] _vertices;
    private int[][] _faces;

    private TrackBar _thetaSlider;
    private TrackBar _phiSlider;
    private TrackBar _distanceSlider;
    private Display _display;
    private double _ambientIntensity = 0.3;
    private double _diffuseIntensity = 0.8;
    private double _specularIntensity = 10;
    private double _shininess = 100;

    private Point3D _lightPosition = new Point3D(1, 1, 1);
    private Light _light;

    public MainForm()
    {
        this.ClientSize = new Size(800, 800);
        this.Text = "Polyhedron Viewer";
        this.DoubleBuffered = true;

        InitializeDisplay();
        InitializeSliders();
        InitializeCube();

        _light = new Light(_lightPosition, Color.White);
    }


    private void InitializeDisplay()
    {
        _display = new Display(ClientSize.Width, ClientSize.Height);

        this.Resize += (s, e) =>
        {
            _display = new Display(ClientSize.Width, ClientSize.Height);
        };
    }


    private void InitializeSliders()
    {
        _thetaSlider = new TrackBar { Minimum = 0, Maximum = 360, Value = 0, Width = 150 };
        _phiSlider = new TrackBar { Minimum = 0, Maximum = 360, Value = 0, Width = 150 };
        _distanceSlider = new TrackBar { Minimum = 2, Maximum = 10, Value = 6, Width = 150 };

        TextBox thetaTextBox = new TextBox { Width = 40, Text = _thetaSlider.Value.ToString() };
        TextBox phiTextBox = new TextBox { Width = 40, Text = _phiSlider.Value.ToString() };
        TextBox distanceTextBox = new TextBox { Width = 40, Text = _distanceSlider.Value.ToString() };


        _thetaSlider.Scroll += (s, e) =>
        {
            thetaTextBox.Text = _thetaSlider.Value.ToString();
            Invalidate();
        };
        
        _phiSlider.Scroll += (s, e) =>
        {
            phiTextBox.Text = _phiSlider.Value.ToString();
            Invalidate();
        };
        
        _distanceSlider.Scroll += (s, e) =>
        {
            distanceTextBox.Text = _distanceSlider.Value.ToString();
            Invalidate();
        };


        thetaTextBox.TextChanged += (s, e) =>
        {
            if (int.TryParse(thetaTextBox.Text, out int value) && value >= _thetaSlider.Minimum && value <= _thetaSlider.Maximum)
            {
                _thetaSlider.Value = value;
                Invalidate();
            }
        };

        phiTextBox.TextChanged += (s, e) =>
        {
            if (int.TryParse(phiTextBox.Text, out int value) && value >= _phiSlider.Minimum && value <= _phiSlider.Maximum)
            {
                _phiSlider.Value = value;
                Invalidate();
            }
        };

        distanceTextBox.TextChanged += (s, e) =>
        {
            if (int.TryParse(distanceTextBox.Text, out int value) && value >= _distanceSlider.Minimum && value <= _distanceSlider.Maximum)
            {
                _distanceSlider.Value = value;
                Invalidate();
            }
        };

        this.Controls.Add(_thetaSlider);
        this.Controls.Add(_phiSlider);
        this.Controls.Add(_distanceSlider);
        this.Controls.Add(thetaTextBox);
        this.Controls.Add(phiTextBox);
        this.Controls.Add(distanceTextBox);

        this.Resize += (s, e) =>
        {
            int formCenter = this.ClientSize.Width / 2;
            int totalWidth = _thetaSlider.Width + _phiSlider.Width + _distanceSlider.Width + thetaTextBox.Width + phiTextBox.Width + distanceTextBox.Width + 40;

            int startX = formCenter - totalWidth / 2;
            int posY = this.ClientSize.Height - 50;

            thetaTextBox.Location = new Point(startX, posY - 30);
            _thetaSlider.Location = new Point(startX + thetaTextBox.Width + 10, posY);

            phiTextBox.Location = new Point(_thetaSlider.Right + 10, posY - 30);
            _phiSlider.Location = new Point(phiTextBox.Right + 10, posY);

            distanceTextBox.Location = new Point(_phiSlider.Right + 10, posY - 30);
            _distanceSlider.Location = new Point(distanceTextBox.Right + 10, posY);
        };

        this.OnResize(EventArgs.Empty);
    }


    private void InitializeCube()
    {
        _vertices = new Point3D[]
        {
            // Cube 1
            new Point3D(-1, -1, -1), 
            new Point3D(1, -1, -1),
            new Point3D(1, 1, -1), 
            new Point3D(-1, 1, -1),
            new Point3D(-1, -1, 1), 
            new Point3D(1, -1, 1),
            new Point3D(1, 1, 1), 
            new Point3D(-1, 1, 1),

            // Cube 2
            new Point3D(-0.25, -0.25, -0.25), 
            new Point3D(1.75, -0.25, -0.25),
            new Point3D(1.75, 1.75, -0.25), 
            new Point3D(-0.25, 1.75, -0.25),
            new Point3D(-0.25, -0.25, 1.75),
            new Point3D(1.75, -0.25, 1.75),
            new Point3D(1.75, 1.75, 1.75), 
            new Point3D(-0.25, 1.75, 1.75)
        };

        _faces = new int[][]
        {
            // Cube 1 faces
            new int[] { 0, 3, 2, 1 },
            new int[] { 4, 5, 6, 7 },
            new int[] { 0, 4, 7, 3 },
            new int[] { 1, 2, 6, 5 },
            new int[] { 3, 7, 6, 2 },
            new int[] { 0, 1, 5, 4 },

            // Cube 2 faces
            new int[] { 8, 11, 10, 9 },
            new int[] { 12, 13, 14, 15 },
            new int[] { 8, 12, 15, 11 }, 
            new int[] { 9, 10, 14, 13 },
            new int[] { 11, 15, 14, 10 }, 
            new int[] { 8, 9, 13, 12 }
        };
    }


    private Matrix4x4 GetViewMatrix(double theta, double phi, double distance)
    {
        Matrix4x4 translate = Matrix4x4.CreateTranslation(0, 0, -distance);
        Matrix4x4 rotateY = Matrix4x4.RotateOnY(theta);
        Matrix4x4 rotateX = Matrix4x4.RotateOnX(phi);

        return translate * rotateX * rotateY;
    }


    private Matrix4x4 GetProjectionMatrix()
    {
        double fov = 60 * Math.PI / 180;
        double aspect = ClientSize.Width / ClientSize.Height;
        double near = 0.1;
        double far = 1000;

        return Matrix4x4.GetPerspective(fov, aspect, near, far);

        //double near = 1;
        //double far = 100;

        //return Matrix4x4.GetOrthographic(near, far);
    }


    public static Color MultiplyColor(Color color, double factor)
    {
        int r = (int)(color.R * factor);
        int g = (int)(color.G * factor);
        int b = (int)(color.B * factor);
        int a = color.A;

        r = Math.Max(0, Math.Min(255, r));
        g = Math.Max(0, Math.Min(255, g));
        b = Math.Max(0, Math.Min(255, b));

        return Color.FromArgb(a, r, g, b);
    }


    public static Color MultiplyColor(Color color1, Color color2)
    {
        int r = (color1.R * color2.R) / 255;
        int g = (color1.G * color2.G) / 255;
        int b = (color1.B * color2.B) / 255;
        int a = (color1.A * color2.A) / 255;

        return Color.FromArgb(a, r, g, b);
    }


    public static Color AddColors(Color color1, Color color2)
    {
        int r = color1.R + color2.R;
        int g = color1.G + color2.G;
        int b = color1.B + color2.B;
        int a = color1.A + color2.A;

        r = Math.Max(0, Math.Min(255, r));
        g = Math.Max(0, Math.Min(255, g));
        b = Math.Max(0, Math.Min(255, b));
        a = Math.Max(0, Math.Min(255, a));

        return Color.FromArgb(a, r, g, b);
    }


    private Color CalculateLighting(Point3D normal, Point3D points, Point3D viewDirection)
    {
        Point3D lightDirection = (_light.Position - points).GetNormilizeVector();

        double dotProduct = normal.DotProduct(lightDirection);
        double diffuse = Math.Max(0, dotProduct) * _diffuseIntensity;

        Point3D reflectionVector = (normal * 2 * dotProduct - lightDirection).GetNormilizeVector();
        double specular = Math.Pow(Math.Max(0, reflectionVector.DotProduct(viewDirection)), _shininess) * _specularIntensity;

        Color ambientColor = MultiplyColor(_light.Color, _ambientIntensity);
        Color diffuseColor = MultiplyColor(_light.Color, diffuse);
        Color specularColor = MultiplyColor(_light.Color, specular);

        Color finalColor = AddColors(AddColors(ambientColor, diffuseColor), specularColor);

        return finalColor;
    }


    private Point3D[] TransformedAllVertices(Matrix4x4 viewMatrix, Point3D[] vertices, int distance)
    {
        Point3D[] transformedPoints = new Point3D[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            Point3D transformed = viewMatrix.MultiplyToPoint(vertices[i]);
            transformedPoints[i] = new Point3D(transformed.X, transformed.Y, transformed.Z - distance, transformed.W);
        }

        return transformedPoints;
    }


    private Dictionary<int, List<int>> BuildVertexToFaceMap(int[][] faces)
    {
        var vertexToFaces = new Dictionary<int, List<int>>();

        for (int faceIndex = 0; faceIndex < faces.Length; faceIndex++)
        {
            foreach (var vertexIndex in faces[faceIndex])
            {
                if (!vertexToFaces.ContainsKey(vertexIndex))
                    vertexToFaces[vertexIndex] = new List<int>();

                vertexToFaces[vertexIndex].Add(faceIndex);
            }
        }

        return vertexToFaces;
    }


    private Point3D CalculateFaceNormal(Point3D[] vertices, int[] face)
    {
        Point3D v1 = vertices[face[0]];
        Point3D v2 = vertices[face[1]];
        Point3D v3 = vertices[face[2]];

        Point3D edge1 = v3 - v2;
        Point3D edge2 = v2 - v1;

        Point3D normal = Point3D.CrossProduct(edge1, edge2);
        return normal.GetNormilizeVector();
    }


    private Point3D CalculateVertexNormal(int vertexIndex, Dictionary<int, List<int>> vertexToFaceMap, int[][] faces, Point3D[] vertices)
    {
        if (!vertexToFaceMap.ContainsKey(vertexIndex))
            return new Point3D(0, 0, 0);

        Point3D normalSum = new Point3D(0, 0, 0);

        int count = 0;
        foreach (var faceIndex in vertexToFaceMap[vertexIndex])
        {
            Point3D faceNormal = CalculateFaceNormal(vertices, faces[faceIndex]);
            normalSum += faceNormal;
            count++;
        }

        return (normalSum / count).GetNormilizeVector();
    }


    private Point3D[] CalculateVertexNormals(Point3D[] vertices, int[][] faces)
    {
        var vertexToFaceMap = BuildVertexToFaceMap(faces);
        Point3D[] normals = new Point3D[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            normals[i] = CalculateVertexNormal(i, vertexToFaceMap, faces, vertices);
        }

        return normals;
    }


    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        _display.Clear();

        double theta = _thetaSlider.Value * Math.PI / 180;
        double phi = _phiSlider.Value * Math.PI / 180;
        int distance = _distanceSlider.Value;

        Matrix4x4 viewMatrix = GetViewMatrix(theta, phi, distance);
        Matrix4x4 projectionMatrix = GetProjectionMatrix();

        Color[] colors = new Color[6] { Color.Green, Color.Red, Color.Blue, Color.Orange, Color.Pink, Color.Yellow };
        int colorIndex = 0;

        Point3D[] transformedVertices = TransformedAllVertices(viewMatrix, _vertices, distance);
        Point3D[] vertexNormals = CalculateVertexNormals(transformedVertices, _faces);
        Point3D observerDirection = new Point3D(0, 0, -1);

        foreach (var face in _faces)
        {
            Point3D[] projectedPoints = new Point3D[face.Length];
            Point3D[] pointOnFace = new Point3D[face.Length];
            Color[] colorByPoint = new Color[face.Length];

            for (int i = 0; i < face.Length; i++)
                pointOnFace[i] = transformedVertices[face[i]];

            RobertsData rdN = new RobertsData(pointOnFace);

            for (int i = 0; i < face.Length; i++)
            {
                Point3D transformed = projectionMatrix.MultiplyToPoint(pointOnFace[i]);

                projectedPoints[i] = new Point3D(
                    (transformed.X + 1) * 0.5 * ClientSize.Width,
                    ClientSize.Height - (transformed.Y + 1) * 0.5 * ClientSize.Height,
                    (transformed.Z + 1) * 0.5
                );

                Point3D normal = vertexNormals[face[i]];
                Color light = CalculateLighting(normal, projectedPoints[i], observerDirection);
                Color color = MultiplyColor(light, colors[colorIndex]);

                colorByPoint[i] = color;
            }
            
            RobertsData rd = new RobertsData(projectedPoints);

            if (rd.normal.DotProduct(observerDirection) < 0)
            {
                //_display.DrawPolygon(projectedPoints, Color.Black, rd, 2);
                //_display.DrawNormal(projectedPoints, rd, Color.Red, 5);
                _display.FillPolygon(projectedPoints, colorByPoint, rd);
            }

            colorIndex = (colorIndex + 1) % colors.Length;

        }

        e.Graphics.DrawImage(_display.Bitmap, 0, 0);
    }


    [STAThread]
    public static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new MainForm());
    }
}