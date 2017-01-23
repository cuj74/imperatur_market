using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Imperatur_v2.shared;

namespace Imperatur_v2.rest
{
    public class Rest : IRestInterface
    {
        public string GetResultFromURL(string URL)
        {
            string json = "";
            try
            {
                using (WebClient wc = new WebClient())
                {
                    json = wc.DownloadString(URL);
                }
            }
            catch(Exception ex)
            {
                ImperaturGlobal.GetLog().Error(string.Format("Error downloading from {0}", URL), ex);
            }
            return json;
        }
    }
}
