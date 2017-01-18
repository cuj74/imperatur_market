using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Imperatur_v2.trade.rss
{
    public class RSSReader : IRSSReader
    {
        private List<Tuple<DateTime, string, XDocument>> m_oSearcCache;
        public int GetOccurancesOfString(string[] URLs, string[] SearchData)
        {
            //remove entries older than 10 minutes
            var rem = m_oSearcCache.Where(x => x.Item1 < DateTime.Now.AddMinutes(-10));
            m_oSearcCache.Remove((Tuple<DateTime, string, XDocument>)rem);

            int count = 0;
            var Search = SearchData.Select(s=>s.ToLower()).ToList();
            foreach (string URL in URLs)
            {
                XDocument feedXML;
                if (m_oSearcCache.Where(x=>x.Item2.Equals(URL)).Count() > 0)
                {
                    feedXML = m_oSearcCache.Where(x => x.Item2.Equals(URL)).Last().Item3;
                }
                else
                {
                    feedXML = XDocument.Load(URL);
                    m_oSearcCache.Add(new Tuple<DateTime, string, XDocument>(DateTime.Now, URL, feedXML));
                }
                var feeds = from feed in feedXML.Descendants("item")
                            select new
                            {
                                Title = feed.Element("title").Value,
                                Description = feed.Element("description").Value
                            };
                foreach (string se in SearchData)
                {
                    count += feeds.GroupBy(x => x.Title)
                     .Select(g => new { Value = g.Key, Count = g.Count() })
                     .Where(s => s.Value.Contains(se)).First().Count;

                    count += feeds.GroupBy(x => x.Description)
                     .Select(g => new { Value = g.Key, Count = g.Count() })
                     .Where(s => s.Value.Contains(se)).First().Count;
                }

            }

            return count;

        }
    }
}
