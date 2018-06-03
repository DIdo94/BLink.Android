using BLink.Business.Enums;
using System;

namespace BLink.Business.Models
{
    public class MemberDetails
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhotoPath { get; set; }

        public string Thumbnail { get; set; }

        public double? Weight { get; set; }

        public double? Height { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public int? ClubId { get; set; }

        public Position? PreferedPosition { get; set; }
    }
}