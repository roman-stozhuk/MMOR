public static class FuncMinimizer
{
    public static double FibonacciSearch(
        Func<double, double> f, 
        double leftBound, 
        double rightBound,
        double precision)
    {
        int n = 0;
        {
            var baseCondition = (rightBound - leftBound) / precision;

            while (Fibonacci.Number(n) < baseCondition) 
                n++;
        }

        double x1 = leftBound + (rightBound - leftBound) 
            * Fibonacci.Number(n-2) 
            / Fibonacci.Number(n);
        double x2 = leftBound + (rightBound - leftBound) 
            * Fibonacci.Number(n-1) 
            / Fibonacci.Number(n);

        double f1 = f(x1);
        double f2 = f(x2);

        Console.WriteLine($"Initial interval: [{leftBound}, {rightBound}]");
        int iteration = 1;
        while (rightBound - leftBound >= precision)
        {
            if (f1 <= f2)
            {
                rightBound = x2;

                x2 = x1;
                f2 = f1;

                x1 = leftBound + (rightBound - leftBound) 
                    * Fibonacci.Number(n - iteration - 2)
                    / Fibonacci.Number(n - iteration);
                f1 = f(x1);
            }
            else
            {
                leftBound = x1;
                
                x1 = x2;
                f1 = f2;

                x2 = leftBound + (rightBound - leftBound) 
                    * Fibonacci.Number(n - iteration - 1) 
                    / Fibonacci.Number(n - iteration);
                f2 = f(x2);
            }

            Console.WriteLine($"Updated interval: [{leftBound:F6}, {rightBound:F6}] on {iteration} iteration.");
            iteration++;
        }

        return (f1 < f2) ? x1 : x2;
    }
}