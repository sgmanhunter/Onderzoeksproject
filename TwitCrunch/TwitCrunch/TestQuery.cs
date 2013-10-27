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
        OAuthTokenResponse accessTokenResponse = OAuthUtility.GetAccessToken("T3TaQWzzZiVrihJW0GLBg", "OJo28FZzFfXZtFcVOF1FDJpiOpatQMQSvI4aetQ", "2159075078-0NdpXHjpvi46rFpBw3iAx2SGhU0i8LunCGsJCsd",null);

            OAuthTokens tokens = new OAuthTokens();
            tokens.AccessToken = accessTokenResponse.Token;
            tokens.AccessTokenSecret = accessTokenResponse.TokenSecret;
            tokens.ConsumerKey = consumerKey;
            tokens.ConsumerSecret = consumerSecret;


            TwitterResponse<TwitterSearchResultCollection> tr = TwitterSearch.Search("your query");

        TwitterSearchResultCollection results= tr.ResponseObject;
        List<TwitterSearchResult> resultList= results.ToList();
    }
}
