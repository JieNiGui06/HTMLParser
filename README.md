>Tip: Only for .net,use with https://github.com/JieNiGui06/JType

>**Explain:**
>>**Install:** 
>>>dotnet add package htmlparser_light --version 1.0.0.2
>>>Or search at vsNugetManage: htmlparser_light
>>>Or goto the website: [NuGet Gallery | htmlparser_light 1.0.0.2](https://www.nuget.org/packages/htmlparser_light)

>

## Code Example (C#):

>>**1.**
>>>HTMLParser.HtmlNode htmlNode = new HTMLParser.HtmlNode("\r\n<a id=\"dd\" href=\"dssgg1111\" href=\"dgeargerghrahfd1111\" / >\r\n<!--\r\n    \"\"\r\n-->\r\n<a class=\"outc\" href=\"dsvfz\">\r\n    <a id=\"inna\" href=\"adfsgrg\">\r\n    <a class=\"sssss\" id=\"innad\" href=\"adfsgrg\">\r\n\r\n    </a>\r\n\r\n    </a>\r\n</a>\r\n<img name=\"img\" class=\"iimg\" src=\"你正则表达式写的也太好了！！！\" alt=\"agsc\"></img>");
>var requrl = htmlNode["img", UniqueTags._name].GetProperty("src");
>Console.WriteLine(requrl);
>requrl = htmlNode.GetChildByUniqueTags("img", UniqueTags._name).GetProperty("src");
>Console.WriteLine(requrl);
>requrl = htmlNode[new string[]{"innad","sssss"},UniqueTags._id|UniqueTags._class].GetProperty("href");
>Console.WriteLine(requrl);
>requrl = htmlNode["inna"]["innad"].GetProperty("class");
>Console.WriteLine(requrl);
>return;

>> **Output:** 
>>>你正则表达式写的也太好了！！！
>>>你正则表达式写的也太好了！！！
>>>adfsgrg
>>>sssss
>
>>**2.**
>>>HttpClient httpClient = new HttpClient();
>string roothtml = await httpClient.GetStringAsync($"https://cn.bing.com/search?q=SPDX+License+List%E6%80%8E%E4%B9%88%E5%86%99&qs=n&form=QBRE&sp=-1&lq=0&pq=spdx+license+list%E6%80%8E%E4%B9%88%E5%86%99&sc=1-20&sk=&cvid=B041837E3D4B4A8686B8A8A6F2F3566C&ghsh=0&ghacc=0&ghpl=");
>HTMLParser.HtmlNode htmlNode = new HTMLParser.HtmlNode(roothtml);
>var requrl = htmlNode["sb_form_q"].GetProperty("placeholder");
>Console.WriteLine(requrl);
>return;

>>**Output:** 
>>>有问题尽管问我...

>>**3.**
>>>HTMLParser.HtmlNode htmlNode = new HTMLParser.HtmlNode("\r\n<a id=\"dd\" href=\"dssgg1111\" href=\"dgeargerghrahfd1111\" / >\r\n<!--\r\n    \"\"\r\n-->\r\n<div class=\"outc\" href=\"dsvfz\">\r\n    <a id=\"inna\" href=\"adfsgrg\">\r\n    <a class=\"sssss\" id=\"innad\" href=\"adfsgrg\">\r\n\r\n    </a>\r\n\r\n    </a>\r\n</div>\r\n<img name=\"img\" class=\"iimg\" src=\"你正则表达式写的也太好了！！！\" alt=\"agsc\"></img>");
>var requrl = htmlNode["outc", UniqueTags._class].ByTypes()["a"].GetProperty("id");
>Console.WriteLine(requrl);

>>**Output:**
>>>inna
