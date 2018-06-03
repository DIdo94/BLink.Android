using System;
using System.IO;
using BLink.Business.Enums;

namespace BLink.Business.Models
{
    public class EditMemberDetails
    {
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public double? Weight { get; set; }

        public double? Height { get; set; }

        public Position? PreferedPosition { get; set; }

        public Stream File { get; set; }

        public DateTime? DateOfBirth { get; set; }
    }
}