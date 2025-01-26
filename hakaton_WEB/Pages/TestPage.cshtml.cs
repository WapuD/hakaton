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

        public TestPageModel(ILogger<TestPageModel> logger, IApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        public async Task OnGetAsync(int testId)  // Изменено на OnGetAsync
        {
            test = await _apiClient.GetTestAsync(testId);  // Получение теста из API
            tests = await _apiClient.GetTestAsync();  // Получение тестов из API
            if (tests != null && test.ArticleTest != null)
            {
                _questions = tests.Where(t => t.ArticleTest == test.ArticleTest).ToList();
            }
            if (_questions != null)
            {
                CurrentTest = new Test { ArticleTest = _questions[0].ArticleTest }; // Установите артикул теста
                CurrentQuestion = _questions[0]; // Установите первый вопрос
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
                UserAnswers.Add(answerIndex); // Сохранение ответа пользователя
            }
            int currentIndex = _questions.FindIndex(q => q.Id == _questions[UserAnswers.Count()].Id);
            if (currentIndex + 1 < _questions.Count)
            {
                CurrentQuestion = _questions[currentIndex + 1]; // Переход к следующему вопросу
                IsLastQuestion = (currentIndex + 2) == _questions.Count; // Проверка, является ли это последним вопросом
                return Page();
            }
            else
            {
                return RedirectToPage("ResultsPage", new { answers = UserAnswers }); // Перенаправление на страницу результатов с передачей ответов
            }
        }
    }
}