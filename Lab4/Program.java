public class Program
{
    public static final double A = .0;
    public static final double B = 1.0;
    public static final int M = 10;

    private static final double step = (B - A) / M;

    public static double f(double x)
    {
//        return x;
//        return x * x;
        return Math.exp(6 * x);
    }

    public static double w(double x)
    {
        return 1;
    }

    public static double exactIntegral()
    {
        return 1.0 / 6 * (Math.exp(6 * B) - Math.exp(6 * A));
//        return B - A;
//        return 1.0 / 3 * (B * B * B - A * A * A);
    }

    public static double rightRectangleMethod()
    {
        double result = .0;
        for (int i = 1; i <= M; ++i)
        {
            double point = A + i * step;
            result += w(point) * f(point);
        }
        result *= step;

        return result;
    }

    public static double leftRectangleMethod()
    {
        double result = .0;
        for (int i = 0; i < M; ++i)
        {
            double point = A + i * step;
            result += w(point) * f(point);
        }
        result *= step;

        return result;
    }

    public static double middleRectangleMethod()
    {
        double result = .0;
        for (int i = 0; i < M; ++i)
        {
            double point = A + (step * (2 * i + 1)) / 2;
            result += w(point) * f(point);
        }
        result *= step;

        return result;
    }

    public static double trapezeMethod()
    {
        double result = .0;
        for (int i = 0; i < M; ++i)
        {
            double point1 = A + i * step;
            double point2 = A + (i + 1) * step;

            result += w(point1) * f(point1) + w(point2) * f(point2);
        }
        result *= step / 2;

        return result;
    }

    public static double simpsonMethod()
    {
        double result = .0;
        for (int i = 0; i < M; ++i)
        {
            final double current = A + i * step;
            result +=
                    w(current) * f(current) +
                            4 * w(current + step / 2) * f(current + step / 2) +
                            w(current + step) * f(current + step);
        }
        result *= step / 6;

        return result;
    }

    public static void main(String[] args)
    {
        System.out.println(
                "Приближенное вычисление интеграла по " +
                        "составным квадратурным формулам");
        System.out.println(String.format("A = %e; B = %e; M = %d", A, B, M));

        final double J = exactIntegral();
        System.out.println(
                String.format("Точное значение интеграла (верно, если w == 1): %e", J));
        System.out.println();

        final double leftRectangle = leftRectangleMethod();
        System.out.println(
                String.format("Формула левых прямоугольников: %e", leftRectangle));
        System.out.println(
                String.format("Модуль погрешности: %e", Math.abs(J - leftRectangle)));
        System.out.println(
                String.format(
                        "Теоретическая оценка погрешности e ^ (6x): %e",
                        1.0 / 2 * Math.pow(B - A, 2) * (6 * Math.exp(6))));
        System.out.println();


        final double rightRectangle = rightRectangleMethod();
        System.out.println(
                String.format("Формула правых прямоугольников: %e", rightRectangle));
        System.out.println(
                String.format("Модуль погрешности: %e", Math.abs(J - rightRectangle)));
        System.out.println(
                String.format(
                        "Теоретическая оценка погрешности e ^ (6x): %e",
                        1.0 / 2 * Math.pow(B - A, 2) * (6 * Math.exp(6))));
        System.out.println();

        final double middleRectangle = middleRectangleMethod();
        System.out.println(
                String.format("Формула средних прямоугольников: %e", middleRectangle));
        System.out.println(
                String.format("Модуль погрешности: %e", Math.abs(J - middleRectangle)));
        System.out.println(
                String.format(
                        "Теоретическая оценка погрешности e ^ (6x): %e",
                        1.0 / 24 * Math.pow(B - A, 3) * (36 * Math.exp(6))));
        System.out.println();

        final double trapeze = trapezeMethod();
        System.out.println(
                String.format("Формула трапеций: %e", trapeze));
        System.out.println(
                String.format("Модуль погрешности: %e", Math.abs(J - trapeze)));
        System.out.println(
                String.format(
                        "Теоретическая оценка погрешности e ^ (6x): %e",
                        1.0 / 12 * Math.pow(B - A, 3) * (36 * Math.exp(6))));
        System.out.println();

        final double simpson = simpsonMethod();
        System.out.println(
                String.format("Формула Симпсона: %e", simpson));
        System.out.println(
                String.format("Модуль погрешности: %e", Math.abs(J - simpson)));
        System.out.println(
                String.format(
                        "Теоретическая оценка погрешности e ^ (6x): %e",
                        1.0 / 2880 * Math.pow(B - A, 5) * (1296 * Math.exp(6))));
        System.out.println();
    }
}
