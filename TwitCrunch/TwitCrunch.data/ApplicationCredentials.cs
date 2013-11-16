using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetSharp;


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
        public ArrayList ApiTest()
        {
            ArrayList to_return = new ArrayList();
            var service = new TwitterService(_consumerKey, _consumerSecret);
            service.AuthenticateWith(_accessToken, _accessTokenSecret);

            IEnumerable<TwitterStatus> tweets = service.ListTweetsOnHomeTimeline(new ListTweetsOnHomeTimelineOptions());
            
            foreach (var tweet in tweets)
            {
                to_return.Add(tweet.Text);
            }

            return to_return;
        }

    }
}
