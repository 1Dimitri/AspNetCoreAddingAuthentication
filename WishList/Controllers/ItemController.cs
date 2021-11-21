using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WishList.Data;
using WishList.Models;

namespace WishList.Controllers
{
    [Authorize]
    public class ItemController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ItemController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {

            System.Threading.Tasks.Task<ApplicationUser> user = _userManager.GetUserAsync(HttpContext.User);
            var model = _context.Items.Where(i => i.User == user.Result).ToList();

            return View("Index", model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View("Create");
        }

        [HttpPost]
        public IActionResult Create(Models.Item item)
        {
            var user = _userManager.GetUserAsync(HttpContext.User);
            item.User = user.Result; 
            _context.Items.Add(item);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var item = _context.Items.FirstOrDefault(e => e.Id == id);

            var user = _userManager.GetUserAsync(HttpContext.User).Result;

            if (user == item.User)
            {
                _context.Items.Remove(item);
                _context.SaveChanges();
                return RedirectToAction("Index");
            } else
            {
                return Unauthorized();
            }
        }
    }
}
