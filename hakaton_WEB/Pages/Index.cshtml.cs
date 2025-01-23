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
        public Employee? Users { get; set; }  // Исправлено имя свойства на множественное число
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

        public async Task<IActionResult> OnPostAsync()  // Изменено на OnGetAsync
        {
            var testError = await _apiClient.GetEmployeeAuthAsync(login, password);  // Получение тестов из API
            if (testError == null)
            {
                errorlabel = "Ошибка. Неверный логин или пароль";
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