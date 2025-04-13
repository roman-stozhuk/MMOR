using MathNet.Symbolics;
using Expr = MathNet.Symbolics.SymbolicExpression;

public static class GradientDescent
{
    private static Dictionary<string, FloatingPoint> ToDictionary(string[] vars, double[] values)
    {
        var dict = new Dictionary<string, FloatingPoint>();
        for (int i = 0; i < vars.Length; i++)
            dict[vars[i]] = values[i];
        return dict;
    }

    private static double EvaluateExpr(
        Expr expr, 
        string varName, 
        double val)
    {
        var dict = new Dictionary<string, FloatingPoint>{ [varName] = val };
        return expr.Evaluate(dict).RealValue;
    }

    public static (double, double) DichotomySearch(
        Expr expression,
        string variable,
        double leftBound,
        double rightBound,
        double epsilon)
    {
        double delta = epsilon / 5.0;

        while ((rightBound - leftBound) > epsilon)
        {
            double mid = (leftBound + rightBound) / 2.0;
            double x1 = mid - delta;
            double x2 = mid + delta;

            double f1 = EvaluateExpr(expression, variable, x1);
            double f2 = EvaluateExpr(expression, variable, x2);

            if (f1 < f2)
                rightBound = x2;
            else
                leftBound = x1;
        }

        double beta = (leftBound + rightBound) / 2.0;
        double fmin = EvaluateExpr(expression, variable, beta);
        return (beta, fmin);
    }

    public static (double[], double) Run(
        Expr expression,
        string[] variables,
        double[] x0,
        double precision,
        bool toLog)
    {
        double searchLeft = 0.0;
        double searchRight = 1.0;

        int n = variables.Length;
        int iteration = 0;

        var x1 = (double[])x0.Clone();

        while (true)
        {
            var gradient = new double[n];
            for (int i = 0; i < n; i++)
            {
                var substitutedExpr = expression.Differentiate(variables[i]);

                for (int j = 0; j < n; j++)
                {
                    substitutedExpr = substitutedExpr.Substitute(variables[j], x0[j]);
                }
                gradient[i] = substitutedExpr.RealNumberValue;
            }


            string stepName = "B";
            Expr stepExpr = expression;
            for (int i = 0; i < n; i++)
            {
                stepExpr = stepExpr.Substitute(
                            variables[i], 
                            Expr.Parse($"{x0[i]} - {stepName} * {gradient[i]}"));
            }


            (double step, _) = DichotomySearch(stepExpr, stepName, searchLeft, searchRight, precision);


            for (int i = 0; i < n; i++)
            {
                x0[i] = x1[i];
                x1[i] = x1[i] - step * gradient[i];
            }

            double fx_cur = expression.Evaluate(ToDictionary(variables, x1)).RealValue;
            double fx_prev = expression.Evaluate(ToDictionary(variables, x0)).RealValue;
            var error = Math.Abs(fx_cur - fx_prev);

            if (toLog)
            {
                Console.WriteLine($"\n--- Iteration {iteration} ---");
                Console.WriteLine($"x = ({string.Join(", ", x1.Select(v => v.ToString("F6")))})");
                Console.WriteLine($"error = {error}; beta = {step}");
            }

            if (error < precision)
            {
                return (x1, fx_cur);
            }

            iteration++;
        }
    }
}