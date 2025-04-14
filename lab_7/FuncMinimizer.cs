using MathNet.Symbolics;
using Expression = MathNet.Symbolics.SymbolicExpression;

public static class FuncMinimizer
{
    public static double EvaluateExpr(
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

        double f1 = EvaluateExpr(expression, variable, x1);
        double f2 = EvaluateExpr(expression, variable, x2);

        Console.WriteLine($"\nInitial interval: [{leftBound:F7}, {rightBound:F7}]");
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
                f1 = EvaluateExpr(expression, variable, x1);
            }
            else
            {
                leftBound = x1;
                
                x1 = x2;
                f1 = f2;

                x2 = leftBound + (rightBound - leftBound) 
                    * Fibonacci.Number(n - i - 1) 
                    / Fibonacci.Number(n - i);
                f2 = EvaluateExpr(expression, variable, x2);
            }

            Console.WriteLine(
                $"Updated interval: [{leftBound:F7}, {rightBound:F7}] on {i} iteration.");
        }

        double minimum = (x1 + x2) / 2;
        double minValue = EvaluateExpr(expression, variable, minimum);
        return (minimum, minValue);
    }
}