﻿using System;
using System.Collections.Generic;

namespace ComicRentalSystem_14Days.Models
{
    public class Member : BaseEntity 
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; } 
        public string Username { get; set; } 


        public Member()
        {
            Name = string.Empty;
            PhoneNumber = string.Empty;
            Username = string.Empty;
        }
    }
}


