using System.ComponentModel.DataAnnotations;

namespace hakaton_API.Data.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}