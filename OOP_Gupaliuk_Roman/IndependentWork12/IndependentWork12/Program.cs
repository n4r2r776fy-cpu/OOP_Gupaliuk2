using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace IndependentWork12
{
    internal class Program
    {
        // Обчислювально інтенсивна операція: перевірка, чи число просте
        static bool IsPrime(int number)
        {
            if (number < 2) return false;
            for (int i = 2; i * i <= number; i++)
                if (number % i == 0) return false;
            return true;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("=== IndependentWork12: PLINQ Performance ===");

            int[] sizes = { 1_000_000, 5_000_000, 10_000_000 };
            Random rnd = new Random();

            foreach (int size in sizes)
            {
                Console.WriteLine($"\n--- Колекція: {size} елементів ---");

                // Генерація даних
                List<int> data = new List<int>(size);
                for (int i = 0; i < size; i++)
                    data.Add(rnd.Next(1, 10_000));

                // === LINQ ===
                Stopwatch sw = Stopwatch.StartNew();
                var linqResult = data
                    .Where(x => IsPrime(x))
                    .Select(x => x * 2)
                    .ToList();
                sw.Stop();
                long linqTime = sw.ElapsedMilliseconds;

                Console.WriteLine($"LINQ: {linqTime} ms");

                // === PLINQ ===
                sw.Restart();
                var plinqResult = data
                    .AsParallel()
                    .Where(x => IsPrime(x))
                    .Select(x => x * 2)
                    .ToList();
                sw.Stop();
                long plinqTime = sw.ElapsedMilliseconds;

                Console.WriteLine($"PLINQ: {plinqTime} ms");
            }

            Console.WriteLine("\n=== Тест побічних ефектів ===");

            int unsafeCounter = 0;

            // ПОГАНО — неконтрольовані побічні ефекти
            Enumerable.Range(0, 1_000_000)
                .AsParallel()
                .ForAll(i =>
                {
                    unsafeCounter++; // гонка потоків
                });

            Console.WriteLine($"Некоректний результат (без lock): {unsafeCounter}");

            // ПРАВИЛЬНО
            int safeCounter = 0;
            object locker = new object();

            Enumerable.Range(0, 1_000_000)
                .AsParallel()
                .ForAll(i =>
                {
                    lock (locker)
                    {
                        safeCounter++;
                    }
                });

            Console.WriteLine($"Коректний результат (з lock): {safeCounter}");

            Console.WriteLine("\nГотово!");
            Console.WriteLine(" Натисныть будь яку кнопку для завершення програми: ");
            Console.ReadLine();
        }
    }
}
