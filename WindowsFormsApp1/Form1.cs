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
        List<Edge> marked_edges = new List<Edge>();
        public Form1()
        {
            InitializeComponent();
            context = BufferedGraphicsManager.Current;                              // инициализация буфера из которого будет загружаться картинка
            context.MaximumBuffer = new Size(panel1.Width + 1, panel1.Height + 1);  // следующий фрейм будет заменяться предыдущим, если он уже готов к отображению
            b_graphics = context.Allocate(panel1.CreateGraphics(),                  // это нужно для того, чтобы перемещать элементы на форме
                 new Rectangle(0, 0, panel1.Width, panel1.Height));                 // без этого картинка будет мерцать
            graphics = b_graphics.Graphics;

            if (!vertexInfo.Exists) File.Create(vertexInfo.FullName); // считывание информации о графе из файла
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
        }

        private void timer1_Tick(object sender, System.EventArgs e)
        {
            graphics.Clear(Color.White); // отчистка панели
            foreach (Edge edge in edges) // отрисовка ребер
            {
                if (marked_edges.Contains(edge))
                    edge.color = Color.Blue;
                else edge.color = Color.Black;
                edge.Draw(graphics);
            }
            foreach (Node node in nodes) // отрисовка узлов
            {
                node.Draw(graphics);
            }
            b_graphics.Render(); // прогрузка фрейма из буфера
        }
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                switch (currentAction) // отработка нажатия 
                {
                    case (Act.none):
                        set_active_node(e.Location);
                        break;
                    case (Act.addNode):
                        create_node(e.Location);
                        break;
                    case (Act.rmNode):
                        remove_node(e.Location);
                        break;
                    case (Act.addEdge):
                        add_edge(e.Location);
                        break;
                    case (Act.rmEdge):
                        remove_edge(e.Location);
                        break;
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
            //протягивание ребра
            if (currentAction == Act.addEdge && activeNode != null)
            {
                graphics.DrawLine(new Pen(Brushes.Black, 2), activeNode.x, activeNode.y, e.Location.X, e.Location.Y);
                b_graphics.Render();
            }
            //перемещение узла
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
            //добавление ребра
            if (currentAction == Act.addEdge && activeNode != null)
            {
                foreach (Node node in nodes)
                {
                    if (node.IsHit(e.Location) && activeNode != node) //проверка на вхождение курсора в отличный от активного узел
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
            //добавление в файл
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
        private void setAct(Act act) // изменение выбранного инструмента
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
        //изменение выбранного инструмента
        private void button1_Click(object sender, System.EventArgs e) // add node
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
        //отображение кратчайшего пути
        private void button5_Click(object sender, EventArgs e)
        {
            int start = (int)numericUpDown1.Value - 1;
            int dest = (int)numericUpDown2.Value - 1;
            if (start >= nodes.Count || dest >= nodes.Count) return;
            marked_edges.Clear();
            Dijkstra dj = new Dijkstra(nodes, edges);
            int[] path = dj.AlgoStart(start, dest);
            int length = 0;
            for (int i = 1; i < path.Length; i++)
            {
                foreach (Edge edge in edges)
                {
                    if ((edge.v1.n == path[i] && edge.v2.n == path[i - 1]) || (edge.v2.n == path[i] && edge.v1.n == path[i - 1]))
                    {
                        marked_edges.Add(edge);
                        length += edge.len;
                    }
                }
            }
            label2.Text = "Кратчайший путь: " + length;
        }
    }
}