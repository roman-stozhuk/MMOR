static class Fibonacci
{
    static private List<int> cache = [1, 1];

    static public int Number(int index)
    {
        if (cache.Count-1 < index)
        {
            cache[index] = Number(index - 1) + Number(index - 2);
        }

        return cache[index];
    }
}