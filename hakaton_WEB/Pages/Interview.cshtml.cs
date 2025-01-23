using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace hakaton_WEB.Pages
{
    public class InterviewModel : PageModel
    {
        [BindProperty]
        public string Id { get; set; }
        public void OnGet()
        {
            Id = HttpContext.Session.GetString("User");

        }



    }
}
