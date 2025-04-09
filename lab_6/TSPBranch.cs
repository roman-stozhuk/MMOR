class TSPBranch
{
    public int[,] RouteMatrix;

    public int[] Path;
    
    public int LowerBound;

    public TSPBranch(int[,] routeMatrix, int[] path, int lowerBound)
    {
        RouteMatrix = routeMatrix;
        Path = path;
        LowerBound = lowerBound;
    }
}
