namespace Domain
{
    public class LoginDto
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }

    public class NewUserDto
    {
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

    }
}