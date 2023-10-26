using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp1.Data;
using WebApp1.Models;

namespace WebApp1.Pages_Events
{
    public class CreateModel : PageModel
    {
        private readonly WebApp1.Data.ApplicationDbContext _context;

        public CreateModel(WebApp1.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return Page();
        }

        [BindProperty]
        public Event Event { get; set; } = default!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Event.CreatedAt = DateTime.SpecifyKind(Event.CreatedAt, DateTimeKind.Utc); 
            Event.FinishesAt = DateTime.SpecifyKind(Event.FinishesAt, DateTimeKind.Utc); 
            Event.StartedAt = DateTime.SpecifyKind(Event.StartedAt, DateTimeKind.Utc); 

            _context.Events.Add(Event);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
