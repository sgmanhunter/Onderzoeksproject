using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twitterizer;

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
        private string _accessKey, _accessSecret;
        private string _oathAuthorizeLink = @"https://api.twitter.com/oauth/authorize?oauth_token=";
        private string _pin;

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
        public string AccessKey
        {
            set
            {
                _accessKey = value;
            }
        }
        public string AccessSecret
        {
            set
            {
                _accessSecret = value;
            }
        }
        public string Pin 
        {
            set
            {
                _pin = value;
            }
        }


        private User()
        {}


        public void Connect()
        {
           /* OAuthTokenResponse tokenResponse = new OAuthTokenResponse();
            tokenResponse = OAuthUtility.GetRequestToken(_consumerKey, _consumerSecret,"oob");
            string target = _oathAuthorizeLink + tokenResponse.Token;
            try
            {
                System.Diagnostics.Process.Start(target);
            }
            catch (System.ComponentModel.Win32Exception noBrowser)
            {
                //if (noBrowser.ErrorCode == -2147467259)
            }
            catch (System.Exception other)
            {
                //
            }
            tokenResponse = OAuthUtility.GetAccessToken(_consumerKey, _consumerSecret, tokenResponse.Token, _pin);*/

        }

    }
}
