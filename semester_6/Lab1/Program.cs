using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab1
{
    using Matrix = List<List<double>>;
    using Row = List<double>;
    using Column = List<double>;

    class Program
    {
        static Matrix ReadMatrix(int height, int width)
        {
            var matrix = new Matrix(height);
            
            for (int i = 0; i < height; ++i)
            {
                matrix[i] = new Column(Console
                    .ReadLine()
                    .Split(new char[] { ' ' }, width)
                    .Select(ch => double.Parse(ch)));
            }

            return matrix;
        }

        static Row SolveSystemSingleDivision(Matrix matrix)
        {
            Matrix triangularMatrix = new Matrix(matrix.Count);
            for (int i = 0; i < matrix.Count; ++i)
            {
                triangularMatrix[i] = new Column(matrix.Count + 1);
            }
            
            // Forward movement
            for (int k = 0; k < matrix.Count; ++k)
            {
                var temp = triangularMatrix[k][k];

                for (int j = k + 1; j < matrix.Count + 1; ++j)
                {
                     triangularMatrix[k][j] /= temp;
                } 

                for (int i = k + 1; i < matrix.Count; ++i)
                {
                    temp = triangularMatrix[i][k];

                    for (int j = k + 1; j < matrix.Count + 1; ++j)
                    {
                        triangularMatrix[i][j] -= triangularMatrix[k][j] * temp;
                    }   
                }
            }

            // Backward movement
            var solution = new Row(matrix.Count);
            for (int i = matrix.Count - 1; i >= 0; --i)
            {
                solution[i] = triangularMatrix[i][matrix.Count];
                for (int j = i + 1; j < matrix.Count; ++j)
                {
                    solution[i] -= triangularMatrix[i][j] * solution[j];
                }
            }

            return solution;
        }

        static Row SolveSystemLU(Matrix matrix)
        {

        }

        static Row SolveSystemMainElement(Matrix matrix)
        {

        }

        static Row CalcError(Matrix matrix, Row solution)
        {


        }

        static void PrintColumn(Column column)
        {
            foreach (var element in column)
            {
                Console.Write($"{element} ");
            }
        }

        static void PrintMatrix(Matrix matrix)
        {
            foreach (var row in matrix)
            {
                foreach (var element in row)
                {
                    Console.Write($"{element} ");
                }

                Console.WriteLine();
            }
        }

        static void SolveLinearEquation(int order)
        {
            // Extended system matrix
            var matrix = ReadMatrix(order, order + 1);

            var solutionDivision = SolveSystemSingleDivision(matrix);
            var solutionLU = SolveSystemLU(matrix);
            var solutionMainElement = SolveSystemMainElement(matrix);

            Console.WriteLine("Solution using Gauss single division method: ");
            PrintColumn(solutionDivision);
            Console.WriteLine("Error: ");
            PrintColumn(CalcError(matrix, solutionDivision));

            Console.WriteLine("Solution using LU decomposition: ");
            PrintColumn(solutionLU);
            Console.WriteLine("Error: ");
            PrintColumn(CalcError(matrix, solutionLU));

            Console.WriteLine("Solution using Gauss main element swap method: ");
            PrintColumn(solutionMainElement);
            Console.WriteLine("Error: ");
            PrintColumn(CalcError(matrix, solutionMainElement));
        }

        static void GetInverseMatrix(int order)
        {
            var matrix = ReadMatrix(order, order);
            for (int i = 0; i < order; ++i)
            {
                matrix[i].Add(0.0); 
            }

            Matrix inverse = new Matrix(order);
            // Calculating transponed inverse matrix
            for (int i = 0; i < order; ++i)
            {
                matrix[i][order] = 1.0;
                if (i != 0)
                {
                    matrix[i - 1][order] = 0.0;
                }
    
                // Getting transponed result
                inverse[i] = SolveSystemLU(matrix);                        
            }

            // Transponing inverse in order to get actual result
            for (int i = 0; i < order; ++i)
            {
                for (int j = i + 1; j < order; ++j)
                {
                    inverse[i][j] = inverse[j][i];
                }
            }

            Console.WriteLine("Inverse matrix: ");
            PrintMatrix(inverse);
        }

        static void Main(string[] args)
        {
            Console.Write("Enter system order: ");
            var order = int.Parse(Console.ReadLine());
            
            Console.WriteLine("Enter 0 to solve linear system");
            Console.WriteLine("Enter anything else to get reversed matrix");
            var mode = Console.ReadLine();

            if (mode == "0")
            {
                SolveLinearEquation(order);
            }
            else
            {
                GetInverseMatrix(order);    
            }
        }
    }
}
