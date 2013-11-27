using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using oAuthConnection;
using Streaminvi;
using TweetinCore.Enum;
using TweetinCore.Events;
using TweetinCore.Interfaces;
using TweetinCore.Interfaces.oAuth;
using TweetinCore.Interfaces.StreamInvi;
using TweetinCore.Interfaces.TwitterToken;
using Tweetinvi;
using Tweetinvi.Model;
using TwitterToken;
using System.Configuration;

namespace TwitCrunchStream
{
    class StreamManagement
    {
        public StreamManagement()
        {
            createToken();
        }

        public void Init(String woord)
        {
            StreamFilterBasicTrackExample(TokenSingleton.Token, woord);
        }

        private void createToken()
        {
            IToken token = new Token(
            ConfigurationManager.AppSettings["token_AccessToken"],
            ConfigurationManager.AppSettings["token_AccessTokenSecret"],
            ConfigurationManager.AppSettings["token_ConsumerKey"],
            ConfigurationManager.AppSettings["token_ConsumerSecret"]);

            TokenSingleton.Token = token;
        }

        // Track Keywords
        private void StreamFilterBasicTrackExample(IToken token, String zoekwoord)
        {
            IFilteredStream stream = new FilteredStream();

            stream.StreamStarted += (sender, args) => Console.WriteLine("Stream has started!");
            stream.AddTrack("#" + zoekwoord);

            stream.LimitReached += (sender, args) =>
            {
                Console.WriteLine("You have missed {0} tweets because you were retrieving more than 1% of tweets", args.Value);
            };

            TwitterContext context = new TwitterContext();
            if (!context.TryInvokeAction(() => stream.StartStream(token, tweet => catchTweets(tweet))))
            {
                Console.WriteLine("An Exception occured : '{0}'", context.LastActionTwitterException.TwitterWebExceptionErrorDescription);
            }
        }

        private void catchTweets(ITweet tweet)
        {
            Console.WriteLine(tweet);
            Tweet nieuweTweet = new Tweet(tweet);
            nieuweTweet.WriteToDatabase();
        }
    }
}
