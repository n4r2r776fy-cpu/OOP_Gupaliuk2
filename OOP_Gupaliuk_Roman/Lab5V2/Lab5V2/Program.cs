using System;
using System.Linq;
using System.Text;

class Program
{
    static void Main()
    {
        // Встановлюємо кодування UTF-8 для консолі
        Console.OutputEncoding = Encoding.UTF8;
        try
        {
            var studentRepo = new Repository<Student>();

            // Додавання студентів
            studentRepo.Add(new Student { Name = "Ivan", Age = 20, Grade = 85 });
            studentRepo.Add(new Student { Name = "Olya", Age = 22, Grade = 92 });
            studentRepo.Add(new Student { Name = "Petro", Age = 19, Grade = 78 });

            // LINQ: студенти з оцінкою більше 80
            var topStudents = studentRepo.Find(s => s.Grade > 80);

            Console.WriteLine("Студенти з оцінкою більше 80:");
            foreach (var student in topStudents)
            {
                Console.WriteLine(student);
            }

            // Генерація власного винятку для перевірки
            var invalidStudent = new Student { Name = "Anna", Age = 21, Grade = 150 };
            if (invalidStudent.Grade > 100)
                throw new InvalidGradeException("Оцінка не може бути більше 100");
        }
        catch (InvalidGradeException ex)
        {
            Console.WriteLine($"Власний виняток: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Стандартний виняток: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("Програма завершила виконання.");
        }
    }
}
