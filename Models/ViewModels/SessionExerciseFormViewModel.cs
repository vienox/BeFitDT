using BeFit.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BeFit.Models.ViewModels
{
    public class SessionExerciseFormViewModel
    {
        public SessionExercise SessionExercise { get; set; } = new SessionExercise();
        public IEnumerable<SelectListItem> Sessions { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> ExerciseTypes { get; set; } = new List<SelectListItem>();
    }
}
