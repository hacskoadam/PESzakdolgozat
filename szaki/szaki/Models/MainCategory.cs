using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace szaki.Models
{
	public class MainCategory
	{
		[Key]
		public int Id { get; set; }
		public String Name { get; set; }
		public virtual ICollection<Category> SubCategories { get; set; }
		public string Icon { get; set; }
	}
}