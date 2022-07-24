using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentApp.Models
{
    public class Student
    {
        [Key] // define id filed as a Primary Key using this "[key]" word
        public int Id { get; set; }
        [Column(TypeName = "nvarchar(20)")] // give table column  length and type
        public string StudentName { get; set; }
        public int StudentAge { get; set; }
        public string StudentAddress { get; set; }
        public int StudentPhone { get; set; }
        public int StudentEmail { get; set; }
        // setting record created date time 
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
    }
}
