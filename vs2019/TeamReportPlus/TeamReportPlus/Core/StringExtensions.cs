using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamReportPlus.Core
{
    /// <summary>
    /// Static class which is defined extension methods.
    /// </summary>
    public static class StringExtensions
    {

        /// <summary>
        /// Escape multiline text for Tsv file format.
        /// </summary>
        /// <param name="me">The instance of the type which is added this extension method.</param>
        /// <returns>
        /// Returns escaped text.
        /// </returns>
        public static string EscapeMultilineForTsv(this String me)
        {
            if (me == null) return null;
            me = me.Replace("\t", "    ");

            if (me.Contains("\n")){
                return "\"" + me.Replace("\"","\"\"").Replace("\r\n", "\n") + "\"";
            }else{
                return me;
            } // end if
        } // end function

        public static string ConvertHtmlToText(this string me)
        {
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(me);

            doc.DocumentNode?.SelectNodes("//br")?.ForEach((br) =>
            {
                br.InnerHtml += "\n";
            });
            doc.DocumentNode?.SelectNodes("//p")?.ForEach((p) =>
            {
                p.InnerHtml += "\n";
            });

            return HtmlAgilityPack.HtmlEntity.DeEntitize(doc.DocumentNode.InnerText);
        } // end function

    } // end class
} // end namespace