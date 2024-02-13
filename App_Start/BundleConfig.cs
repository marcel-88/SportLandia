using System.Web;
using System.Web.Optimization;

namespace TW_WebSite
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            /*bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));*/


            // CSS bundle
            bundles.Add(new StyleBundle("~/bundles/css").Include(
                      "~/Content/css/bootstrap.min.css",
                      "~/Content/css/templatemo.css",
                      "~/Content/css/custom.css",
                      "~/Content/css/fontawesome.min.css"));

            // JS bundle
            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                      "~/Content/js/jquery-1.11.0.min.js",
                      "~/Content/js/jquery-migrate-1.2.1.min.js",
                      "~/Content/js/bootstrap.bundle.min.js",
                      "~/Content/js/templatemo.js",
                      "~/Content/js/custom.js"));

            bundles.Add(new StyleBundle("~/bundles/bootstrap").Include(
                      "~/Content/bootstrap.min.css"));

            BundleTable.EnableOptimizations = true;

        }
    }
}
