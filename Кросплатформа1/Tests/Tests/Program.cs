using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace YourNamespace.Tests
{
    [TestFixture]
    public class ProgramTests
    {
        // Тест для коректного зчитування даних з файлу
        [Test]
        public void TestFileReading()
        {
            // Підготовка тестового файлу
            string inputFile = "test_input.txt";
            File.WriteAllLines(inputFile, new[] { "2", "0.5 1.2", "0.3 1.0" });

            var lines = File.ReadAllLines(inputFile);
            int N = int.Parse(lines[0]); // Читання кількості листів

            var sheets = new List<Sheet>();
            for (int i = 0; i < N; i++)
            {
                var values = lines[i + 1].Split(' ');
                double ai = double.Parse(values[0]);
                double bi = double.Parse(values[1]);
                sheets.Add(new Sheet(i + 1, ai, bi));
            }

            // Перевірка, що дані були зчитані коректно
            Assert.AreEqual(2, sheets.Count);
            Assert.AreEqual(0.5, sheets[0].A);
            Assert.AreEqual(1.2, sheets[0].B);
            Assert.AreEqual(0.3, sheets[1].A);
            Assert.AreEqual(1.0, sheets[1].B);

            // Видаляємо тестовий файл
            File.Delete(inputFile);
        }

        // Тест для правильності сортування листів
        [Test]
        public void TestSortingSheets()
        {
            var sheets = new List<Sheet>
            {
                new Sheet(1, 0.5, 1.2),
                new Sheet(2, 0.3, 1.0)
            };

            sheets.Sort((x, y) => Math.Min(y.A, y.B).CompareTo(Math.Min(x.A, x.B)));

            // Перевірка, що листи відсортовані за мінімумом між A та B
            Assert.AreEqual(2, sheets[0].Index); // Лист з мінімальним значенням
            Assert.AreEqual(1, sheets[1].Index);
        }

        // Тест для обчислення часу розчинення
        [Test]
        public void TestDissolutionTimeCalculation()
        {
            var sheets = new List<Sheet>
            {
                new Sheet(1, 0.5, 1.2),
                new Sheet(2, 0.3, 1.0)
            };

            sheets.Sort((x, y) => Math.Min(y.A, y.B).CompareTo(Math.Min(x.A, x.B)));

            double totalTime = 0;
            double dissolutionTimeA = 0;
            foreach (var sheet in sheets)
            {
                dissolutionTimeA += sheet.A;
                totalTime = Math.Max(totalTime, dissolutionTimeA) + sheet.B;
            }

            // Перевірка обчисленого результату
            Assert.AreEqual(2.2, totalTime, 0.0001);
        }

        // Тест для перевірки неправильного формату введених даних
        [Test]
        public void TestInvalidInputFormat()
        {
            string inputFile = "test_invalid_input.txt";
            File.WriteAllLines(inputFile, new[] { "1", "0.5 a" });

            var lines = File.ReadAllLines(inputFile);
            bool validInput = true;

            try
            {
                int N = int.Parse(lines[0]); // Зчитування кількості листів
                var sheets = new List<Sheet>();
                for (int i = 0; i < N; i++)
                {
                    var values = lines[i + 1].Split(' ');
                    double ai = double.Parse(values[0]);
                    double bi = double.Parse(values[1]);
                    sheets.Add(new Sheet(i + 1, ai, bi));
                }
            }
            catch (FormatException)
            {
                validInput = false;
            }

            Assert.IsFalse(validInput); // Тест на неправильний формат
            File.Delete(inputFile);
        }

        // Тест для перевірки запису результатів у файл
        [Test]
        public void TestFileWriting()
        {
            // Підготовка тестових вхідних даних
            string inputFile = "test_input.txt";
            File.WriteAllLines(inputFile, new[] { "2", "0.5 1.2", "0.3 1.0" });

            string outputFile = "test_output.txt";

            // Запускаємо основний метод програми
            Program.Main();  // Викликаємо ваш метод Main

            // Перевіряємо результат виведення у файл
            var outputLines = File.ReadAllLines(outputFile);
            Assert.AreEqual("2.200", outputLines[0]); // Перевірка загального часу
            Assert.AreEqual("2 1", outputLines[1]); // Перевірка індексів листів

            // Видалення тестових файлів
            File.Delete(inputFile);
            File.Delete(outputFile);
        }

        // Клас для зберігання інформації про лист
        public class Sheet
        {
            public int Index { get; }
            public double A { get; }
            public double B { get; }

            public Sheet(int index, double a, double b)
            {
                Index = index;
                A = a;
                B = b;
            }
        }
    }
}
