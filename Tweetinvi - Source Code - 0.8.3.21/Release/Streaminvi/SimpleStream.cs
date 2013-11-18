using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TweetinCore.Enum;
using TweetinCore.Interfaces;
using TweetinCore.Interfaces.TwitterToken;
using Tweetinvi;

namespace Streaminvi
{
    /// <summary>
    /// Mehtods providing access to the Twitter stream API
    /// </summary>
    public class SimpleStream : BaseTrackedStream
    {
        private readonly string _streamURL;

        /// <summary>
        /// Constructor defining the delegate used each time a Tweet is retrieved from the Streaming API
        /// </summary>
        /// <param name="url">Url of the expected stream</param>
        public SimpleStream(string url)
        {
            _streamURL = url;
        }

        public override void StartStream(IToken token, Func<ITweet, bool> processTweetDelegate)
        {
            Func<HttpWebRequest> generateWebRequest = delegate
            {
                return token.GetQueryWebRequest(_streamURL, HttpMethod.GET);
            };

            if (processTweetDelegate == null)
            {
                processTweetDelegate = tweet => true;
            }

            Func<string, bool> generateTweetDelegate = x =>
            {
                var jsonTweet = _jsSerializer.Deserialize<dynamic>(x) as Dictionary<string, object>;

                if (jsonTweet != null)
                {
                    if (jsonTweet.ContainsKey("delete"))
                    {
                        return true;
                    }

                    ITweet t = new Tweet(jsonTweet);

                    if (_trackManager.TracksCount != 0)
                    {
                        var matches = _trackManager.Matches(t.Text);
                        return !matches || processTweetDelegate(t);
                    }

                    return processTweetDelegate(t);
                }

                // The information sent from Twitter was not the expected object
                return true;
            };

            _streamResultGenerator.StartStream(generateTweetDelegate, generateWebRequest);
        }

        public override void StartStream(IToken token, Func<ITweet, List<string>, bool> processTweetDelegate)
        {
            Func<HttpWebRequest> generateWebRequest = delegate
            {
                return token.GetQueryWebRequest(_streamURL, HttpMethod.GET);
            };

            if (processTweetDelegate == null)
            {
                processTweetDelegate = (tweet, list) => true;
            }

            Func<string, bool> generateTweetDelegate = x =>
            {
                var jsonTweet = _jsSerializer.Deserialize<dynamic>(x) as Dictionary<string, object>;

                if (jsonTweet != null)
                {
                    if (jsonTweet.ContainsKey("delete"))
                    {
                        return true;
                    }

                    ITweet t = new Tweet(jsonTweet);

                    var matchingTracks = _trackManager.MatchingTracks(t.Text);
                    if (matchingTracks.Any())
                    {
                        return processTweetDelegate(t, matchingTracks);
                    }

                    return true;
                }

                // The information sent from Twitter was not the expected object
                return true;
            };

            _streamResultGenerator.StartStream(generateTweetDelegate, generateWebRequest);
        }
    }
}
