using LoCoMPro.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LoCoMPro.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        // Context of the data base 
        private readonly LoCoMPro.Data.LoCoMProContext _context;

        public IndexModel(ILogger<IndexModel> logger, LoCoMProContext context)
        {
            _logger = logger;
            _context = context;
        }

        //public void OnGet()
        //{
        //    //throw new NotImplementedException();
        //}

        // Method called in response to an HTTP GET request to retrieve the list of cantones associated with a specific province
        public JsonResult OnGetCantones(string provincia)
        {
            var cantones = _context.Cantones
                .Where(c => c.ProvinciaName == provincia)
                .ToList();

            var result = cantones
                .Select(canton => new SelectListItem
                {
                    Value = canton.CantonName,
                    Text = canton.CantonName
                })
            .ToList();
            result!.Insert(0, new SelectListItem { Value = "", Text = "ElegirCanton" });
            return new JsonResult(result);
        }

        public JsonResult OnGetProvinces()
        {
            var provincias = _context.Provincias.ToList();
            var result = provincias
                .Select(provincia => new SelectListItem
                {
                    Value = provincia.Name,
                    Text = provincia.Name
                })
                .ToList();
            result!.Insert(0, new SelectListItem { Value = "", Text = "ElegirProvincia" });
            return new JsonResult(result);

        }
    }
}