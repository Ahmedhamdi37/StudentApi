using Microsoft.AspNetCore.Mvc;
using StudentApi.Model;
using StudentDataAccessLayer;
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
        public ActionResult<IEnumerable<StudentDTO>> GetPassedStudents()
        {
            var passedStudents = StudentAPIBusinessLayer.Student.AllStudentsPassed();
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
            var AvgGrade = StudentData.GetAverageStudent();

            if (AvgGrade == 0)
            {
                return NotFound("No Students found.");
            }
            return Ok(AvgGrade);
        }

        [HttpGet("{id:int}", Name = "GetStudentByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<StudentDTO>> GetStudentByID(int id)
        {
            if (id < 1)
            {
                return BadRequest("Entry Anthor ID");
            }
            var Student = StudentAPIBusinessLayer.Student.Find(id);
            if (Student == null)
            {
                return NotFound($"Student with ID {id} not found.");
            }
            StudentDTO SDTO = Student.SDTO;
            return Ok(SDTO);
        }

        [HttpPost(Name = "AddStudent")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<StudentDTO> AddStudent(StudentDTO SDTO)
        {
            if (SDTO == null || string.IsNullOrEmpty(SDTO.Name) || SDTO.Age < 0 || SDTO.Grade < 0)
                return BadRequest("Invalid student data.");

            var Student = new StudentAPIBusinessLayer.Student(SDTO);

            if (Student.Save())
            {
                SDTO.Id = Student.ID;
                return CreatedAtRoute("GetStudentById", new { id = SDTO.Id }, SDTO);
            }

            return StatusCode(500, "Faild To Add This Student");
        }

        [HttpDelete("{id:int}", Name = "DeleteStudent")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteStudent(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid Student ID.");
            try
            {
                lock (_lockObj)
                {
                    var student = StudentAPIBusinessLayer.Student.Find(id);

                    if (student == null)
                    {
                        return NotFound("This ID Not Found");
                    }

                    bool remove = StudentAPIBusinessLayer.Student.Delete(id);

                    if (!remove)
                        return StatusCode(500, "Faild To Delete This Student");
                }
                return NoContent();
            }
            catch
            {
                return StatusCode(500, $"An Error Ocured when Delete Student with ID: {id}");
            }
        }

        [HttpPut("{id:int}", Name = "UpdateStudent")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateStudent(int id, [FromBody] StudentDTO st)
        {
            if (id < 1 || st == null || string.IsNullOrEmpty(st.Name) || st.Age < 0 || st.Grade < 0)
                return BadRequest("Invalid Student object");

            var findStudent = StudentAPIBusinessLayer.Student.Find(id);

            lock (_lockObj)
            {
                if (findStudent != null)
                {
                    findStudent.Name = st.Name;
                    findStudent.Age = st.Age;
                    findStudent.Grade = st.Grade;

                    if (findStudent.Save())
                        return NoContent();
                }
            }
            if (findStudent == null)
                return NotFound($"Student with ID: {id} not found");

            return StatusCode(500, "Faild To Update This Student");
        }
    }
}
