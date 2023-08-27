using System.Text.RegularExpressions;

namespace HTMLParser
{
    //public class HtmlDocument
    //{
    //    public HtmlDocument(string str)
    //    {

    //    }
    //}

    public class HtmlNode
    {
        const string ptr = @"<!--.*?-->";
        //const string pat = @"<(.+?)(\/(.?)>)";
        string? _alltext;
        public HtmlNode(string str)
        {

            _alltext = Regex.Replace(str, ptr, "", RegexOptions.Singleline);

        }
        public HtmlNode this[string type, int f_l = 0]
        {
            get
            {
                return (GetChild(type, f_l));
            }
        }
        public HtmlNode GetChild(string type, int f_l = 0)
        {
            string ptr = $"<{type}((.*?)(/>)|(.*?\n*?)*?(</{type}([.\n]*?)>))";
            MatchCollection collection = Regex.Matches(_alltext, ptr, RegexOptions.Multiline);

            return new HtmlNode(f_l == 0 ? collection.First().Value : collection.Last().Value);
        }
        public List<HtmlNode> GetChildren(string type)
        {
            string ptr = $"<{type}((.*?)(/>)|(.*?\n*?)*?(</{type}([.\n]*?)>))";
            MatchCollection collection = Regex.Matches(_alltext, ptr, RegexOptions.Multiline);
            List<HtmlNode> Children = new List<HtmlNode>();
            foreach (Match m in collection)
            {
                if (m.Success)
                {
                    Children.Add(new HtmlNode(m.Value));
                }
            }
            return Children;
        }

        public string GetProperty(string p)
        {
            string ptr = $"(?<={p}.*?=.*?\").*?(?=\"( |>))";
            Match r = Regex.Match(_alltext, ptr, RegexOptions.Multiline);
            //string rr = Regex.Replace(r.Value, $"\\b{p}(.*?)=(.*?)\"(?=(.*?))\"", "");
            return r.Value;
        }
        public string? GetString()
        {
            return _alltext;
        }


        public static HtmlNode Parse(string str)
        {
            HtmlNode htmlNode = new HtmlNode(str);
            return htmlNode;
        }
    }
}