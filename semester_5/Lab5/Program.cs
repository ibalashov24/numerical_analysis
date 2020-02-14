    using System;
using System.Collections.Generic;

namespace Lab5
{
    public static class Program
    {
        /// <summary>
        /// Шаг аргумента, с которым рассматриваем значения функции
        /// </summary>
        const double StandardStep = 1e-2;

        /// <summary>
        /// Вычисляет значение многочлена Лежандра
        /// </summary>
        /// <param name="x">Точка, в которой вычисляем</param>
        /// <param name="n">Степень многочлена</param>
        /// <returns>Значение многочлена в данной точке</returns>
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
        
        /// <summary>
        /// Вычисляет значение многочлена Чебышёва 1 рода
        /// </summary>
        /// <param name="x">Точка, в которой вычисляем</param>
        /// <param name="n">Степень многочлена</param>
        /// <returns>Значение многочлена в данной точке</returns>
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

        /// <summary>
        /// Вычисляет факториал
        /// </summary>
        /// <param name="n">Число, факториал которого вычисляем</param>
        /// <returns>Факториал</returns>
        private static int CalcFactorial(int n)
        {
            var result = 1;
            for (int i = 1; i <= n; ++i)
            {
                result *= i;
            }

            return result;
        }
        
        /// <summary>
        /// Вычисляет значение многочлена Чебышёва-Эрмита
        /// </summary>
        /// <param name="x">Точка, в которой вычисляем</param>
        /// <param name="n">Степень многочлена</param>
        /// <returns>Значение многочлена в данной точке</returns>
        public static double ChebyshevHermitePolynom(double x, int n)
        {
            double result = 0;
            for (int i = 0; i <= (int) (n / 2.0); ++i)
            {
                result += (i % 2 == 0 ? 1.0 : -1.0)
                          / CalcFactorial(i)
                          / CalcFactorial(n - 2 * i)
                          * Math.Pow(2 * x, n - 2 * i);
            }
            result *= CalcFactorial(n);

            return result;
        }
        
        /// <summary>
        /// Вычисляет значение многочлена Чебышёва-Лагерра
        /// </summary>
        /// <param name="x">Точка, в которой вычисляем</param>
        /// <param name="n">Степень многочлена</param>
        /// <returns>Значение многочлена в данной точке</returns>
        public static double ChebyshevLegarrPolynom(double x, int n)
        {
            const int alpha = 1;
            
            double result = 0;
            double powerX = 1;
            for (int i = 0; i <= n; ++i)
            {
                result += (i % 2 == 0 ? 1.0 : -1.0)
                          / CalcFactorial(n - i)
                          / CalcFactorial(alpha + i)
                          * powerX
                          / CalcFactorial(i);
                
                powerX *= x;
            }
            result *= CalcFactorial(n + alpha);

            return result;
        }

        /// <summary>
        /// Строит график заданной фукнции в указанных границах. График записывается в файл по заданному пути.
        /// </summary>
        /// <param name="function">Функция, график которой строим</param>
        /// <param name="leftBorder">Левая граница промежутка</param>
        /// <param name="rightBorder">Правая граница промежутка</param>
        /// <param name="outputFileName">Путь итогового графика</param>
        private static void PlotFunction(
            Func<double, double> function, double leftBorder, double rightBorder, String outputFileName)
        {
            const double step = 0.01;
            
            GnuPlot.Set("terminal png size 400,400", $"output '{outputFileName}'");

            var pointsCount = (int) ((rightBorder - leftBorder) / step) + 1;
            var x = new double[pointsCount];
            var y = new double[pointsCount];
            int i = 0;
            for (double point = leftBorder; point <= rightBorder; point += step)
            {
                x[i] = point;
                y[i] = function(point);

                ++i;
            }
            
            GnuPlot.Plot(x, y);
        }
        
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
        /// Печатает экстремумы для многочлена Чебышёва 1 рода на промежутке от -1 до 1
        /// </summary>
        /// <param name="polynomPower">Степень многочлена Чебышёва</param>
        private static void WriteChebyshevExtremas(int polynomPower)
        {
            Console.WriteLine($"Экстремумы многочлена Чебышёва 1 рода степени {polynomPower}: ");
            for (int i = 0; i <= polynomPower; ++i)
            {
                Console.WriteLine($"ext_{i} = {Math.Cos(Math.PI * i / polynomPower)}");
            }
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

        public static void Main()
        {
            Console.WriteLine("Классические ортогональные многочлены");

            Console.Write("Введите степень многочлена: ");
            int n = int.Parse(Console.ReadLine());

            Console.WriteLine("Корни многочлена Лежандра: ");
            WriteAllRoots((double x) => LegendrePolynom(x, n), -1, 1);
            PlotFunction(x => LegendrePolynom(x, n), -1, 1, "legendre.png");

            Console.WriteLine("Корни многочлена Чебышева 1 рода:");
            WriteAllRoots(x => ChebyshevPolynom(x, n), -1, 1);
            PlotFunction(function: x => ChebyshevPolynom(x, n), -1, 1, "chebyshev.png");
            PlotFunction(function: x => ChebyshevPolynom(x, n) / Math.Pow(2, n - 1),
                -1, 1, "chebyshev-reduced.png");
            WriteChebyshevExtremas(n);

            Console.WriteLine("Корни многочлена Чебышёва-Эрмита:");
            WriteAllRoots((double x) => ChebyshevHermitePolynom(x, n), -5, 5);
            PlotFunction(x => ChebyshevHermitePolynom(x, n), -2, 2, "hermite.png");

            Console.WriteLine("Корни многочлена Чебышева-Легерра:");
            WriteAllRoots((double x) => ChebyshevLegarrPolynom(x, n), 0, 20);
            PlotFunction(x => ChebyshevLegarrPolynom(x, n), 0, 20, "legerre.png");
            
            // TODO: убрать этот костыль
            GnuPlot.Replot();
        }
    }

}
