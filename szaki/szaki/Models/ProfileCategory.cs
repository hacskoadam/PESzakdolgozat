using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace szaki.Models
{
	public class ProfileCategory
	{
		public int ProfileId { get; set; }
		public int CategoryId { get; set; }
		public virtual Profile Profile { get; set; }
		public virtual Category Category { get; set; }
	}
}