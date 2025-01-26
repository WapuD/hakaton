using hakaton_API.Data.Models;
using hakaton_WEB.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace hakaton_WEB.Pages
{
    public class TestsPageModel : PageModel
    {
        private readonly ILogger<TestsPageModel> _logger;
        private readonly IApiClient _apiClient;

        [BindProperty]
        public List<Test>? Tests { get; set; }  // Исправлено имя свойства на множественное число

        public TestsPageModel(ILogger<TestsPageModel> logger, IApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        public async Task OnGetAsync()
        {
            var tests = await _apiClient.GetTestAsync();

            // Группируем по ArticleTest и выбираем первый тест из каждой группы
            Tests = tests
                .GroupBy(t => t.ArticleTest)
                .Select(g => g.First()) // Берем только первый тест из каждой группы
                .ToList();
        }

    }
}