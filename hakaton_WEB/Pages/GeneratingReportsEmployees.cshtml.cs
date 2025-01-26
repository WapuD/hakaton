using hakaton_API.Data.Models;
using hakaton_WEB.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static System.Net.Mime.MediaTypeNames;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;
using DocumentFormat.OpenXml;

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

            TempData["TestingsList"] = JsonConvert.SerializeObject(TestingsList);
        }


        public async Task<IActionResult> OnPostGenerateReport()
        {
            if (TempData["TestingsList"] != null)
            {
                TestingsList = JsonConvert.DeserializeObject<IEnumerable<Testing>>(TempData["TestingsList"].ToString());
            }

            // Получаем данные о сотрудниках из TestingsList
            var employeeResults = TestingsList
                .GroupBy(t => new { t.EmployeeId, t.Employee.Name, t.Employee.Surname })
                .Select(g => new
                {
                    EmployeeName = $"{g.Key.Name} {g.Key.Surname}",
                    EmployeeId = g.Key.EmployeeId,
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

                        // Установка стиля таблицы
                        TableProperties tblProperties = new TableProperties(
                            new TableBorders(
                                new TopBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                                new BottomBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                                new LeftBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                                new RightBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                                new InsideHorizontalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                                new InsideVerticalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 }
                            )
                        );
                        table.AppendChild(tblProperties);

                        // Заголовки таблицы
                        var headerRow = new TableRow();
                        headerRow.Append(
                            CreateTableCell("Компетенция"),
                            CreateTableCell("Оценка"),
                            CreateTableCell("Дата")
                        );
                        table.Append(headerRow);

                        // Заполнение таблицы данными тестирования для текущего сотрудника
                        foreach (var testing in TestingsList.Where(t => t.EmployeeId == employee.EmployeeId))
                        {
                            var row = new TableRow();
                            row.Append(
                                CreateTableCell(testing.Competency?.Name),
                                CreateTableCell(testing.Score.ToString()),
                                CreateTableCell(testing.Date.ToShortDateString())
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

        // Метод для создания ячейки таблицы с заданным текстом и стилем
        private TableCell CreateTableCell(string text)
        {
            var cellText = new DocumentFormat.OpenXml.Wordprocessing.Text(text);
            var run = new Run(cellText);

            // Установка шрифта Times New Roman и выравнивание по ширине
            run.RunProperties = new RunProperties(
                //new RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman" },
                new Justification() { Val = JustificationValues.Both }
            );

            var paragraph = new Paragraph(run);
            return new TableCell(paragraph);
        }




    }
}
