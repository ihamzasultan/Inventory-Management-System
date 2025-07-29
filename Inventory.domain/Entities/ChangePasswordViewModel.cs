using System.ComponentModel.DataAnnotations;

public class ChangePasswordViewModel
{
    [Required]
    [DataType(DataType.Password)]
    public required string CurrentPassword { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public required string NewPassword { get; set; }

    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
    public required string ConfirmNewPassword { get; set; }
}
