using LoCoMPro.Data;
using LoCoMPro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LoCoMPro.Pages
{
    public class ProductPageModel : PageModel
    {
        /* Context of the data base */
        private readonly LoCoMPro.Data.LoCoMProContext _context;

        /* Product Page constructor */
        public ProductPageModel(LoCoMProContext context)
        {
            _context = context;
        }

        /* List of the product that exist in the database */
        public IList<Register> register { get; set; } = default!;

        /* Amount Register Displayed */
        public int amountRegisterDisplayed { get; set; } = 3;

        /* Amount total pages*/
        public int amountPaginationPages { get; set; }

        /* Product resquested name */
        [BindProperty(SupportsGet = true)]
        public string? requestedProductName { get; set; }

        public async Task OnGetAsync() 
        {
            // Initial request that request all the registers in the database
            var registers = from r in _context.Registers select r;

            // If the name of th productResquested is not null
            if (!string.IsNullOrEmpty(requestedProductName) ) 
            {
                // Delimits registers with a where the properties are same 
                registers = registers.Where(x => x.ProductName != null && x.ProductName.Contains(requestedProductName));
                // TODO: add AND for store/model/band
            }

            // Consult amount of pages for pagination 
            if (registers is IQueryable<Register> queryrableRegisters)
            {
                // Use LINQ to asyncronic count 
                amountPaginationPages = (int)Math.Ceiling(await registers.CountAsync() / (double)amountRegisterDisplayed);
            } else
            {
                // if not Iquerable, count syncronic
                var countRegisters = registers.Count();
                amountPaginationPages = (int)Math.Ceiling(countRegisters / (double)amountRegisterDisplayed);
            }
            



        }


    }
}
