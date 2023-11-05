using LoCoMPro.Data;
using LoCoMPro.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LoCoMPro.Pages
{
    public class UserInfoPageModel : LoCoMProPageModel
    {
        public UserInfoPageModel(LoCoMProContext context, IConfiguration configuration)
            : base(context, configuration)
        {

        }
        public void OnGet()
        {
        }
    }
}
