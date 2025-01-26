using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;
using hakaton_API.Data.Models;
using hakaton_WEB.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OxyPlot.Wpf;

namespace hakaton_WEB.Pages
{
    public class EmployeeCompetencyChart
    {
        public string EmployeeName { get; set; }
        public string CompetencyName { get; set; }
        public List<int> Scores { get; set; }
        public List<DateTime> Dates { get; set; }
    }


    public class TrackingChangesLvlEmployeeModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IApiClient _apiClient;
        public List<EmployeeCompetencyChart> EmployeeCompetencyCharts { get; set; } = new List<EmployeeCompetencyChart>();


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

            // Apply filters
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

            // Group by Employee and Competency
            var groupedResults = TestingsList.GroupBy(t => new { t.EmployeeId, t.CompetencyId, t.Employee.Name, t.Employee.Surname, CompetencyName = t.Competency.Name })
                                              .Select(g => new EmployeeCompetencyChart
                                              {
                                                  EmployeeName = $"{g.Key.Name} {g.Key.Surname}",
                                                  CompetencyName = g.Key.CompetencyName,
                                                  Scores = g.OrderBy(t => t.Date).Select(t => t.Score).ToList(),
                                                  Dates = g.OrderBy(t => t.Date).Select(t => t.Date).ToList()
                                              }).ToList();

            EmployeeCompetencyCharts = groupedResults;

            // Sort results
            TestingsList = TestingsList.OrderBy(t => t.EmployeeId)
                                       .ThenBy(t => t.CompetencyId)
                                       .ThenBy(t => t.Date);
        }



    }
}
