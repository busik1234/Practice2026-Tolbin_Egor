using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace task13
{
    public class GenerationJSON
    {
        public class JSONCustomConverterByDate : JsonConverter<DateTime>
        {
            private const string Format = "dd.MM.yyyy";

            public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return DateTime.ParseExact(reader.GetString(), Format, null);
            }

            public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString(Format));
            }
        }

        public class StudentOperationJSON
        {
            public string Serialize(Student student)
            {
                var options = new JsonSerializerOptions()
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    WriteIndented = true
                };
                string serializeStudent = JsonSerializer.Serialize(student, options);
                return serializeStudent;
            }

            public Student DeSerialize(string jsonstudent)
            {
                var options = new JsonSerializerOptions()
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    WriteIndented = true
                };
                Student student = JsonSerializer.Deserialize<Student>(jsonstudent, options);
                if (student == null)
                {
                    throw new ArgumentNullException("Студент не может являться пустым значением");
                }
                if (string.IsNullOrWhiteSpace(student.FirstName) || string.IsNullOrWhiteSpace(student.LastName))
                {
                    throw new ArgumentException("Имя и фамилия не могут быть пустыми значениями");
                }
                if (student.Grades != null)
                {
                    var gradesAll = student.Grades.Select(g => g.Grade).ToList();
                    var fakegrade = from g in gradesAll
                                    where g < 2 || g > 5
                                    select g;
                    if (fakegrade.Any())
                    {
                        throw new Exception("Неправильно проставлены оценки");
                    }

                    var subAll = student.Grades.Select(s => s.Name).ToList();
                    var fakesub = from s in subAll
                                  where s == null || s == ""
                                  select s;
                    if (fakesub.Any())
                    {
                        throw new Exception("Названия предметов не могут быть пустыми значениями");
                    }
                }

                if (student.BirthDate.Year > 2011 || student.BirthDate.Year < 1960)
                {
                    throw new ArgumentException("Указан неправильный год рождения");
                }
                if (student.BirthDate > DateTime.Today)
                {
                    throw new ArgumentException("Дата рождения не может быть в будущем.");
                }
                return student;
            }

            public void SaveStudentInFile(Student student, string path)
            {
                string studjson = Serialize(student);
                File.WriteAllText(path, studjson);
            }

            public Student GetStudentInFile(string path)
            {
                if (!File.Exists(path))
                {
                    throw new Exception("Не найден путь к файлу");
                }
                string studystr = File.ReadAllText(path);
                Student student = DeSerialize(studystr);
                return student;
            }
        }
    }
}
