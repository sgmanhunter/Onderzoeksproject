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
        public void Stream()
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
                List<String> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(responseStream.Read().ToString());



                foreach (string item in list)
                {
                    Console.WriteLine(item);
                }

            }
        }
    }

}
