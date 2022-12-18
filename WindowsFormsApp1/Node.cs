using System.Drawing;

namespace WindowsFormsApp1
{
    public class Node
    {
        public int n;
        public int x;
        public int y;
        public int R;
        public Color color;
        public bool isVisited;

        public Node()
        {
        }

        public Node(int n, int x, int y, int R, Color color = default, bool isVisited = false)
        {
            this.n = n;
            this.x = x;
            this.y = y;
            this.R = R;
            this.color = color;
            this.isVisited = isVisited;
        }
        public void Draw(Graphics gr)
        {
            Pen pen = new Pen(color, 2);
            gr.FillEllipse(Brushes.White, (x - R), (y - R), 2 * R, 2 * R);
            gr.DrawEllipse(pen, (x - R), (y - R), 2 * R, 2 * R);
            PointF point = new PointF(x - 9, y - 9);
            gr.DrawString(n.ToString(), new Font("sans-serif", 14), Brushes.Black, point);
        }
        public bool IsHit(Point p)
        {
            if (x - R <= p.X && x + R >= p.X && y - R <= p.Y && y + R >= p.Y)
                return true;
            return false;
        }
    }
}