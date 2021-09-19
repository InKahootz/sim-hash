using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SimHashLib
{
    public class Shingling
    {
        public List<string> Slide(string content, int width = 4)
        {
            var listOfShingles = new List<string>();
            for (int i = 0; i < (content.Length + 1 - width); i++)
            {
                string piece = content.Substring(i, width);
                listOfShingles.Add(piece);
            }
            return listOfShingles;
        }
        public string Scrub(string content)
        {
            MatchCollection matches = Regex.Matches(content, @"[\w\u4e00-\u9fcc]+");
            string ans = "";
            foreach (Match match in matches)
            {
                ans += match.Value;
            }

            return ans;
        }

        public List<string> Tokenize(string content, int width = 4)
        {
            content = content.ToLower();
            content = Scrub(content);
            return Slide(content, width);
        }
    }
}
