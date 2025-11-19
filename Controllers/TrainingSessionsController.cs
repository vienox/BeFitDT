using BeFit.Data;
using BeFit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BeFit.Controllers
{
    [Authorize]
    public class TrainingSessionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TrainingSessionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Challenge();
            }

            var sessions = await _context.TrainingSessions
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.StartTime)
                .ToListAsync();

            return View(sessions);
        }

        public IActionResult Create()
        {
            var now = DateTime.Now;

            return View(new TrainingSession
            {
                StartTime = now,
                EndTime = now.AddHours(1)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TrainingSession trainingSession)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Challenge();
            }

            if (trainingSession.EndTime <= trainingSession.StartTime)
            {
                ModelState.AddModelError(nameof(TrainingSession.EndTime), "Czas zakończenia musi być późniejszy niż rozpoczęcia.");
            }

            if (!ModelState.IsValid)
            {
                return View(trainingSession);
            }

            trainingSession.UserId = userId;
            _context.TrainingSessions.Add(trainingSession);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Challenge();
            }

            var session = await _context.TrainingSessions
                .Include(s => s.Exercises)
                    .ThenInclude(e => e.ExerciseType)
                .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);

            if (session == null)
            {
                return NotFound();
            }

            return View(session);
        }

        private string? GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
