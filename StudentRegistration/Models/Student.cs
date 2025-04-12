using System.ComponentModel.DataAnnotations;

namespace StudentRegistration.Models;

public class Student
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [Display(Name = "Name")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Mobile number is required")]
    [Display(Name = "Mobile")]
    [StringLength(10, MinimumLength = 10, ErrorMessage = "Mobile number must be 10 digits")]
    [RegularExpression(@"^[0-9]*$", ErrorMessage = "Mobile number should contain only digits")]
    public string Mobile { get; set; }

    [Required(ErrorMessage = "Gender is required")]
    [Display(Name = "Gender")]
    public string Gender { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Display(Name = "Address")]
    public string Address { get; set; }

    [Required(ErrorMessage = "Date of Birth is required")]
    [Display(Name = "Date of Birth")]
    [DataType(DataType.Date)]
    public DateTime DOB { get; set; }

    [Required(ErrorMessage = "Class is required")]
    [Display(Name = "Class")]
    public string Class { get; set; }

    [Required(ErrorMessage = "Father's name is required")]
    [Display(Name = "Father's Name")]
    [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "Father's name should contain only alphabets")]
    public string FatherName { get; set; }

    [Required(ErrorMessage = "Mother's name is required")]
    [Display(Name = "Mother's Name")]
    [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "Mother's name should contain only alphabets")]
    public string MotherName { get; set; }
}
