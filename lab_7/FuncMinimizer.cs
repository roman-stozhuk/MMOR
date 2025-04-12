using MathNet.Symbolics;
using Expression = MathNet.Symbolics.SymbolicExpression;

public static class FuncMinimizer
{
    private static double EvaluateExpression(
        Expression expr, 
        string varName, 
        double val)
    {
        var dict = new Dictionary<string, FloatingPoint>{ [varName] = val };
        return expr.Evaluate(dict).RealValue;
    }

    public static (double, double) FibonacciSearch(
        Expression expression,
        string variable, 
        double leftBound, 
        double rightBound,
        double precision,
        bool toLog)
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

        double f1 = EvaluateExpression(expression, variable, x1);
        double f2 = EvaluateExpression(expression, variable, x2);

        if (toLog) Console.WriteLine($"Initial interval: [{leftBound}, {rightBound}]");
        for (int i = 1; i <= n - 2; i++)
        {
            if (f1 <= f2)
            {
                rightBound = x2;

                x2 = x1;
                f2 = f1;

                x1 = leftBound + (rightBound - leftBound) 
                    * Fibonacci.Number(n - i - 2)
                    / Fibonacci.Number(n - i);
                f1 = EvaluateExpression(expression, variable, x1);
            }
            else
            {
                leftBound = x1;
                
                x1 = x2;
                f1 = f2;

                x2 = leftBound + (rightBound - leftBound) 
                    * Fibonacci.Number(n - i - 1) 
                    / Fibonacci.Number(n - i);
                f2 = EvaluateExpression(expression, variable, x2);
            }

            if (toLog) Console.WriteLine(
                $"Updated interval: [{leftBound:F6}, {rightBound:F6}] on {i} iteration.");
        }

        double minimum = (x1 + x2) / 2;
        double minValue = expression.Evaluate(
            new Dictionary<string, FloatingPoint> { { "x", minimum } }).RealValue;
        return (minimum, minValue);
    }
}