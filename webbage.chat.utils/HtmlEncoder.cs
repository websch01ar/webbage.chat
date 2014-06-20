using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace webbage.chat.utils {
    public class HtmlEncoder {

        private static Regex _tags = new Regex("<[^>]*(>|$)",
            RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled);

        private static Regex _whitelist = new Regex(@"
            ^</?(b(lockquote)?|code|d(d|t|l|el)|em|h(1|2|3)|i|kbd|li|ol|p(re)?|s(ub|up|trong|trike)?|ul)>$|
            ^<(b|h)r\s?/?>$",
            RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        private static Regex _whitelist_a = new Regex(@"
            ^<a\s
            href=""(\#\d+|(http|https?|ftp)://[-a-z0-9+&@#/%?=~_|!:,.;\(\)]+)""
            (\stitle=""[^""<>]+"")?\s?>$|
            ^</a>$",
            RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        private static Regex _whitelist_img = new Regex(@"
            ^<img\s
            src=""https?://[-a-z0-9+&@#/%?=~_|!:,.;\(\)]+""
            (\swidth=""\d{1,3}"")?
            (\sheight=""\d{1,3}"")?
            (\salt=""[^""<>]*"")?
            (\stitle=""[^""<>]*"")?
            \s?/?>$",
            RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        /// <summary>
        /// sanitize any potentially dangerous tags from the provided raw HTML input using 
        /// a whitelist based approach, leaving the "safe" HTML tags
        /// </summary>
        public static string Sanitize(string html) {
            if (String.IsNullOrEmpty(html))
                return html;

            string tagname;
            Match tag;

            // match every HTML tag in the input
            MatchCollection tags = _tags.Matches(html);
            for (int i = tags.Count - 1; i > -1; i--) {
                tag = tags[i];
                tagname = tag.Value.ToLowerInvariant();

                if (!(_whitelist.IsMatch(tagname) || _whitelist_a.IsMatch(tagname) || _whitelist_img.IsMatch(tagname))) {
                    html = html.Remove(tag.Index, tag.Length);
                    System.Diagnostics.Debug.WriteLine("tag sanitized: " + tagname);
                }
            }

            return html;
        }



        private static Regex _links = new Regex(@"(www.+|https?.+)([\s]|$)",
            RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        /// <summary>
        /// Find any urls in the code, return them as properly formatted urls that people
        /// can click on
        /// </summary>
        public static string EncodeUrl(string html) {
            if (String.IsNullOrWhiteSpace(html))
                return html;

            string linkName;
            Match link;
            
            // match every link in the input
            MatchCollection links = _links.Matches(html);
            for (int i = links.Count - 1; i > -1; i--) {
                link = links[i];
                linkName = link.Value;
                
                Uri uri = new UriBuilder(linkName).Uri;
                if (!isImage(linkName)) {                    
                    html = html.Replace(linkName, "<a href='" + uri.AbsoluteUri + "' target='_blank'>" + "</a>");
                } else {
                    html = html.Replace(linkName, "<a href='" + uri.AbsoluteUri + "' target='_blank'><img src='" + uri.AbsoluteUri + "' /></a>");
                }
            }

            return html;
        }
        private static bool isImage(string link) {
            return (
                link.EndsWith(".jpg") ||
                link.EndsWith(".gif") ||
                link.EndsWith(".png")
            );
        }
    }
}
