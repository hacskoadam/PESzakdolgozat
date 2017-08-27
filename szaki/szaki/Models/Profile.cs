using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace szaki.Models
{
	public class Profile
	{
		[Key]
		public int Id { get; set; }
		public string Name { get; set; }
		public City City { get; set; }
		//[Display(Name = "Helyszín")]
		public string Street { get; set; }
		public double LocationCoordX { get; set; }
		public double LocationCoordY { get; set; }
		public int? Distance { get; set; }
		public Boolean IsCompany { get; set; }
		public string ProfilePicture { get; set; }
		[DataType(DataType.MultilineText)]
		public string Description { get; set; }
		public string Phoneno { get; set; }
		public string Email { get; set; }
		public float? Rate { get; set; }
		public DateTime LastActivity{ get; set; }
		public Boolean IsDiscarded { get; set; }
		public DateTime NextDiscardDate { get; set; }
		public virtual ICollection<Feedback> Feedbacks { get; set; }
		public virtual ICollection<Skill> Skills { get; set; }
		public virtual ICollection<Affirm> Affirms { get; set; }
		public DateTime RegDate { get; set; }
		public string CustomURL { get; set; }
		public virtual ICollection<ProfileCategory> ProfileCategories { get; set; }
	}
}