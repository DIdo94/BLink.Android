﻿using BLink.Business.Enums;

namespace BLink.Business.Models
{
    public class ClubEventFilterRequest
    {
        public int ClubId { get; set; }

        public int MemberId { get; set; }

        public EventTimeSpan EventTimeSpan { get; set; }
    }
}