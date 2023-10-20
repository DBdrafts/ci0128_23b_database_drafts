using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LoCoMPro.Pages.Shared
{
    public class _OrderButtonModel : PageModel
    {
        // Set the content of the button
        public IActionResult SetContent(string buttonText)
        {
            string buttonContent = buttonText;

            return Content(buttonContent);
        }
    }
}
