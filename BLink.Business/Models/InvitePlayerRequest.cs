namespace BLink.Business.Models
{
    public class InvitePlayerRequest
    {
        public int PlayerId { get; set; }

        public int ClubId { get; set; }

        public string Description { get; set; }
    }
}