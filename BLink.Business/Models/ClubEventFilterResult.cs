using System;
using BLink.Business.Enums;

namespace BLink.Business.Models
{
    public class ClubEventFilterResult
    {
        public int Id { get; set; }

        public EventType EventType { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime StartTime { get; set; }
    }
}