using System;
using System.Collections.Generic;
using Xunit;
using task13; 

namespace task13tests
{
    public class GenerationJSONTests
    {
        private readonly GenerationJSON.StudentOperationJSON _operation = new GenerationJSON.StudentOperationJSON();

        [Fact]
        public void SerializeAndDeserialize_ValidStudent_ReturnsCorrectObject()
        { 
            var student = new Student
            {
                FirstName = "Егор",
                LastName = "Толбин",
                BirthDate = new DateTime(2005, 4, 15),
                Grades = new List<Subject>
                {
                    new Subject { Name = "Алгебра", Grade = 5 },
                    new Subject { Name = "Физика", Grade = 4 }
                }
            };
            string json = _operation.Serialize(student);
            var result = _operation.DeSerialize(json);

            Assert.NotNull(result);
            Assert.Equal("Егор", result.FirstName);
            Assert.Equal("Толбин", result.LastName);
            Assert.Equal(new DateTime(2005, 4, 15), result.BirthDate);
            Assert.Equal(2, result.Grades.Count);
        }

        [Theory]
        [InlineData(null, "Толбин")]
        [InlineData("Егор", null)]
        [InlineData("", "Толбин")]
        [InlineData("Егор", "  ")]
        public void Deserialize_EmptyNames_ThrowsArgumentException(string firstName, string lastName)
        {
            var student = new Student
            {
                FirstName = firstName,
                LastName = lastName,
                BirthDate = new DateTime(2005, 4, 15)
            };
            string json = _operation.Serialize(student);

            Assert.Throws<ArgumentException>(() => _operation.DeSerialize(json));
        }

        [Theory]
        [InlineData(1959)]
        [InlineData(2012)]
        public void Deserialize_InvalidBirthYear_ThrowsArgumentException(int year)
        {
            var student = new Student
            {
                FirstName = "Иван",
                LastName = "Иванов",
                BirthDate = new DateTime(year, 1, 1)
            };
            string json = _operation.Serialize(student);

            Assert.Throws<ArgumentException>(() => _operation.DeSerialize(json));
        }

        [Fact]
        public void Deserialize_FutureBirthDate_ThrowsArgumentException()
        {
            var student = new Student
            {
                FirstName = "Иван",
                LastName = "Иванов",
                BirthDate = DateTime.Today.AddDays(1)
            };
            string json = _operation.Serialize(student);

            Assert.Throws<ArgumentException>(() => _operation.DeSerialize(json));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(6)]
        public void Deserialize_InvalidGrades_ThrowsException(int grade)
        {
            var student = new Student
            {
                FirstName = "Иван",
                LastName = "Иванов",
                BirthDate = new DateTime(2005, 1, 1),
                Grades = new List<Subject>
                {
                    new Subject { Name = "Математика", Grade = grade }
                }
            };
            string json = _operation.Serialize(student);

            var ex = Assert.Throws<Exception>(() => _operation.DeSerialize(json));
            Assert.Equal("Неправильно проставлены оценки", ex.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Deserialize_EmptySubjectName_ThrowsException(string subjectName)
        {
            var student = new Student
            {
                FirstName = "Иван",
                LastName = "Иванов",
                BirthDate = new DateTime(2005, 1, 1),
                Grades = new List<Subject>
                {
                    new Subject { Name = subjectName, Grade = 4 }
                }
            };
            string json = _operation.Serialize(student);
            var ex = Assert.Throws<Exception>(() => _operation.DeSerialize(json));
            Assert.Equal("Названия предметов не могут быть пустыми значениями", ex.Message);
        }
    }
}
