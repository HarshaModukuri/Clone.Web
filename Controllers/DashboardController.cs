using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OktaClone.Web.Data;
using OktaClone.Web.Models;
using OktaClone.Web.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace OktaClone.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var userApplications = await _context.UserApplications
                .Where(ua => ua.UserId == user.Id)
                .Include(ua => ua.Application)
                .ToListAsync();

            var assignedApps = userApplications.Where(ua => ua.Status == "Assigned").Select(ua => ua.Application).ToList();
            var requestedApps = userApplications.Where(ua => ua.Status == "Requested").Select(ua => ua.Application).ToList();

            var allApps = await _context.Applications.ToListAsync();
            var assignedAppIds = assignedApps.Select(a => a.Id);
            var requestedAppIds = requestedApps.Select(a => a.Id);

            var unassignedApps = allApps.Where(a => !assignedAppIds.Contains(a.Id) && !requestedAppIds.Contains(a.Id)).ToList();

            var model = new DashboardViewModel
            {
                AssignedApplications = assignedApps,
                UnassignedApplications = unassignedApps
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestAccess(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var application = await _context.Applications.FindAsync(id);

            if (application == null)
            {
                return NotFound();
            }

            var userApplication = new UserApplication
            {
                UserId = user.Id,
                ApplicationId = application.Id,
                Status = "Requested"
            };

            _context.UserApplications.Add(userApplication);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
