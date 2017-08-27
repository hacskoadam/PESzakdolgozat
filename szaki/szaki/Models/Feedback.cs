using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace szaki.Models
{
	public class Feedback
	{
		[Key]
		public int Id { get; set; }
		public String Comment { get; set; }
		public int Val { get; set; }
		public string FingerPrint { get; set; }
		public string UserId { get; set; }
	}
}