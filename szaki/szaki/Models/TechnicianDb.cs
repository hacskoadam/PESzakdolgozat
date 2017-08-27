using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace szaki.Models
{
	public class TechnicianDb: DbContext
	{
		public DbSet<Profile> Profiles { get; set; }
		public DbSet<Affirm> Affirms { get; set; }
		public DbSet<MainCategory> MainCategories { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<Feedback> Fedbacks { get; set; }
		public DbSet<Report> Reports { get; set; }
		public DbSet<Skill> Skills { get; set; }
		public DbSet<City> Cities { get; set; }
		public DbSet<ProfileCategory> ProfileCategories { get; set; }


		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<ProfileCategory>()
				.HasKey(c => new { c.ProfileId, c.CategoryId });

			modelBuilder.Entity<Profile>()
				.HasMany(c => c.ProfileCategories)
				.WithRequired()
				.HasForeignKey(c => c.ProfileId);

			modelBuilder.Entity<Category>()
				.HasMany(c => c.ProfileCategories)
				.WithRequired()
				.HasForeignKey(c => c.CategoryId);
		}
	}
}