using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab6_V2
{
    // Власний делегат для логічної перевірки студента
    // (аналог Predicate<Student>, але наш власний тип)
    delegate bool StudentCondition(Student s);

    // Власний делегат для арифметичних операцій над балами
    delegate double GradeOperation(double grade);

    class Student
    {
        public string Name { get; set; }
        public int Grade { get; set; }   // Бал
        public string Group { get; set; }

        public Student(string name, int grade, string group)
        {
            Name = name;
            Grade = grade;
            Group = group;
        }

        public override string ToString()
        {
            return $"{Name} (група {Group}) – бал: {Grade}";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // ===== 1. Створюємо колекцію студентів (List<Student>) =====
            List<Student> students = new List<Student>
            {
                new Student("Андрій", 95, "КН-31"),
                new Student("Марія", 88, "КН-31"),
                new Student("Олег", 72, "КН-32"),
                new Student("Ірина", 81, "КН-32"),
                new Student("Софія", 60, "КН-31"),
                new Student("Дмитро", 49, "КН-33"),
                new Student("Віктор", 100, "КН-33")
            };

            Console.WriteLine("=== Початковий список студентів ===");
            students.ForEach(s => Console.WriteLine(s));
            Console.WriteLine();

            // ===== 2. Власний делегат + анонімний метод =====
            // StudentCondition – наш власний делегат (булева перевірка студента)
            // АНОНІМНИЙ МЕТОД (delegate(...) { ... })
            StudentCondition isExcellentAnon = delegate (Student s)
            {
                return s.Grade >= 90;
            };

            var excellentStudents = FilterStudents(students, isExcellentAnon);

            Console.WriteLine("=== Студенти з відмінними балами (анонімний метод + власний делегат) ===");
            PrintStudents(excellentStudents);
            Console.WriteLine();

            // ===== 3. Власний делегат + ЛЯМБДА-ВИРАЗ =====
            // Лямбда-вираз замість анонімного методу
            StudentCondition isPassedLambda = s => s.Grade >= 60;

            var passedStudents = FilterStudents(students, isPassedLambda);

            Console.WriteLine("=== Студенти, які склали (лямбда + власний делегат) ===");
            PrintStudents(passedStudents);
            Console.WriteLine();

            // ===== 4. Вбудований делегат Predicate<Student> =====
            // Predicate<Student> – стандартний делегат для булевої перевірки
            Predicate<Student> highScorePredicate = s => s.Grade > 80;

            var highScoreStudents = students.FindAll(highScorePredicate);

            Console.WriteLine("=== Студенти з балом > 80 (Predicate<Student>) ===");
            PrintStudents(highScoreStudents);
            Console.WriteLine();

            // ===== 5. Вбудований делегат Func<Student, string> =====
            // Func<T, TResult> – метод із результатом
            // Тут формуємо текстовий звіт по студенту
            Func<Student, string> studentReport = s =>
                $"Студент: {s.Name}, група: {s.Group}, бал: {s.Grade}";

            Console.WriteLine("=== Текстовий звіт по студентах (Func<Student, string> + Select) ===");
            var reports = students.Select(studentReport);
            foreach (var r in reports)
            {
                Console.WriteLine(r);
            }
            Console.WriteLine();

            // ===== 6. Вбудований делегат Action<Student> =====
            // Action<T> – метод без повернення значення
            Action<Student> printAction =
                s => Console.WriteLine($"[Action] {s.Name} – {s.Grade} балів ({s.Group})");

            Console.WriteLine("=== Вивід студентів за допомогою Action<Student> ===");
            students.ForEach(printAction);
            Console.WriteLine();

            // ===== 7. LINQ: Where, OrderBy, Select =====
            // Вибираємо студентів з балом >= 80 та сортуємо за спаданням балів
            var goodStudentsQuery = students
                .Where(s => s.Grade >= 80)            // Where – фільтрація
                .OrderByDescending(s => s.Grade)      // OrderByDescending – сортування
                .Select(s => s);                      // Select – проєкція (тут просто повертаємо s)

            Console.WriteLine("=== Студенти з балом >= 80 (Where + OrderByDescending) ===");
            PrintStudents(goodStudentsQuery.ToList());
            Console.WriteLine();

            // ===== 8. LINQ: Aggregate, Average (обробка колекції) =====
            // Aggregate – накопичення значень (тут рахуємо суму балів)
            double sumGrades = students
                .Select(s => s.Grade)
                .Aggregate((acc, next) => acc + next); // LINQ Aggregate

            double avgGrade = students.Average(s => s.Grade); // LINQ Average

            Console.WriteLine("=== Статистика ===");
            Console.WriteLine($"Сума балів: {sumGrades}");
            Console.WriteLine($"Середній бал: {avgGrade:F2}");
            Console.WriteLine();

            // ===== 9. Приклад використання нашого делегата GradeOperation =====
            // GradeOperation – власний арифметичний делегат
            GradeOperation addBonus = g => g + 5;       // лямбда
            GradeOperation clampTo100 = g => g > 100 ? 100 : g;

            Console.WriteLine("=== Оновлення балів (GradeOperation) ===");

            foreach (var s in students)
            {
                // Додаємо бонусні бали
                double withBonus = addBonus(s.Grade);
                // Обмежуємо максимум 100
                s.Grade = (int)clampTo100(withBonus);
                Console.WriteLine($"{s.Name} – новий бал: {s.Grade}");
            }

            Console.WriteLine();
            Console.WriteLine("=== Остаточний список студентів після оновлення балів ===");
            PrintStudents(students);

            Console.WriteLine();
            Console.WriteLine("Натисніть будь-яку клавішу для завершення...");
            Console.ReadKey();
        }

        // Метод для фільтрації студентів за власним делегатом StudentCondition
        static List<Student> FilterStudents(List<Student> list, StudentCondition condition)
        {
            List<Student> result = new List<Student>();
            foreach (var s in list)
            {
                if (condition(s))
                    result.Add(s);
            }
            return result;
        }

        // Метод для гарного виводу списку студентів
        static void PrintStudents(List<Student> list)
        {
            foreach (var s in list)
            {
                Console.WriteLine(s);
            }
        }
    }
}
