import math
import plotly
import plotly.graph_objs as go


def p(x): return - (x - 2.0) / (x + 2.0) 


def q(x): return x


def r(x): return 1.0 - math.sin(x)


def f(x): return x ** 2


a1, a2, a3, b1, b2, b3 = 1, 0, 0, 1, 0, 0
a, b = -1, 1


def coeff(n):
    A, B, C, G = [], [], [], []
    h = (b - a) / n
    X = [a + i * h for i in range(n + 1)]
    A.append(0)
    B.append(-a1 - a2 / h)
    C.append(-a2 / h)
    G.append(a3)
    for i in range(1, n):
        A.append(-p(X[i]) / (h ** 2) - q(X[i]) / (2 * h))
        B.append(-2 * p(X[i]) / (h ** 2) - r(X[i]))
        C.append(-p(X[i]) / (h ** 2) + q(X[i]) / (2 * h))
        G.append(f(X[i]))
    A.append(-b2 / h)
    B.append(-b1 - b2 / h)
    C.append(0)
    G.append(b3)
    return A, B, C, G, X


def coeff2(n):
    A, B, C, G = [], [], [], []
    h = (b - a) / n
    X = [a - h / 2 + i * h for i in range(n + 2)]
    A.append(0)
    B.append(-a1 / 2 - a2 / h)
    C.append(a1 / 2 - a2 / h)
    G.append(a3)
    for i in range(1, n + 1):
        A.append(-p(X[i]) / (h ** 2) - q(X[i]) / (2 * h))
        B.append(-2 * p(X[i]) / (h ** 2) - r(X[i]))
        C.append(-p(X[i]) / (h ** 2) + q(X[i]) / (2 * h))
        G.append(f(X[i]))
    A.append(b1 / 2 - b2 / h)
    B.append(-b1 / 2 - b2 / h)
    C.append(0)
    G.append(b3)
    return A, B, C, G, X


def sweep(n, A, B, C, G):
    S, T = [], []
    S.append(C[0] / B[0])
    T.append(-G[0] / B[0])
    for i in range(1, n + 1):
        S.append(C[i] / (B[i] - A[i] * S[i - 1]))
        T.append((A[i] * T[i - 1] - G[i]) / (B[i] - A[i] * S[i - 1]))
    Y = [0 for i in range(n + 1)]
    Y[-1] = T[-1]
    for i in range(n - 1, -1, -1):
        Y[i] = S[i] * Y[i + 1] + T[i]
    return S, T, Y


def runge(n, o, Y1):
    A2, B2, C2, G2, X2 = coeff2(2 * n)
    S2, T2, Y0 = sweep(2 * n + 1, A2, B2, C2, G2)
    Y2 = [(Y0[i] + Y0[i + 1]) / 2 for i in range(2 * n + 1)]
    Y = []
    for i in range(n + 1):
        Y.append(Y2[i * 2] + (Y2[i * 2] - Y1[i]) / (2 ** o - 1))
    return Y


Y_ex1 = [0., 0.0299001, 0.0481537, 0.0593588, 0.0679361, \
         0.0774568, 0.0898408, 0.104016, 0.113049, 0.0968661, \
         0.]

Y_ex2 = [0., 0.0166811, 0.0299001, 0.0401992, 0.0481537, \
         0.054349, 0.0593588, 0.0637247, 0.0679361, 0.0724082, \
         0.0774568, 0.0832654, 0.0898408, 0.0969484, 0.104016, \
         0.109981, 0.113049, 0.110281, 0.0968661, 0.0647636, \
         0.]


def rnd(arr, n):
    return list(map(lambda x: round(x, n), arr))


def table1(n):
    A, B, C, G, X = coeff2(n)
    S, T, Y = sweep(n + 1, A, B, C, G)
    trace = go.Table(
        header=dict(values=['i', 'xi', 'Ai', 'Bi', 'Ci', 'Gi', 'Si', 'Ti', 'Yi']),
        cells=dict(values=[[i for i in range(n + 2)], rnd(X, 2), rnd(A, 5), rnd(B, 5), rnd(C, 5), rnd(G, 5), rnd(S, 5),
                           rnd(T, 5), rnd(Y, 5)]))
    data = [trace]
    plotly.offline.plot(data)


def table2(n, Y_ex):
    A1, B1, C1, G1, X1 = coeff(n)
    A2, B2, C2, G2, X2 = coeff2(n)
    S1, T1, Y1 = sweep(n, A1, B1, C1, G1)
    S2, T2, Y0 = sweep(n + 1, A2, B2, C2, G2)
    Y3 = [(Y0[i] + Y0[i + 1]) / 2 for i in range(n + 1)]
    Y1 = runge(n, 1, Y1)
    Y2 = runge(n, 2, Y3)
    trace = go.Table(
        header=dict(values=['x', 'Y_ex', 'Y_ut(h)', 'Y_ut - Y_ex', 'Y_ut(h^2)', 'Y_ut - Y_ex']),
        cells=dict(
            values=[rnd(X1, 2), Y_ex, rnd(Y1, 6), [round(abs(Y1[i] - Y_ex[i]), 6) for i in range(n + 1)], rnd(Y2, 5),
                    [round(abs(Y2[i] - Y_ex[i]), 10) for i in range(n + 1)]]))
    data = [trace]
    plotly.offline.plot(data)


if __name__ == '__main__':
    table1(10)
    table1(20)
    table2(10, Y_ex1)
    table2(20, Y_ex2)
