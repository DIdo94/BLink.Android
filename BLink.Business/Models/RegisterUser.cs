using BLink.Business.Enums;

namespace BLink.Business.Models
{
    public class RegisterUser
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public Role Role { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public double? Weight { get; set; }

        public double? Height { get; set; }
    }
}