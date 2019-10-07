using System;
using System.Collections.Generic;

namespace Problem2
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

        static double DF(double x) => 6 * Math.Exp(6 * x);

        static double DDF(double x) => 36 * Math.Exp(6 * x);

        /// <summary>
        /// Приблиительно высчитывает первую производную данной таблично заданной функции
        /// </summary>
        /// <param name="uniformTable">Таблица интерполирования с равноудаленными узлами</param>
        /// <returns>Таблично заданную первую производную</returns>
        static InterpolationNodes CalcFirstDerivatives(InterpolationNodes uniformTable)
        {
            double step = uniformTable[1].X - uniformTable[0].X;
            var result = new InterpolationNodes();
            
            result.Add(new InterpolationNode 
            {
                X = uniformTable[0].X, 
                Fx = uniformTable.Count > 2 ? 
                    (-3 * uniformTable[0].Fx + 4 * uniformTable[1].Fx - uniformTable[2].Fx) / (2 * step) :
                    (uniformTable[1].Fx - uniformTable[0].Fx) / step
            });
            for (int i = 1; i < uniformTable.Count - 1; ++i)
            {
                result.Add(new InterpolationNode
                {
                    X = uniformTable[i].X, Fx = (uniformTable[i + 1].Fx - uniformTable[i - 1].Fx) / (2 * step)
                });
            }
            result.Add(new InterpolationNode
            {
                X = uniformTable[^1].X,        
                Fx = uniformTable.Count > 2 ?
                    (3 * uniformTable[^1].Fx - 4 * uniformTable[^2].Fx + uniformTable[^3].Fx) / (2 * step) :
                    (uniformTable[^1].Fx - uniformTable[^2].Fx) / step
            });

            return result;
        }

        /// <summary>
        /// Приблиительно высчитывает вторую производную данной таблично заданной функции
        /// </summary>
        /// <param name="uniformTable">Таблица интерполирования с равноудаленными узлами</param>
        /// <returns>Таблично заданную вторую производную</returns>
        static InterpolationNodes CalcSecondDerivatives(InterpolationNodes uniformTable)
        {
            var step = uniformTable[1].X - uniformTable[0].X;
            var result = new InterpolationNodes();
            
            result.Add(new InterpolationNode { X = uniformTable[0].X, Fx = 0 });
            for (int i = 1; i < uniformTable.Count - 1; ++i)
            {
                result.Add(new InterpolationNode
                {
                    X = uniformTable[i].X,
                    Fx = (uniformTable[i + 1].Fx - 2 * uniformTable[i].Fx + uniformTable[i - 1].Fx) / (step * step)
                });
            }
            result.Add(new InterpolationNode { X = uniformTable[^1].X, Fx = 0 });

            return result;
        }

        /// <summary>
        /// Генерирует таблицу интерполяции заданного размера 
        /// </summary>
        /// <param name="function">Функция, по которой генерируем таблицу</param>
        /// <param name="tableSize">Размер таблицы минус 1</param>
        /// <param name="leftBorder">Левая граница интерполирования</param>
        /// <param name="step">Шаг равноотстоящих узлов интерполирования (x_i = leftBorder + i * step)</param>
        /// <returns></returns>
        static InterpolationNodes GenerateInterpolationTable(
            Func<double, double> function, 
            int tableSize,
            double leftBorder,
            double step)
        {
            var resultNodes = new InterpolationNodes();

            for (int i = 0; i <= tableSize; ++i)
            {
                var position = leftBorder + i * step;
                resultNodes.Add(new InterpolationNode {X = position, Fx = function(position)});
            }

            return resultNodes;
        }


        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("Введите m (число значений в таблице - 1): ");
                var tableSize = int.Parse(Console.ReadLine());
                Console.Write("Введите a > 0 (левая граница промежутка интерполирования): ");
                var leftBorder = double.Parse(Console.ReadLine());
                Console.Write("Введите h > 0 (шаг равноотстоящих узлов интерполирования): ");
                var step = double.Parse(Console.ReadLine());

                var interpolationTable = GenerateInterpolationTable(F, tableSize, leftBorder, step);

                var firstDerivatives = CalcFirstDerivatives(interpolationTable);
                var secondDerivatives = CalcSecondDerivatives(interpolationTable);
            
                for (int i = 0; i <= tableSize; ++i)
                {
                    Console.Write($"X_{i} = {interpolationTable[i].X}, " +
                                  $"F(x) = {interpolationTable[i].Fx}, " +
                                  $"DF = {firstDerivatives[i].Fx}, " +
                                  $"Погрешность DF = {Math.Abs(firstDerivatives[i].Fx - DF(firstDerivatives[i].X))}, ");
                
                    if (i != 0 && i != tableSize)
                    {
                        Console.WriteLine($"DDF = {secondDerivatives[i].Fx}, " +
                                          $"Погрешность DDF = {Math.Abs(secondDerivatives[i].Fx - DDF(secondDerivatives[i].X))}");    
                    }
                    else
                    {
                        Console.WriteLine("DDF не вычисляется");
                    }
                }
                
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