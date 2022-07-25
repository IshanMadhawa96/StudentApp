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
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Student Name must be between 3 and 20 chars")]
        public string StudentName { get; set; }

        [Display(Name = "Student Age")]
        [Required(ErrorMessage = "Student Age is Required")]
        [Range(5, 25, ErrorMessage = "Age must be between 5-25 in years.")]
        public int StudentAge { get; set; }

        [Display(Name = "Student Address")]
        [Required(ErrorMessage = "Student Address is Required")]
        public string StudentAddress { get; set; }

        [Display(Name = "Student Phone")]
        [Required(ErrorMessage = "Student Phone is Required")]
        //[RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Phone Number.")]
        public int StudentPhone { get; set; }

        [Display(Name = "Student Email")]
        [Required(ErrorMessage = "Student Email is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string StudentEmail { get; set; }
        // setting record created date time 
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
    }
}
