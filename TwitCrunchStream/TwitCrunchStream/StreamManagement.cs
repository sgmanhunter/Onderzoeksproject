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

      
        public StreamManagement(String woorden)
        {
         
            StreamFilterBasicTrackExample(TokenSingleton.Token,woorden);
        }

        private static int _processedFilteredTweetCount;
        private static bool ProcessFilteredTweet(ITweet tweet, List<string> list)
        {

            Console.WriteLine(tweet.Text);
            Console.WriteLine("Matched {0} tracks", list.Count);
            for (int i = 0; i < list.Count; ++i)
            {
                Console.WriteLine("\t- {0}", list[i]);
            }

            ++_processedFilteredTweetCount;

            // Stop the stream after 500 tweets
            return _processedFilteredTweetCount < 500;
        }

        // Track Keywords
        private static void StreamFilterBasicTrackExample(IToken token, String zoekwoord)
        {
            IFilteredStream stream = new FilteredStream();

            stream.StreamStarted += (sender, args) => Console.WriteLine("Stream has started!");
            stream.AddTrack("#" + zoekwoord);


            stream.LimitReached += (sender, args) =>
            {
                Console.WriteLine("You have missed {0} tweets because you were retrieving more than 1% of tweets", args.Value);
            };

            TwitterContext context = new TwitterContext();
            if (!context.TryInvokeAction(() => stream.StartStream(token, tweet => Console.WriteLine(tweet))))
            {
                Console.WriteLine("An Exception occured : '{0}'", context.LastActionTwitterException.TwitterWebExceptionErrorDescription);
            }
        }
    }
}
