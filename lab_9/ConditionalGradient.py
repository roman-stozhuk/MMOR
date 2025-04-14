import autograd.numpy as np
from autograd import grad
from scipy.optimize import linprog

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

def conditional_descent(f, x0, A, b, precision):
    computed_grad = grad(f) 
    x_cur = np.array(x0)
    x_prev = x_cur.copy()
    
    iteration = 0
    while True:
        lin_gradient = computed_grad(x_prev) * x_cur

        # There should be simplex method
        y_cur = linprog(lin_gradient, A_ub=A, b_ub=b, method='highs')
        # There should be simplex method

        direction = y_cur.x - x_cur

        phi = lambda beta: f(x_cur + beta * direction)
        
        step = dichotomy_method(phi, 0.0, 1.0, precision)

        x_prev = x_cur
        x_cur = x_cur + step * direction

        x_string = "[" + ", ".join(f"{xi:.10f}" for xi in x_cur) + "]"
        print(f"Iteration = {iteration}, x = {x_string}, f(x) = {f(x_cur):.9f}")


        if abs(f(x_cur) - f(x_prev)) < precision:
            return x_cur, f(x_cur)

        iteration += 1



expression = input("Введіть функцію (наприклад: x1**2 + 3*x2 + x2):\n>>> ")
x0 = list(map(float, input("Введіть початкове наближення (через пробіл):\n>>> ").strip().split()))
n = len(x0)

m = int(input("Введіть кількість обмежень (рядків):\n>>> "))
print(f"Введіть обмеження у форматі: a1 a2 ... an b (через пробіл):")

A = []
b = []
for i in range(m):
    constraint = list(map(float, input(f"Обмеження {i+1}: ").strip().split()))
    *a_row, b_elem = constraint

    A.append(a_row)
    b.append(b_elem)

A = np.array(A)
b = np.array(b)
epsilon = float(input("Введіть точність (наприклад 0.05):\n>>> "))


f = build_function_from_input(expression, n)
x_min, f_min = conditional_descent(f, x0, A, b, epsilon)

x_string = "[" + ", ".join(f"{xi:.10f}" for xi in x_min) + "]"
print(f"\nМінімум f(x) = {f_min:.10f} досягнуто при:")
print(f"x = {x_string}")