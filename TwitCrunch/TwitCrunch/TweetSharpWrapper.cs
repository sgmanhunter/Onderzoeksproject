using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetSharp;

namespace TwitCrunch.data
{
    public class TweetSharpWrapper
    {
        private TwitterService _service;
        private ApplicationCredentials _cred = ApplicationCredentials.Singleton;

        public TweetSharpWrapper()
        { }

        private void AuthenticateService()
        {
            /*_service = new TwitterService(_cred.ConsumerKey, _cred.ConsumerSecret);
            _service.AuthenticateWith(_cred.AccessToken, _cred.AccessTokenSecret);*/
        }

       

    }
}
