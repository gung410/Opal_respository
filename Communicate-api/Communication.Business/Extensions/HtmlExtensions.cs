using Communication.Business.Models;
using Communication.Business.Models.Email;
using HandlebarsDotNet;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Communication.Business.Extensions
{
    public static class HtmlExtensions
    {
        public static string CommunicationContentToPlainText(string textPath, TemplateData templateData, string referenceKey = null)
        {
            var file = Path.Combine(textPath);
            var source = File.ReadAllText(file);
            var template = !string.IsNullOrEmpty(source) ? Handlebars.Compile(source) : null;
            if (template == null)
                throw new ArgumentNullException("template cannot be null");
            var templateDataDict = new Dictionary<string, object>(templateData.Data);
            if (referenceKey != null && templateData.ReferenceData.ContainsKey(referenceKey))
            {
                templateDataDict.Add("UserData", templateData.ReferenceData[referenceKey]);
            }

            string bodyContent = template(templateDataDict);
            var doc = new HtmlDocument();
            doc.LoadHtml(bodyContent);
            var stringBuilder = new StringBuilder();
            foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//div[@class='" + "email-container" + "']/table/tr[2]"))
            {
                var text = Regex.Replace(node.InnerText, @"[^\S\r\n]", " ").Trim();
                stringBuilder.Append(text);
            }
            var fullText = stringBuilder.ToString();
            var result = Regex.Split(fullText, "\r\n|\r|\n").Where(x => x.Length > 0);
            stringBuilder = new StringBuilder();
            foreach (var rawString in result)
            {
                var rawStringTemp = rawString.Trim();
                if (rawStringTemp.Length > 0)
                    stringBuilder.AppendLine(rawStringTemp);
            }
            return stringBuilder.ToString();
        }

        public static string HtmlToPlainText(string html, bool fullHtmlDocument = true)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var stringBuilder = new StringBuilder();
            HtmlNodeCollection nodes;
            if (!fullHtmlDocument)
            {
                nodes = doc.DocumentNode.ChildNodes;
            }
            else
            {
                nodes = doc.DocumentNode.SelectNodes("//body");
            }

            if (nodes != null)
            {
                foreach (HtmlNode node in nodes)
                {
                    var text = Regex.Replace(node.InnerText, @"[^\S\r\n]", " ").Trim();
                    stringBuilder.Append(text);
                }
                var fullText = stringBuilder.ToString();
                var result = Regex.Split(fullText, "\r\n|\r|\n").Where(x => x.Length > 0);
                stringBuilder = new StringBuilder();
                foreach (var rawString in result)
                {
                    var rawStringTemp = rawString.Trim();
                    if (rawStringTemp.Length > 0)
                        stringBuilder.AppendLine(rawStringTemp);
                }
                return stringBuilder.ToString();
            }
            return html;
        }
    }
}
