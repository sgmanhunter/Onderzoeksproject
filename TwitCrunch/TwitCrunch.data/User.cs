using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetSharp;


namespace TwitCrunch.data
{
    public class User
    {

        private static User _user = new User();
        public static User Singleton
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
        }
        public string ConsumerSecret
        {
            set
            {
                _consumerSecret = value;
            }
        }
        public string AccessToken
        {
            set
            {
                _accessToken = value;
            }
        }
        public string AccessTokenSecret
        {
            set
            {
                _accessTokenSecret = value;
            }
        }
 

        private User()
        {}


        public void Connect()
        {
            // Pass your credentials to the service
            TwitterService service = new TwitterService(_consumerKey, _consumerSecret);

            // Step 1 - Retrieve an OAuth Request Token
            OAuthRequestToken requestToken = service.GetRequestToken("oob");

            // Step 2 - Redirect to the OAuth Authorization URL
            Uri uri = service.GetAuthorizationUri(requestToken);
            Process.Start(uri.ToString());

            // Step 3 - Exchange the Request Token for an Access Token
            string verifier = "123456"; // <-- This is input into your application by your user
            OAuthAccessToken access = service.GetAccessToken(requestToken, verifier);

            // Step 4 - User authenticates using the Access Token
            service.AuthenticateWith(access.Token, access.TokenSecret);
            //IEnumerable<TwitterStatus> mentions = service.ListTweetsMentioningMe();

           
        }

    }
}
