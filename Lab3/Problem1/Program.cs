using System;
using System.Collections.Generic;
using System.Linq;

namespace Problem1
{
    using InterpolationNodes = List<InterpolationNode>;
        
    /// <summary>
    /// Запись в таблице интерполирования
    /// </summary>
    class InterpolationNode
    {
        public double X { get; set; }
        public double Fx { get; set; }
    }    
    
    class Program
    {
        static double F(double x) => Math.Exp(6 * x); 
        
        /// <summary>
        /// Генерирует интерполяционный многочлен Лагранжа по узлам интерполяции 
        /// </summary>
        /// <param name="interpolationNodes">Список узлов интерполяции</param>
        /// <param name="interpolationPoint">Точка интерполяции</param>
        /// <param name="polynomDegree">Степень искомого многочлена</param>
        /// <returns>Значение многочлена Лагранжа в точке интерполяции</returns>
        static double CalcLagrangePolynom(
            InterpolationNodes interpolationNodes,
            int polynomDegree,
            double interpolationPoint)
        {
            SortInterpolationTable(interpolationNodes, interpolationPoint);
            
            double result = 0;
            for (int i = 0; i <= polynomDegree; ++i)
            {
                double currentSum = 1;
                for (int j = 0; j <= polynomDegree; ++j)
                {
                    if (j != i)
                    {
                        currentSum *= (interpolationPoint - interpolationNodes[j].X) /
                                      (interpolationNodes[i].X - interpolationNodes[j].X);
                    }
                }

                result += currentSum * interpolationNodes[i].Fx;
            }

            return result;
        }

        /// <summary>
        /// Генерирует таблицу интерполяции заданного размера 
        /// </summary>
        /// <param name="function">Функция, по которой генерируем таблицу</param>
        /// <param name="tableSize">Размер таблицы минус 1</param>
        /// <param name="leftBorder">Левая граница интерполирования</param>
        /// <param name="rightBorder">Правая граница интерполирования</param>
        /// <returns></returns>
        static InterpolationNodes GenerateInterpolationTable(
            Func<double, double> function, 
            int tableSize,
            double leftBorder,
            double rightBorder)
        {
            var step = (rightBorder - leftBorder) / tableSize;
            var resultNodes = new InterpolationNodes();

            for (double position = leftBorder; position < rightBorder; position += step)
            {
                resultNodes.Add(new InterpolationNode {X = position, Fx = function(position)});
            }

            return resultNodes;
        }

        /// <summary>
        /// Выполняет уточнение корней уравнения F(x)=0 методом половинного деления
        /// </summary>
        /// <param name="function">Функция, по значениям которой проводится бинарный поиск</param>
        /// <param name="segment">Отрезок, на котором выполняется поиск</param>
        /// <param name="maxError">Допустимая погрешность вычислений</param>
        private static double BinarySearch(Func<double, double> function, Tuple<double, double> segment, double maxError)
        {
            var currentLeft = segment.Item1;
            var currentRight = segment.Item2;

            while (currentRight - currentLeft > 2 * maxError)
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

            return (currentRight + currentLeft) / 2;
        }

        /// <summary>
        /// Отыскивает нули функции на заданном промежутке
        /// </summary>
        /// <param name="function">Функция, у которой отыскиваются нули</param>
        /// <param name="leftBorder">Левая граница поиска</param>
        /// <param name="rightBorder">Правая граница поиска</param>
        /// <param name="maxError">Предельная погрешность</param>
        /// <returns>Список нулей функции на указанном промежутке</returns>
        static List<double> FindRoots(
            Func<double, double> function, 
            double leftBorder,
            double rightBorder,
            double maxError)
        {
            const double step = 1e-3;
            
            var currentLeft = leftBorder;
            var roots = new List<double>();
            while (currentLeft + step <= rightBorder)
            {
                var currentRight = Math.Min(currentLeft + step, rightBorder);
                
                if (function(currentLeft) * function(currentRight) <= 0)
                {
                    var newRoot = BinarySearch(function, new Tuple<double, double>(currentLeft, currentRight), maxError);
                    roots.Add(newRoot);
                }

                currentLeft += step;
            }

            return roots;
        }

        /// <summary>
        /// Сортирует таблицу интерполирования по удаленности узлов от точки интерполирования
        /// </summary>
        /// <param name="interpolationTable">Таблица узлов интерполирования</param>
        /// <param name="interpolationPoint">Точк интерполирования</param>
        static void SortInterpolationTable(InterpolationNodes interpolationTable, double interpolationPoint)
        {
            interpolationTable.Sort((node1, node2) => 
                (int) (Math.Abs(interpolationPoint - node1.X) - Math.Abs(interpolationPoint - node2.X)));
        }

        static void Main(string[] args)
        {
            Console.Write("Введите m (число значений в таблице - 1): ");
            var tableSize = int.Parse(Console.ReadLine());
            Console.Write("Введите a (левая граница промежутка интерполирования): ");
            var leftBorder = double.Parse(Console.ReadLine());
            Console.Write("Введите b (правая граница промежутка интерполирования): ");
            var rightBorder = double.Parse(Console.ReadLine());

            var interpolationTable = GenerateInterpolationTable(F,tableSize, leftBorder, rightBorder);
            var reverseInterpolationTable = new InterpolationNodes(interpolationTable.Select(
                entry => new InterpolationNode {X = entry.Fx, Fx = entry.X }));
            
            Console.WriteLine("Таблица интерполирования (x, f(x))");
            foreach (var entry in interpolationTable)
            {
                Console.WriteLine($"x = {entry.X}; f(x) = {entry.Fx}");
            }
            Console.WriteLine();
            
            while (true)
            {
                Console.Write("Введите F (f(x) = F): ");
                var reverseInterpolationPoint = double.Parse(Console.ReadLine());
                Console.Write("Введите n (степень интерполяционного многочлена): ");
                var polynomDegree = int.Parse(Console.ReadLine());
                
                Console.WriteLine("В предположении, что функция строго монтонна:");
                var interpolationResult =
                    CalcLagrangePolynom(reverseInterpolationTable, polynomDegree, reverseInterpolationPoint); 
                Console.WriteLine($"x = {interpolationResult}; " +
                                  $"Модуль невязки: {Math.Abs(F(interpolationResult) - reverseInterpolationPoint)}");
                Console.WriteLine();
                
                Console.WriteLine("В общем случае (2й метод): ");
                Console.Write("Введите ε (погрешность вычисления корней многочлена): ");
                var computationError = double.Parse(Console.ReadLine());
                Func<double, double> shiftedInterpolationPolynom = 
                    point => CalcLagrangePolynom(interpolationTable, polynomDegree, point) - reverseInterpolationPoint;
                var interpolationPolynomRoots =
                    FindRoots(shiftedInterpolationPolynom, leftBorder, rightBorder, computationError);
                foreach (var entry in interpolationPolynomRoots)
                {
                    Console.WriteLine($"x = {entry}; Модуль невязки: {Math.Abs(F(entry) - reverseInterpolationPoint)}");
                }
                Console.WriteLine();

                Console.Write("Введите 0, чтобы выйти из программы; иное чтобы повторить");
                var shouldExit = Console.ReadLine();
                if (shouldExit == "0")
                {
                    break;
                }
            }
            
            
        }
    }
}
