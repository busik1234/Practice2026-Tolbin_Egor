using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using task13;

namespace task13console
{
    public class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            string input = "0";
            Dictionary<int, Student> students = new Dictionary<int, Student>();
            int currentID = 0;

            var studentJsonManager = new GenerationJSON.StudentOperationJSON();

            while (input != "6")
            {
                Console.WriteLine("Меню");
                Console.WriteLine("1.Создать нового студента в приложении");
                Console.WriteLine("2.Загрузить студента из файла");
                Console.WriteLine("3.Удалить студента из списка");
                Console.WriteLine("4.Сохранить данные о студенте в файл");
                Console.WriteLine("5.Вывести список студентов");
                Console.WriteLine("6.Выход");

                input = Console.ReadLine();

                if (input == "1")
                {
                    EnterStudentByHand(ref currentID, students, studentJsonManager);
                }
                else if (input == "2")
                {
                    EnterStudentFromFile(ref currentID, students, studentJsonManager);
                }
                else if (input == "3")
                {
                    DeleteStudent(students);
                }
                else if (input == "4")
                {
                    SaveStudentData(students, studentJsonManager);
                }
                else if (input == "5")
                {
                    OutputStudents(students);
                }
            }
        }

        public static void OutputStudents(Dictionary<int, Student> students)
        {
            Console.WriteLine("Список студентов:");
            try
            {
                foreach (int ID in students.Keys)
                {
                    Console.WriteLine($"ID: {ID}");
                    PrintStudent(students[ID]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void SaveStudentData(Dictionary<int, Student> students, GenerationJSON.StudentOperationJSON studentJsonManager)
        {
            Console.WriteLine("Введите ID студента для сохранения в файл:");
            try
            {
                foreach (int ID in students.Keys)
                {
                    Console.WriteLine($"ID: {ID}");
                    PrintStudent(students[ID]);
                }
                int saveID = Convert.ToInt32(Console.ReadLine());
                if (!students.ContainsKey(saveID))
                {
                    throw new KeyNotFoundException("Студента с данным ID не существует");
                }
                Console.WriteLine("Введите директорию сохранения файла(напр., student.json):");
                string path = Console.ReadLine();

                studentJsonManager.SaveStudentInFile(students[saveID], path);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void DeleteStudent(Dictionary<int, Student> students)
        {
            Console.WriteLine("Введите ID студента для удаления:");
            try
            {
                foreach (int ID in students.Keys)
                {
                    Console.WriteLine($"ID: {ID}");
                    PrintStudent(students[ID]);
                }
                int deleteID = Convert.ToInt32(Console.ReadLine());
                if (!students.ContainsKey(deleteID))
                {
                    throw new KeyNotFoundException("Студента с данным ID не существует");
                }
                students.Remove(deleteID);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void EnterStudentFromFile(ref int currentID, Dictionary<int, Student> students, GenerationJSON.StudentOperationJSON studentJsonManager)
        {
            Console.WriteLine("Укажите путь к файлу:");
            try
            {
                string path = Console.ReadLine();
                students.Add(currentID++, studentJsonManager.GetStudentInFile(path));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void PrintStudent(Student student)
        {
            Console.WriteLine("Имя: " + student.FirstName);
            Console.WriteLine("Фамилия: " + student.LastName);
            Console.WriteLine("Дата рождения: " + student.BirthDate.ToString("dd.MM.yyyy"));
            Console.WriteLine("Оценки:");
            if (student.Grades != null)
            {
                foreach (Subject subject in student.Grades)
                {
                    Console.WriteLine($"  {subject.Name} - {subject.Grade}");
                }
            }
            else
            {
                Console.WriteLine(" Оценок нет");
            }
        }

        public static void EnterStudentByHand(ref int currentID, Dictionary<int, Student> students, GenerationJSON.StudentOperationJSON studentJsonManager)
        {
            try
            {
                Console.WriteLine("Введите имя студента:");
                string firstName = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(firstName))
                {
                    throw new ArgumentException("Имя студента - это обязательное поле");
                }
                Console.WriteLine("Введите фамилию студента:");
                string lastName = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(lastName))
                {
                    throw new ArgumentException("Фамилия студента - это обязательное поле");
                }
                Console.WriteLine("Введите дату рождения(формат dd-MM-yyyy)");
                string date = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(date))
                {
                    throw new ArgumentException("Дата рождения студента - это обязательное поле");
                }
                List<Subject> subjects = new List<Subject>();
                Console.WriteLine("Введите количество предметов с оценкой у студента:");
                int subjectAmount = Convert.ToInt32(Console.ReadLine());
                if (subjectAmount < 0)
                {
                    throw new ArgumentException("Ошибка - число предметов должно быть больше или равно 0");
                }
                if (subjectAmount == 0)
                {
                    subjects = null;
                }
                if (subjectAmount > 0)
                {
                    for (int i = 0; i < subjectAmount; i++)
                    {
                        Console.WriteLine("Введите название предмета");
                        string subjectName = Console.ReadLine();
                        if (subjectName == null)
                        {
                            throw new ArgumentException("Название предмета не может быть пустым");
                        }
                        Console.WriteLine("Введите оценку за предмет(2-5)");
                        int subjectGrade = Convert.ToInt32(Console.ReadLine());
                        subjects.Add(new Subject() { Name = subjectName, Grade = subjectGrade });
                    }
                }

                string[] formats = { "dd-MM-yyyy", "dd.MM.yyyy" };
                DateTime.TryParseExact(date, formats, null, DateTimeStyles.None, out DateTime dateTime);

                Student studentForAdding = new Student()
                {
                    FirstName = firstName,
                    LastName = lastName,
                    BirthDate = dateTime,
                    Grades = subjects
                };

                string checkJson = studentJsonManager.Serialize(studentForAdding);
                studentJsonManager.DeSerialize(checkJson);

                students.Add(currentID++, studentForAdding);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}