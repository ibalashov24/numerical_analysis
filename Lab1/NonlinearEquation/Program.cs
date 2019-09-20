using System;
using System.Collections.Generic;

namespace NonlinearEquation
{
    internal static class Program
    {
        private const double H = 1e-2;
        private const double A = -5;
        private const double B = 10;
        private const double Eps = 1e-6;

        private static double F(double x) => Math.Pow(2.0, -x) - Math.Sin(x);

        private static double FDerivative(double x) => -Math.Pow(2.0, -x) * Math.Log(2) - Math.Cos(x);
        
        /// <summary>
        /// Выполняет отделение корней уравнения F(x)=0 на отрезке [A, B]
        /// </summary>
        /// <returns>Список отрезков сущестоввания и единственности корня нечетной кратности</returns>
        private static List<Tuple<double, double>> SeparateRoots()
        {
            var result = new List<Tuple<double, double>>();
            var currentLeft = A;

            while (currentLeft + H <= B)
            {
                var currentRight = Math.Min(currentLeft + H, B);
                
                if (F(currentLeft) * F(currentRight) <= 0)
                {
                    result.Add(new Tuple<double, double>(currentLeft, currentRight));
                }

                currentLeft += H;
            }

            return result;
        }

        /// <summary>
        /// Выполняет уточнение корней уравнения F(x)=0 методом половинного деления
        /// </summary>
        /// <param name="segment">Отрезок, на котором выполняется поиск</param>
        private static void BinarySearch(Tuple<double, double> segment)
        {
            var currentLeft = segment.Item1;
            var currentRight = segment.Item2;
            Console.WriteLine($"Начальное приближение к корню: x_0 = {(currentRight + currentLeft) / 2}");
            var stepCounter = 0;

            while (currentRight - currentLeft > 2 * Eps)
            {
                var middlePoint = (currentRight + currentLeft) / 2;
                var valueInMiddle = F(middlePoint);

                if (F(currentRight) * valueInMiddle < 0)
                {
                    currentLeft = middlePoint;
                }
                else
                {
                    currentRight = middlePoint;
                }

                stepCounter++;
            }
            
            Console.WriteLine($"Количество шагов для достижения точности \u03B5: {stepCounter}");
            Console.WriteLine($"Приближенное решение: {(currentRight + currentLeft) / 2}");
            Console.WriteLine($"Длина последнего отрезка: {currentRight - currentLeft}");
            Console.WriteLine($"Модуль невязки для приближенного решения: {Math.Abs(F(currentRight))}");
            Console.WriteLine();
        }

        /// <summary>
        /// Выполняет уточнение корней уравнения F(x)=0 методом Ньютона
        /// </summary>
        /// <param name="segment">Отрезок, на котором выполняется поиск</param>
        private static void NewtonMethod(Tuple<double, double> segment)
        {
            const double zeroEpsilon = 1e-5;
            Func<double, int, double> calcNext = (previous, p) => previous - p * F(previous) / FDerivative(previous);
            
            var multiplicity = 1;
            var currentEstimation = segment.Item2;
            var nextEstimation = calcNext(currentEstimation, multiplicity);
            Console.WriteLine($"Начальное приближение к корню: {currentEstimation}");

            var stepCounter = 0;
            while (Math.Abs(nextEstimation - currentEstimation) > Eps)
            {
                currentEstimation = nextEstimation;

                var derivative = FDerivative(nextEstimation);
                if (Math.Abs(derivative) < zeroEpsilon)
                {
                    currentEstimation = segment.Item2;
                    multiplicity += 2;
                }

                nextEstimation = calcNext(currentEstimation, multiplicity);
                stepCounter++;
            }
            
            Console.WriteLine($"Количество шагов для достижения точности \u03B5: {stepCounter}");
            Console.WriteLine($"Приближенное решение x_N: {nextEstimation}");
            Console.WriteLine(
                $"Расстояние между последними приближениями: {Math.Abs(nextEstimation - currentEstimation)}");
            Console.WriteLine($"Модуль невязки для x_N: {Math.Abs(F(nextEstimation))}");
            Console.WriteLine();
        }
        
        /// <summary>
        /// Выполняет уточнение корней уравнения F(x)=0 модифицированным методом Ньютона
        /// </summary>
        /// <param name="segment">Отрезок, на котором выполняется поиск</param>
        private static void ModifiedNewtonMethod(Tuple<double, double> segment)
        {
            var calcNext = (Func<double, double>) ((previous) => previous - F(previous) / FDerivative(segment.Item2));

            var currentEstimation = segment.Item2;
            var nextEstimation = calcNext(currentEstimation);
            Console.WriteLine($"Начальное приближение к корню: {currentEstimation}");

            var stepCounter = 0;
            while (Math.Abs(nextEstimation - currentEstimation) > Eps)
            {
                currentEstimation = nextEstimation;
                nextEstimation = calcNext(nextEstimation);

                stepCounter++;
            }
            
            Console.WriteLine($"Количество шагов для достижения точности \u03B5: {stepCounter}");
            Console.WriteLine($"Приближенное решение x_N: {nextEstimation}");
            Console.WriteLine(
                $"Расстояние между последними приближениями: {Math.Abs(nextEstimation - currentEstimation)}");
            Console.WriteLine($"Модуль невязки для x_N: {Math.Abs(F(nextEstimation))}");
            Console.WriteLine();
        }
        
        /// <summary>
        /// Выполняет уточнение корней уравнения F(x)=0 методом секущих
        /// </summary>
        /// <param name="segment">Отрезок, на котором выполняется поиск</param>
        private static void SecantMethod(Tuple<double, double> segment)
        {
            double CalcNext(double last, double penult) => last - F(last) * (last - penult) / (F(last) - F(penult));
            
            var previousEstimation = segment.Item1;
            var currentEstimation = segment.Item2;
            Console.WriteLine($"Начальное приближение к корню: {previousEstimation}");

            var stepCounter = 0;
            while (Math.Abs(currentEstimation - previousEstimation) > Eps)
            {
                var nextEstimation = CalcNext(currentEstimation, previousEstimation);
                previousEstimation = currentEstimation;
                currentEstimation = nextEstimation;

                stepCounter++;
            }
            
            Console.WriteLine($"Количество шагов для достижения точности \u03B5: {stepCounter}");
            Console.WriteLine($"Приближенное решение x_N: {currentEstimation}");
            Console.WriteLine(
                $"Расстояние между последними приближениями: {Math.Abs(currentEstimation - previousEstimation)}");
            Console.WriteLine($"Модуль невязки для x_N: {Math.Abs(F(currentEstimation))}");
            Console.WriteLine();
        }

        /// <summary>
        /// Находит все корни нечетной кратности уравнения F(x)=0
        /// </summary>
        private static void Main()
        {
            Console.WriteLine("Численные методы решения нелинейных уравнений");
            Console.WriteLine();
            
            Console.WriteLine("Исходные данные:");
            Console.WriteLine("f(x) = 2^(-x) - Sin(x)");
            Console.WriteLine($"A = {A}\nB = {B}\n\u03B5 = {Eps}");
            Console.WriteLine();

            Console.WriteLine($"Отрезки в [{A}, {B}], содержащие корни уравнения:");
            var segments = SeparateRoots();
            foreach (var segment in segments)
            {
                Console.WriteLine($"[{segment.Item1}, {segment.Item2}]");
            }
            Console.WriteLine($"Всего отрезков, содержащих перемену знака: {segments.Count}");
            Console.WriteLine();
            
            Console.WriteLine("Вычисление корней:");
            foreach (var segment in segments)
            {
                Console.WriteLine($"Поиск корня на отрезке [{segment.Item1}, {segment.Item2}]:");
                
                Console.WriteLine("1) Метод половинного деления: ");
                BinarySearch(segment);
                
                Console.WriteLine("2) Метод Ньютона");
                NewtonMethod(segment);
                
                Console.WriteLine("3) Модифицированный метод Ньютона");
                ModifiedNewtonMethod(segment);
                
                Console.WriteLine("4) Метод секущих");
                SecantMethod(segment);
            }
        }
    }
}