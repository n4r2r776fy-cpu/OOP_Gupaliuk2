using System;
using System.IO;
using System.Net.Http;
using System.Threading;

namespace Lab7_V2
{
    // =============================
    // КЛАС, ЩО ІМІТУЄ РОБОТУ З ФАЙЛОМ
    // =============================
    class FileProcessor
    {
        private int writeFailCount = 0;

        public void WriteToFile(string path, string content)
        {
            writeFailCount++;

            if (writeFailCount <= 3)
                throw new IOException($"Помилка запису у файл (імітація), спроба {writeFailCount}");

            File.WriteAllText(path, content);
        }
    }

    // =============================
    // КЛАС, ЩО ІМІТУЄ МЕРЕЖЕВУ РОБОТУ
    // =============================
    class NetworkClient
    {
        private int uploadFailCount = 0;

        public bool UploadFile(string url, string filePath)
        {
            uploadFailCount++;

            if (uploadFailCount <= 2)
                throw new HttpRequestException($"Помилка мережі при завантаженні (імітація), спроба {uploadFailCount}");

            return true;
        }
    }

    // =============================
    // Retry Helper — універсальний
    // =============================
    public static class RetryHelper
    {
        public static T ExecuteWithRetry<T>(
            Func<T> operation,
            int retryCount = 3,
            TimeSpan initialDelay = default,
            Func<Exception, bool> shouldRetry = null)
        {
            if (initialDelay == default)
                initialDelay = TimeSpan.FromMilliseconds(500);

            for (int attempt = 1; attempt <= retryCount; attempt++)
            {
                try
                {
                    Console.WriteLine($"[INFO] Виконання спроби {attempt}...");
                    return operation();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[WARN] Помилка: {ex.Message}");

                    if (shouldRetry != null && !shouldRetry(ex))
                    {
                        Console.WriteLine("[INFO] shouldRetry каже: більше не повторюємо.");
                        throw;
                    }

                    if (attempt == retryCount)
                    {
                        Console.WriteLine("[ERROR] Досягнуто максимум спроб.");
                        throw;
                    }

                    // Експоненціальна затримка
                    var delay = TimeSpan.FromMilliseconds(initialDelay.TotalMilliseconds * Math.Pow(2, attempt - 1));
                    Console.WriteLine($"[INFO] Очікування {delay.TotalMilliseconds} мс перед наступною спробою...");
                    Thread.Sleep(delay);
                }
            }

            throw new Exception("Непередбачувана помилка RetryHelper.");
        }
    }

    // =============================
    // MAIN() — ПОВНА ДЕМОНСТРАЦІЯ
    // =============================
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Лабораторна №7 — Варіант 2 ===");

            var fileProcessor = new FileProcessor();
            var networkClient = new NetworkClient();
            string testFilePath = "testfile.txt";

            // =============================
            // shouldRetry — логіка для конкретних винятків
            // =============================
            Func<Exception, bool> retryOnlyFileAndNetwork = (ex) =>
            {
                return ex is IOException || ex is HttpRequestException;
            };

            Console.WriteLine("\n=== 1) Тестуємо FileProcessor.WriteToFile() ===");

            RetryHelper.ExecuteWithRetry(
                () =>
                {
                    fileProcessor.WriteToFile(testFilePath, "Hello world!");
                    return true;
                },
                retryCount: 5,
                initialDelay: TimeSpan.FromMilliseconds(400),
                shouldRetry: retryOnlyFileAndNetwork
            );

            Console.WriteLine("\nФайл записано успішно!");

            Console.WriteLine("\n=== 2) Тестуємо NetworkClient.UploadFile() ===");

            RetryHelper.ExecuteWithRetry(
                () =>
                {
                    return networkClient.UploadFile("https://example.com/upload", testFilePath);
                },
                retryCount: 5,
                initialDelay: TimeSpan.FromMilliseconds(400),
                shouldRetry: retryOnlyFileAndNetwork
            );

            Console.WriteLine("\nФайл успішно завантажено на сервер!");

            Console.WriteLine("\n=== Кінець роботи програми ===");

            Console.WriteLine("Натисніть будь-яку кнопку для завершення програми: ");
            Console.ReadLine();
        }
    }
}
