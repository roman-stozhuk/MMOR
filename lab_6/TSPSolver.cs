using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using Microsoft.VisualBasic;
using Utils;

class TSProblemSolver
{
    private const int INF = int.MaxValue / 4 * 3;

    private const int INF_THRESHOLD = int.MaxValue / 2;

    public int[,] BaseRouteTable;

    public int n;

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
    
    private int CountCities(int[] path)
    {
        return path.Count(x => x != -1);
    }

    private int CalculateRating(int[,] matrix, int row, int col)
    {
        matrix[row, col] = INF;
        int lowestInColumn = INF;
        for (int i = 0; i < n; i++)
        {
            if (matrix[i, col] < lowestInColumn) lowestInColumn = matrix[i, col]; 
        }

        int lowestInRow = INF;
        for (int j = 0; j < n; j++)
        {
            if (matrix[row, j] < lowestInRow) lowestInRow = matrix[row, j]; 
        }
        matrix[row, col] = 0;
        return lowestInColumn + lowestInRow; 
    } 

    private (int, int) FindHighestRating(int[,] matrix)
    {
        int maxRating = -1;
        (int, int) pos = (-1, -1);

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (matrix[i, j] == 0) {
                    int rating = CalculateRating(matrix, i, j);
                    if (rating > maxRating)
                    {
                        maxRating = rating;
                        pos = (i, j);
                    }
                }
            }
        }

        return pos;
    }
    
    private TSPBranch IncludeIntoBranch(TSPBranch originalBranch, (int, int) branchingPoint)
    {
        (int i, int j) = branchingPoint;
        // "erase" row and column
        int[,] newMatrix = (int[,])originalBranch.RouteMatrix.Clone();
        for (int index = 0; index < n; index++)
        {
            newMatrix[i, index] = INF;
            newMatrix[index, j] = INF;
        }
        newMatrix[j, i] = INF;

        int[] newPath = (int[])originalBranch.Path.Clone();
        newPath[i] = j;

        int newBound = originalBranch.LowerBound + ReduceMatrix(newMatrix);

        if (CountCities(newPath) == n-1)
        {
            i = 0;
            j = 0;
            // findLastElement
            while (newMatrix[i, j] > INF_THRESHOLD)
            {
                i++;
                if (i == n) 
                {
                    i = 0;
                    j++;
                }
            }

            newBound += newMatrix[i, j];
            newPath[i] = j;
        }

        return new TSPBranch(newMatrix, newPath, newBound);
    }

    private TSPBranch ExcludeFromBranch(TSPBranch originalBranch, (int, int) branchingPoint)/////////////
    {
        (int i, int j) = branchingPoint;

        int[,] newMatrix = (int[,])originalBranch.RouteMatrix.Clone();
        newMatrix[i, j] = INF;

        int[] newPath = (int[])originalBranch.Path.Clone();

        int newBound = originalBranch.LowerBound + ReduceMatrix(newMatrix);

        return new TSPBranch(newMatrix, newPath, newBound);
    }

    public void PrintRoute(int[] path)
    {
        if (CountCities(path) < n) Console.WriteLine("Шлях неповний!");

        int length = 0;
        int i = 0;
        int count = n;
        string route = "Оптимальний маршрут:";

        while (count > 0)
        {
            int j = path[i];
            route += $" {i + 1} -{BaseRouteTable[i, j]}->";
            length += BaseRouteTable[i, j];
            i = j;
            count--;
        }

        route += $" {i+1}";

        Console.WriteLine(route);
        Console.WriteLine("Мінімальна вартість: " + length);
    }

    public void Solve()
    {
        int[,] reducedMatrix = (int[,])BaseRouteTable.Clone();
        int lowerBound = ReduceMatrix(reducedMatrix);

        var root = new TSPBranch(reducedMatrix, Initializer.InitArray(n, -1), lowerBound);
        PriorityQueue<TSPBranch, int> CurrentBranches = new();
        CurrentBranches.Enqueue(root, lowerBound);
        
        int[] shortestPath = null!;
        int shortestBound = INF;
        
        while (CurrentBranches.Count > 0)
        {
            var branch = CurrentBranches.Dequeue();

            if (branch.LowerBound >= shortestBound) continue;

            if (CountCities(branch.Path) == n)
            {
                shortestBound = branch.LowerBound;
                shortestPath = branch.Path;
                continue;
            }

            var branchingPoint = FindHighestRating(branch.RouteMatrix);

            var includingBranch = IncludeIntoBranch(branch, branchingPoint);
            var excludingBranch = ExcludeFromBranch(branch, branchingPoint);

            CurrentBranches.Enqueue(includingBranch, includingBranch.LowerBound);
            CurrentBranches.Enqueue(excludingBranch, excludingBranch.LowerBound);
        }

        PrintRoute(shortestPath);
    }
}