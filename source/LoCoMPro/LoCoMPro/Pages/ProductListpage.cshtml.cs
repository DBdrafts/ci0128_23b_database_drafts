using LoCoMPro.Data;
using LoCoMPro.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LoCoMPro.Pages
{
    public class ProductListPageModel : LoCoMProPageModel
    {

        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserProductList _userProductList { get; set; }

        public ProductListPageModel(LoCoMProContext context, IConfiguration configuration
            , IHttpContextAccessor httpContextAccessor)
            : base(context, configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _userProductList = new UserProductList(_httpContextAccessor);
        }

        public void OnGet()
        {
           
        }
    }
}
