using MathNet.Symbolics;
using Expr = MathNet.Symbolics.SymbolicExpression;

public static class GradientDescent
{
    public static double[] Run(
        Expr expression,
        string[] variables,
        double[] x0,
        double precision,
        bool toLog)
    {
        double searchLeft = 0.0;
        double searchRight = 10.0;

        int n = variables.Length;
        int iteration = 0;

        var x1 = (double[])x0.Clone();

        while (true)
        {
            var gradient = new double[n];
            // for (int i = 0; i < n; i++)
            //     gradient[i] = expression.DifferentiateAt(variables[i], x0[i])
            //                             .RealNumberValue;
            for (int i = 0; i < n; i++)
            {
                var substitutedExpr = expression.Differentiate(variables[i]);

                // підставити всі значення x0 у похідну
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




            (double step, _) = FuncMinimizer.FibonacciSearch(
                stepExpr, stepName, searchLeft, searchRight, precision, false);



            // Array.Copy(x1, x0, n);
            // for (int i = 0; i < n; i++)
            //     x1[i] = x0[i] - step * gradient[i];

            for (int i = 0; i < n; i++)
            {
                x0[i] = x1[i];
                x1[i] = x1[i] - step * gradient[i];
            }


            double error = 0;
            for (int i = 0; i < n; i++)
                error += Math.Pow(x1[i] - x0[i], 2);
            error = Math.Sqrt(error);

            if (toLog)
            {
                Console.WriteLine($"\n--- Iteration {iteration} ---");
                Console.WriteLine($"x = ({string.Join(", ", x1.Select(v => v.ToString("F6")))})");
                Console.WriteLine($"error = {error}; beta = {step}");
            }

            if (error < precision)
            {
                return x1;
            }

            iteration++;
        }
    }
}