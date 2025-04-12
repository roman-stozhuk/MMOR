Console.WriteLine("1: f(x) = x^2 - 2x - 2xcos(x)");
Console.WriteLine("2: f(x) = x^2 - 2x + e^-x");
Console.WriteLine("Select function: ");
int functionChoice = int.Parse(Console.ReadLine()!);

Func<double, double> f = functionChoice switch
{
    1 => (x) => x * x - 2 * x * (1 + Math.Cos(x)),
    2 => (x) => x * x - 2 * x + Math.Pow(Math.E, (-x)), 
    _ => throw new ArgumentException("Invalid function number")
};

Console.Write("Enter left bound: ");
double a = double.Parse(Console.ReadLine()!);

Console.Write("Enter right bound: ");
double b = double.Parse(Console.ReadLine()!);

Console.Write("Enter precision: ");
double epsilon = double.Parse(Console.ReadLine()!);

double minimum = FuncMinimizer.FibonacciSearch(f, a, b, epsilon);

Console.WriteLine($"Minimum of function: x = {minimum}, f(x) = {f(minimum)}");