using System.ComponentModel.DataAnnotations;

namespace secondVersionFlowSync.DTOs.Auth
{
    public class ChangePassDto
    {
        [Required]
        public string CurrentPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } 

        [Required(ErrorMessage = "confirm password is required..")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
