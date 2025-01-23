using hakaton_API.Data.Models;
using hakaton_WEB.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace hakaton_WEB.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IApiClient _apiClient;

        [BindProperty]
        public List<Test>? Tests { get; set; }  // ���������� ��� �������� �� ������������� �����

        public IndexModel(ILogger<IndexModel> logger, IApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        public async Task OnGetAsync()  // �������� �� OnGetAsync
        {
            Tests = (await _apiClient.GetTestAsync()).ToList();  // ��������� ������ �� API
        }
    }
}