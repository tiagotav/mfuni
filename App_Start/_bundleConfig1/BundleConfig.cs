using System.Web;
using System.Web.Optimization;

namespace mfuni
{
    public class BundleConfig
    {
        // Para obter mais informações sobre o agrupamento, visite https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new Bundle("~/Script/Bundles").Include(
                    "~/dist/main.*",
                    "~/dist/polyfills.*",
                    "~/dist/runtime.*"
                ));

            bundles.Add(new StyleBundle("~/Content/Styles").Include(
                    "~/dist/styles.*"
                ));
        }
    }
}
