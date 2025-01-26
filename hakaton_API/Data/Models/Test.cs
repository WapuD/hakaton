using System.ComponentModel.DataAnnotations;

namespace hakaton_API.Data.Models
{
    public class Test
    {
        [Key]
        public int Id { get; set; }
        public string? ArticleTest { get; set; }
        public int CompetencyId { get; set; }
        public string? Question { get; set; }
        public int? Answer { get; set; }
        public string? Answer0 { get; set; }
        public string? Answer1 { get; set; }
        public string? Answer2 { get; set; }
        public string? Answer3 { get; set; }


        public Competency? Competency { get; set; }
    }

    public class TestDto
    {
        public int Id { get; set; }
        public string? ArticleTest { get; set; }
        public int CompetencyId { get; set; }
        public string? Question { get; set; }
        public string? Answer { get; set; }
        public string? Answer0 { get; set; }
        public string? Answer1 { get; set; }
        public string? Answer2 { get; set; }
        public string? Answer3 { get; set; }
    }

    public class GroupedTests
    {
        public string ArticleTest { get; set; }
        public IEnumerable<Competency> Competencies { get; set; }
        public List<Test> TestItems { get; set; }
    }
}
