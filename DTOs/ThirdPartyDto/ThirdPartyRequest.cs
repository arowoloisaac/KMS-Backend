﻿using Key_Management_System.Enums;

namespace Key_Management_System.DTOs.ThirdPartyDto
{
    public class ThirdPartyRequest
    {
        public Guid Id { get; set; }

        public Activity Activity { get; set; }
    }
}