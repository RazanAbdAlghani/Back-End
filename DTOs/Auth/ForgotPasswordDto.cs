using System.ComponentModel.DataAnnotations;

namespace secondVersionFlowSync.DTOs.Auth
{
    public class ForgotPasswordDto
    {
        [Required , EmailAddress]
        public string Email { get; set; }
    }
}
