using System;

namespace lab1v2
{
    // Клас Car
    class Car
    {
        //  Поля
        private string brand;
        private string model;

        //  Властивість
        public int Year { get; set; }

        //  Конструктор
        public Car(string brand, string model, int year)
        {
            this.brand = brand;
            this.model = model;
            Year = year;

            Console.WriteLine($" Створено автомобіль: {brand} {model}, {year} року");
        }

        //  Метод
        public void Drive()
        {
            Console.WriteLine($" {brand} {model} ({Year}) вирушає в дорогу!");
        }

        //  Деструктор
        ~Car()
        {
            Console.WriteLine($" Об’єкт {brand} {model} знищується...");
        }
    }

    // Основна програма
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Лабораторна робота №1 ===");
            Console.WriteLine("Тема: Класи, об’єкти, конструктори, деструктори");
            Console.WriteLine("Варіант 2 — Клас Car\n");

            // Створення об'єктів класу Car
            Car car1 = new Car("Toyota", "Camry", 2020);
            Car car2 = new Car("BMW", "X5", 2023);
            Car car3 = new Car("Tesla", "Model 3", 2024);

            // Виклик методів
            car1.Drive();
            car2.Drive();
            car3.Drive();

            Console.WriteLine("\n=== Кінець програми ===");
        }
    }
}
