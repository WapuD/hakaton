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
        public async Task<IActionResult> OnPostSubmitRating(int RatingValue, string Comment)
        {
            var survey = new Survey()
            {
                EmployeeId = 1, //Convert.ToInt32(HttpContext.Session.GetString("User")),
                Comment = Comment,
                Score = RatingValue
            };
            _apiClient.PostSurveyAsync(survey);
            return RedirectToPage(); // Перенаправление на ту же страницу после обработки
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
                // Создание документа Word
                using (var document = DocX.Create(memoryStream))
                {
                    // Заголовок
                    document.InsertParagraph("Индивидуальный план развития")
                        .FontSize(24)
                        .Bold();

                    // Добавление информации о сотруднике
                    document.InsertParagraph($"ФИО сотрудника: {employee.Surname} {employee.Name} {employee.Patronymic}");
                    document.InsertParagraph($"Роль: {role.Name}");
                    document.InsertParagraph($"Дата заполнения: {DateTime.Now.ToShortDateString()}");

                    // Компетенции соответствующие стандартам
                    document.InsertParagraph("Компетенции соответствующие стандартам:");
                    foreach (var competency in testingsListStr)
                    {
                        document.InsertParagraph($"- {competency.Competency.Name}");
                    }

                    // Компетенции не соответствующие стандартам
                    document.InsertParagraph("Компетенции не соответствующие стандартам:");
                    foreach (var competency in testingsListLow)
                    {
                        document.InsertParagraph($"- {competency.Competency.Name}");
                    }

                    // Рекомендации для восполнения пробелов
                    document.InsertParagraph("Рекомендации для восполнения пробелов:");
                    foreach (var recommendation in testingsListLow)
                    {
                        document.InsertParagraph($"- {recommendation.Competency.Recommendations}");
                    }

                    // Сохранение документа в поток памяти
                    document.Save();
                }

                // Возвращаем файл Word
                return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "report.docx");
            }
        }
    }
}