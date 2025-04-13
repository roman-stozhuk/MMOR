using Expr = MathNet.Symbolics.SymbolicExpression;

Console.WriteLine("Введіть вашу функцію(приклад: 2*x1 + 2*x2 - x3^3)");
string function = Console.ReadLine()!;
string[] variables = { "x1", "x2", "x3" };
var expression = Expr.Parse(function);

Console.WriteLine("Введи x0 через пробіл:");
double[] x0 = Console.ReadLine()!
    .Split()
    .Select(double.Parse)
    .ToArray();

Console.Write("Введи точність (наприклад, 0.05): ");
var precision = double.Parse(Console.ReadLine()!);

// Виклик методу градієнтного спуску
(double[] x, double f) = GradientDescent.Run(expression, variables, x0, precision, true);

Console.WriteLine("Знайдений мінімум:");
for (int i = 0; i < x.Length; i++)
{
    Console.WriteLine($"x{i + 1} = {x[i]}");
}
Console.WriteLine($"F = {f}");