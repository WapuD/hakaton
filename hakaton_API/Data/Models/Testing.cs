using System.ComponentModel.DataAnnotations;

namespace hakaton_API.Data.Models
{
    public class Testing
    {
        [Key]
        public int Id { get; set; }
        public string? ArticleTest { get; set; }
        public int CompetencyId { get; set; }
        public int EmployeeId { get; set; }
        public int Score { get; set; }
        public bool Relevance { get; set; }
        public DateTimeOffset Date { get; set; }


        public Competency? Competency { get; set; }
        public Employee? Employee { get; set; }
    }
    public class TestingDto
    {
        [Key]
        public int Id { get; set; }
        public string? ArticleTest { get; set; }
        public int CompetencyId { get; set; }
        public int EmployeeId { get; set; }
        public int Score { get; set; }
    }
}
