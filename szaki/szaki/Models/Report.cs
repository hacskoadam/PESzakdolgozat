using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace szaki.Models
{
	public enum Reason
	{
		Insulting,
		Fake,
		Abandoned
	}

	public class Report
	{
		[Key]
		public int Id { get; set; }
		public Reason Cause { get; set; }
		public string UserId { get; set; }
		public string Fingerprint { get; set; }

	}
}