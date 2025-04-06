using System.Web;
using Utils;

class TranProblemSolver
{
    private int[] supplies;
    private int m;
    private int[] demands;
    private int n;
    private int[,] prices;

    public int?[,] deliveries;


    public TranProblemSolver(int[] A, int[] B, int[,] C)
    {
        this.supplies = A;
        this.demands = B;

        this.m = supplies.Length;
        this.n = demands.Length;

        this.prices = C;

        this.deliveries = MathUtils.InitMatrix(m, n, (int?)null);
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

        this.deliveries = MathUtils.InitMatrix(m, n, (int?)null);
    }

    public int?[,] NorthWestCorner()
    {
        int[] suppliesCopy = (int[])supplies.Clone();
        int[] demandsCopy = (int[])demands.Clone();

        int i = 0, j = 0;
        while (i != m && j != n)
        {
            int amount = Math.Min(suppliesCopy[i], demandsCopy[j]);
            deliveries[i, j] = amount; 
            suppliesCopy[i] -= amount;
            demandsCopy[j] -= amount;

            if (suppliesCopy[i] == 0) i++;
            else if (demandsCopy[j] == 0) j++;
        }

        return deliveries;
    }

    public int[,] PotentialMethod(int?[,] plan) 
    {

    }
}


class Program
{
    static void Main()
    {
        // Запис плану у файл
        using (StreamWriter writer = new StreamWriter(outputPath))
        {
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    writer.Write(plan[i, j]);
                    if (j < n - 1) writer.Write(" ");
                }
                writer.WriteLine();
            }
        }
    }
}
