import scipy.integrate as integrate
import numpy as np
from scipy.misc import derivative
from scipy.optimize import fsolve
from math import sin, sqrt, cos, pi, factorial, pow

def f(x):
    return sin(x)

def w(x):
    return 1.0 / sqrt(x)

def omega(x, roots, N, j):
    result = 1.0
    for k in range(0, N):
        if (k != j):
            result *= (x - roots[k])
            result /= (roots[j] - roots[k])

    return result

def likeGauss(func, weight, N, M, A, B, isDebugInfo=False):
    if isDebugInfo: print("Like-Gauss integral calculation:")

    integral = .0
    for i in range(0, M):
        a = A + (B - A) / M * i
        b = a + (B - A) / M 

        if isDebugInfo: print("Range from {0} to {1}".format(a, b))

        # Calculating moments
        if isDebugInfo: print("Moments of the weight function:")
        moment = []
        for j in range(0, 2*N):
            moment.append(integrate.quad(lambda x: weight(x) * pow(x, j), a, b)[0])
            if isDebugInfo: print("mu_{0} = {1}".format(j, moment[-1]))

        # Solving system of linear equations
        linalgLeft = np.array([])
        for j in range(N - 1, 2*N - 1):
            for k in range(0, N):
                linalgLeft = np.append(linalgLeft, moment[j - k])
        linalgLeft = linalgLeft.reshape(N, N)

        linalgRight = np.array([])
        for j in range(N, 2*N):
            linalgRight = np.append(linalgRight, [-moment[j]])

        # Orthogonal polynom
        polynom = np.linalg.solve(linalgLeft, linalgRight)
        polynom = np.insert(polynom, 0, [1.0])

        if isDebugInfo: print("Orthogonal polynom:")
        for i in range(0, N):
            if isDebugInfo: print("{0}x^{1}".format(polynom[i], N - i), end='')
            if isDebugInfo and polynom[i+1] >= 0:
                print("+", end='')
        if isDebugInfo: print(polynom[N])

        # Finding roots of the polynom
        roots = np.roots(polynom)
        if isDebugInfo:
            print("Nodes of quadrature polynom:")
            print(roots)

        # Finding quadrature polynom coefficients
        bigA = []
        for j in range(0, N):
            bigA.append(integrate.quad(
                lambda x: weight(x) * omega(x, roots, N, j), a,  b)[0])

        if isDebugInfo:
            print("Quadrature polynom coefficients:")
            print(bigA)

        # Calculating integral
        result = .0
        for j in range(0, N):
            result += bigA[j] * func(roots[j])

        integral += result

        if isDebugInfo: print()

    return integral

def legendre(x, N):
    """
    Legendre polynom
    """
    return 1.0 / (2**N * factorial(N)) * derivative(lambda y: pow(pow(y, 2) - 1, N), x, dx=1e-6, n=N)

def findLegendreRoots(N):
    roots = []
    for i in range(1, N+1):
        root = cos(pi*(4*i - 1) / (4*N + 2))
        for j in range(1, 10):
            root = root - legendre(root, N) / derivative(lambda x: legendre(x, N), root) 
        roots.append(root)

    return roots

def gauss(func, N, M, A, B):
    legendreRoots = findLegendreRoots(N)

    coeff = []
    for i in range(0, N):
        coeff.append(2*(1 - legendreRoots[i]**2) / 
                (N**2 * (legendre(legendreRoots[i], N - 1))**2))

    integral = .0
    for k in range(0, M):
        a = A + (B - A) / M * k
        b = a + (B - A) / M 

        result = .0
        for i in range(0, N):
            result += coeff[i] * func((b - a) / 2 * legendreRoots[i] + (b + a) / 2)
        result *= (b - a) / 2

        integral += result

    return integral

def meler(func, N):
    result = .0

    for i in range(1, N+1):
        result += func(cos((2*i - 1) / (2*N) * pi)) 

    result *= pi/N

    return result

print("Number of nodes:")
N = int(input())
print("Number of division intervals:")
M = int(input())
print("Left border:")
A = float(input())
print("Right border: ")
B = float(input())

print()
print("3) Интеграл методом Гаусса: ", gauss(lambda x: f(x) * w(x), N, M, A, B)) 
print("4) Интеграл методом типа Гаусса: ", likeGauss(f, w, N, M, A, B))
print("5) Интеграл по формуле Мелера: ", meler(f, N))

