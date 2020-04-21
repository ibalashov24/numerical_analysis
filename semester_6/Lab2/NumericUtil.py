''' 
Some utility for LS solution iterative metods
'''

import numpy as np

# Transfoms Ax = b to x = Hx + g (if A[i][i] != 0)
def transform_system(matrix, free_column):
    # H =  E - D^(-1) * A
    h = np.empty(matrix.shape)
    for i in range(h.shape[0]):
        for j in range(h.shape[0]):
            h[i, j] = (-matrix[i][j] / matrix[i][i]) if i != j else .0

    # g = D ^ (-1) * b
    g = np.empty(free_column.shape)
    for i in range(g.shape[0]):
        g[i][0] = free_column[i][0] / matrix[i][i]

    return (h, g)


# Returns eigenvalue with maximan absolute value
def get_max_abs_eigen(matrix):
    (eigen, _) = np.linalg.eig(matrix)
    return max([abs(v) for v in eigen])

# Lusternik enchancement for iterative LS-solution algorithms
def lusternik_improvment(matrix, approx_prev, appox_curr):
    max_eigen = get_max_abs_eigen(matrix)

    if max_eigen < 1:
        return approx_prev + (1.0 / (1 - max_eigen)) * (appox_curr - approx_prev)
    else:
        return appox_curr
