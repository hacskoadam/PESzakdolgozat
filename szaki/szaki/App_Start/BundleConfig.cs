using System.Web;
using System.Web.Optimization;

namespace szaki
{
	public class BundleConfig
	{
		// For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
		public static void RegisterBundles(BundleCollection bundles)
		{
			bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
						"~/Scripts/jquery-{version}.js",
						"~/Scripts/jquery-ui.js"));

			bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
						"~/Scripts/jquery.validate*"));

			// Use the development version of Modernizr to develop with and learn from. Then, when you're
			// ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
			bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
						"~/Scripts/modernizr-*"));

			bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
					  "~/Scripts/bootstrap.js",
					  "~/Scripts/respond.js",
					  "~/Scripts/bootstrap-select.min.js"));
			bundles.Add(new ScriptBundle("~/bundles/main").Include(
					  "~/Scripts/main.js"));

			bundles.Add(new StyleBundle("~/Content/css").Include(
					"~/Content/jquery-ui.min.css",
					  "~/Content/bootstrap.css",
					  "~/Content/Site.css",
					  "~/Content/bootstrap-select.min.css"));

			bundles.Add(new StyleBundle("~/Content/mainpagecss").Include(
					  "~/Content/mainpage.css"));
			bundles.Add(new StyleBundle("~/Content/languagescss").Include(
					  "~/Content/languages.css"));
			bundles.Add(new StyleBundle("~/Content/fontawesome2").Include(
					  "~/Content/fontawesome/font-awesome.css"));
			bundles.Add(new StyleBundle("~/Content/admincss").Include(
				"~/Content/bootstrap.min.css",
				"~/Content/metisMenu.min.css",
				"~/Content/timeline.css",
				"~/Content/morris.css",
				"~/Content/font-awesome.min.css",
				"~/Content/style.css"));
			bundles.Add(new ScriptBundle("~/Content/adminjs").Include(
				"~/Scripts/jquery.min.js",
				"~/Scripts/bootstrap.min.js",
				"~/Scripts/metisMenu.min.js",
				"~/Scripts/timeline-min.js",
				"~/Scripts/raphael-min.js",
				"~/Scripts/morris.min.js",
				"~/Scripts/admin.js"));
		}
	}
}
