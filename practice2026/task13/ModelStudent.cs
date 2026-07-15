using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace task13
{
    public class Subject
    {
        public string Name { get; set; }
        public int Grade { get; set; }
    }

    public class Student
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [JsonConverter(typeof(GenerationJSON.JSONCustomConverterByDate))]
        public DateTime BirthDate { get; set; }
        public List<Subject> Grades { get; set; }
    }
}
