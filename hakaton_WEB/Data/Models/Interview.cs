using System.ComponentModel.DataAnnotations;

namespace hakaton_API.Data.Models
{
    public class Interview
    {
        [Key]
        public int Id { get; set; }
        public string? Surname { get; set; }
        public string? Name { get; set; }
        public string? Patronymic { get; set; }
        public int EmployeeId { get; set; }
        public DateTime Date { get; set; }
        public string? Result { get; set; }
        public string? Comments { get; set; }


        public Employee? Employee { get; set; }
    }
}
