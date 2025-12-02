using System;
using System.Net.Http;
using System.Threading;
using Polly;
using Polly.Timeout;

namespace IndependentWork13
{
    internal class Program
    {
        private static int _apiCallAttempts = 0;
        private static int _dbAttempts = 0;

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== IndependentWork13 — Кейси Polly ===\n");

            Scenario1_ExternalApiRetry();
            Separator();

            Scenario2_DatabaseRetryWithCircuitBreaker();
            Separator();

            Scenario3_TimeoutWithFallback();
            Separator();

            Console.WriteLine("Усі сценарії виконано. Натисніть будь-яку клавішу...");
            Console.ReadKey();
        }

        private static void Separator()
        {
            Console.WriteLine("\n------------------------------------------------------------\n");
        }

        // =========================================================
        // СЦЕНАРІЙ 1: Виклик зовнішнього API + Retry
        // =========================================================
        private static void Scenario1_ExternalApiRetry()
        {
            Console.WriteLine("=== Сценарій 1: Зовнішній API + Retry ===\n");

            _apiCallAttempts = 0;

            var retryPolicy = Policy
                .Handle<HttpRequestException>()
                .WaitAndRetry(
                    retryCount: 3,
                    sleepDurationProvider: attempt =>
                        TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                    onRetry: (exception, delay, attempt, context) =>
                    {
                        Console.WriteLine(
                            $"[{DateTime.Now:HH:mm:ss}] Retry #{attempt}, затримка {delay.TotalSeconds} c. Причина: {exception.Message}");
                    });

            try
            {
                string result = retryPolicy.Execute(() =>
                    CallExternalApi("https://api.example.com/data"));

                Console.WriteLine($"\n[{DateTime.Now:HH:mm:ss}] Результат: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Операція завершилась помилкою: {ex.Message}");
            }

            Console.WriteLine("\n--- Кінець сценарію 1 ---");
        }

        private static string CallExternalApi(string url)
        {
            _apiCallAttempts++;
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Спроба #{_apiCallAttempts}: API {url}");

            if (_apiCallAttempts <= 2)
                throw new HttpRequestException("Тимчасова помилка API (симуляція).");

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] API відповів успішно!");
            return "Дані з API (симуляція)";
        }

        // =========================================================
        // СЦЕНАРІЙ 2: Робота з БД + Retry + CircuitBreaker
        // =========================================================
        private static void Scenario2_DatabaseRetryWithCircuitBreaker()
        {
            Console.WriteLine("=== Сценарій 2: Підключення до БД + Retry + CircuitBreaker ===\n");

            _dbAttempts = 0;

            var circuitBreaker = Policy
                .Handle<Exception>()
                .CircuitBreaker(
                    exceptionsAllowedBeforeBreaking: 2,
                    durationOfBreak: TimeSpan.FromSeconds(5),
                    onBreak: (ex, breakDelay) =>
                    {
                        Console.WriteLine(
                            $"[{DateTime.Now:HH:mm:ss}] Circuit OPEN ({breakDelay.TotalSeconds} c). Причина: {ex.Message}");
                    },
                    onReset: () =>
                    {
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Circuit RESET — БД відновилась.");
                    },
                    onHalfOpen: () =>
                    {
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Circuit HALF-OPEN — тестуємо підключення...");
                    });

            var retry = Policy
                .Handle<Exception>()
                .Retry(
                    retryCount: 2,
                    onRetry: (ex, attempt) =>
                    {
                        Console.WriteLine(
                            $"[{DateTime.Now:HH:mm:ss}] Retry #{attempt} до БД. Причина: {ex.Message}");
                    });

            var combined = Policy.Wrap(circuitBreaker, retry);

            try
            {
                combined.Execute(() => SimulateDatabaseCall());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка роботи з БД: {ex.Message}");
            }

            Console.WriteLine("\n--- Кінець сценарію 2 ---");
        }

        private static void SimulateDatabaseCall()
        {
            _dbAttempts++;
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Спроба підключення #{_dbAttempts}");

            if (_dbAttempts <= 3)
                throw new Exception("Помилка підключення до БД (симуляція).");

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Запит до БД виконано успішно!");
        }

        // =========================================================
        // СЦЕНАРІЙ 3: Довга операція + Timeout + Fallback (ВИПРАВЛЕНО)
        // =========================================================
        private static void Scenario3_TimeoutWithFallback()
        {
            Console.WriteLine("=== Сценарій 3: Timeout + Fallback ===\n");

            var timeoutPolicy = Policy
                .Timeout(
                    seconds: 2,
                    timeoutStrategy: TimeoutStrategy.Pessimistic,
                    onTimeout: (context, timespan, task) =>
                    {
                        Console.WriteLine(
                            $"[{DateTime.Now:HH:mm:ss}] Timeout після {timespan.TotalSeconds} c.");
                    });

            // 🔥 ВИПРАВЛЕНИЙ Fallback (DelegateResult має .Exception)
            var fallbackPolicy = Policy<string>
                .Handle<TimeoutRejectedException>()
                .Fallback(
                    fallbackValue: "Fallback: операція перевищила ліміт часу.",
                    onFallback: fallbackResult =>
                    {
                        Console.WriteLine(
                            $"[{DateTime.Now:HH:mm:ss}] Спрацював Fallback. Причина: {fallbackResult.Exception?.Message}");
                    });

            var combined = fallbackPolicy.Wrap(timeoutPolicy);

            string result = combined.Execute(() => HeavyOperation());

            Console.WriteLine($"\nРезультат сценарію 3: {result}");
            Console.WriteLine("\n--- Кінець сценарію 3 ---");
        }

        private static string HeavyOperation()
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Запуск важкої операції (5 сек)...");
            Thread.Sleep(5000);
            return "Успішне завершення важкої операції.";
        }
    }
}
