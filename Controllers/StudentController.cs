using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApplication.Models;

namespace MyApplication.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StudentController : Controller
    {
        private readonly SampleDBContext _context;

        public StudentController(SampleDBContext context)
        {
            _context = context;
        }

        // GET: api/Student/GetAll
        [HttpGet]
        [Authorize(Roles = "Get, Admin")]
        public ActionResult<IEnumerable<Student>> GetAll()
        {
            return _context.Student.ToList();
        }

        // POST: api/Student/CreateStudent
        [HttpPost]
        [Authorize(Roles = "Create,Admin")]
        public ActionResult<Student> CreateStudent(Student student)
        {
            _context.Student.Add(student);
            _context.SaveChanges();
            return student;
        }

        // PUT: api/Student/UpdateStudent/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Update, Admin")]
        public IActionResult UpdateStudent(int id, Student newstudent)
        {
            var student = _context.Student.FirstOrDefault(s => s.ID == id);
            if (student == null)
            {
                return NotFound();
            }

            student.Name = newstudent.Name;
            _context.SaveChanges();
            return Ok(student);
        }

        // DELETE: api/Student/DeleteStudent/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Delete,Admin")]
        public IActionResult DeleteStudent(int id)
        {
            var student = _context.Student.FirstOrDefault(s => s.ID == id);
            if (student == null)
            {
                return NotFound();
            }

            _context.Student.Remove(student);
            _context.SaveChanges();
            return Ok(student);
        }
    }
}
