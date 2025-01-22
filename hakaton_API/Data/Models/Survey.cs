using System.ComponentModel.DataAnnotations;

namespace hakaton_API.Data.Models
{
    public class Survey
    {
        [Key]
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int Score { get; set; }
        public string? Comment { get; set; }


        public Employee? Employee { get; set; }
    }
}
