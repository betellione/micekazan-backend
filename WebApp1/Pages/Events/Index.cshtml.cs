using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp1.Data;
using WebApp1.Models;

namespace WebApp1.Pages_Events
{
    public class IndexModel : PageModel
    {
        private readonly WebApp1.Data.ApplicationDbContext _context;

        public IndexModel(WebApp1.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Event> Event { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Event = await _context.Events
                .Include(x => x.User).ToListAsync();
        }
    }
}
