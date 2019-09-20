using System;
using System.Collections.Generic;

namespace Interpolation
{
    using InterpolationNodes = List<double>;

    class Program
    {
        private static double _interpolationRangeWidth = 1;

        static double F(double x) => Math.Exp(x) - x;

        /// <summary>
        /// Производит чтение количества значений в таблице узлов интерполяции,
        /// степень интерполяционного многочлена и точки интерполяции 
        /// </summary>
        /// <returns>Число узлов интерполяции, степень интерполяционного
        /// многочлена, точка интерполяции</returns>
        static (int tableEntriesCount, int polynomDegree, double interpolationPoint) ReadInput()
        {
            Console.Write("Введите m: ");
            var tableEntriesCount = int.Parse(Console.ReadLine() ?? throw new ArgumentNullException());

            int polynomDegree = -1;
            while (polynomDegree == -1 || tableEntriesCount < polynomDegree)
            {
                Console.Write("Введите n: ");
                polynomDegree = int.Parse(Console.ReadLine() ?? throw new ArgumentNullException());

                if (tableEntriesCount < polynomDegree)
                {
                    Console.WriteLine("Введено недопустимое значение n");
                }
            }

            Console.Write("Введите точку интерполирования: ");
            double interpolationPoint = Double.Parse(Console.ReadLine() ?? throw new ArgumentNullException());

            return (tableEntriesCount, polynomDegree, interpolationPoint);
        }

        /// <summary>
        /// Генерирует равноотстоящие узлы интерполяции вокруг точки интерполяции 
        /// </summary>
        /// <param name="tableEntriesCount">Количество узлов интерполяции для генерации</param>
        /// <param name="interpolationPoint">Точка интерполяции</param>
        /// <returns>Список узлов интерполяции</returns>
        static InterpolationNodes GenerateinterpolationNodes(int tableEntriesCount, double interpolationPoint)
        {
            double rangeStartPoint = interpolationPoint - _interpolationRangeWidth / 2;
            double rangeEndPoint = interpolationPoint + _interpolationRangeWidth / 2;
            double stepWidth = _interpolationRangeWidth / tableEntriesCount;

            var result = new InterpolationNodes();
            for (int i = 0; i < tableEntriesCount; ++i)
            {
                var currentPoint = rangeStartPoint + i * stepWidth;
                result.Add(currentPoint);
            }

            result.Add(rangeEndPoint);

            return result;
        }

        /// <summary>
        /// Сортирует узлы интерполяции по расстоянию до точки интерполяции
        /// </summary>
        /// <param name="table">Список узлов интерполяции для сортировки</param>
        /// <param name="interpolationPoint">Точка интерполяции</param>
        static void SortInterpolationNodes(InterpolationNodes table, double interpolationPoint)
        {
            table.Sort((first, second) => 
                Math.Abs(first - interpolationPoint) < Math.Abs(second - interpolationPoint) ? -1 : 1);
        }

        /// <summary>
        /// Генерирует интерполяционный многочлен Лагранжа по узлам интерполяции 
        /// </summary>
        /// <param name="interpolationNodes">Список узлов интерполяции</param>
        /// <param name="interpolationPoint">Точка интерполяции</param>
        /// <param name="polynomDegree">Степень искомого многочлена</param>
        /// <returns>Значение многочлена Лагранжа в точке интерполяции</returns>
        static double CalcLagrangePolynom(
            InterpolationNodes interpolationNodes,
            double interpolationPoint,
            int polynomDegree)
        {
            double result = 0;
            for (int i = 0; i <= polynomDegree; ++i)
            {
                double numerator = 1;
                double denominator = 1;
                for (int j = 0; j <= polynomDegree; ++j)
                {
                    if (j != i)
                    {
                        numerator *= interpolationPoint - interpolationNodes[j];
                        denominator *= interpolationNodes[i] - interpolationNodes[j];
                    }
                }

                result += numerator / denominator * F(interpolationNodes[i]);
            }

            return result;
        }

        /// <summary>
        /// Генерирует интерполяционный многочлен Ньютона по узлам интерполяции 
        /// </summary>
        /// <param name="interpolationNodes">Список узлов интерполяции</param>
        /// <param name="interpolationPoint">Точка интерполяции</param>
        /// <param name="polynomDegree">Степень искомого многочлена</param>
        /// <returns>Значение многочлена Ньютона в точке интерполяции</returns>
        static double CalcNewtonPolynom(
            InterpolationNodes interpolationNodes,
            double interpolationPoint,
            int polynomDegree)
        {
            var dividedDifference = new double[polynomDegree + 1, polynomDegree + 1];
            for (int i = 0; i <= polynomDegree; ++i)
            {
                dividedDifference[i, 0] = F(interpolationNodes[i]);
            }

            for (int j = 1; j <= polynomDegree; ++j)
            {
                for (int i = 0; i <= polynomDegree - j; ++i)
                {
                    dividedDifference[i, j] = (dividedDifference[i + 1, j - 1] - dividedDifference[i, j - 1]) /
                                              (interpolationNodes[i + j] - interpolationNodes[i]);
                }
            }

            double result = 0;
            double factor = 1;
            for (int j = 0; j <= polynomDegree; ++j)
            {
                result += dividedDifference[0, j] * factor;
                factor *= interpolationPoint - interpolationNodes[j];
            }

            return result;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Задача алгебраического интерполирования");
            Console.WriteLine("Вариант 3");

            while (true)
            {
                var (tableEntriesCount, polynomDegree, interpolationPoint) = ReadInput();
                Console.WriteLine();

                Console.WriteLine($"Число значений в таблице: {tableEntriesCount}");
                var interpolationNodes = GenerateinterpolationNodes(tableEntriesCount, interpolationPoint);
                Console.WriteLine("Исходная таблица значений функции:");
                foreach (var interpolationNode in interpolationNodes)
                {
                    Console.WriteLine($"x = {interpolationNode}; f(x) = {F(interpolationNode)}");
                }

                Console.WriteLine($"Точка интерполирования: x = {interpolationPoint}");
                Console.WriteLine($"Степень многочлена: n = {polynomDegree}");

                Console.WriteLine();
                SortInterpolationNodes(interpolationNodes, interpolationPoint);
                Console.WriteLine("Отсортированная таблица интерполирования:");
                foreach (var interpolationNode in interpolationNodes)
                {
                    Console.WriteLine($"x = {interpolationNode}; f(x) = {F(interpolationNode)}");
                }
                Console.WriteLine();

                var polynomLagrange = CalcLagrangePolynom(interpolationNodes, interpolationPoint, polynomDegree);
                Console.WriteLine($"Значение интерполяционного многочлена в форме Лагранжа: {polynomLagrange}");
                Console.WriteLine($"Абсолютная фактическая погрешность в форме Лагранжа: " +
                                  $"{Math.Abs(polynomLagrange - F(interpolationPoint))}");
                Console.WriteLine();

                var polynomNewton = CalcNewtonPolynom(interpolationNodes, interpolationPoint, polynomDegree);
                Console.WriteLine($"Значение интерполяционного многочлена в форме Ньютона: {polynomNewton}");
                Console.WriteLine($"Абсолютная фактическая погрешность в форме Ньютона: " +
                                  $"{Math.Abs(polynomNewton - F(interpolationPoint))}");
                Console.WriteLine();

                Console.WriteLine("Введите 0, чтобы выйти; что-либо другое для ввода новых x и n:");
                var isRepeatString = Console.ReadLine()?.Trim().TrimEnd();
                if (isRepeatString == "0")
                {
                    break;
                }
            }
        }
    }
}