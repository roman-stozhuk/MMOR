class TSPBranch
{
    public int[,] RouteMatrix;

    public List<int> Path;
    
    public int CurrentLength;
    
    public int LowerBound;
    
    public int CurrentCity => Path[^1];
    
    public int Depth => Path.Count - 1;

    public TSPBranch(int[,] routeMatrix, List<int> path, int length, int bound)
    {
        RouteMatrix = routeMatrix;
        Path = path;
        CurrentLength = length;
        LowerBound = bound;
    }
}
