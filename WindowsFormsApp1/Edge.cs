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
        }
    }
}