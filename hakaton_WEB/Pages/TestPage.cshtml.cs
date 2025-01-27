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


        [BindProperty]
        public string SuccessMessage { get; set; }


        public TestPageModel(ILogger<TestPageModel> logger, IApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        public async Task OnGetAsync(int testId)
        {
            test = await _apiClient.GetTestAsync(testId);  // ��������� ����� �� API
            tests = await _apiClient.GetTestAsync();  // ��������� ������ �� API

            if (tests != null && test.ArticleTest != null)
            {
                _questions = tests.Where(t => t.ArticleTest == test.ArticleTest).ToList();
                TempData["Questions"] = JsonConvert.SerializeObject(_questions);

                // ���������� ������ ������ ��� �������� ��������
                if (_questions.Count > 0)
                {
                    CurrentQuestion = _questions[0]; // ���������� ������ ������
                    IsLastQuestion = (_questions.Count == 1); // ���� ������ ���� ������, ��� ���������
                }
            }
        }


        public IActionResult OnPost(string selectedAnswer)
        {
            if (TempData["Questions"] != null)
            {
                _questions = JsonConvert.DeserializeObject<List<Test>>(TempData["Questions"].ToString());
                TempData["Questions"] = JsonConvert.SerializeObject(_questions);
            }

            if (int.TryParse(selectedAnswer, out int answerIndex))
            {
                if (TempData["Answers"] != null)
                {
                    UserAnswers = JsonConvert.DeserializeObject<List<int>>(TempData["Answers"].ToString());
                }
                UserAnswers.Add(answerIndex); // ���������� ������ ������������
                TempData["Answers"] = JsonConvert.SerializeObject(UserAnswers);
            }

            // ��������� �������� ������� �������
            int currentIndex = UserAnswers.Count;

            // ��������, ���� �� ��������� ������
            if (currentIndex < _questions.Count)
            {
                CurrentQuestion = _questions[currentIndex]; // ������� � ���������� �������
                IsLastQuestion = currentIndex == _questions.Count; // ��������, �������� �� ��� ��������� ��������
                return Page();
            }
            else
            {
                int Score = 0;
                for (int i = 0; i < UserAnswers.Count; i++)
                {
                    if (UserAnswers[i] == _questions[i].Answer) // ��������������, ��� CorrectAnswerIndex - ��� ������ ����������� ������
                    {
                        Score++;
                    }
                }

                var newTesting = new TestingDto();
                newTesting.Score = Score;
                newTesting.ArticleTest = _questions[0].ArticleTest;
                newTesting.CompetencyId = _questions[0].CompetencyId;
                newTesting.EmployeeId = Convert.ToInt32(HttpContext.Session.GetString("User"));

                testTest(newTesting);

                TempData.Remove("Answers");
                TempData.Remove("Questions");
                SuccessMessage = "�� ������� ������ ����!"; // ��������� ��������� �� ������
                return Page(); // ������� �� �� �� �������� ��� ����������� ���������
                //return RedirectToPage("ResultsPage", new { answers = UserAnswers }); // ��������������� �� �������� ����������� � ��������� �������
            }
        }

        public async Task testTest(TestingDto test)
        {
            await _apiClient.PostTestingAsync(test);
        }
    }
}