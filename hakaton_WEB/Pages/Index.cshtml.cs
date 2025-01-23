using System.Net;
using hakaton_API.Data.Models;
using hakaton_WEB.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Refit;

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

        public async Task<IActionResult> OnPostAsync()  
        {
            try
            {
                var user = await _apiClient.GetEmployeeAuthAsync(login, password);
                HttpContext.Session.SetString("User", user.Id.ToString());
                if (user != null) 
                {
                    return RedirectToPage("Interview");
                }
                return Page();

            }
            catch (ApiException ex)
            {
                // Обработка ошибки 401 Unauthorized
                errorlabel = "Ошибка. Неверный логин или пароль.";
                return Page();
            }
           
        }
    }
}