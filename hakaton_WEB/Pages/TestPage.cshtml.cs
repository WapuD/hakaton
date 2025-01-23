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
        public Test? test { get; set; }  // Исправлено имя свойства на множественное число
        public IEnumerable<Test>? tests { get; set; }  // Исправлено имя свойства на множественное число

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

        public async Task OnGetAsync()  // Изменено на OnGetAsync
        {
            test = await _apiClient.GetTestAsync(id);  // Получение теста из API
            tests = await _apiClient.GetTestAsync();  // Получение тестов из API
        }

        public Test CurrentTest { get; set; }
        public TestDto CurrentQuestion { get; set; }
        public bool IsLastQuestion { get; set; }

        private List<TestDto> _questions;

        public void OnGet(int testId)
        {
            // Загрузка теста из базы данных или сервиса по testId
            // Пример загрузки теста с вопросами с одинаковым ArticleTest

            _questions = LoadQuestionsByTestId(testId); // Метод для загрузки вопросов

            if (_questions != null && _questions.Count > 0)
            {
                CurrentTest = new Test { ArticleTest = _questions[0].ArticleTest }; // Установите артикул теста
                CurrentQuestion = _questions[0]; // Установите первый вопрос
                IsLastQuestion = _questions.Count == 1; // Если только один вопрос, это последний
            }
        }

        public IActionResult OnPost(string selectedAnswer)
        {
            // Здесь вы можете обработать выбранный ответ, если это необходимо

            int currentIndex = _questions.FindIndex(q => q.Id == CurrentQuestion.Id);
            if (currentIndex + 1 < _questions.Count)
            {
                CurrentQuestion = _questions[currentIndex + 1]; // Переход к следующему вопросу
                IsLastQuestion = (currentIndex + 2) == _questions.Count; // Проверка, является ли это последним вопросом
                return Page();
            }
            else
            {
                return RedirectToPage("ResultsPage"); // Перенаправление на страницу результатов или завершения теста
            }
        }

        private List<Test> LoadQuestionsByTestId(int testId)
        {
            return List <Test> filteredTests = tests.Where(t => t.ArticleTest == test.ArticleTest).ToList();

            // Здесь вы должны реализовать логику для загрузки вопросов из базы данных по testId.
            // Пример:
            return new List<TestDto>
            {
                new TestDto { Id = 1, ArticleTest = "Тест по программированию", Question = "Что такое C#?", Answer0 = "Язык программирования", Answer1 = "Операционная система", Answer2 = "База данных", Answer3 = "Фреймворк" },
                new TestDto { Id = 2, ArticleTest = "Тест по программированию", Question = "Что такое ASP.NET?", Answer0 = "Фреймворк для веб-приложений", Answer1 = "Язык программирования", Answer2 = "Система управления базами данных", Answer3 = "Редактор кода" }
                // Добавьте другие вопросы по мере необходимости
            };
        }
    }
}