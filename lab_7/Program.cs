using MathNet.Symbolics;

Console.Write("Enter your function to minimize(e.g., x^2 - 2*x - 2*x*cos(x)):\n>>> ");
string input = Console.ReadLine()!;

SymbolicExpression expression;
try
{
    expression = SymbolicExpression.Parse(input);
}
catch
{
    Console.WriteLine("Invalid function.");
    return;
}

Console.Write("Enter left bound:\n>>> ");
double a = double.Parse(Console.ReadLine()!);

Console.Write("Enter right bound:\n>>> ");
double b = double.Parse(Console.ReadLine()!);

Console.Write("Enter precision (e.g., 0.05):\n>>> ");
double epsilon = double.Parse(Console.ReadLine()!);


(double minimum, double minValue) = FuncMinimizer
    .FibonacciSearch(
        expression,
        "x", 
        a, b, 
        epsilon);

Console.WriteLine($"Minimum of function: x = {minimum:F9}, f(x) = {minValue:F9}");