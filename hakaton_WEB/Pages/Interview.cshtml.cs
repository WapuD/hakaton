using hakaton_API.Data.Models;
using hakaton_WEB.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace hakaton_WEB.Pages
{
    public class InterviewModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IApiClient _apiClient;
        [BindProperty]
        public string Id { get; set; }
        [BindProperty]
        public IEnumerable<Interview> InterviewList { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SearchQuery { get; set; } // Поле для поискового запроса
        [BindProperty]
        public string CommentToEdit { get; set; }

        public InterviewModel(ILogger<IndexModel> logger, IApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            Id = HttpContext.Session.GetString("User");
            InterviewList = await _apiClient.GetInterviewsAsync();

            if (!string.IsNullOrEmpty(SearchQuery))
            {
                InterviewList = InterviewList.Where(i =>
                    (i.Surname != null && SearchQuery.Contains(i.Surname)) ||
                    (i.Name != null && SearchQuery.Contains(i.Name)));
                return Page();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostEditCommentAsync(int interviewId, string commentToEdit)
        {
            var interview = new InterviewDTO()
            {
                Id = interviewId,
                Comments = commentToEdit
            };

            if (interview != null)
            {
                await _apiClient.UpdateInterviewAsync(interview);
            }

            return RedirectToPage();
        }



    }
}
