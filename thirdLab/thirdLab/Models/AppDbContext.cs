using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace thirdLab.Models
{
	public class AppDbContext : DbContext
	{
		public DbSet<Student> Students { get; set; }

		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
	}
}
