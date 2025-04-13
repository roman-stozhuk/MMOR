import autograd.numpy as np
from autograd import grad

def build_function_from_input(expression, n):
    def f(x):
        vars_to_values = {f'x{i+1}': x[i] for i in range(n)}
        return eval(expression, vars_to_values)
    return f

def dichotomy_method(f, a, b, precision):
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

def gradient_descent(f, x0, precision):
    grad_func = grad(f) 
    x_cur = np.array(x0, dtype=float)
    x_prev = x_cur.copy()
    iteration = 0

    while True:
        gradient = grad_func(x_cur)

        phi = lambda step: f(x_cur - step * gradient)

        step = dichotomy_method(phi, 0.0, 1.0, precision)

        x_prev = x_cur
        x_cur = x_cur - step * gradient

        print(f"Iteration = {iteration}, x = {x_cur}, f(x) = {f(x_cur):.9f}")

        if np.abs(f(x_cur) - f(x_prev)) < precision:
            return x_cur, f(x_cur)

        iteration += 1


expression = input("Введіть функцію (наприклад: x1**2 + 8*x2 - x1*x3):\n>>> ")

x0 = list(map(float, input("Введіть початкове наближення (координати через пробіл):\n>>> ").split()))

epsilon = float(input("Введіть точність (наприклад 0.05):\n>>> "))

f = build_function_from_input(expression, len(x0))

x_min, f_min = gradient_descent(f, x0, epsilon)

print(f"\nМінімум f(x) = {f_min:.10f} досягнуто при:")
print(f"x = [{x_min}]")