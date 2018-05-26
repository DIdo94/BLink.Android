﻿using System.IO;

namespace BLink.Business.Models
{
    public class CreateClub
    {
        public string Email { get; set; }

        public string Name { get; set; }

        public Stream ClubPhoto { get; set; }
    }
}