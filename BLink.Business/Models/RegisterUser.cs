using BLink.Business.Enums;
using System;
using System.IO;

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

        public Position? PreferedPosition { get; set; }

        public Stream File { get; set; }

        public DateTime? DateOfBirth { get; set; }
    }
}