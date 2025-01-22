using System.ComponentModel.DataAnnotations;

namespace hakaton_API.Data.Models
{
    public class Competency
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Recommendations { get; set; }
    }
}
