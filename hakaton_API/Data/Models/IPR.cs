using System.ComponentModel.DataAnnotations;

namespace hakaton_API.Data.Models
{
    public class IPR
    {
        [Key]
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public DateTime Date { get; set; }


        public Employee? Employee { get; set; }
    }
}
