import autograd.numpy as np
from autograd import grad

def dichotomy_search(f, a, b, precision):
    delta = precision / 5
    while (b - a) > precision:
        mid = (a + b) / 2
        x1 = mid - delta
        x2 = mid + delta
        if f(x1) < f(x2):
            b = x2
        else:
            a = x1
    return (a + b) / 2

def gradient_descent(f, x0, precision, log=False):
    grad_f = grad(f) 
    x = np.array(x0, dtype=float)
    prev_x = x.copy()
    iteration = 0

    while True:
        g = grad_f(x)

        phi = lambda beta: f(x - beta * g)

        step = dichotomy_search(phi, 0.0, 1.0, precision)

        prev_x = x
        x = x - step * g

        error = np.abs(f(x) - f(prev_x))

        if log:
            print(f"Iteration = {iteration}, x = {x}, f(x) = {f(x):.8f}, error = {error:.8f}")

        if error < precision:
            return x, f(x)

        iteration += 1

def build_function_from_input(expression, n):
    def f(x):
        vars_to_values = {f'x{i+1}': x[i] for i in range(n)}
        return eval(expression, vars_to_values)
    return f

expression = input("Введіть функцію (наприклад: x1**2 + 8*x2 - x1*x3):\n> ")

x0 = list(map(float, input("Введіть початкову точку (координати через пробіл):\n> ").split()))

epsilon = float(input("Введіть точність (наприклад 0.05):\n> "))

f = build_function_from_input(expression, len(x0))

x_min, f_min = gradient_descent(f, x0, epsilon, log=True)

print(f"\nМінімум F = {f_min:.10f} досягнуто при:")
print(x_min)
# for i, val in enumerate(x_min):
#     print(f"x{i+1} = {val:.10f}")
