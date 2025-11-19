using BeFit.Data;
using BeFit.Models;
using BeFit.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BeFit.Controllers
{
    [Authorize]
    public class SessionExercisesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SessionExercisesController(ApplicationDbContext context)
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

            var exercises = await _context.SessionExercises
                .Include(e => e.TrainingSession)
                .Include(e => e.ExerciseType)
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.TrainingSession!.StartTime)
                .ToListAsync();

            return View(exercises);
        }

        public async Task<IActionResult> Create(int? sessionId)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Challenge();
            }

            var viewModel = new SessionExerciseFormViewModel
            {
                SessionExercise = new SessionExercise
                {
                    TrainingSessionId = sessionId ?? 0
                },
                Sessions = await GetSessionsSelectList(userId),
                ExerciseTypes = await GetExerciseTypesSelectList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SessionExerciseFormViewModel viewModel)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Challenge();
            }

            if (!await _context.TrainingSessions.AnyAsync(s => s.Id == viewModel.SessionExercise.TrainingSessionId && s.UserId == userId))
            {
                ModelState.AddModelError(nameof(SessionExercise.TrainingSessionId), "Nieprawidłowa sesja treningowa.");
            }

            if (!await _context.ExerciseTypes.AnyAsync(e => e.Id == viewModel.SessionExercise.ExerciseTypeId))
            {
                ModelState.AddModelError(nameof(SessionExercise.ExerciseTypeId), "Nieprawidłowy typ ćwiczenia.");
            }

            if (!ModelState.IsValid)
            {
                viewModel.Sessions = await GetSessionsSelectList(userId);
                viewModel.ExerciseTypes = await GetExerciseTypesSelectList();
                return View(viewModel);
            }

            viewModel.SessionExercise.UserId = userId;
            _context.SessionExercises.Add(viewModel.SessionExercise);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task<IEnumerable<SelectListItem>> GetSessionsSelectList(string userId)
        {
            return await _context.TrainingSessions
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.StartTime)
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = $"{s.StartTime:g} - {s.EndTime:g}"
                })
                .ToListAsync();
        }

        private async Task<IEnumerable<SelectListItem>> GetExerciseTypesSelectList()
        {
            return await _context.ExerciseTypes
                .OrderBy(e => e.Name)
                .Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.Name
                })
                .ToListAsync();
        }

        private string? GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
