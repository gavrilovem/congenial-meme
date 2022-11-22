using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
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
                nodes.Add(new Node((nodes.Count != 0 ? nodes.Last().n + 1 : 1), int.Parse(line[0]), int.Parse(line[1]), 15, Color.Black));
            }
            while (!er.EndOfStream)
            {
                string[] line = er.ReadLine().Split(' ');
                edges.Add(new Edge(nodes[int.Parse(line[0])], nodes[int.Parse(line[1])], Color.Black, int.Parse(line[2])));
            }
            vr.Close();
            er.Close();
            Dijkstra d = new Dijkstra();
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
        private void create_node(Point p)
        {
            nodes.Add(new Node((nodes.Count != 0 ? nodes.Last().n + 1 : 1), p.X, p.Y, 15, Color.Black));
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
        private void remove_edge(Point p)
        {
            Matrix m = new Matrix();
            foreach (Edge e in edges)
            {
                Rectangle rec = new Rectangle((e.v1.x + e.v2.x) / 2, (e.v1.y + e.v2.y) / 2, 10, 10);
                graphics.DrawRectangle(new Pen(e.color, 2), rec);
                Rectangle rec2 = new Rectangle(p.X, p.Y, 10, 10);
                if (rec2.IntersectsWith(rec))
                {
                    edges.Remove(e);
                    break;
                }
            }
        }
        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            if (currentAction == Act.addEdge && activeNode != null)
            {
                foreach (Node node in nodes)
                {
                    if (node.IsHit(e.Location) && activeNode != node)
                    {
                        int length;
                        while (true)
                        {
                            if (int.TryParse(Interaction.InputBox("Введите число", "Длинна ребра"), out length))
                            {
                                edges.Add(new Edge(activeNode, node, Color.Black, length));
                                break;
                            }
                        }
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
                ew.WriteLine((edge.v1.n - 1) + " " + (edge.v2.n - 1) + " " + edge.len);
            }
            nw.Close();
            ew.Close();
            activeNode = null;
        }
        private void setAct(Act act)
        {
            currentAction = currentAction != act ? act : Act.none;
        }
        private void setActiveButtonColor()
        {
            Color c = SystemColors.Control;
            foreach (Control btn in this.Controls)
            {
                if (btn as Button is Button)
                {
                    btn.BackColor = c;
                }
            }
            switch (currentAction)
            {
                case (Act.addNode):
                    button1.BackColor = button1.BackColor == c ? Color.Yellow : c;
                    return;
                case (Act.rmNode):
                    button2.BackColor = button2.BackColor == c ? Color.Yellow : c;
                    return;
                case (Act.rmEdge):
                    button3.BackColor = button3.BackColor == c ? Color.Yellow : c;
                    return;
                case (Act.addEdge):
                    button4.BackColor = button4.BackColor == c ? Color.Yellow : c;
                    return;
            }
        }
        private void button1_Click(object sender, System.EventArgs e) // addNode
        {
            setAct(Act.addNode);
            setActiveButtonColor();
        }
        private void button2_Click(object sender, System.EventArgs e) // add edge
        {
            setAct(Act.rmNode);
            setActiveButtonColor();
        }
        private void button3_Click(object sender, System.EventArgs e) // rm edge
        {
            setAct(Act.rmEdge);
            setActiveButtonColor();
        }
        private void button4_Click(object sender, System.EventArgs e) // rmNode
        {
            setAct(Act.addEdge);
            setActiveButtonColor();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            int start_ind = (int)numericUpDown1.Value;
            int end_ind = (int)numericUpDown2.Value;
            if (start_ind == end_ind) return;
            Node start = new Node(), end = new Node();
            foreach (Node node in nodes)
            {
                node.isVisited = false;
                if (node.n == start_ind)
                {
                    start = node;
                }
                if (node.n == end_ind)
                {
                    end = node;
                }
            }
            if (start.n == 0 || end.n == 0) return;
            List<Node> visited = new List<Node>();
            List<int> results = new List<int>();
            visited.Add(start);
            foreach (Edge edge in edges)
            {
                if (edge.v1 == start)
                {
                    visited.Add(edge.v2);
                    results = step_in(edge.v2, end, edge.len, visited.ToList(), results);
                }
                if (edge.v2 == start)
                {
                    visited.Add(edge.v1);
                    results = step_in(edge.v1, end, edge.len, visited.ToList(), results);
                }
            }
            label2.Text = "";
            results.Sort();
            label2.Text = "Кратчайший путь: " + results[0].ToString() + "\r\n";
        }
        private List<int> step_in(Node step, Node end, int len, List<Node> visited, List<int> results)
        {
            foreach (Edge edge in edges)
            {
                if ((edge.v1 == end || edge.v2 == end) && (edge.v1 == step || edge.v2 == step))
                {
                    results.Add(edge.len + len);
                    break;
                }
                if (edge.v1 == step && !visited.Contains(edge.v2))
                {
                    visited.Add(edge.v2);
                    step_in(edge.v2, end, edge.len + len, visited.ToList(), results);
                }
                if (edge.v2 == step && !visited.Contains(edge.v1))
                {
                    visited.Add(edge.v1);
                    step_in(edge.v1, end, edge.len + len, visited.ToList(), results);
                }
            }
            return results;
        }
    }
    class Node
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

    class Edge
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
            graphics.DrawString(len.ToString(), new Font("sans-serif", 14), Brushes.Black, new PointF((v1.x + v2.x) / 2 - 7, (v1.y + v2.y) / 2 - 7));
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