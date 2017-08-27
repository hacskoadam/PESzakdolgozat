using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using szaki.Models;
using System.Data.Entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using PagedList;

namespace szaki.Controllers
{
	public class HomeController : Controller
	{

		TechnicianDb _db = new TechnicianDb();
		public ActionResult Index()
		{
			if (_db.MainCategories.Count() == 0)
			{
				FillDB();
			}
			ViewBag.MostUsedMainCategories = _db.MainCategories.OrderByDescending(o => o.SubCategories.Count).Take(8);
			ViewBag.CountAdds = _db.Profiles.Count();
			ViewBag.LastProfiles = _db.Profiles.Take(4);
			ViewBag.Locations = _db.Cities.OrderBy(o => o.State).GroupBy(g => g.State).AsEnumerable();

			return View();
		}
		[HttpPost]
		public JsonResult LocationAutocomplete(string str)
		{
			List<String> LocationList = new List<string>();
			foreach (var x in _db.Profiles.Where(w => w.City.Place_Name.Contains(str)))
			{
				LocationList.Add(x.City.Place_Name);
			}
			return Json(LocationList, JsonRequestBehavior.AllowGet);
		}
		public ActionResult Keres(
			int? searchMainCategory,
			string searchkeyword,
			int[] searchlocation, 
			string searchDistance,
			int[] searchCategories,
			string searchRange,
			int? Page
		)
		{
			List<int> categories = (searchCategories == null ? new List<int>() : searchCategories.ToList());
			int minRange = searchRange != null ? Convert.ToInt32(searchRange.Replace('$', ' ').Split('-')[0]) : 0;
			int maxRange = searchRange != null ? Convert.ToInt32(searchRange.Replace('$', ' ').Split('-')[1]) : 10000;
			ViewData.Add("Cities", _db.Cities.OrderBy(o => o.State).GroupBy(g=>g.State).AsEnumerable());

			ViewData.Add("searchkeyword", searchkeyword);
			ViewData.Add("searchlocation", searchlocation);
			ViewData.Add("searchDistance", searchDistance);
			ViewData.Add("searchRange", searchRange);
			ViewData.Add("searchRangeMin", minRange);
			ViewData.Add("searchRangeMax", maxRange);
			ViewData.Add("searchPage", Page);

			//get all profile, except discarded
			var model = _db.Profiles.OrderBy(o => o.RegDate).Where(w=>w.IsDiscarded == false);

			//get main category if has
			if (searchMainCategory != null)
			{
				foreach(var x in _db.MainCategories.Include(i=>i.SubCategories).Where(w=>w.Id == searchMainCategory).FirstOrDefault().SubCategories)
				{
					categories.Add(x.Id);
				}
			}
			ViewData.Add("searchCategories", categories.ToArray());
			//filter by categories if has any to filter
			if(categories.Count != 0)
			{
				var old_list = model.Include(i=>i.ProfileCategories).ToList();
				var new_list = new List<Profile>();

				foreach( var m in old_list)
				{
					foreach( var c in m.ProfileCategories)
					{
						if(categories.Contains(c.Category.Id))
						{
							new_list.Add(m);
							break;
						}
					}
				}
				model = new_list.AsQueryable();

			}

			if (!String.IsNullOrEmpty(searchkeyword))
			{
				model = model.Where(w=>w.Name.Contains(searchkeyword) || w.Description.Contains(searchkeyword) || w.Street.Contains(searchkeyword));
			}
			if(searchlocation != null)
			{
				model = model.Where(w => searchlocation.Contains(w.City.Id));
			}
			if (searchRange != null)
			{
				model = model.Where(w => w.Rate>= minRange && w.Rate<= maxRange);
			}

			ViewData.Add("RangeMax", Math.Max(maxRange, Convert.ToInt32(model.Max(m=>m.Rate))));
			ViewData.Add("RangeMin", 0);
			ViewBag.categories = _db.MainCategories.Include(i => i.SubCategories);

			int pageSize = 5;
			int pageNumber = (Page ?? 1);
			return View(model.ToPagedList(pageNumber, pageSize));
		}
		public ActionResult Reszletek(int id)
		{
			var model = _db.Profiles.Find(id);
			List<Profile> related = new List<Models.Profile>();
			foreach (var x in model.Skills)
			{
				related.AddRange(_db.Profiles.Where(w => w.Id != model.Id).Where(w => w.Skills.Contains(x)));
			}
			//Remove redundancy from list
			related = related.Distinct().OrderBy(x => x).ToList();

			ViewBag.relatedAdds = related;
			return View(model);
		}

		public ActionResult Kategoriak()
		{
			return View(_db.MainCategories.Include(i => i.SubCategories));
		}
















		public string FillDB()
		{
			

			using (StreamReader r = new StreamReader(Server.MapPath("~/Scripts/json.json")))
			{
				string json = r.ReadToEnd();
				List<City> items = JsonConvert.DeserializeObject<List<City>>(json);

				var cities = items.Distinct().GroupBy(g => g.Place_Name).ToList();
				foreach(var x in cities)
				{
					_db.Cities.Add(x.FirstOrDefault());
				}

			}

			Category auto1 = new Category() { Name = "Autófóliázás" };
			Category auto2 = new Category() { Name = "Szélvédő javítás" };
			Category auto3 = new Category() { Name = "Futómű beállítás" };
			Category auto4 = new Category() { Name = "Karosszéria lakatos" };
			Category auto5 = new Category() { Name = "Motor szervíz" };
			Category auto6 = new Category() { Name = "Egyéb" };

			MainCategory Mainauto = new MainCategory() { Name = "Autó, motor", Icon = "fa-car" };
			Mainauto.SubCategories = new List<Category>();
			Mainauto.SubCategories.Add(auto1);
			Mainauto.SubCategories.Add(auto2);
			Mainauto.SubCategories.Add(auto3);
			Mainauto.SubCategories.Add(auto4);
			Mainauto.SubCategories.Add(auto5);
			Mainauto.SubCategories.Add(auto6);

			Category house1 = new Category() { Name = "Ablakcsere" };
			Category house2 = new Category() { Name = "Ács" };
			Category house3 = new Category() { Name = "Asztalos" };
			Category house4 = new Category() { Name = "Burkoló" };
			Category house5 = new Category() { Name = "Kertész" };
			Category house6 = new Category() { Name = "Üveges" };
			Category house7 = new Category() { Name = "Bádogos" };
			Category house8 = new Category() { Name = "Egyéb" };

			MainCategory MainHouse = new MainCategory() { Name = "Ház, kert", Icon = "fa-home" };
			MainHouse.SubCategories = new List<Category>();
			MainHouse.SubCategories.Add(house1);
			MainHouse.SubCategories.Add(house2);
			MainHouse.SubCategories.Add(house3);
			MainHouse.SubCategories.Add(house4);
			MainHouse.SubCategories.Add(house5);
			MainHouse.SubCategories.Add(house6);
			MainHouse.SubCategories.Add(house7);
			MainHouse.SubCategories.Add(house8);

			Category beauty1 = new Category() { Name = "Fodrász" };
			Category beauty2 = new Category() { Name = "Henna" };
			Category beauty3 = new Category() { Name = "Manikür" };
			Category beauty4 = new Category() { Name = "Piercing" };
			Category beauty5 = new Category() { Name = "Tetoválás" };
			Category beauty6 = new Category() { Name = "Sminktetoválás" };
			Category beauty7 = new Category() { Name = "Egyéb" };

			MainCategory MainBeauty = new MainCategory() { Name = "Szépségápolás", Icon = "fa-heart" };
			MainBeauty.SubCategories = new List<Category>();
			MainBeauty.SubCategories.Add(beauty1);
			MainBeauty.SubCategories.Add(beauty2);
			MainBeauty.SubCategories.Add(beauty3);
			MainBeauty.SubCategories.Add(beauty4);
			MainBeauty.SubCategories.Add(beauty5);
			MainBeauty.SubCategories.Add(beauty6);
			MainBeauty.SubCategories.Add(beauty7);

			Category It1 = new Category() { Name = "Honlapkészítés" };
			Category It2 = new Category() { Name = "PC javítás" };
			Category It3 = new Category() { Name = "Rendszergazda" };
			Category It4 = new Category() { Name = "Egyéb" };

			MainCategory MainIt = new MainCategory() { Name = "Informatika", Icon = "fa-laptop" };
			MainIt.SubCategories = new List<Category>();
			MainIt.SubCategories.Add(It1);
			MainIt.SubCategories.Add(It2);
			MainIt.SubCategories.Add(It3);
			MainIt.SubCategories.Add(It4);

			Category kid1 = new Category() { Name = "Angoltanár" };
			Category kid2 = new Category() { Name = "Babysitter" };
			Category kid3 = new Category() { Name = "Magántanár" };
			Category kid4 = new Category() { Name = "Dada" };
			Category kid5 = new Category() { Name = "Egyéb" };

			MainCategory MainKid = new MainCategory() { Name = "Gyerek, tanulás", Icon = "fa-book" };
			MainKid.SubCategories = new List<Category>();
			MainKid.SubCategories.Add(kid1);
			MainKid.SubCategories.Add(kid2);
			MainKid.SubCategories.Add(kid3);
			MainKid.SubCategories.Add(kid4);
			MainKid.SubCategories.Add(kid5);

			Category Finance1 = new Category() { Name = "Könyvelő" };
			Category Finance2 = new Category() { Name = "Adótanácsadás" };
			Category Finance3 = new Category() { Name = "Pénzügyi tanácsadó" };
			Category Finance4 = new Category() { Name = "Egyéb" };

			MainCategory MainFinance = new MainCategory() { Name = "Pénzügy", Icon = "fa-eur" };
			MainFinance.SubCategories = new List<Category>();
			MainFinance.SubCategories.Add(Finance1);
			MainFinance.SubCategories.Add(Finance2);
			MainFinance.SubCategories.Add(Finance3);
			MainFinance.SubCategories.Add(Finance4);

			Category Hosting1 = new Category() { Name = "Szakács" };
			Category Hosting2 = new Category() { Name = "Takarító" };
			Category Hosting3 = new Category() { Name = "Pultos" };
			Category Hosting4 = new Category() { Name = "Felszolgáló" };
			Category Hosting5 = new Category() { Name = "Egyéb" };

			MainCategory MainHosting = new MainCategory() { Name = "Vendéglátás", Icon = "fa-building" };
			MainHosting.SubCategories = new List<Category>();
			MainHosting.SubCategories.Add(Hosting1);
			MainHosting.SubCategories.Add(Hosting2);
			MainHosting.SubCategories.Add(Hosting3);
			MainHosting.SubCategories.Add(Hosting4);
			MainHosting.SubCategories.Add(Hosting5);

			Category transport1 = new Category() { Name = "Futár" };
			Category transport2 = new Category() { Name = "Költöztető" };
			Category transport3 = new Category() { Name = "Sofőr" };
			Category transport4 = new Category() { Name = "Szállítmányozás" };
			Category transport5 = new Category() { Name = "Autómentés" };
			Category transport6 = new Category() { Name = "Egyéb" };

			MainCategory MainTransport = new MainCategory() { Name = "Szállítás", Icon = "fa-truck" };
			MainTransport.SubCategories = new List<Category>();
			MainTransport.SubCategories.Add(transport1);
			MainTransport.SubCategories.Add(transport2);
			MainTransport.SubCategories.Add(transport3);
			MainTransport.SubCategories.Add(transport4);
			MainTransport.SubCategories.Add(transport5);
			MainTransport.SubCategories.Add(transport6);

			Category other1 = new Category() { Name = "Gravírozás" };
			Category other2 = new Category() { Name = "Eskövői fotós" };
			Category other3 = new Category() { Name = "Varrónő" };
			Category other4 = new Category() { Name = "Ötvös" };
			Category other5 = new Category() { Name = "Egyéb" };


			MainCategory MainOther = new MainCategory() { Name = "Egyéb", Icon = "fa-ellipsis-h" };
			MainOther.SubCategories = new List<Category>();
			MainOther.SubCategories.Add(other1);
			MainOther.SubCategories.Add(other2);
			MainOther.SubCategories.Add(other3);
			MainOther.SubCategories.Add(other4);
			MainOther.SubCategories.Add(other5);

			_db.MainCategories.Add(Mainauto);
			_db.MainCategories.Add(MainHouse);
			_db.MainCategories.Add(MainBeauty);
			_db.MainCategories.Add(MainIt);
			_db.MainCategories.Add(MainKid);
			_db.MainCategories.Add(MainFinance);
			_db.MainCategories.Add(MainHosting);
			_db.MainCategories.Add(MainTransport);
			_db.MainCategories.Add(MainOther);
			_db.SaveChanges();
			List < Profile > profiles = new List<Profile>();
			profiles.Add(new Profile()
			{
				Name = "Bence Gaál - Autó szélvédő javítás",
				ProfileCategories = new List<ProfileCategory>(),
				City = _db.Cities.Where(w => w.Postal_Code == 8100).FirstOrDefault(),
				Description = "Minden Autót vállalok!",
				Email = "bence.gaal@fakemail.com",
				LastActivity = DateTime.Now,
				IsDiscarded = false,
				Phoneno = "012345678",
				NextDiscardDate = DateTime.Now,
				Distance = 30,
				IsCompany = false,
				LocationCoordX = 47.1028087,
				LocationCoordY = 17.909301899999946,
				RegDate = DateTime.Now
			});
			profiles.Add(new Profile()
			{
				Name = "Simon Pataki - Üveges",
				ProfileCategories = new List<ProfileCategory>(),
				City = _db.Cities.Where(w => w.Postal_Code == 8900).FirstOrDefault(),
				Description = "Kitűnő munka, megfeizethető áron",
				Email = "simon.pataki@fakemail.com",
				LastActivity = DateTime.Now,
				IsDiscarded = false,
				Phoneno = "0987654321",
				NextDiscardDate = DateTime.Now,
				Distance = 20,
				IsCompany = false,
				LocationCoordX = 46.8416936,
				LocationCoordY = 16.8416322,
				RegDate = DateTime.Now
			});
			profiles.Add(new Profile()
			{
				Name = "Benjámin Vastag - Informatika",
				ProfileCategories = new List<ProfileCategory>(),
				City = _db.Cities.Where(w => w.Postal_Code == 8220).FirstOrDefault(),
				Description = "Mindent megjavítok ami PC",
				Email = "benjamin.vastag@fakemail.com",
				LastActivity = DateTime.Now,
				IsDiscarded = false,
				Phoneno = "124356879",
				NextDiscardDate = DateTime.Now,
				Distance = 25,
				IsCompany = false,
				LocationCoordX = 47.031703,
				LocationCoordY = 18.0087,
				RegDate = DateTime.Now
			});
			profiles.Add(new Profile()
			{
				Name = "Béla Vörös - szállító",
				ProfileCategories = new List<ProfileCategory>(),
				City = _db.Cities.Where(w => w.Postal_Code == 8900).FirstOrDefault(),
				Description = "Mindent elszálít, bárhonnan bárhova",
				Email = "bela.voros@fakemail.com",
				LastActivity = DateTime.Now,
				IsDiscarded = false,
				Phoneno = "012222222",
				NextDiscardDate = DateTime.Now,
				Distance = 90,
				IsCompany = false,
				LocationCoordX = 46.8416936,
				LocationCoordY = 16.8416322,
				RegDate = DateTime.Now
			});
			profiles.Add(new Profile()
			{
				Name = "Dániel Kelemen - ékszeres",
				ProfileCategories = new List<ProfileCategory>(),
				City = _db.Cities.Where(w => w.Postal_Code == 8100).FirstOrDefault(),
				Description = "Aranyékszerek készítése",
				Email = "daniel.kelemens@fakemail.com",
				LastActivity = DateTime.Now,
				IsDiscarded = false,
				Phoneno = "033333333",
				NextDiscardDate = DateTime.Now,
				Distance = 5,
				IsCompany = false,
				LocationCoordX = 47.1028087,
				LocationCoordY = 17.909301899999946,
				RegDate = DateTime.Now
			});
			profiles.Add(new Profile()
			{
				Name = "Richárd Rigó - Tetőfedő",
				ProfileCategories = new List<ProfileCategory>(),
				City = _db.Cities.Where(w => w.Postal_Code == 8800).FirstOrDefault(),
				Description = "Gyors pontos, saját anaggal",
				Email = "richard.rigo@fakemail.com",
				LastActivity = DateTime.Now,
				IsDiscarded = false,
				Phoneno = "0987654321",
				NextDiscardDate = DateTime.Now,
				Distance = 15,
				IsCompany = false,
				LocationCoordX = 46.4590218,
				LocationCoordY = 16.9896796,
				RegDate = DateTime.Now
			});
			profiles.Add(new Profile()
			{
				Name = "Norbert Kis - Fotós",
				ProfileCategories = new List<ProfileCategory>(),
				City = _db.Cities.Where(w => w.Postal_Code == 8100).FirstOrDefault(),
				Description = "Igényes képek készítése. Nagyfokú együttműködés",
				Email = "norber.kis@fakemail.com",
				LastActivity = DateTime.Now,
				IsDiscarded = false,
				Phoneno = "05555555",
				NextDiscardDate = DateTime.Now,
				Distance = 50,
				IsCompany = false,
				LocationCoordX = 47.1028087,
				LocationCoordY = 17.909301899999946,
				RegDate = DateTime.Now
			});
			profiles.Add(new Profile()
			{
				Name = "Aliz Sass - Műköröm",
				ProfileCategories = new List<ProfileCategory>(),
				City = _db.Cities.Where(w => w.Postal_Code == 8000).FirstOrDefault(),
				Description = "Gyönyörű körmök hihetetlen gyorsan",
				Email = "aliz.sass@fakemail.com",
				LastActivity = DateTime.Now,
				IsDiscarded = false,
				Phoneno = "666-666",
				NextDiscardDate = DateTime.Now,
				Distance = 1,
				IsCompany = false,
				LocationCoordX = 47.186026,
				LocationCoordY = 18.422136,
				RegDate = DateTime.Now
			});
			profiles.Add(new Profile()
			{
				Name = "Kis Martina - Műköröm",
				ProfileCategories = new List<ProfileCategory>(),
				City = _db.Cities.Where(w => w.Postal_Code == 8100).FirstOrDefault(),
				Description = "Igényes manikűr készítése és sminktetoválás",
				Email = "kis.martina@fakemail.com",
				LastActivity = DateTime.Now,
				IsDiscarded = false,
				Phoneno = "123-456",
				NextDiscardDate = DateTime.Now,
				Distance = 0,
				IsCompany = false,
				LocationCoordX = 47.1028087,
				LocationCoordY = 17.909301899999946,
				RegDate = DateTime.Now
			});
			profiles.Add(new Profile()
			{
				Name = "Bíró Kinga - Magántanár",
				ProfileCategories = new List<ProfileCategory>(),
				City = _db.Cities.Where(w => w.Postal_Code == 8900).FirstOrDefault(),
				Description = "Gyerekfelügyelet és magánórák ",
				Email = "biro.kinga@fakemail.com",
				LastActivity = DateTime.Now,
				IsDiscarded = false,
				Phoneno = "987-654",
				NextDiscardDate = DateTime.Now,
				Distance = 20,
				IsCompany = false,
				LocationCoordX = 46.8416936,
				LocationCoordY = 16.8416322,
				RegDate = DateTime.Now
			});
			_db.Profiles.AddRange(profiles);
			_db.SaveChanges();

			_db.ProfileCategories.Add(new ProfileCategory() { Category = auto1, Profile = profiles[0] });
			_db.ProfileCategories.Add(new ProfileCategory() { Category = auto2, Profile = profiles[0] });
			_db.SaveChanges();

			_db.ProfileCategories.Add(new ProfileCategory() { Category = house1, Profile = profiles[1] });
			_db.ProfileCategories.Add(new ProfileCategory() { Category = house6, Profile = profiles[1] });
			_db.SaveChanges();

			_db.ProfileCategories.Add(new ProfileCategory() { Category = It2, Profile = profiles[2] });
			_db.SaveChanges();

			_db.ProfileCategories.Add(new ProfileCategory() { Category = transport1, Profile = profiles[3] });
			_db.ProfileCategories.Add(new ProfileCategory() { Category = transport2, Profile = profiles[3] });
			_db.SaveChanges();

			_db.ProfileCategories.Add(new ProfileCategory() { Category = other1, Profile = profiles[4] });
			_db.ProfileCategories.Add(new ProfileCategory() { Category = other4, Profile = profiles[4] });
			_db.SaveChanges();

			_db.ProfileCategories.Add(new ProfileCategory() { Category = house2, Profile = profiles[5] });
			_db.ProfileCategories.Add(new ProfileCategory() { Category = house3, Profile = profiles[5] });
			_db.SaveChanges();

			_db.ProfileCategories.Add(new ProfileCategory() { Category = other2, Profile = profiles[6] });
			_db.SaveChanges();


			_db.ProfileCategories.Add(new ProfileCategory() { Category = beauty3, Profile = profiles[7] });
			_db.SaveChanges();

			_db.ProfileCategories.Add(new ProfileCategory() { Category = beauty3, Profile = profiles[8] });
			_db.SaveChanges();

			_db.ProfileCategories.Add(new ProfileCategory() { Category = kid1, Profile = profiles[9] });
			_db.ProfileCategories.Add(new ProfileCategory() { Category = kid2, Profile = profiles[9] });

			_db.SaveChanges();

			return "Sikeres Feltöltés";
		}
	}
}