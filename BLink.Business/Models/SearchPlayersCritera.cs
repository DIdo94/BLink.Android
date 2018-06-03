using BLink.Business.Enums;

namespace BLink.Business.Models
{
    public class SearchPlayersCritera
    {
        public int? ClubId { get; set; }

        public string Name { get; set; }

        public double MinHeight { get; set; }

        public double MaxHeight { get; set; }

        public double MinWeight { get; set; }

        public double MaxWeight { get; set; }

        public Position Position { get; set; }

        public double MinAge { get; set; }

        public double MaxAge { get; set; }
    }
}