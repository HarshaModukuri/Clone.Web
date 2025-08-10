using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OktaClone.Web.Data;
using System.Linq;
using System.Threading.Tasks;

namespace OktaClone.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var requests = await _context.UserApplications
                .Where(ua => ua.Status == "Requested")
                .Include(ua => ua.User)
                .Include(ua => ua.Application)
                .ToListAsync();

            return View(requests);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(string userId, int applicationId)
        {
            var userApplication = await _context.UserApplications
                .FirstOrDefaultAsync(ua => ua.UserId == userId && ua.ApplicationId == applicationId);

            if (userApplication == null)
            {
                return NotFound();
            }

            userApplication.Status = "Assigned";
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
