using Microsoft.AspNetCore.Mvc;
using StudentApi.DataSimulation;
using StudentApi.Model;
using StudentModels.DTOs;
namespace StudentApi.Controllers
{
    [Route("api/Student")]
    [ApiController]
    public class StudentApiController : ControllerBase
    {
        private static readonly object _lockObj = new object();

        [HttpGet("All", Name = "GetAllStudents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<IEnumerable<StudentDTO>> GetAllStudents()
        {
            List<StudentDTO> StudentList = StudentAPIBusinessLayer.Student.AllStudents();

            if (StudentList.Count == 0)
            {
                return NotFound("No Students found.");
            }
            return Ok(StudentList);
        }


        [HttpGet("Passed", Name = "GetPassedStudents")]

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<Student>> GetPassedStudents()
        {
            var passedStudents = StudentDataSimulation.StudentsList.Where(s => s.Grade >= 50).ToList();
            if (passedStudents.Count == 0)
            {
                return NotFound("No Passed Students found.");
            }
            return Ok(passedStudents);
        }
        [HttpGet("Avg", Name = "GetAvgStudents")]

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<double> GetAvgStudents()
        {
            StudentDataSimulation.StudentsList.Clear();
            if (StudentDataSimulation.StudentsList.Count == 0)
            {
                return NotFound("No Students found.");
            }
            var AvgGrade = StudentDataSimulation.StudentsList.Average(s => s.Grade);
            return Ok(AvgGrade);
        }
        [HttpGet("{id}", Name = "GetStudentByID")]

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<IEnumerable<Student>> GetStudentByID(int id)
        {
            if (id < 1)
            {
                return BadRequest("Entry Anthor ID");
            }
            var Student = StudentDataSimulation.StudentsList.FirstOrDefault(s => s.Id == id);
            if (Student == null)
            {
                return NotFound("Not Found!");
            }
            return Ok(Student);
        }

        [HttpPost(Name = "AddStudent")]

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public ActionResult<Student> AddStudent(Student newStudent)
        {
            if (newStudent == null || string.IsNullOrEmpty(newStudent.Name) || newStudent.Age < 0 || newStudent.Grade < 0)
            {
                return BadRequest("Invalid student data.");
            }
            newStudent.Id = StudentDataSimulation.StudentsList.Count > 0 ? StudentDataSimulation.StudentsList.Max(s => s.Id) + 1 : 1;
            StudentDataSimulation.StudentsList.Add(newStudent);

            return CreatedAtRoute("GetStudentById", new { id = newStudent.Id }, newStudent);
        }

        [HttpDelete("{id}", Name = "DeleteStudent")]
        public IActionResult DeleteStudent(int ID)
        {
            if (ID <= 0)
            {
                return BadRequest("Invalid Student ID.");
            }
            try
            {
                lock (StudentDataSimulation.StudentsList)
                {
                    var student = StudentDataSimulation.StudentsList.FirstOrDefault(s => s.Id == ID);
                    if (student == null)
                    {
                        return NotFound("This ID Not Found");
                    }

                    bool remove = StudentDataSimulation.StudentsList.Remove(student);

                    if (!remove)
                    {
                        return StatusCode(500, "Faild To Delete This Student");
                    }
                }

                return NoContent();
            }
            catch
            {
                return StatusCode(500, $"An Error Ocured when Delete Student with ID: {ID}");
            }
        }

        [HttpPut("{id:int}", Name = "UpdateStudent")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]

        public IActionResult UpdateStudent(int id, [FromBody] Student st)
        {
            if (st == null)
                return BadRequest("Invalid Student object");

            if (id != st.Id)
                return BadRequest("ID in URL does not match ID in body");
            Student findStudent = null;

            lock (_lockObj)
            {
                findStudent = StudentDataSimulation.StudentsList.FirstOrDefault(s => s.Id == id);

                if (findStudent != null)
                {
                    findStudent.Name = st.Name;
                    findStudent.Age = st.Age;
                    findStudent.Grade = st.Grade;
                }
            }
            if (findStudent == null)
                return NotFound($"Student with ID: {id} not found");

            return NoContent();
        }
    }
}
