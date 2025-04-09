using Utils;

class TSProblemSolver
{
    private const int INF = int.MaxValue / 4 * 3;

    private const int INF_THRESHOLD = int.MaxValue / 2;

    public int[,] BaseRouteTable;

    public int n;

    private PriorityQueue<TSPBranch, int> CurrentBranches = new();

    public TSProblemSolver(int[,] routeTable)
    {
        this.BaseRouteTable = routeTable;
        this.n = routeTable.GetLength(0);
    }

    public TSProblemSolver(string problemPath)
    {
        string[] lines = File.ReadAllLines(problemPath);
        this.n = lines.GetLength(0);

        this.BaseRouteTable = new int[n, n];
        for (int i = 0; i < n; i++)
        {
            int[] row = [.. lines[i].Split().Select(val => val == "INF" ? INF : int.Parse(val))];

            for (int j = 0; j < n; j++) BaseRouteTable[i, j] = row[j];
        }
    }
    
    private int ReduceMatrix(int[,] matrix)
    {
        int n = matrix.GetLength(0);
        int reduction = 0;

        for (int i = 0; i < n; i++)
        {
            int min = Enumerable.Range(0, n).Select(j => matrix[i, j]).Min();
            if (min < INF_THRESHOLD && min > 0)
            {
                for (int j = 0; j < n; j++) matrix[i, j] -= min;
                reduction += min;
            }
        }

        for (int j = 0; j < n; j++)
        {
            int min = Enumerable.Range(0, n).Select(i => matrix[i, j]).Min();
            if (min < INF_THRESHOLD && min > 0)
            {
                for (int i = 0; i < n; i++) matrix[i, j] -= min;
                reduction += min;
            }
        }

        return reduction;
    }

    public void Solve()
    {
        int[,] reducedMatrix = (int[,])BaseRouteTable.Clone();
        int lowerBound = ReduceMatrix(reducedMatrix);

        var root = new TSPBranch(reducedMatrix, new List<int> { 0 }, 0, lowerBound);
        CurrentBranches.Enqueue(root, lowerBound);

        int bestCost = INF;
        List<int> bestPath = null;

        while (pq.Count > 0)
        {
            var node = pq.Dequeue();

            if (node.LowerBound >= bestCost) continue;

            if (node.Level == n - 1)
            {
                int finalCost = node.CurrentCost + graph[node.CurrentCity, 0];
                if (finalCost < bestCost)
                {
                    bestCost = finalCost;
                    bestPath = new List<int>(node.Path);
                    bestPath.Add(0); // повернення до початку
                }
                continue;
            }

            for (int next = 0; next < n; next++)
            {
                if (!node.Path.Contains(next) && node.CostMatrix[node.CurrentCity, next] != INF)
                {
                    int[,] newMatrix = (int[,])node.CostMatrix.Clone();

                    // Обнулення рядка та стовпця
                    for (int i = 0; i < n; i++)
                    {
                        newMatrix[node.CurrentCity, i] = INF;
                        newMatrix[i, next] = INF;
                    }
                    newMatrix[next, 0] = INF;

                    int newCost = node.CurrentCost + node.CostMatrix[node.CurrentCity, next];
                    int newBound = newCost + ReduceMatrix(newMatrix);
                    var newPath = new List<int>(node.Path);
                    newPath.Add(next);

                    pq.Enqueue(new TSPNode(newMatrix, newPath, node.Level + 1, newCost, newBound, next), newBound);
                }
            }
        }

        Console.WriteLine("Оптимальний маршрут: " + string.Join(" -> ", bestPath));
        Console.WriteLine("Мінімальна вартість: " + bestCost);
    }

    public static void Main()
    {
        int[,] graph = {
            { INF, 20, 42, 35 },
            { 20, INF, 30, 34 },
            { 42, 30, INF, 12 },
            { 35, 34, 12, INF }
        };

        Solve(graph);
    }
}
