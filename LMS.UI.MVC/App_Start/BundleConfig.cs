using System.Web.Optimization;

namespace LMS.UI.MVC
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/jquery.min.js",
                      "~/Scripts/bootstrap.bundle.min.js",
                      "~/Scripts/respond.js",
                      //"~/Scripts/jquery.min.js",
                      "~/Scripts/isotope.pkgd.js",
                      "~/Scripts/templatemo.js",
                      "~/Scripts/DataTables/jquery.dataTables.min.js",
                      "~/Scripts/custom.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/css/bootstrap.min.css",
                      "~/Content/DataTables/css/jquery.dataTables.min.css",
                      "~/Content/css/boxicon.min.css",
                      "~/Content/css/templatemo.css",
                      "~/Content/css/custom.css"));
        }
    }
}
