using System;
using System.Collections.Generic;

namespace lab3v2
{
    //  Базовий абстрактний клас Transport
    abstract class Transport
    {
        public string Name { get; set; }
        public double FuelConsumptionPer100Km { get; set; } // літрів на 100 км

        // Конструктор базового класу
        public Transport(string name, double fuelConsumption)
        {
            Name = name;
            FuelConsumptionPer100Km = fuelConsumption;
        }

        // Віртуальний метод для обчислення витрат пального
        public virtual double Move(double distance)
        {
            return (FuelConsumptionPer100Km / 100) * distance;
        }

        // Віртуальний метод для виводу інформації
        public virtual void Info()
        {
            Console.WriteLine($"Транспорт: {Name}, витрата: {FuelConsumptionPer100Km} л/100 км");
        }

        // Абстрактний метод для опису транспорту
        public abstract void Describe();

        // Деструктор
        ~Transport()
        {
            Console.WriteLine($" Об'єкт {Name} знищується...");
        }
    }

    // Похідний клас Car
    class Car : Transport
    {
        public int Passengers { get; set; }

        // Виклик конструктора базового класу через base
        public Car(string name, double fuelConsumption, int passengers)
            : base(name, fuelConsumption)
        {
            Passengers = passengers;
        }

        // Перевизначення методу Move()
        public override double Move(double distance)
        {
            double fuel = base.Move(distance);
            Console.WriteLine($" {Name} проїхала {distance} км. Витрати пального: {fuel:F2} л");
            return fuel;
        }

        public override void Info()
        {
            Console.WriteLine($" Автомобіль: {Name}, пасажирів: {Passengers}, витрата: {FuelConsumptionPer100Km} л/100 км");
        }

        // Реалізація абстрактного методу
        public override void Describe()
        {
            Console.WriteLine($"[Describe] Автомобіль {Name} може перевозити {Passengers} пасажирів.");
        }
    }

    // Похідний клас Bike
    class Bike : Transport
    {
        public bool HasEngine { get; set; }

        public Bike(string name, double fuelConsumption, bool hasEngine)
            : base(name, fuelConsumption)
        {
            HasEngine = hasEngine;
        }

        public override double Move(double distance)
        {
            if (!HasEngine)
            {
                Console.WriteLine($" {Name} проїхав {distance} км без палива!");
                return 0;
            }

            double fuel = base.Move(distance);
            Console.WriteLine($" {Name} проїхав {distance} км. Витрати пального: {fuel:F2} л");
            return fuel;
        }

        public override void Info()
        {
            string type = HasEngine ? "мотоцикл" : "велосипед";
            Console.WriteLine($" {Name} ({type}), витрата: {FuelConsumptionPer100Km} л/100 км");
        }

        public override void Describe()
        {
            string type = HasEngine ? "мотоцикл" : "велосипед";
            Console.WriteLine($"[Describe] {Name} це {type} з витратою {FuelConsumptionPer100Km} л/100 км.");
        }
    }

    // Основна програма
    public class Program
    {
        // Публічний статичний метод Main – точка входу
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== Лабораторна робота №3 ===");
            Console.WriteLine("Тема: Наслідування. Поліморфізм. Виклик base\n");

            // Колекція транспорту для демонстрації поліморфізму
            List<Transport> transports = new List<Transport>
            {
                new Car("Toyota", 8.5, 5),
                new Bike("Yamaha", 3.2, true),
                new Bike("Giant", 0, false)
            };

            double totalFuel = 0;
            double distance = 100; // км

            Console.WriteLine(" Інформація про транспорт:\n");
            foreach (var t in transports)
            {
                t.Info();       // Виклик перевизначеного методу
                t.Describe();   // Виклик абстрактного методу
            }

            Console.WriteLine("\n Розрахунок витрат пального на 100 км:\n");
            foreach (var t in transports)
            {
                totalFuel += t.Move(distance);
            }

            Console.WriteLine($"\n Загальні витрати пального на {distance} км: {totalFuel:F2} л");
            Console.WriteLine("\n=== Кінець програми ===");
        }
    }
}
