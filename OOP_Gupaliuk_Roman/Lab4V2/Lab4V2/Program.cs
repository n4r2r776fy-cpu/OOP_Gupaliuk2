using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab4v2
{
    // Інтерфейс визначає контракт для всіх конвертерів валют
    public interface IConverter
    {
        decimal Convert(decimal amount);
        string CurrencyName { get; }
    }

    // Абстрактний клас, який може містити спільну логіку для всіх конвертерів
    public abstract class CurrencyConverter : IConverter
    {
        public abstract string CurrencyName { get; }
        public abstract decimal Rate { get; }

        public decimal Convert(decimal amount)
        {
            return amount * Rate;
        }
    }

    // Реалізація конвертера доларів у гривні
    public class UsdToUah : CurrencyConverter
    {
        public override string CurrencyName => "USD → UAH";
        public override decimal Rate => 41.5m; // курс долара
    }

    // Реалізація конвертера євро у гривні
    public class EurToUah : CurrencyConverter
    {
        public override string CurrencyName => "EUR → UAH";
        public override decimal Rate => 45.2m; // курс євро
    }

    // Клас CurrencyManager виконує композицію: він містить колекцію конвертерів
    public class CurrencyManager
    {
        private readonly List<IConverter> _converters = new();

        public void AddConverter(IConverter converter)
        {
            _converters.Add(converter);
        }

        // Метод перетворення масиву сум для кожного конвертера
        public void ProcessConversions(decimal[] amounts)
        {
            foreach (var converter in _converters)
            {
                Console.WriteLine($"\nКонвертація за курсом: {converter.CurrencyName}");
                var results = amounts.Select(a => converter.Convert(a)).ToArray();

                Console.WriteLine("Вхідні суми: " + string.Join(", ", amounts));
                Console.WriteLine("Результати:  " + string.Join(", ", results));

                decimal total = results.Sum();
                decimal average = results.Average();

                Console.WriteLine($"Підсумкова сума: {total:F2} грн");
                Console.WriteLine($"Середнє значення: {average:F2} грн");
            }
        }
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Конвертер валют ===");

            // Масив сум у доларах/євро
            decimal[] amounts = { 10, 25, 50, 100 };

            // Створюємо менеджер валют (композиція)
            CurrencyManager manager = new CurrencyManager();

            // Додаємо два різні конвертери (агрегація об'єктів)
            manager.AddConverter(new UsdToUah());
            manager.AddConverter(new EurToUah());

            // Виконуємо обчислення
            manager.ProcessConversions(amounts);
        }
    }
}
