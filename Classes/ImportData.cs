using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using ekz.Models;

namespace ekz.Classes
{
    public static class CsvImporter
    {
        public static void ImportCarsFromCsv(string filePath)
        {
            try
            {
                using (var context = new qwsEntities1())
                {
                    // Читаем CSV файл
                    var lines = File.ReadAllLines(filePath, Encoding.UTF8);

                    // Пропускаем первые 2 строки (пустые) и заголовок
                    for (int i = 3; i < lines.Length; i++)
                    {
                        var columns = lines[i].Split(';');

                        if (columns.Length >= 3)
                        {
                            var car = new car
                            {
                                marka = columns[1].Trim('"'), // убираем кавычки
                                number = columns[2].Trim()
                            };

                            // Парсим ID если есть
                            if (int.TryParse(columns[0], out int id))
                            {
                                // ID автоинкрементный, не устанавливаем
                            }

                            context.car.Add(car);
                        }
                    }

                    context.SaveChanges();
                    MessageBox.Show($"Импортировано {lines.Length - 3} автомобилей", "Успех");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка импорта: {ex.Message}", "Ошибка");
            }
        }

        public static void ImportClientsFromCsv(string filePath)
        {
            try
            {
                using (var context = new qwsEntities1())
                {
                    var lines = File.ReadAllLines(filePath, Encoding.UTF8);

                    // Определяем кодировку данных
                    var dataLines = lines.Skip(3); // Пропускаем первые 3 строки

                    foreach (var line in dataLines)
                    {
                        var columns = line.Split(';');

                        if (columns.Length >= 12)
                        {
                            var user = new users
                            {
                                login = columns[1].Trim(),
                                password = columns[2].Trim(),
                                FullName = columns[3].Trim(),
                                role = columns[4].Trim(),
                                IsBlocked = string.IsNullOrEmpty(columns[5].Trim()) ? "0" : "1",
                                Заказ = columns[6].Trim(),
                                Пункт_отправления = columns[7].Trim(),
                                Пункт_выдачи = columns[8].Trim(),
                                Дата_доставки = columns[9].Trim(),
                                Дата_оплаты = ParseDateTime(columns[10]),
                                Сумма = columns[11].Trim(),
                                id_car = ParseInt(columns[12])
                            };

                            context.users.Add(user);
                        }
                    }

                    context.SaveChanges();
                    MessageBox.Show($"Импортировано {dataLines.Count()} клиентов", "Успех");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка импорта клиентов: {ex.Message}", "Ошибка");
            }
        }

        private static DateTime? ParseDateTime(string text)
        {
            if (string.IsNullOrWhiteSpace(text) || text == "NULL")
                return null;

            if (DateTime.TryParse(text, out DateTime result))
                return result;

            return null;
        }

        private static int? ParseInt(string text)
        {
            if (string.IsNullOrWhiteSpace(text) || text == "NULL")
                return null;

            if (int.TryParse(text, out int result))
                return result;

            return null;
        }
    }
}