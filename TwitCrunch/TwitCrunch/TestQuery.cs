using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twitterizer;

namespace TwitCrunch
{
    class TestQuery
    {
        private String consumerKey = "T3TaQWzzZiVrihJW0GLBg";
        private String consumerSecret = "OJo28FZzFfXZtFcVOF1FDJpiOpatQMQSvI4aetQ";
        private String accessKey = "2159075078-0NdpXHjpvi46rFpBw3iAx2SGhU0i8LunCGsJCsd";
        private String accessSecret = "fg4E3KIi0wJy8kmu8RR2lI2WhuS5VoIuy5AwVtPxPvjua";

        public List<TwitterSearchResult> queryTwitter()
        {

            OAuthTokenResponse accessTokenResponse = OAuthUtility.GetAccessToken(consumerKey, consumerSecret, accessKey, accessSecret);

            OAuthTokens tokens = new OAuthTokens();
            tokens.AccessToken = accessTokenResponse.Token;
            tokens.AccessTokenSecret = accessTokenResponse.TokenSecret;
            tokens.ConsumerKey = consumerKey;
            tokens.ConsumerSecret = consumerSecret;


            TwitterResponse<TwitterSearchResultCollection> tr = TwitterSearch.Search("#twitter");

            TwitterSearchResultCollection results = tr.ResponseObject;
            return results.ToList();
        }

    }
}
