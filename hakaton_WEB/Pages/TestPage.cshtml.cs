using hakaton_API.Data.Models;
using hakaton_WEB.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
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


        public int TestId { get; private set; }  // �������� ��� �������� testId


        public Test CurrentTest { get; set; }
        public Test CurrentQuestion { get; set; }
        public bool IsLastQuestion { get; set; }

        private List<Test> _questions;


        [BindProperty]
        public List<int> UserAnswers { get; set; } = new List<int>(); // ������ ��� �������� ������� ������������

        public TestPageModel(ILogger<TestPageModel> logger, IApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        public async Task OnGetAsync(int testId)  // �������� �� OnGetAsync
        {
            test = await _apiClient.GetTestAsync(testId);  // ��������� ����� �� API
            tests = await _apiClient.GetTestAsync();  // ��������� ������ �� API
            if (tests != null && test.ArticleTest != null)
            {
                _questions = tests.Where(t => t.ArticleTest == test.ArticleTest).ToList();
            }
            if (_questions != null)
            {
                CurrentTest = new Test { ArticleTest = _questions[0].ArticleTest }; // ���������� ������� �����
                CurrentQuestion = _questions[0]; // ���������� ������ ������
                TempData["Questions"] = JsonConvert.SerializeObject(_questions);
            }
        }

        public IActionResult OnPost(string selectedAnswer)
        {
            if (TempData["Questions"] != null)
            {
                _questions = JsonConvert.DeserializeObject<List<Test>>(TempData["Questions"].ToString());
            }

            if (int.TryParse(selectedAnswer, out int answerIndex))
            {
                UserAnswers.Add(answerIndex); // ���������� ������ ������������
            }
            int currentIndex = _questions.FindIndex(q => q.Id == _questions[UserAnswers.Count()].Id);
            if (currentIndex + 1 < _questions.Count)
            {
                CurrentQuestion = _questions[currentIndex + 1]; // ������� � ���������� �������
                IsLastQuestion = (currentIndex + 2) == _questions.Count; // ��������, �������� �� ��� ��������� ��������
                return Page();
            }
            else
            {
                return RedirectToPage("ResultsPage", new { answers = UserAnswers }); // ��������������� �� �������� ����������� � ��������� �������
            }
        }
    }
}