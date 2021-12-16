﻿using DatingApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.DTOS
{
    public class UserDetailsDTO
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string KnownAs { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastActive { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PhotoUrl { get; set; }
        public ICollection<PhotoDTO> Photos { get; set; }
    }
}
