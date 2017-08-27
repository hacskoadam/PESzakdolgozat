using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace szaki.Models
{
	public class Affirm
	{
		[Key]
		public int Id { get; set; }
		public String WhoId { get; set; }
		public String WhoFPrint { get; set; }
		public String UserId { get; set; }
		public Skill SkillToAffirm { get; set; } 
	}
}