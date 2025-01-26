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
        public string SearchQuery { get; set; } // ���� ��� ���������� �������



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

            
            // ���������� �� EmployeeId, CompetencyId � Date
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
            // �������� ������ � ����������� �� TestingsList
            var employeeResults = TestingsList
                .GroupBy(t => new { t.EmployeeId, t.Employee.Name, t.Employee.Surname })
                .Select(g => new
                {
                    EmployeeName = $"{g.Key.Name} {g.Key.Surname}",
                    EmployeeId = g.Key.EmployeeId, // ��������� EmployeeId ��� ����������� �������������
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
                        // ���������� ���������� � ����������
                        body.AppendChild(new Paragraph(new Run(new DocumentFormat.OpenXml.Wordprocessing.Text($"���������: {employee.EmployeeName}"))));
                        body.AppendChild(new Paragraph(new Run(new DocumentFormat.OpenXml.Wordprocessing.Text("������� �������: " + string.Join(", ", employee.Strengths)))));
                        body.AppendChild(new Paragraph(new Run(new DocumentFormat.OpenXml.Wordprocessing.Text("������ �������: " + string.Join(", ", employee.Weaknesses)))));

                        // ���������� ������� � ������������ ������������
                        var table = new DocumentFormat.OpenXml.Wordprocessing.Table();

                        // ��������� �������
                        var headerRow = new TableRow();
                        headerRow.Append(
                            new TableCell(new Paragraph(new Run(new DocumentFormat.OpenXml.Wordprocessing.Text("�����������")))),
                            new TableCell(new Paragraph(new Run(new DocumentFormat.OpenXml.Wordprocessing.Text("������")))),
                            new TableCell(new Paragraph(new Run(new DocumentFormat.OpenXml.Wordprocessing.Text("����"))))
                        );
                        table.Append(headerRow);

                        // ���������� ������� ������� ������������ ��� �������� ����������
                        foreach (var testing in TestingsList.Where(t => t.EmployeeId == employee.EmployeeId)) // ���������� employee.EmployeeId
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
                        body.AppendChild(new Paragraph()); // ��������� ������ ������ ����� �������� �����������
                    }
                }

                return File(memoryStream.ToArray(),
                             "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                             "�����_�_�����������.docx");
            }
        }



    }
}
