﻿using System.ComponentModel.DataAnnotations;

namespace secondVersionFlowSync.DTOs.Auth
{
    public class LoginDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
