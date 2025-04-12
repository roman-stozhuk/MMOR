static class Fibonacci
{
    static private List<int> cache = [1, 1];

    static public int Number(int index)
    {
        if (index < 0)
            throw new ArgumentOutOfRangeException(nameof(index), "Fibonacci index must be non-negative.");

        while (cache.Count <= index)
        {
            int last = cache[^1] + cache[^2];
            cache.Add(last);
        }

        return cache[index];
    }
}