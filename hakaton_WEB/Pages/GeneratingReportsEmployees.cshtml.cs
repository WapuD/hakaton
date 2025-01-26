using hakaton_API.Data.Models;
using hakaton_WEB.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static System.Net.Mime.MediaTypeNames;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace hakaton_WEB.Pages
{
    public class GeneratingReportsEmployeesModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IApiClient _apiClient;

        [BindProperty]
        public IEnumerable<Testing> TestingsList { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SearchQuery { get; set; } // Поле для поискового запроса



        public GeneratingReportsEmployeesModel(ILogger<IndexModel> logger, IApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }
        public async Task OnGetAsync()
        {
            TestingsList = await _apiClient.GetTestingsAsync();
            TestingsList = TestingsList.Where(t => t.Relevance).ToList();

            if (!string.IsNullOrEmpty(SearchQuery))
            {
                TestingsList = TestingsList.Where(i =>
                    (i.Employee != null &&
                    (i.Employee.Name.Contains(SearchQuery) || i.Employee.Surname.Contains(SearchQuery))));
            }

            
            // Сортировка по EmployeeId, CompetencyId и Date
            TestingsList = TestingsList.OrderBy(t => t.EmployeeId)
                                       .ThenBy(t => t.CompetencyId)
                                       .ThenBy(t => t.Date);
        }


        public async Task<IActionResult> OnPostGenerateReport()
        {
            //await OnGetAsync();
            //if (!string.IsNullOrEmpty(SearchQuery))
            //{
            //    TestingsList = TestingsList.Where(i =>
            //        (i.Employee != null &&
            //        (i.Employee.Name.Contains(SearchQuery) || i.Employee.Surname.Contains(SearchQuery))));
            //}
            // Получаем данные о сотрудниках из TestingsList
            var employeeResults = TestingsList
                .GroupBy(t => new { t.EmployeeId, t.Employee.Name, t.Employee.Surname })
                .Select(g => new
                {
                    EmployeeName = $"{g.Key.Name} {g.Key.Surname}",
                    EmployeeId = g.Key.EmployeeId, // Сохраняем EmployeeId для дальнейшего использования
                    Strengths = g.Where(t => t.Score >= 6).Select(t => t.Competency?.Name).ToList(),
                    Weaknesses = g.Where(t => t.Score <= 5).Select(t => t.Competency?.Name).ToList()
                }).ToList();

            using (var memoryStream = new MemoryStream())
            {
                using (var wordDocument = WordprocessingDocument.Create(memoryStream, DocumentFormat.OpenXml.WordprocessingDocumentType.Document))
                {
                    var mainPart = wordDocument.AddMainDocumentPart();
                    mainPart.Document = new Document();
                    var body = mainPart.Document.AppendChild(new Body());

                    foreach (var employee in employeeResults)
                    {
                        // Добавление информации о сотруднике
                        body.AppendChild(new Paragraph(new Run(new DocumentFormat.OpenXml.Wordprocessing.Text($"Сотрудник: {employee.EmployeeName}"))));
                        body.AppendChild(new Paragraph(new Run(new DocumentFormat.OpenXml.Wordprocessing.Text("Сильные стороны: " + string.Join(", ", employee.Strengths)))));
                        body.AppendChild(new Paragraph(new Run(new DocumentFormat.OpenXml.Wordprocessing.Text("Слабые стороны: " + string.Join(", ", employee.Weaknesses)))));

                        // Добавление таблицы с результатами тестирования
                        var table = new DocumentFormat.OpenXml.Wordprocessing.Table();

                        // Заголовки таблицы
                        var headerRow = new TableRow();
                        headerRow.Append(
                            new TableCell(new Paragraph(new Run(new DocumentFormat.OpenXml.Wordprocessing.Text("Компетенция")))),
                            new TableCell(new Paragraph(new Run(new DocumentFormat.OpenXml.Wordprocessing.Text("Оценка")))),
                            new TableCell(new Paragraph(new Run(new DocumentFormat.OpenXml.Wordprocessing.Text("Дата"))))
                        );
                        table.Append(headerRow);

                        // Заполнение таблицы данными тестирования для текущего сотрудника
                        foreach (var testing in TestingsList.Where(t => t.EmployeeId == employee.EmployeeId)) // Используем employee.EmployeeId
                        {
                            var row = new TableRow();
                            row.Append(
                                new TableCell(new Paragraph(new Run(new DocumentFormat.OpenXml.Wordprocessing.Text(testing.Competency?.Name)))),
                                new TableCell(new Paragraph(new Run(new DocumentFormat.OpenXml.Wordprocessing.Text(testing.Score.ToString())))),
                                new TableCell(new Paragraph(new Run(new DocumentFormat.OpenXml.Wordprocessing.Text(testing.Date.ToShortDateString()))))
                            );
                            table.Append(row);
                        }

                        body.Append(table);
                        body.AppendChild(new Paragraph()); // Добавляем пустую строку между отчетами сотрудников
                    }
                }

                return File(memoryStream.ToArray(),
                             "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                             "Отчет_о_сотрудниках.docx");
            }
        }



    }
}
