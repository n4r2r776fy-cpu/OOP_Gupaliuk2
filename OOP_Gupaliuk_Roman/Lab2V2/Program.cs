using System;

namespace lab2v2
{
    // Клас Matrix
    class Matrix
    {
        //  Приватне поле
        private double[,] data;

        //  Властивості для кількості рядків і стовпців
        public int Rows { get; private set; }
        public int Cols { get; private set; }

        //  Конструктор
        public Matrix(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
            data = new double[rows, cols];
        }

        //  Індексатор
        public double this[int i, int j]
        {
            get
            {
                if (i < 0 || i >= Rows || j < 0 || j >= Cols)
                    throw new IndexOutOfRangeException("Невірний індекс матриці!");
                return data[i, j];
            }
            set
            {
                if (i < 0 || i >= Rows || j < 0 || j >= Cols)
                    throw new IndexOutOfRangeException("Невірний індекс матриці!");
                data[i, j] = value;
            }
        }

        //  Перевантаження оператора множення (*)
        public static Matrix operator *(Matrix a, Matrix b)
        {
            if (a.Cols != b.Rows)
                throw new InvalidOperationException("Неможливо перемножити матриці: розміри не збігаються!");

            Matrix result = new Matrix(a.Rows, b.Cols);

            for (int i = 0; i < a.Rows; i++)
            {
                for (int j = 0; j < b.Cols; j++)
                {
                    double sum = 0;
                    for (int k = 0; k < a.Cols; k++)
                    {
                        sum += a[i, k] * b[k, j];
                    }
                    result[i, j] = sum;
                }
            }

            return result;
        }

        //  Метод для виведення матриці
        public void Print()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    Console.Write($"{data[i, j],6:F1} ");
                }
                Console.WriteLine();
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Лабораторна робота №2 ===");
            Console.WriteLine("Тема: Інкапсуляція, індексатори, перевантаження операторів");
            Console.WriteLine("Варіант 2 — Клас Matrix\n");

            //  Створюємо дві матриці 2x2
            Matrix A = new Matrix(2, 2);
            Matrix B = new Matrix(2, 2);

            // Заповнюємо через індексатор
            A[0, 0] = 1; A[0, 1] = 2;
            A[1, 0] = 3; A[1, 1] = 4;

            B[0, 0] = 5; B[0, 1] = 6;
            B[1, 0] = 7; B[1, 1] = 8;

            Console.WriteLine("Матриця A:");
            A.Print();
            Console.WriteLine("\nМатриця B:");
            B.Print();

            // Множення матриць
            Matrix C = A * B;

            Console.WriteLine("\nРезультат множення (A * B):");
            C.Print();

            Console.WriteLine("\n=== Кінець програми ===");
        }
    }
}
