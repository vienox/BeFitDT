using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BeFit.Models
{
    public class SessionExercise
    {
        public int Id { get; set; }

        [Required]
        public int TrainingSessionId { get; set; }
        public TrainingSession? TrainingSession { get; set; }

        [Required]
        public int ExerciseTypeId { get; set; }
        public ExerciseType? ExerciseType { get; set; }

        [Required]
        public float Weight { get; set; }

        [Required]
        public int Sets { get; set; }

        [Required]
        public int Repetitions { get; set; }

        public string UserId { get; set; } = string.Empty;
        public IdentityUser? User { get; set; }
    }
}
