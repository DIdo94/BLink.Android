using BLink.Business.Enums;
using System;

namespace BLink.Business.Models
{
    public class ClubEventCreateRequest
    {
        public int ClubId { get; set; }

        public EventType EventType { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        //public ICollection<int> InvitedMemberIds { get; set; }

        public DateTime StartTime { get; set; }

        //public PlayerStatus IncludePlayerStatuses { get; set; }
    }
}