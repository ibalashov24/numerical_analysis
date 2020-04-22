#!/usr/bin/python3

import numpy as np

ACCURACY = 1e-3

def find_power_method(matrix):
    def step(y):
        vect = np.matmul(matrix, y.T)

        eigen = vect
        for i in range(y.shape[0]):
            eigen[i] /= y[i]
        
        return (vect, eigen)

    y = np.ones(matrix.shape[0])
    eigen_old = .0
    y, eigen = step(y)

    while np.linalg.norm(eigen - eigen_old, ord=np.inf) >= ACCURACY:
        eigen_old = eigen
        y, eigen = step(y)

    return (y / np.linalg.norm(y), max(eigen))

def find_scalar_method(matrix):
    def step(y):
        vect = np.matmul(matrix, y.T)

        denom = np.inner(y.T, y.T)
        num = np.inner(vect.T, y.T)
        eigen = num / denom

        return (vect, eigen)
    
    y = np.zeros(matrix.shape[0])
    y[1] = 1.0
    eigen_old = .0
    y, eigen = step(y)
    while abs(eigen - eigen_old) >= 1e-10:
        eigen_old = eigen
        y, eigen = step(y)

    return (y / np.linalg.norm(y), eigen)


# Main
matrix = np.array([[-1.48213, -0.03916, 1.08254],
                   [-0.03916, 1.13958, 0.01617],
                   [1.08254, 0.01617, -1.48271]])

print("Power method:")
v, eigen = find_power_method(matrix)
print("Eigen vector:", v)
print("Eigen value: ", eigen)

print("Scalar multiplication method: ")
v, eigen = find_scalar_method(matrix)
print("Eigen vector:", v)
print("Eigen value: ", eigen)



