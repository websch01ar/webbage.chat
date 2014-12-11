using System.Web.Optimization;

namespace webbage.chat.web {
    public class BundleConfig {
        public static void RegisterBundles(BundleCollection bundles) {
            bundles.Add(new ScriptBundle("~/scripts/lib")
                .IncludeDirectory("~/assets/libs", "*.js", true)
                .IncludeDirectory("~/assets/js", "*.js", true));

            bundles.Add(new ScriptBundle("~/scripts/angular")
                .IncludeDirectory("~/app/webbage.app", "*.js", true)
                .IncludeDirectory("~/app/webbage.chat", "*.js", true)
                .IncludeDirectory("~/app/webbage.directives", "*.js", true)
                .IncludeDirectory("~/app/webbage.services", "*.js", true));

            bundles.Add(new StyleBundle("~/styles/site")
                .IncludeDirectory("~/assets/css", "*.css", true));
        }
    }
}