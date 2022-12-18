using System.Collections.Generic;

namespace WindowsFormsApp1
{
    public class Dijkstra
    {
        int[,] graph;
        public Dijkstra(List<Node> nodes, List<Edge> edges)
        {
            graph = new int[nodes.Count, nodes.Count];
            foreach (Edge edge in edges)
            {
                graph[edge.v1.n - 1, edge.v2.n - 1] = edge.len;
                graph[edge.v2.n - 1, edge.v1.n - 1] = edge.len;
            }
        }
        public int[] AlgoStart(int start, int end)
        {
            point[] step = new point[graph.GetLength(0)]; // алгоритм Дийкстры
            for (int i = 0; i < step.Length; i++)
            {
                step[i] = new point();
            }
            step[start].weight = 0;
            bool f = true;
            int index, min, length;
            do
            {
                index = int.MaxValue;
                min = int.MaxValue;
                for (int i = 0; i < graph.GetLength(0); i++)
                {
                    if (step[i].not_visited && step[i].weight < min)
                    {
                        min = step[i].weight;
                        index = i;
                    }
                }
                if (index != int.MaxValue)
                {
                    for (int i = 0; i < graph.GetLength(0); i++)
                    {
                        if (graph[index, i] > 0)
                        {
                            length = min + graph[index, i];
                            if (length < step[i].weight)
                            {
                                step[i].weight = length;
                            }
                        }
                    }
                    if (f)
                    {
                        if (index == end) f = false;
                    }
                    step[index].not_visited = false;
                }
            } while (index < int.MaxValue && f);
            // Восстановление пути
            int[] ver = new int[graph.GetLength(0)]; // массив посещенных вершин
            ver[0] = end + 1; // начальный элемент - конечная вершина
            int k = 1; // индекс предыдущей вершины
            int weight = step[end].weight; // вес конечной вершины
            if (weight == int.MaxValue) return ver;
            while (end != start) // пока не дошли до начальной вершины
            {
                for (int i = 0; i < graph.GetLength(0); i++) // просматриваем все вершины
                    if (graph[i, end] != 0)   // если связь есть
                    {
                        int temp = weight - graph[i, end]; // определяем вес пути из предыдущей вершины
                        if (temp == step[i].weight) // если вес совпал с рассчитанным
                        {                 // значит из этой вершины и был переход
                            weight = temp; // сохраняем новый вес
                            end = i;       // сохраняем предыдущую вершину
                            ver[k] = i + 1; // и записываем ее в массив
                            k++;
                        }
                    }
            }
            return ver;
        }
    }
    class point
    {
        public int weight;
        public bool not_visited;
        public point()
        {
            weight = int.MaxValue;
            not_visited = true;
        }
    }
}