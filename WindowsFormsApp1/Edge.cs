using System;
using System.Drawing;

namespace WindowsFormsApp1
{
    public class Edge
    {
        public Node v1, v2;
        public int len;
        public Color color;

        public Edge(Node v1, Node v2, Color color, int len)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.color = color;
            this.len = len;
        }
        public void Draw(Graphics graphics)
        {
            graphics.DrawLine(new Pen(color, 2), new Point(v1.x, v1.y), new Point(v2.x, v2.y));
            graphics.DrawString(len.ToString(), new Font("sans-serif", 14), new SolidBrush(color), new PointF((v1.x + v2.x) / 2 - 7, (v1.y + v2.y) / 2 - 7));
            /*using (Matrix m = new Matrix())
            {
                Rectangle rec = new Rectangle(v1.x - 4, v1.y, 8, (int)Math.Sqrt(Math.Pow(v1.x - v2.x, 2) + Math.Pow(v1.y - v2.y, 2)));
                graphics.DrawRectangle(new Pen(color, 2), rec);
                m.RotateAt((float)(90 - AngleBetweenPoints(new Point(v1.x, v1.y), new Point(v2.x, v2.y))), new PointF(v1.x, v1.y));
                graphics.Transform = m;
                graphics.DrawRectangle(new Pen(color, 2), rec);
                graphics.ResetTransform();
            }*/
        }
        public double AngleBetweenPoints(Point pointF1, Point pointF2)
        {
            float X = pointF1.X - pointF2.X;
            float Y = pointF1.Y - pointF2.Y;
            double angle = Math.Atan(Y / X);
            double grad_angle = angle * 180 / Math.PI;
            if (pointF1.X > pointF2.X && pointF1.Y < pointF2.Y)
            {
                grad_angle = -grad_angle;
            }
            if (pointF1.X < pointF2.X)
            {
                grad_angle = 180 - grad_angle;
            }
            if (pointF1.X > pointF2.X && pointF1.Y > pointF2.Y)
            {
                grad_angle = 360 - grad_angle;
            }
            return grad_angle;
        }
    }
}