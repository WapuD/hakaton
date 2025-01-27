using hakaton_API.Data.Models;
using hakaton_WEB.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System.IO;
using System.Threading.Tasks;
using iText.IO.Font;
using iText.Kernel.Font;
using Xceed.Words.NET;

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
        public async Task<IActionResult> OnPostSubmitRating(int RatingValue, string Comment)
        {
            var survey = new Survey()
            {
                EmployeeId = 1, //Convert.ToInt32(HttpContext.Session.GetString("User")),
                Comment = Comment,
                Score = RatingValue
            };
            _apiClient.PostSurveyAsync(survey);
            return RedirectToPage(); // ��������������� �� �� �� �������� ����� ���������
        }
        public async Task<IActionResult> OnGetDownloadReport()
        {
            var employee = await _apiClient.GetEmployeeByIdAsync(1);
            var role = await _apiClient.GetRoleAsync(employee.RoleId);

            var testingsListStrPre = await _apiClient.GetTestingsAsync();
            var testingsListLowPre = await _apiClient.GetTestingsAsync();

            var testingsListStr = testingsListStrPre.Where(t => t.Relevance && t.Score >= 3).ToList();
            var testingsListLow = testingsListLowPre.Where(t => t.Relevance && t.Score < 3).ToList();

            using (var memoryStream = new MemoryStream())
            {
                // �������� ��������� Word
                using (var document = DocX.Create(memoryStream))
                {
                    // ���������
                    document.InsertParagraph("�������������� ���� ��������")
                        .FontSize(24)
                        .Bold();

                    // ���������� ���������� � ����������
                    document.InsertParagraph($"��� ����������: {employee.Surname} {employee.Name} {employee.Patronymic}");
                    document.InsertParagraph($"����: {role.Name}");
                    document.InsertParagraph($"���� ����������: {DateTime.Now.ToShortDateString()}");

                    // ����������� ��������������� ����������
                    document.InsertParagraph("����������� ��������������� ����������:");
                    foreach (var competency in testingsListStr)
                    {
                        document.InsertParagraph($"- {competency.Competency.Name}");
                    }

                    // ����������� �� ��������������� ����������
                    document.InsertParagraph("����������� �� ��������������� ����������:");
                    foreach (var competency in testingsListLow)
                    {
                        document.InsertParagraph($"- {competency.Competency.Name}");
                    }

                    // ������������ ��� ����������� ��������
                    document.InsertParagraph("������������ ��� ����������� ��������:");
                    foreach (var recommendation in testingsListLow)
                    {
                        document.InsertParagraph($"- {recommendation.Competency.Recommendations}");
                    }

                    // ���������� ��������� � ����� ������
                    document.Save();
                }

                // ���������� ���� Word
                return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "report.docx");
            }
        }
    }
}