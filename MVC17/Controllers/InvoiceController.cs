using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC17.Data;
using MVC17.Helpers.Constants.Sessions;
using MVC17.Models;

namespace MVC17.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly Dbmvc05Context _context;
        private readonly IMapper _mapper;

        public InvoiceController(Dbmvc05Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index()
        {
            var invoices = await _context.VwInvoices
                .OrderByDescending(iv => iv.OrderDate)
                .ToListAsync();
            return View(invoices);
        }
        public async Task<IActionResult> Details(int id)
        {
            var invoice = await _context.VwInvoices
                .FirstOrDefaultAsync(iv => iv.InvoiceId == id);
            if (invoice == null)
            {
                return NotFound();
            }

            ViewBag.InvoiceDetails = await _context.VwInvoiceDetails
                .Where(ivd => ivd.InvoiceId == id)
                .ToListAsync();

            return View(invoice);
        }

        
    }
}
