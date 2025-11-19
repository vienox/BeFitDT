using BeFit.Data;
using BeFit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeFit.Controllers
{
    [Authorize]
    public class ExerciseTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExerciseTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var types = await _context.ExerciseTypes.AsNoTracking()
                .OrderBy(t => t.Name)
                .ToListAsync();

            return View(types);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View(new ExerciseType());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(ExerciseType exerciseType)
        {
            if (!ModelState.IsValid)
            {
                return View(exerciseType);
            }

            _context.ExerciseTypes.Add(exerciseType);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
