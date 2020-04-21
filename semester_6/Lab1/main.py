#!/usr/bin/python3

import numpy as np


'''
Solving system of linear equations using different methods
'''

def backward_movement(a):
    power = a.shape[0]

    solution = np.empty((power, 1))
    for i in reversed(range(power)):
        solution[i][0] = a[i][power]
        for j in range(i + 1, power):
            solution -= a[i][j] * solution[j]

    return solution

def solve_gauss(matrix, free_column):
    a = np.append(matrix, free_column, axis=1)
    power = a.shape[0]

    # Forward movement
    for k in range(0, power):
        tmp = a[k, k]
        if tmp != 0:
            for i in range(k + 1, power + 1):
                a[k, i] /= tmp

        for i in range(k + 1, power):
            tmp = a[i, k]
            for j in range(k + 1, power + 1): 
                a[i, j] -= a[k, j] * tmp

    return backward_movement(a)


def solve_jordan(matrix, free_column):
    a = np.append(matrix, free_column, axis=1)
    power = a.shape[0]

    # Forward movement
    for k in range(0, power):
        tmp = a[k, k]
        if tmp != 0:
            for i in range(k, power + 1):
                a[k, i] /= tmp

        for i in range(0, power):
            if i != k:
                tmp = a[i, k]
                for j in range(k, power + 1): 
                    a[i, j] -= a[k, j] * tmp

    return backward_movement(a)

def solve_LU(matrix, free_column):
    power = matrix.shape[0]

    # LU-decomposition with Doolitle method
    l = np.empty(matrix.shape)
    u = np.empty(matrix.shape)
    for i in range(0, power):
        for j in range(i, power):
            u[i][j] = matrix[i][j]
            for k in range(0, i): 
                u[i][j] -= l[i][k] * u[k][j]

        for j in range(i, power):
            if i == j:
                l[i][i] = 1.0
                continue

            l[j][i] = matrix[j][i]
            for k in range(0, i):
                l[j][i] -= l[j][k] * u[k][i] 
            l[j][i] /= u[i][i]

    # Custom backward movement

    # Ly = b
    solution_L = np.empty((power, 1))
    for i in range(power):
        solution_L[i] = free_column[i][0]
        for j in range(i):
            solution_L[i] -= l[i][j] * solution_L[j][0]

    # Ux = y
    solution = np.empty((power, 1))
    for i in reversed(range(solution.shape[0])):
        solution[i] = solution_L[i][0]
        for j in range(i + 1, power):
            solution[i] -= u[i][j] * solution[j][0]
        solution[i] /= u[i][i]
    
    return solution

# Main
matrix = np.array([[8.29381, 0.995516, -0.50617],
                  [0.995516, 6.298197, 0.595772],
                  [-0.560617, 0.595772, 0.595772]])
free_column = np.array([[0.766522], [3.844422], [5.239231]])

print("Solution with Gauss method")
print(solve_gauss(matrix, free_column))

print("Solution with Jordan method")
print(solve_jordan(matrix, free_column))

print("Solution with LU-decomposition method")
print(solve_LU(matrix, free_column))

