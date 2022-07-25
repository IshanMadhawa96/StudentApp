using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentApp.Models
{
    public class Student
    {
        [Key] // define id filed as a Primary Key using this "[key]" word
        public int Id { get; set; }

        [Display(Name = "Student Name")]
        [Required(ErrorMessage = "Student Name is Required")]
        [Column(TypeName = "nvarchar(20)")] // give table column  length and type
        public string StudentName { get; set; }

        [Display(Name = "Student Age")]
        [Required(ErrorMessage = "Student Age is Required")]
        [Range(1, 120, ErrorMessage = "Age must be between 1-120 in years.")]
        public int StudentAge { get; set; }

        [Display(Name = "Student Address")]
        [Required(ErrorMessage = "Student Address is Required")]
        public string StudentAddress { get; set; }

        [Display(Name = "Student Phone")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        [Required(ErrorMessage = "Student Phone is Required")]
        public int StudentPhone { get; set; }

        [Display(Name = "Student Email")]
        [Required(ErrorMessage = "Student Email is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public int StudentEmail { get; set; }
        // setting record created date time 
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
    }
}
