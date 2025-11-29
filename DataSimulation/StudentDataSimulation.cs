using StudentApi.Model;

namespace StudentApi.DataSimulation
{
    public class StudentDataSimulation
    {
       public static readonly List<Student> StudentsList = new List<Student>
        {
            new Student { Id = 1, Name = "Alice Johnson", Age = 20, Grade = 88 },
            new Student { Id = 2, Name = "Bob Smith", Age = 22, Grade = 92 },
            new Student { Id = 3, Name = "Charlie Brown", Age = 19, Grade = 85 },
            new Student { Id = 4, Name = "Diana Prince", Age = 21, Grade = 95 },
            new Student { Id = 5, Name = "Ethan Hunt", Age = 23, Grade = 90 }
        };

    }
}
