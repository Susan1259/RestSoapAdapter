namespace MyApplication.Models
{
    public class UserRegisterModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public List<int> RoleIds { get; set; }
    }
}
