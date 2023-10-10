﻿using LoCoMPro.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LoCoMPro.Pages
{
    /// <summary>
    /// Model for Index page, handles requests for the lists of provinces and cantons.
    /// </summary>
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        // Context of the data base 
        private readonly LoCoMPro.Data.LoCoMProContext _context;

        /// <summary>
        /// Creates an IndexModel instance, needs a loger and a context.
        /// </summary>
        /// <param name="logger">Logger for Index Page.</param>
        /// <param name="context">DB context for Index Page.</param>
        public IndexModel(ILogger<IndexModel> logger, LoCoMProContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Gets the list of Cantons for selected province.
        /// </summary>
        /// <param name="provincia"> Province to get the Cantons of.</param>
        /// <returns>List of Cantons for selected province.</returns>
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
            result!.Insert(0, new SelectListItem { Value = "", Text = "Elegir Cantón" });
            return new JsonResult(result);
        }

        /// <summary>
        /// Gets the list of provinces for the app.
        /// </summary>
        /// <returns>A List of provinces known to the app.</returns>
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
            result!.Insert(0, new SelectListItem { Value = "", Text = "Elegir Provincia" });
            return new JsonResult(result);

        }
    }
}