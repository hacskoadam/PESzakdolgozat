using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;	
using System.Web.Mvc;
using szaki.Models;
using System.Data.Entity;
using System.Web.Security;

namespace szaki.Controllers
{
    public class ProfileController : Controller
    {
		TechnicianDb _db = new TechnicianDb();
        // GET: Profile
        public ActionResult Index()
        {
            return View();
        }
		public ActionResult Create()
		{
			ViewBag.categories = _db.MainCategories.Include(i => i.SubCategories);
			ViewBag.Locations = _db.Cities.OrderBy(o => o.State).GroupBy(g => g.State).AsEnumerable();
			return View();
		}
		[HttpPost]
		public ActionResult Create(Profile model, string [] selectcategories, HttpPostedFileBase ProfileImage, int? location)
		{
			if (ModelState.IsValid)
			{

				if(User.Identity.IsAuthenticated){
					model.Email = Request.QueryString["username"];
				}
				if (location != null)
				{
					model.City = _db.Cities.Find(location);
				}
				model.RegDate = DateTime.Now;
				model.ProfileCategories = new List<ProfileCategory>();
				model.LastActivity = DateTime.Now;
				model.NextDiscardDate = DateTime.Now.AddMonths(6);
				_db.Profiles.Add(model);
				_db.SaveChanges();

				foreach (var x in selectcategories)
				{
					var pc = new ProfileCategory();
					pc.Profile = model;
					pc.Category = _db.Categories.Find(Convert.ToInt32(x));
					_db.ProfileCategories.Add(pc);
				}

				if (ProfileImage != null)
				{
					var id = _db.Entry(model);
					string pic = "profile" + model.Id + System.IO.Path.GetExtension(ProfileImage.FileName);
						/*System.IO.Path.GetFileName(ProfileImage.FileName);*/
					string path = System.IO.Path.Combine(
										   Server.MapPath("~/Uploads/profile/"), pic);
					if (!System.IO.Directory.Exists(Server.MapPath("~/Uploads/profile/")))
					{
						System.IO.Directory.CreateDirectory(Server.MapPath("~/Uploads/profile/"));
					}
					model.ProfilePicture = pic;
					// file is uploaded
					ProfileImage.SaveAs(path);
				}
				_db.SaveChanges();

				return RedirectToAction("Index", "Home");
			}
			return View(model);
		}
	}
}