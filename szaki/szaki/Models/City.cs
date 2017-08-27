using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace szaki.Models
{
	public class City
	{
		[Key]
		public int Id { get; set; }
		public int Postal_Code { get; set; }
		public String Place_Name { get; set; }
		public string State { get; set; }
		public string State_Abbreviation { get; set; }
		public float Latitude { get; set; }
		public float Longitude { get; set; }
	}
}