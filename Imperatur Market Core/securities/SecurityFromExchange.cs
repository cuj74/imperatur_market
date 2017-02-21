using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net;

namespace Imperatur_Market_Core.securities
{
    public class SecurityFromExchange : ISecurityFromExchange
    {
        public List<Security> GetSecuritiesFromExchange(string ExchangeCode)
        {
            using (WebClient client = new WebClient())
            {
                HtmlAgilityPack.HtmlWeb oH = new HtmlWeb();
                List<Tuple<int, string, string>> Securities = new List<Tuple<int, string, string>>();

                HtmlDocument doc = oH.Load(string.Format("https://www.google.com/finance?q=%5B(exchange%20%3D%3D%20%22{0}%22)%5D&auto=0&restype=company&noIL=1&start=1&num=524&ei=gAZtWKiSO9aFsAGf3Z_4Bw", ExchangeCode));

                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a[starts-with(@id, 'rc')]"))
                {
                    //get the id 

                    HtmlAttribute att = link.Attributes["id"];
                    string Name = "";
                    string Symbol = "";

                    bool bName = false;
                    if (att.Value.StartsWith("rc-"))
                    {
                        Name = link.InnerText;
                        bName = true;
                    }
                    else
                    {
                        Symbol = link.InnerText;
                    }

                    int id = Convert.ToInt32(att.Value.Split('-').Last());
                    if (Securities.Where(x => x.Item1.Equals(id)).Count() == 0)
                    {
                        Securities.Add(
                            new Tuple<int, string, string>(
                                id,
                               bName ? Name : null,
                               !bName ? Symbol : null
                                )

                            );
                    }
                    else
                    {
                        var T = Securities.Where(x => x.Item1.Equals(id)).First();
                        var T2 = T;

                        Securities.Remove(T);
                        Securities.Add(
                                new Tuple<int, string, string>(
                                    T2.Item1,
                                    bName ? Name : T2.Item2,
                                    !bName ? Symbol : T2.Item3
                                    )

                                );
                    }

                }

                return Securities.Select(x => new Security
                {
                    Exchange = ExchangeCode,
                    Name = x.Item2,
                    Symbol = x.Item3
                }).ToList();

            }
        }
    }
}
