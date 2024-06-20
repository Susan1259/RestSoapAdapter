﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApplication.Models;

namespace MyApplication.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StudentController : Controller
    {
        private static List<Student> _students = new List<Student>();

        

        [HttpGet]
        [Authorize(Roles ="Admin")]
        public List<Student> GetAll()
        {
            return _students;
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public Student CreateStudent(Student student)
        {
            _students.Add(student);
            return student;
        }

        [HttpPut]
        [Authorize(Roles = "User2")]
        public Student UpdateStudent(int id, Student newstudent)
        {
            var student = _students.FirstOrDefault(s=>s.ID == id);
            student.Name = newstudent.Name;
            return newstudent;
        }

        [HttpDelete]
        [Authorize(Roles = "User2")]
        public Student DeleteStudent(int id)
        {
            var student = _students.FirstOrDefault(s => s.ID == id);
            _students.Remove(student);
            return student;
        }
    }
}
