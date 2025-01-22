using hakaton_API.Data.Models;
using hakaton_WEB.Data;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace hakaton_WEB.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IApiClient _apiClient;

        [BindProperty]
        public string Name { get; set; }
        [BindProperty]
        public int Id { get; set; }
        public Role role { get; set; }
        public IndexModel(ILogger<IndexModel> logger, IApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        public async Task OnGetAsync()
        {
            var roles = await _apiClient.GetRoleAsync();
        }
        public async Task OnPostAsync()
        {
            if (Id != 0)
            {
                role = await _apiClient.GetRoleAsync(Id);
                Name = role.Name;
            }
        }
    }
}