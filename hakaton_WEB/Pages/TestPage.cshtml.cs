using hakaton_API.Data.Models;
using hakaton_WEB.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace hakaton_WEB.Pages.Shared
{
    public class TestPageModel : PageModel
    {
        private readonly ILogger<TestPageModel> _logger;
        private readonly IApiClient _apiClient;

        [BindProperty]
        public Test? test { get; set; }  // ���������� ��� �������� �� ������������� �����
        public IEnumerable<Test>? tests { get; set; }  // ���������� ��� �������� �� ������������� �����

        public TestPageModel(ILogger<TestPageModel> logger, IApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        public int id;
        public void OnGet(int testId)
        {
            id = testId;
        }

        public async Task OnGetAsync()  // �������� �� OnGetAsync
        {
            test = await _apiClient.GetTestAsync(id);  // ��������� ����� �� API
            tests = await _apiClient.GetTestAsync();  // ��������� ������ �� API
        }

        public Test CurrentTest { get; set; }
        public TestDto CurrentQuestion { get; set; }
        public bool IsLastQuestion { get; set; }

        private List<TestDto> _questions;

        public void OnGet(int testId)
        {
            // �������� ����� �� ���� ������ ��� ������� �� testId
            // ������ �������� ����� � ��������� � ���������� ArticleTest

            _questions = LoadQuestionsByTestId(testId); // ����� ��� �������� ��������

            if (_questions != null && _questions.Count > 0)
            {
                CurrentTest = new Test { ArticleTest = _questions[0].ArticleTest }; // ���������� ������� �����
                CurrentQuestion = _questions[0]; // ���������� ������ ������
                IsLastQuestion = _questions.Count == 1; // ���� ������ ���� ������, ��� ���������
            }
        }

        public IActionResult OnPost(string selectedAnswer)
        {
            // ����� �� ������ ���������� ��������� �����, ���� ��� ����������

            int currentIndex = _questions.FindIndex(q => q.Id == CurrentQuestion.Id);
            if (currentIndex + 1 < _questions.Count)
            {
                CurrentQuestion = _questions[currentIndex + 1]; // ������� � ���������� �������
                IsLastQuestion = (currentIndex + 2) == _questions.Count; // ��������, �������� �� ��� ��������� ��������
                return Page();
            }
            else
            {
                return RedirectToPage("ResultsPage"); // ��������������� �� �������� ����������� ��� ���������� �����
            }
        }

        private List<Test> LoadQuestionsByTestId(int testId)
        {
            return List <Test> filteredTests = tests.Where(t => t.ArticleTest == test.ArticleTest).ToList();

            // ����� �� ������ ����������� ������ ��� �������� �������� �� ���� ������ �� testId.
            // ������:
            return new List<TestDto>
            {
                new TestDto { Id = 1, ArticleTest = "���� �� ����������������", Question = "��� ����� C#?", Answer0 = "���� ����������������", Answer1 = "������������ �������", Answer2 = "���� ������", Answer3 = "���������" },
                new TestDto { Id = 2, ArticleTest = "���� �� ����������������", Question = "��� ����� ASP.NET?", Answer0 = "��������� ��� ���-����������", Answer1 = "���� ����������������", Answer2 = "������� ���������� ������ ������", Answer3 = "�������� ����" }
                // �������� ������ ������� �� ���� �������������
            };
        }
    }
}