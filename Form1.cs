using System;
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
        Node activeNode;
        private enum Act
        {
            none,
            addNode,
            rmNode,
            addEdge,
            rmEdge,
        }
        private Act currentAction;

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
        private double AngleBetweenPoints(Point pointF1, Point pointF2)
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
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            switch (currentAction)
            {
                case (Act.none):
                    if (e.Button == MouseButtons.Left) set_active_node(e.Location);
                    return;
                case (Act.addNode):
                    create_node(e.Location);
                    return;
                case (Act.rmNode):
                    remove_node(e.Location);
                    return;
                case (Act.addEdge):
                    add_edge(e.Location);
                    return;
                case (Act.rmEdge):
                    remove_edge(e.Location);
                    return;
            }
        }

        private void remove_edge(Point p)
        {
            foreach (Edge edge in edges)
            {
                /*Point ap1 = new Point(edge.v1.x + 10, edge.v1.y + 10);
                Point ap2 = new Point(edge.v2.x + 10, edge.v2.y - 10);
                Point bp1 = new Point(edge.v1.x - 10, edge.v1.y + 10);
                Point bp2 = new Point(edge.v2.x - 10, edge.v2.y - 5);
                graphics.DrawLine(new Pen(Brushes.Yellow, 2), ap1, ap2);
                graphics.DrawLine(new Pen(Brushes.Yellow, 2), bp1, bp2);
                Rectangle rec = new Rectangle(ap1, 5, Math.Abs(Math.Abs(ap1.X - ap2.X) ^ 2 - Math.Abs(ap1.Y - ap2.Y) ^ 2));
                graphics.DrawRectangle(rec);
                b_graphics.Render();*/
                return;
            }
        }

        private void set_active_node(Point p)
        {
            foreach (Node node in nodes)
            {
                if (node.IsHit(p))
                {
                    activeNode = node;
                }
            }
        }
        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (currentAction == Act.addEdge && activeNode != null)
            {
                graphics.DrawLine(new Pen(Brushes.Black, 2), activeNode.x, activeNode.y, e.Location.X, e.Location.Y);
                b_graphics.Render();
            }
            if (currentAction == Act.none && activeNode != null && e.Location.X < panel1.Width && e.Location.X > 0 && e.Location.Y < panel1.Height && e.Location.Y > 0)
            {
                nodes[nodes.IndexOf(activeNode)].x = e.Location.X;
                nodes[nodes.IndexOf(activeNode)].y = e.Location.Y;
            }
        }
        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            if (currentAction == Act.addEdge && activeNode != null)
            {
                foreach (Node node in nodes)
                {
                    if (node.IsHit(e.Location))
                    {
                        edges.Add(new Edge(activeNode, node, Color.Black));
                    }
                }
            }
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
            activeNode = null;
        }
        private void create_node(Point p)
        {
            nodes.Add(new Node(nodes.Count + 1, p.X, p.Y, 15, Color.Black));
        }
        private void remove_node(Point p)
        {
            foreach (Node node in nodes)
            {
                if (node.IsHit(p))
                {
                    nodes.Remove(node);
                    for (int i = 0; i < edges.Count; i++)
                    {
                        if (edges[i].v1 == node || edges[i].v2 == node)
                        {
                            edges.Remove(edges[i]);
                            i--;
                        }
                    }
                    return;
                }
            }
        }
        private void add_edge(Point p)
        {
            foreach (Node node in nodes)
            {
                if (node.IsHit(p))
                {
                    activeNode = node;
                }
            }
        }
        private void setAct(Act act)
        {
            currentAction = currentAction != act ? act : Act.none;
        }
        private void button1_Click(object sender, System.EventArgs e) // addNode
        {
            setAct(Act.addNode);
        }
        private void button4_Click(object sender, System.EventArgs e) // rmNode
        {
            setAct(Act.rmNode);
        }
        private void button2_Click(object sender, System.EventArgs e) // add edge
        {
            setAct(Act.addEdge);
        }
        private void button3_Click(object sender, System.EventArgs e) // rm edge
        {
            setAct(Act.rmEdge);
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
            if (x - R <= p.X && x + R >= p.X && y - R <= p.Y && y + R >= p.Y)
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