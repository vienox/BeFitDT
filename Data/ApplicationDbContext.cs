using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BeFit.Data;

using BeFit.Models;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
{
	public DbSet<ExerciseType> ExerciseTypes { get; set; }
	public DbSet<TrainingSession> TrainingSessions { get; set; }
	public DbSet<SessionExercise> SessionExercises { get; set; }
}
