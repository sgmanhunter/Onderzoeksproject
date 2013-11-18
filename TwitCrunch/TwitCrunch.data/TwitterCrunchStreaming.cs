using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Streaminvi;
using TweetinCore;
using TwitterToken;
using Tweetinvi;

namespace TwitCrunch.data
{
    class TwitterCrunchStreaming
    {
        public void Stream()
        {

            // efkes hier verkeerd bezig zzzz 
            TokenCreator i = new TokenCreator(ApplicationCredentials.Singleton.ConsumerKey, ApplicationCredentials.Singleton.ConsumerSecret);

           // i.GenerateToken(ApplicationCredentials.Singleton.ConsumerKey);
            

            TokenSingleton.Token = token;

            SimpleStream twitterStream = new SimpleStream("https://stream.twitter.com/1.1/statuses/sample.json");

            twitterStream.StartStream(token,x => Console.WriteLine(x.Text));
            
            
            
       
        }
    }

}
