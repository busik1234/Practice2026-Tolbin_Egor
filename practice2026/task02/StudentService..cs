namespace task02
{
    public class Student
    {
        public string Name { get; set; }
        public string Faculty { get; set; }
        public List<int> Grades { get; set; }
    }
    public class StudentService
    {
        private readonly List<Student> _students;

        public StudentService(List<Student> students) => _students = students;

        // 1. Возвращает студентов указанного факультета
        public IEnumerable<Student> GetStudentsByFaculty(string faculty)
        {
            var result = from student in _students
                         where student.Faculty == faculty
                         select student;
            return result;
        }

        // 2. Возвращает студентов со средним баллом >= minAverageGrade
        public IEnumerable<Student> GetStudentsWithMinAverageGrade(double minAverageGrade)
        {
            var result = from student in _students
                         where ((double)student.Grades.Sum() / student.Grades.Count()) >= minAverageGrade
                         select student;
            return result;
        }

        // 3. Возвращает студентов, отсортированных по имени (A-Z)
        public IEnumerable<Student> GetStudentsOrderedByName()
        {
            var result = _students.OrderBy(x => x.Name);
            return result;

            //return _students.OrderBy(x => x.Name).Select(x => x.Name) - если нужны только имена
        }


        // 4. Группировка по факультету
        public ILookup<string, Student> GroupStudentsByFaculty()
        {
            var result = _students.ToLookup(x => x.Faculty);
            return result;
        }



        // 5. Находит факультет с максимальным средним баллом
        public string GetFacultyWithHighestAverageGrade()
        {
            if (!_students.Any()) return "Нет данных";
            var groups = from student in _students
                         group student by student.Faculty into facultygroup
                         select new
                         {
                             Facname = facultygroup.Key,
                             MidlleGrades = ((double)facultygroup.SelectMany(student => student.Grades).Sum() / facultygroup.SelectMany(student => student.Grades).Count())
                         };
            double res = groups.Max(x => x.MidlleGrades);
            var result = from s in groups
                         where s.MidlleGrades == res
                         select s.Facname;

            string max = result.FirstOrDefault();
            return max;
        }

    }
}
