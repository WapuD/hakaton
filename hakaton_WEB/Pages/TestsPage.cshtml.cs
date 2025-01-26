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
        public List<Test>? Tests { get; set; }  // ���������� ��� �������� �� ������������� �����

        public TestsPageModel(ILogger<TestsPageModel> logger, IApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        public async Task OnGetAsync()
        {
            var tests = await _apiClient.GetTestAsync();

            // ���������� �� ArticleTest � �������� ������ ���� �� ������ ������
            Tests = tests
                .GroupBy(t => t.ArticleTest)
                .Select(g => g.First()) // ����� ������ ������ ���� �� ������ ������
                .ToList();
        }

    }
}