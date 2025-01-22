using System.ComponentModel.DataAnnotations;

namespace hakaton_API.Data.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string? Text { get; set; }


        public Employee? Employee { get; set; }
    }
}
