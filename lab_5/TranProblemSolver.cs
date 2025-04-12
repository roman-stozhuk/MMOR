using Utils;

class TranProblemSolver
{
    private int[] supplies;
    private int m;
    private int[] demands;
    private int n;
    private int[,] prices;

    public TranProblemSolver(int[] A, int[] B, int[,] C)
    {
        this.supplies = A;
        this.demands = B;

        this.m = supplies.Length;
        this.n = demands.Length;

        this.prices = C;
    }

    public TranProblemSolver(string problemPath)
    {
        string[] lines = File.ReadAllLines(problemPath);

        this.supplies = lines[0].Split().Select(int.Parse).ToArray();
        this.demands = lines[1].Split().Select(int.Parse).ToArray();

        this.m = supplies.Length;
        this.n = demands.Length;


        this.prices = new int[m, n];
        for (int i = 0; i < m; i++)
        {
            int[] row = lines[i + 3].Split().Select(int.Parse).ToArray();
            for (int j = 0; j < n; j++)
                prices[i, j] = row[j];
        }
    }

    public int?[,] NorthWestCorner()
    {
        int?[,] plan = Initializer.InitMatrix(m, n, (int?)null);
        int[] suppliesCopy = (int[])supplies.Clone();
        int[] demandsCopy = (int[])demands.Clone();

        int i = 0, j = 0;
        while (i != m && j != n)
        {
            int amount = Math.Min(suppliesCopy[i], demandsCopy[j]);
            plan[i, j] = amount; 
            suppliesCopy[i] -= amount;
            demandsCopy[j] -= amount;

            if (suppliesCopy[i] == 0) i++;
            else if (demandsCopy[j] == 0) j++;
        }

        return plan;
    }

    private List<(int, int)> BuildTheCycle(List<(int, int)> basis)
    {
        var start = basis.Last();
        var cycle = new List<(int, int)>();

        bool Dfs((int, int) current, bool isHorizontal, 
                List<(int, int)> path, HashSet<(int, int)> visited
        )
        {
            foreach (var next in basis)
            {
                bool sameRow = current.Item1 == next.Item1;
                bool sameCol = current.Item2 == next.Item2;

                if ((isHorizontal && sameRow) || (!isHorizontal && sameCol))
                {
                    if (next == start && path.Count >= 4)
                    {
                        cycle.AddRange(path);
                        return true;
                    }

                    if (!visited.Contains(next))
                    {
                        visited.Add(next);
                        path.Add(next);

                        if (Dfs(next, !isHorizontal, path, visited)) return true;

                        path.RemoveAt(path.Count - 1);
                        visited.Remove(next);
                    }
                }
            }

            return false;
        }

        var path = new List<(int, int)> { start };
        var visited = new HashSet<(int, int)> { start };
        Dfs(start, true, path, visited);

        return cycle;
    }

    private List<(int, int)> FindBasis(int?[,] plan)
    {
        List<(int, int)> listOfDeliveries = [];

        for (int i = 0; i < plan.GetLength(0); i++)
        {
            for (int j = 0; j < plan.GetLength(1); j++)
            {
                if (plan[i, j].HasValue)
                {
                    listOfDeliveries.Add((i, j));
                }
            }
        }

        return listOfDeliveries;
    }

    public void SavePlan(string fileName, int?[,] plan)
    {
        int[,] deliveries = ConvertPlanToDeliveries(plan);
        int optimalPrice = 0;
        using (StreamWriter writer = new StreamWriter(fileName))
        {
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    optimalPrice += deliveries[i, j] * prices[i, j];
                    writer.Write(deliveries[i, j]);
                    if (j < n - 1) writer.Write(" ");
                }
                writer.WriteLine();
            }

            writer.WriteLine($"F = {optimalPrice}");
        }
    }

    public void PrintPlan(int?[,] plan)
    {
        int[,] deliveries = ConvertPlanToDeliveries(plan);
        int optimalPrice = 0;
        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                optimalPrice += deliveries[i, j] * prices[i, j];
                Console.Write(deliveries[i, j]);
                if (j < n - 1) Console.Write(" ");
            }
            Console.WriteLine();
        }

        Console.WriteLine($"F = {optimalPrice}\n");
    }

    public int[,] ConvertPlanToDeliveries(int?[,] plan)
    {
        int[,] converted = new int[m, n];
        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                converted[i, j] = plan[i, j] ?? 0;
            }
        }
        return converted;
    }

    public int?[,] PotentialMethod(int?[,] plan) 
    {
        int?[] u = new int?[m];
        u[0] = 0;
        int?[] v = new int?[n];

        var listOfDeliveries = FindBasis(plan);

        //calculate potentials
        var cloneOfBasis = new List<(int, int)>(listOfDeliveries);
        do
        {
            var toRemove = new List<(int, int)>();

            foreach (var (i, j) in cloneOfBasis)
            {
                if (u[i].HasValue && !v[j].HasValue)
                {
                    v[j] = prices[i, j] - u[i];
                    toRemove.Add((i, j));
                }
                else if (!u[i].HasValue && v[j].HasValue)
                {
                    u[i] = prices[i, j] - v[j];
                    toRemove.Add((i, j));
                }
            }

            foreach (var item in toRemove)
            {
                cloneOfBasis.Remove(item);
            }

        } while (cloneOfBasis.Count != 0);

        //calculate deltas
        var max_delta = (0, 0, u[0].Value + v[0].Value - prices[0, 0]);
        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (!plan[i, j].HasValue)
                {
                    int delta = u[i].Value + v[j].Value - prices[i, j];
                    if (delta > max_delta.Item3)
                        max_delta = (i, j, delta);
                }
            }
        }

        //plan is optimal
        PrintPlan(plan);
        if (max_delta.Item3 <= 0) return plan;

        //find the cycle
        listOfDeliveries.Add((max_delta.Item1, max_delta.Item2));
        plan[max_delta.Item1, max_delta.Item2] = 0;
        var cycle = BuildTheCycle(listOfDeliveries);

        //rebuild the cycle
        var minusCells = cycle.Where((_, index) => index % 2 == 1).ToList();
        int theta = minusCells.Min(cell => plan[cell.Item1, cell.Item2].Value);

        for (int k = 0; k < cycle.Count; k++)
        {
            var (i, j) = cycle[k];
            if (k % 2 == 0) // "+ cell"
                plan[i, j] = plan[i, j].Value + theta;
            else // "- cell"
                plan[i, j] = plan[i, j].Value - theta;
        }

        foreach (var (i, j) in minusCells)
        {
            if (plan[i, j] == 0)
            {
                plan[i, j] = null;
                break;
            }
        }

        return PotentialMethod(plan);
    }
}