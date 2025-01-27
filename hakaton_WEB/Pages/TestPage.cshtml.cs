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
        public Test? test { get; set; }  // Исправлено имя свойства на множественное число
        public IEnumerable<Test>? tests { get; set; }  // Исправлено имя свойства на множественное число


        public int TestId { get; private set; }  // Свойство для хранения testId


        public Test CurrentTest { get; set; }
        public Test CurrentQuestion { get; set; }
        public bool IsLastQuestion { get; set; }

        private List<Test> _questions;


        [BindProperty]
        public List<int> UserAnswers { get; set; } = new List<int>(); // Список для хранения ответов пользователя


        [BindProperty]
        public string SuccessMessage { get; set; }


        public TestPageModel(ILogger<TestPageModel> logger, IApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        public async Task OnGetAsync(int testId)
        {
            test = await _apiClient.GetTestAsync(testId);  // Получение теста из API
            tests = await _apiClient.GetTestAsync();  // Получение тестов из API

            if (tests != null && test.ArticleTest != null)
            {
                _questions = tests.Where(t => t.ArticleTest == test.ArticleTest).ToList();
                TempData["Questions"] = JsonConvert.SerializeObject(_questions);

                // Установите первый вопрос при загрузке страницы
                if (_questions.Count > 0)
                {
                    CurrentQuestion = _questions[0]; // Установите первый вопрос
                    IsLastQuestion = (_questions.Count == 1); // Если только один вопрос, это последний
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
                UserAnswers.Add(answerIndex); // Сохранение ответа пользователя
                TempData["Answers"] = JsonConvert.SerializeObject(UserAnswers);
            }

            // Получение текущего индекса вопроса
            int currentIndex = UserAnswers.Count;

            // Проверка, есть ли следующий вопрос
            if (currentIndex < _questions.Count)
            {
                CurrentQuestion = _questions[currentIndex]; // Переход к следующему вопросу
                IsLastQuestion = currentIndex == _questions.Count; // Проверка, является ли это последним вопросом
                return Page();
            }
            else
            {
                int Score = 0;
                for (int i = 0; i < UserAnswers.Count; i++)
                {
                    if (UserAnswers[i] == _questions[i].Answer) // Предполагается, что CorrectAnswerIndex - это индекс правильного ответа
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
                SuccessMessage = "Вы успешно прошли тест!"; // Установка сообщения об успехе
                return Page(); // Возврат на ту же страницу для отображения сообщения
                //return RedirectToPage("ResultsPage", new { answers = UserAnswers }); // Перенаправление на страницу результатов с передачей ответов
            }
        }

        public async Task testTest(TestingDto test)
        {
            await _apiClient.PostTestingAsync(test);
        }
    }
}