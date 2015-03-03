using Dimeng.LinkToMicrocad.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;

namespace Dimeng.LinkToMicrocad.Web.HtmlHelpers
{
    public static class PagingHelpers
    {
        public static MvcHtmlString PageLinks(this HtmlHelper html,
                                              PagingInfo pagingInfo,
                                              Func<int, string> pageUrl)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 1; i <= pagingInfo.TotalPages; i++)
            {
                string header = "<li>";
                if (i == pagingInfo.CurrentPage)
                {
                    header = @"<li class=""active "">";
                }

                result.Append(header);
                TagBuilder tag = new TagBuilder("a");
                tag.MergeAttribute("href", pageUrl(i));
                tag.InnerHtml = i.ToString();

                result.Append(tag.ToString());
                result.Append("</li>");
            }

            return MvcHtmlString.Create(result.ToString());
        }
    }
}