using hakaton_API.Data.Models;
using hakaton_WEB.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace hakaton_WEB.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IApiClient _apiClient;

        [BindProperty]
        public Employee? Users { get; set; }  // ���������� ��� �������� �� ������������� �����
        [BindProperty]
        public string login { get; set; }
        [BindProperty]
        public string password { get; set; }
        [BindProperty]
        public string errorlabel { get; set; }

        public IndexModel(ILogger<IndexModel> logger, IApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        public async Task<IActionResult> OnPostAsync()  // �������� �� OnGetAsync
        {
            var testError = await _apiClient.GetEmployeeAuthAsync(login, password);  // ��������� ������ �� API
            if (testError == null)
            {
                errorlabel = "������. �������� ����� ��� ������";
                return Page();
            }
            else
            {
                errorlabel = testError.Name;
                return Page();
            }
        }
    }
}