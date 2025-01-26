using hakaton_API.Data.Models;
using hakaton_WEB.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace hakaton_WEB.Pages
{
    public class TrackingChangesLvlEmployeeModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IApiClient _apiClient;
        
        [BindProperty]
        public IEnumerable<Testing> TestingsList { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SearchQuery { get; set; } // Поле для поискового запроса
        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; } // Дата начала
        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; } // Дата окончания


        public TrackingChangesLvlEmployeeModel(ILogger<IndexModel> logger, IApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }
        public async Task OnGetAsync()
        {
            TestingsList = await _apiClient.GetTestingsAsync();

            if (!string.IsNullOrEmpty(SearchQuery))
            {
                TestingsList = TestingsList.Where(i =>
                    (i.Employee != null &&
                    (i.Employee.Name.Contains(SearchQuery) || i.Employee.Surname.Contains(SearchQuery))));
            }

            if (StartDate.HasValue)
            {
                TestingsList = TestingsList.Where(t => t.Date >= StartDate.Value);
            }

            if (EndDate.HasValue)
            {
                TestingsList = TestingsList.Where(t => t.Date <= EndDate.Value);
            }

            // Сортировка по EmployeeId, CompetencyId и Date
            TestingsList = TestingsList.OrderBy(t => t.EmployeeId)
                                       .ThenBy(t => t.CompetencyId)
                                       .ThenBy(t => t.Date);
        }

    }
}
