#!/usr/bin/python3
'''
Solving system of linear equations using iterative methods
'''

import numpy as np
import math
from NumericUtil import *

ACCURACY = 1e-4

def solve_iterative(h, g, step_function):
    iteration_count = 0
    solution = g
    current_solution = step_function(h, g, solution)
    while np.linalg.norm(current_solution - solution) >= ACCURACY:
        solution = current_solution
        current_solution = step_function(h, g, current_solution)

        iteration_count += 1

    optimized = \
        lusternik_improvment(h, solution, current_solution)

    return (current_solution, optimized, iteration_count)

def solve_simple_iteration(h, g):
    def step(h, g, old_x):
        x = np.empty(old_x.shape)
        for i in range(old_x.shape[0]):
            x[i] = g[i]
            for j in range(old_x.shape[0]):
                x[i] += h[i][j] * old_x[j]
        return x

    return solve_iterative(h, g, step)
    
def solve_seidel(h, g):
    def step(h, g, old_x):
        x = np.empty(old_x.shape)
        for i in range(x.shape[0]):
            x[i] = g[i]
            for j in range(i - 1):
                x[i] += h[i][j] * x[j]
            for j in range(i, x.shape[0]):
                x[i] += h[i][j] * old_x[j]
        return x

    return solve_iterative(h, g, step)

def solve_upper_relaxation(matrix, free_column):
    def step(h, g, old_x):
        q = 2.0 / (1 + math.sqrt(1.0 - (get_max_abs_eigen(h)) ** 2)) 
        x = np.empty(old_x.shape)
        for i in range(x.shape[0]):
            x[i] = g[i] - old_x[i]
            for j in range(i - 1):
                x[i] += h[i][j] * x[j]
            for j in range(i + 1, x.shape[0]):
                x[i] += h[i][j] * old_x[j]
            x[i] = old_x[i] + q * x[i]
        return x
    
    return solve_iterative(h, g, step)

def calc_appriori_approx(matrix, free_column, k):
    h_norm = np.linalg.norm(h)
    g_norm = np.linalg.norm(g)

    return (h_norm ** k) * g_norm * (1.0 + 1.0 / (1 - h_norm))

# Main
matrix = np.array([[8.29381, 0.995516, -0.50617],
                  [0.995516, 6.298197, 0.595772],
                  [-0.560617, 0.595772, 0.595772]])
free_column = np.array([[0.766522], [3.844422], [5.239231]])

h, g = transform_system(matrix, free_column)

print("Simple iteration with Lusternig improvement:")
result, optimized, iter_count = solve_simple_iteration(h, g)
print("Solution:\n", result, "\nIteration count: ", iter_count)
print("Seidel-optimized solution:\n", optimized)
print("A priori approximation:", calc_appriori_approx(h, g, iter_count))

print("Seidel method with Lusternig improvement:")
result, optimized, iter_count = solve_seidel(h, g)
print("Solution:\n", result, "\nIteration count: ", iter_count)
print("Seidel-optimized solution:", optimized)
print("A priori approximation:", calc_appriori_approx(h, g, iter_count))

print("Seidel method with Lusternig improvement:")
result, optimized, iter_count = solve_seidel(h, g)
print("Solution:\n", result, "\nIteration count: ", iter_count)
print("Seidel-optimized solution:\n", optimized)
print("A priori approximation:", calc_appriori_approx(h, g, iter_count))

print("Upper relaxation method with Lusternig improvement:")
result, _, iter_count = solve_upper_relaxation(h, g)
print("Solution:\n", result, "\nIteration count: ", iter_count)
print("Seidel-optimized solution:\n", optimized)
print("A priori approximation:", calc_appriori_approx(h, g, iter_count))

