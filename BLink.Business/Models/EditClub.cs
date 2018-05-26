using System.IO;

namespace BLink.Business.Models
{
    public class EditClub
    {
        public int ClubId { get; set; }

        public string Name { get; set; }

        public Stream ClubPhoto { get; set; }

        public string Email { get; set; }
    }
}