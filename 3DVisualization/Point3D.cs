using System.Drawing;

namespace _3DVisualization
{
    class Point3D
    {
        private Color color;
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Point3D() { }
        public Point3D(double x,double y,double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
        public Point3D(double x, double y, double z, double min, double max) : this(x, y, z)
        {
            min++;max++;z++;
            double mid = (min + max) / 2;

            if (z >= max)
                color = Color.FromArgb(255, 0, 0);
            else
            {
                if (z >= mid)
                    color = Color.FromArgb(255, (int)((max - z) / (max - mid) * 255), 0);
                else if (z < mid)
                    color = Color.FromArgb((int)((z - min) / (mid - min) * 255), 255, 0);
            }
        }
        public Color PointColor
        {
            get
            {
                if (color == null)
                    return Color.SkyBlue;
                else
                    return color;
            }
        }
    }
}
