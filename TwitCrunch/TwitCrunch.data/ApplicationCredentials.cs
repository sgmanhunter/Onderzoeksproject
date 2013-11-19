using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Streaminvi;
using TweetinCore.Interfaces;
using TweetinCore.Interfaces.TwitterToken;
using TweetinCore.Interfaces.StreamInvi;
using TwitterToken;
using Tweetinvi;


namespace TwitCrunch.data
{
    public class ApplicationCredentials
    {

        private static ApplicationCredentials _user = new ApplicationCredentials();
        public static ApplicationCredentials Singleton
        {
            get
            {
                return _user;
            }
        }

        private string _consumerKey, _consumerSecret;
        private string _accessToken, _accessTokenSecret;

        public string ConsumerKey 
        { 
           set
            {
                _consumerKey = value;
            }
            get
            {
                return _consumerKey;
            }
        }
        public string ConsumerSecret
        {
            set
            {
                _consumerSecret = value;
            }
            get
            {
                return _consumerSecret;
            }
        }
        public string AccessToken
        {
            set
            {
                _accessToken = value;
            }
            get
            {
                return _accessToken;
            }
        }
        public string AccessTokenSecret
        {
            set
            {
                _accessTokenSecret = value;
            }
            get
            {
                return _accessTokenSecret;
            }
        }
        private ApplicationCredentials()
        {}

        //testen 
        IToken token = new Token(ApplicationCredentials.Singleton.AccessToken,
            ApplicationCredentials.Singleton.AccessTokenSecret,
            ApplicationCredentials.Singleton.ConsumerKey,
            ApplicationCredentials.Singleton.ConsumerSecret);

        private static readonly List<ITweet> _streamList = new List<ITweet>();
        private static void ProcessTweet(ITweet tweet)
        {
            if (tweet == null)
            {
                return;
            }

            if (_streamList.Count % 125 != 124)
            {
                Console.WriteLine("{0} : \"{1}\"", tweet.Creator.Name, tweet.Text);
                _streamList.Add(tweet);
            }
            else
            {
                Console.WriteLine("Processing data");
                _streamList.Clear();
            }
        }
        public void Stream()
        {

            SimpleStream twitterStream = new SimpleStream("https://stream.twitter.com/1.1/statuses/sample.json");

            twitterStream.StartStream(token, x => ProcessTweet(x));




        }
    }
}
