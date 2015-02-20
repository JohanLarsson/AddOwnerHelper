namespace AddOwnerHelper
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml.Linq;

    public class Comment
    {
        public static readonly string Path = @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\";
        private static readonly ConcurrentDictionary<string, XDocument> Cache = new ConcurrentDictionary<string, XDocument>();

        public Comment(IReadOnlyList<string> summaries, IReadOnlyList<string> returns)
        {
            Summaries = summaries ?? new string[0];
            Returns = returns ?? new string[0];
        }

        private Comment(XElement element)
        {
            if (element == null)
            {
                Summaries = new string[0];
                Returns = new string[0];
            }
            else
            {
                Summaries = element.Descendants(XName.Get("summary")).Select(InnerXml).ToArray();
                Returns = element.Descendants(XName.Get("returns")).Select(InnerXml).ToArray();
            }
        }

        public IReadOnlyList<string> Summaries { get; private set; }

        public IReadOnlyList<string> Returns { get; private set; }

        public static Comment CreateFieldComment(FieldInfo dp)
        {
            var xDocument = XmlDocFile(dp);
            if (xDocument == null)
            {
                return new Comment(null, null);
            }
            var value = dp.DeclaringType.FullName + "." + dp.Name;
            var element = xDocument.Descendants().SingleOrDefault(x => x.HasAttributes && x.Attributes().Any(a => a.Name.LocalName == "name" && a.Value.EndsWith(value)));
            return new Comment(element);
        }

        public static Comment CreatePropertyComment(FieldInfo dp)
        {
            var xDocument = XmlDocFile(dp);
            if (xDocument == null)
            {
                return new Comment(null, null);
            }
            var value = dp.DeclaringType.FullName + "." + dp.Name.Replace("Property", "");
            var element = xDocument.Descendants().SingleOrDefault(x => x.HasAttributes && x.Attributes().Any(a => a.Name.LocalName == "name" && a.Value.EndsWith(value)));
            return new Comment(element);

        }


        private static string InnerXml(XElement e)
        {
            var inner = String.Concat(e.Nodes().Select(x => x.ToString()).ToArray());
            return inner;
        }

        private static XDocument XmlDocFile(FieldInfo dp)
        {
            try
            {
                var assembly = dp.DeclaringType.Assembly;
                var location = assembly.Location;
                var nameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(location);
                var xDocument = Cache.GetOrAdd(nameWithoutExtension, CreateXelement);
                return xDocument;
            }
            catch (Exception e)
            {
                return new XDocument();
            }
        }

        private static XDocument CreateXelement(string nameWithoutExtension)
        {
            var xmlDocFile = System.IO.Path.Combine(Path, nameWithoutExtension + ".xml");
            var xml = File.ReadAllText(xmlDocFile);
            var xDocument = XDocument.Parse(xml);
            return xDocument;
        }
    }
}