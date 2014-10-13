using System.Web.Optimization;

namespace webbage.chat.web {
    public class BundleConfig {
        public static void RegisterBundles(BundleCollection bundles) {
            bundles.Add(new ScriptBundle("~/scripts/js")
                .Include("~/scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/scripts/angular")
                .Include("~/scripts/angular.js")
                .Include("~/scripts/angular-animate.js")
                .Include("~/scripts/angular-route.js")
                .Include("~/scripts/angular-sanitize.js")
                .IncludeDirectory("~/scripts/angular-ui", "*.js", true)
                .IncludeDirectory("~/js", "*.js", true)
                .IncludeDirectory("~/factories", "*.js", true)
                .IncludeDirectory("~/directives", "*.js", true)
                .IncludeDirectory("~/ngControllers", "*.js", true));

            bundles.Add(new StyleBundle("~/styles/bootstrap")
                .Include("~/content/*.css"));

            bundles.Add(new StyleBundle("~/styles/site")
                .Include("~/css/*.css"));
        }
    }
}