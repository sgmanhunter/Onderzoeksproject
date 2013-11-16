using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace TwitCrunch.data
{
    class TwitterCrunchStreaming
    {
        private void Stream()
        {


            JsonSerializer serializer = new JsonSerializer();

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("https://stream.twitter.com/1/statuses/sample.json");
            webRequest.Credentials = new NetworkCredential("...", "...");
            webRequest.Timeout = -1;
            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();

            StreamReader responseStream = new StreamReader(webResponse.GetResponseStream());
            while (true)
            {
                var line = responseStream.ReadLine();
                if (String.IsNullOrEmpty(line)) continue;
                // dit moet nog verbeterd worden 
                dynamic obj = serializer.Deserialize<Dictionary<string, object>>(line);

                if (obj["user"] != null)
                    Console.WriteLine(obj["user"]["screen_name"] + ": " + obj["text"]);
            }


        }
    }
}
