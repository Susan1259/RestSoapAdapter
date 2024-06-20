using Microsoft.AspNetCore.Mvc;

namespace MyApplication.Controllers
{
    [Route("api/test")]
    [ApiController]
    public class DepartmentController: ControllerBase
    {
        [HttpGet("GetDepartment")]
        public string GetDepartment(int count)
        {
            Console.WriteLine(count);
            return "Department N: " + count;
        }
    }
}
