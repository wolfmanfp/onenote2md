using System;
using System.Linq;
using HtmlAgilityPack;

namespace Onenote2md.Shared
{
    public static class StringHelper
    {
        
        public static string Sanitize(String str)
        {
            str = str.Replace("^J", ",");
            str = str.Replace("\n", " ");
            return str;
        }

        public static string ReplaceSlash(String str)
        {
            return str.Replace('/', '\uFF0F');
        }

        public static string ReplaceMultiline(string source)
        {
            return source.Replace("\n", " ");
        }
        
        public static string ConvertSpanToMd(string source)
        {
            if (!source.Contains("<span"))
            {
                return source;
            }

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(source);

            foreach (var spanNode in htmlDoc.DocumentNode.Descendants("span").ToList())
            {
                var newTextNode = htmlDoc.CreateTextNode(ProcessSpanStyles(spanNode));
                newTextNode.Attributes.Remove("lang");
                spanNode.ParentNode.ReplaceChild(newTextNode, spanNode);
            }

            return htmlDoc.DocumentNode.OuterHtml;
        }
        
        private static string ProcessSpanStyles(HtmlNode spanNode)
        {
            var text = spanNode.InnerText;
        
            if (!spanNode.Attributes.Contains("style"))
            {
                return text;
            }

            var styles = spanNode.GetAttributeValue("style", "").Replace(": ", ":").Split(';');
            foreach (var style in styles)
            {
                switch (style)
                {
                    case "text-decoration:line-through": text = "~~" + text + "~~"; break;
                    case "text-decoration:underline": text = "<u>" + text + "</u>"; break;
                    case "font-style:italic;": text = "*" + text + "*"; break;
                    case "font-weight:bold": text = "**" + text + "**"; break;
                }
            }

            return text;
        }
    }
}