using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public static Graphics graphics;
        BufferedGraphicsContext context;
        BufferedGraphics b_graphics;
        Node move;

        public static DirectoryInfo stores = new DirectoryInfo("stores\\");
        FileInfo vertexInfo = new FileInfo(stores + "vertex.txt");
        FileInfo edgeInfo = new FileInfo(stores + "edge.txt");
        List<Node> nodes = new List<Node>();
        List<Edge> edges = new List<Edge>();
        public Form1()
        {
            InitializeComponent();
            context = BufferedGraphicsManager.Current;
            context.MaximumBuffer = new Size(panel1.Width + 1, panel1.Height + 1);
            b_graphics = context.Allocate(panel1.CreateGraphics(),
                 new Rectangle(0, 0, panel1.Width, panel1.Height));
            graphics = b_graphics.Graphics;

            if (!vertexInfo.Exists) File.Create(vertexInfo.FullName);
            if (!edgeInfo.Exists) File.Create(edgeInfo.FullName);
            StreamReader vr = new StreamReader(stores + vertexInfo.Name);
            StreamReader er = new StreamReader(stores + edgeInfo.Name);
            while (!vr.EndOfStream)
            {
                string[] line = vr.ReadLine().Split(' ');
                nodes.Add(new Node(nodes.Count + 1, int.Parse(line[0]), int.Parse(line[1]), 15, Color.Black));
            }
            while (!er.EndOfStream)
            {
                string[] line = er.ReadLine().Split(' ');
                edges.Add(new Edge(nodes[int.Parse(line[0])], nodes[int.Parse(line[1])], Color.Black));
            }
            vr.Close();
            er.Close();
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            nodes.Add(new Node(nodes.Count + 1, 15, 15, 15, Color.Black));
        }

        private void timer1_Tick(object sender, System.EventArgs e)
        {
            graphics.Clear(Color.White);

            foreach (Edge edge in edges)
            {
                edge.Draw(graphics);
            }
            foreach (Node node in nodes)
            {
                node.Draw(graphics);
            }
            b_graphics.Render();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            foreach (Node node in nodes)
            {
                if (e.Button == MouseButtons.Left && node.IsHit(e.Location))
                {
                    move = node;
                    break;
                }
            }
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (move != null && e.Location.X < panel1.Width && e.Location.X > 0 && e.Location.Y < panel1.Height && e.Location.Y > 0)
            {
                nodes[nodes.IndexOf(move)].x = e.Location.X;
                nodes[nodes.IndexOf(move)].y = e.Location.Y;
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            StreamWriter nw = new StreamWriter(stores + vertexInfo.Name, false);
            StreamWriter ew = new StreamWriter(stores + edgeInfo.Name, false);
            foreach (Node node in nodes)
            {
                nw.WriteLine(node.x + " " + node.y);
            }
            foreach (Edge edge in edges)
            {
                ew.WriteLine((edge.v1.n - 1) + " " + (edge.v2.n - 1));
            }
            nw.Close();
            ew.Close();
            move = null;
        }
    }
    class Node
    {
        public int n;
        public int x;
        public int y;
        public int R;
        public Color color;

        public Node(int n, int x, int y, int R, Color color = default)
        {
            this.n = n;
            this.x = x;
            this.y = y;
            this.R = R;
            this.color = color;
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
            if (x - 5 <= p.X && x + R * 2 + 5 >= p.X && y - 5 <= p.Y && y + R * 2 + 5 >= p.Y)
                return true;
            return false;
        }
    }

    class Edge
    {
        public Node v1, v2;
        public Color color;

        public Edge(Node v1, Node v2, Color color)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.color = color;
        }
        public void Draw(Graphics graphics)
        {
            graphics.DrawLine(new Pen(Color.Black, 2), new Point(v1.x, v1.y), new Point(v2.x, v2.y));
        }
    }
}