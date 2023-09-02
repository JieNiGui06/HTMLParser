using System.Text.RegularExpressions;
using JTypes;

namespace HTMLParser
{
    //public class HtmlDocument
    //{
    //    public HtmlDocument(string str)
    //    {

    //    }
    //}
    [Flags]
    public enum UniqueTags
    {
        _id = 1,
        _name = 2,
        _class = 4,
    }
    public class HtmlNodeBase
    {
        protected static readonly string[] OpenableTags = new string[] {
            "area" , "base", "br", "col", "command", "embed", "hr",
            "img","input","keygen","link","meta","param","source","track","wbr"
        };

        public ReversibleDictionary<string, string> AllValueCollection { get; protected set; } = new ReversibleDictionary<string, string>();
        public List<string> annotations { get; protected set; } = new List<string>();
        public HtmlNode? InnerContent { get; set; }
        //const string pat = @"<(.+?)(\/(.?)>)";
        public string? AllText { get; protected set; }

        protected HtmlNodeBase(string str, ReversibleDictionary<string, string> vs, List<string> annos)
        {
            AllText = str;
            AllValueCollection = vs;
            annotations = annos;
        }
        //用户初始化入口
        public HtmlNodeBase(string str)
        {

            Regex valuereg = new Regex(@"(?<!<!--([^""-]|-(?!->))*)\b(?<Key>\w+?)\s*?=\s*?""(?<Value>(.|\n)*?)""", RegexOptions.Multiline | RegexOptions.ExplicitCapture);
            //将字符串内容删除并存入字典。
            AllText = valuereg.Replace(str, (v) =>
            {
                string key = v.Groups["Key"].Value;
                string ind = AllValueCollection.Count.ToString();
                //if (AllValueCollection.ContainsKey(key))
                //{
                //    key += AllValueCollection.Count.ToString();
                //}
                string? id1 = AllValueCollection.Add_out1(ind, v.Groups["Value"].Value);
                if (id1 != null)
                {
                    ind = id1;
                }
                else
                {

                }
                return $"{key}=\"{ind}\"";
            });

            Regex annoreg = new Regex(@"<!--(?<anno>(.|\n)*?)-->", RegexOptions.Multiline | RegexOptions.ExplicitCapture);
            //提取注释并删除。 
            AllText = annoreg.Replace(AllText, (v) =>
            {
                annotations.Add(v.Groups["anno"].Value);
                return "";
            });


            //_alltext = Regex.Replace(str, ptr, "", RegexOptions.Singleline | RegexOptions.ExplicitCapture);
            //string string_getstring = @""
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uns">Please by order:id name class,if no some data ,don't input.</param>
        /// <param name="ut"></param>
        /// <returns></returns>
        public HtmlNode GetChildByUniqueTags(object uns, UniqueTags ut = UniqueTags._id)
        {
            var uts = typeof(UniqueTags).GetEnumNames();

            if (uns is string)
            {
                string unstr = (string)uns;
                foreach (var t in uts)
                {
                    UniqueTags unique = (UniqueTags)Enum.Parse(typeof(UniqueTags), t);

                    if (ut.HasFlag(unique))
                    {
                        string key = t.Substring(1);
                        string value = AllValueCollection.Find2(unstr).Value.Item1;
                        string pattern = $"<((?<opab>{string.Join('|', OpenableTags)})|(?<clab>\\w+))[^/<>]*(?(opab)|((?<hc>/)|[^/]))\\s*(?<={key}=\"{value}\"[^<]*)>(?(opab)|(?(hc)|(.|\n)*?</\\k<clab>\\s*>))";
                        Match m = Regex.Match(AllText, pattern);
                        return new HtmlNode(m.Value, AllValueCollection, annotations);
                    }
                }
            }
            else
            {
                string ucl = string.Empty;
                string[] unstrs = (string[])uns;
                int ti = 0;
                for (int i = 0; i < uts.Length; i++)
                {
                    UniqueTags unique = (UniqueTags)Enum.Parse(typeof(UniqueTags), uts[i]);
                    if (ut.HasFlag(unique))
                    {
                        string key = uts[i].Substring(1);
                        string value = AllValueCollection.Find2(unstrs[ti]).Value.Item1;
                        ucl += $"(?<={key}=\"{value}\"[^<]*)";

                        ti++;
                    }
                }
                //ucl = ucl.Substring(1);
                string pattern = $"<(?<opab>({string.Join('|', OpenableTags)})|(?<clab>\\w+))([^<>/]*)(?(opab)|((?<hc>/)|[^/]))\\s*{ucl}>(?(opab)|(?(hc)|(.|\n)*?</\\k<clab>\\s*>))";
                Match m = Regex.Match(AllText, pattern, RegexOptions.Multiline | RegexOptions.ExplicitCapture);
                return new HtmlNode(m.Value, AllValueCollection, annotations);
            }
            throw new Exception("uncorrect data.");
        }

        public HtmlNode GetChildByType(string type, int f_l = 0)
        {
            Regex regex;
            if (OpenableTags.Contains(type.Trim()))
            {
                regex = new Regex(@"<" + type + @"(.|\n)*?>", RegexOptions.Multiline | RegexOptions.ExplicitCapture);
            }
            else
            {
                regex = new Regex(@"<" + type + @"((.*?)(/>)|(.|\n)*?(</" + type + @"(\s*?)>))", RegexOptions.Multiline | RegexOptions.ExplicitCapture);
            }

            if (f_l == 0)
            {
                return new HtmlNode(regex.Match(AllText).Value, AllValueCollection, annotations);

            }
            else
            {
                MatchCollection collection = regex.Matches(AllText);
                if (f_l > 0)
                    return new HtmlNode(collection[f_l].Value, AllValueCollection, annotations);
                else
                    return new HtmlNode(collection.Last().Value, AllValueCollection, annotations);
            }
        }
        public List<HtmlNode> GetChildrenByType(string type)
        {
            Regex regex;
            if (OpenableTags.Contains(type.Trim()))
            {
                regex = new Regex(@"<" + type + @"(.|\n)*?>", RegexOptions.Multiline | RegexOptions.ExplicitCapture);
            }
            else
            {
                regex = new Regex(@"<" + type + @"((.*?)(/>)|(.|\n)*?(</" + type + @"(\s*?)>))", RegexOptions.Multiline | RegexOptions.ExplicitCapture);
            }
            MatchCollection collection = regex.Matches(AllText);
            List<HtmlNode> Children = new List<HtmlNode>();
            foreach (Match m in collection)
            {
                Children.Add(new HtmlNode(m.Value, AllValueCollection, annotations));

            }
            return Children;
        }

        public string GetProperty(string p)
        {
            Regex gvreg = new Regex(@"<\w+(.|\n)*" + p + @"=""(?<value>\d+)""(.|\n)*?>", RegexOptions.Multiline | RegexOptions.ExplicitCapture);
            string index = gvreg.Match(AllText).Groups["value"].Value;
            //string rr = Regex.Replace(r.Value, $"\\b{p}(.*?)=(.*?)\"(?=(.*?))\"", "");
            return AllValueCollection[index].Item2;
        }
        public string? GetString()
        {
            return AllText;
        }
    }
    public class HtmlNode : HtmlNodeBase
    {
        public HtmlNode(string str) : base(str) { }
        public HtmlNode(string str, ReversibleDictionary<string, string> vs, List<string> annos) : base(str, vs, annos) { }

        public HtmlNode this[object uns, UniqueTags ut = UniqueTags._id]
        {
            get
            {
                return GetChildByUniqueTags(uns, ut);
            }
        }


        public HtmlTypesNode ByTypes()
        {
            return new HtmlTypesNode(AllText);
        }


        public static HtmlNode Parse(string str)
        {
            HtmlNode htmlNode = new HtmlNode(str);
            return htmlNode;
        }
    }


    public class HtmlTypesNode : HtmlNodeBase
    {
        public HtmlTypesNode(string str) : base(str)
        {

        }
        public HtmlTypesNode(HtmlNode htmlNode) : base(htmlNode.AllText, htmlNode.AllValueCollection, htmlNode.annotations)
        {

        }
        private HtmlTypesNode(string str, ReversibleDictionary<string, string> vs, List<string> annos) : base(str, vs, annos)
        {

        }

        public HtmlTypesNode this[string type, int f_l = 0]
        {
            get
            {
                return new HtmlTypesNode(GetChildByType(type, f_l));
            }
        }
        public HtmlNode ByTags()
        {
            return new HtmlNode(AllText);
        }
        public static HtmlTypesNode Parse(string str)
        {
            return new HtmlTypesNode(str);
        }

    }

}