using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Mitty
{
    static class Api
    {
        public static async Task<string> Call(string url)
        {
            try
            {
                string html = string.Empty;

                var request = (HttpWebRequest)WebRequest.Create(url);

                WebResponse response = await request.GetResponseAsync();

                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    html = reader.ReadToEnd();
                };

                return html;
            }
            catch (WebException)
            {
                if (url.Contains("osu"))
                    throw new Exception("Could not connect to API. Check https://status.ppy.sh/ for more info");
                else
                    throw new Exception("Could not connect to API");
            }
        }
    }
}
