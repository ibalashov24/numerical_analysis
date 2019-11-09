using System;
using System.Collections.Generic;

namespace Lab5
{
    public static class Program
    {
        const double StandardStep = 1e-2;

        /// <summary>
        /// Выполняет отделение корней уравнения F(x)=0 на заданном отрезке
        /// </summary>
        /// <param name="function">Непрерывная функция, корни которой отделяем</param>
        /// <param name="leftBorder">Левая граница промежутка</param>
        /// <param name="rightBorder">Правая граница промежутка</param>
        /// <returns>Список отрезков существования и единственности корня нечетной кратности</returns>
        private static List<Tuple<double, double>> SeparateRoots(
                Func<double, double> function,
                double leftBorder, 
                double rightBorder)
        {
            var result = new List<Tuple<double, double>>();
            var currentLeft = leftBorder;

            while (currentLeft + StandardStep <= rightBorder)
            {
                var currentRight = Math.Min(currentLeft + StandardStep, rightBorder);

                double valueLeft = function(currentLeft);
                double valueRight = function(currentRight);
                if (valueLeft <= 0 && valueRight > 0 || valueLeft > 0 && valueRight <= 0)
                {
                    result.Add(new Tuple<double, double>(currentLeft, currentRight));
                }

                currentLeft += StandardStep;
            }

            return result;
        }

        /// <summary>
        /// Выполняет уточнение корней уравнения F(x)=0 методом половинного деления
        /// </summary>
        /// <param name="function">Непрерывная функция, 
        /// по значениям которой выполняется поиск</param>
        /// <param name="segment">Отрезок, на котором выполняется поиск</param>
        private static double BinarySearch(
                Func<double, double> function,
                Tuple<double, double> segment)
        {
            var currentLeft = segment.Item1;
            var currentRight = segment.Item2;

            while (currentRight - currentLeft > 2 * StandardStep)
            {
                var middlePoint = (currentRight + currentLeft) / 2;
                var valueInMiddle = function(middlePoint);

                if (function(currentRight) * valueInMiddle < 0)
                {
                    currentLeft = middlePoint;
                }
                else
                {
                    currentRight = middlePoint;
                }
            }

            return (currentLeft + currentRight) / 2;
        }

        /// <summary>
        /// Разыскивает все корни заданной непрерывной функции на заданном промежутке 
        /// </summary>
        /// <param name="function">Непрерывная функция, корни которой ищем</param>
        /// <param name="leftBorder">Левая граница поиска корней</param>
        /// <param name="rightBorder">Правая граница поиска корней</param> 
        private static void WriteAllRoots(
                Func<double, double> function, 
                double leftBorder, 
                double rightBorder) 
        {
            var segments = SeparateRoots(function, leftBorder, rightBorder);
            int i = 0;
            foreach (var segment in segments)
            {   
                var root = BinarySearch(function, segment);
                Console.WriteLine($"x_{i} = {root}");

                ++i;
            }
        }

        public static double LegendrePolynom(double x, int n)
        {
            if (n == 0) 
            {
                return 1;
            }
           
            double last = 1.0;
            double current = x;
            for (int i = 2; i <= n; ++i)
            {
                var newLast = current;
                current = (double) (2 * n - 1) / n * x * current - (double) (n - 1) / n * last;
                last = newLast;
            }

            return current;
        }

        public static double ChebyshevPolynom(double x, int n)
        {
            if (n == 0)
            {
                return 1;
            }

            double last = 1.0;
            double current = x;
            for (int i = 2; i <= n; ++i)
            {
                var newLast = current;
                current = 2 * x * current - last;
                last = newLast;
            }

            return current;
        }

        private static double CalcDerivative(
                Func<double, double> function,
                double x,
                int power)
        {
            /*var functionValues = new double[3] { 
                    function(x), 
                    function(x + STANDARD_STEP),
                    function(x + 2 * STANDARD_STEP)};

            /for (int i = 1; i <= power; ++i)
            {
                var newFunctionValues = new double[3];
                newFunctionValues[0] = 
                    (-3 * functionValues[0] + 4 * functionValues[1] - functionValues[2]) / 
                        (2 * STANDARD_STEP);
                newFunctionValues[1] =
                    (functionValues[2] - functionValuxes[0]) / (2 * STANDARD_STEP);
                newFunctionValues[2] = 
                    (3 * functionValues[2] - 4 * functionValues[1] + functionValues[0]) /
                        (2 * STANDARD_STEP);

                functionValues = newFunctionValues;
            }

            return functionValues[0];*/

            // k should be less or equal than n
            Func<int, int, int> calcCombinations = (int n, int k) =>
            {
                int result = 1;
                for (int i = k + 1; i <= n; ++i)
                {
                    result *= i;
                }
                for (int i = 2; i <= n - k; ++i)
                {
                    result /= i;
                }
                return result;
            };

            double result = 0;
            for (int i = 0; i <= power; ++i)
            {
                result += ((i + power) % 2 == 0 ? 1.0 : -1.0) * function(x + StandardStep * i) *
                          calcCombinations(power, i);
            }
            for (int i = 0; i < power; ++i)
            {
                result /= StandardStep;
            }

            return result;
        }

        private static int CalcFactorial(int n)
        {
            var result = 1;
            for (int i = 1; i <= n; ++i)
            {
                result *= i;
            }

            return result;
        }

        public static double ChebyshevHermitePolynom(double x, int n)
        {
            Func<double, double> funcUnderDerivative = 
                point => Math.Exp(-point * point);

            // Calculating derivative for Rodrique formula
            double multiplicator = CalcFactorial(n);
            double derivative = multiplicator;
            double powerX = 1;
            double divider = 1;
            for (int i = 1; i < n; ++i)
            {
                derivative += (i % 2 == 0 ? 1 : -1) * powerX * multiplicator / divider;

                multiplicator *= i + n;
                divider *= i;
                powerX *= x;
            }

            return (n % 2 == 0 ? 1.0 : -1.0) * Math.Exp(x * x) * derivative;
        }

        public static double ChebyshevLegarrPolynom(double x, int n)
        {
            const double alpha = 1.0 / 2;
            Func<double, double> funcUnderDerivative = 
                point => Math.Pow(point, alpha + n) * Math.Exp(-point);
            
            // Calculating derivative for ... formula
            double multiplicator = 1;
            double derivative = 
                n % 2 == 0 ? 1 : -1 * x / CalcFactorial(n / 2 + 1);
            double powerX = 1;
            double divider = 1;
            for (int i = (int) Math.Ceiling(n / 2.0); i < n; ++i)
            {
                multiplicator *= (2 * i - 1) * (2 * i);
                divider *= i * (2 * i - n - 1) * (2 * i - n);
                powerX *= x * x;
                
                derivative += (i % 2 == 0 ? 1 : -1) * powerX * multiplicator / divider;
            }

            return (n % 2 == 0 ? 1 : -1) * Math.Pow(x, -alpha) * Math.Exp(x) * derivative;
        }

        public static void Main()
        {
            Console.WriteLine("Классические ортогональные многочлены");

            Console.Write("Введите степень многочлена: ");
            int n = int.Parse(Console.ReadLine());

            Console.WriteLine("Корни многочлена Лежандра: ");
            WriteAllRoots((double x) => LegendrePolynom(x, n), -1, 1);

            Console.WriteLine("Корни иногочлена Чебышева 1 рода:");
            WriteAllRoots((double x) => ChebyshevPolynom(x, n), -1, 1);

            Console.WriteLine("Корни многочлена Чебышёва-Эрмита:");
            WriteAllRoots((double x) => ChebyshevHermitePolynom(x, n), -100, 100);

            Console.WriteLine("Корни многочлена Чебышева-Легерра:");
            WriteAllRoots((double x) => ChebyshevLegarrPolynom(x, n), 0, 10);
        }
    }

}
