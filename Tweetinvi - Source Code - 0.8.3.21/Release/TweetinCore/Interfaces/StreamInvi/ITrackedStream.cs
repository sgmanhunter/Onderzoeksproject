using System;
using System.Collections.Generic;
using TweetinCore.Interfaces.TwitterToken;

namespace TweetinCore.Interfaces.StreamInvi
{
    public interface ITrackedStream
    {
        /// <summary>
        /// Start an infinite stream (can be stopped from PauseStream/StopStream)
        /// Foreach tweet received, you will get the following information : 
        /// Tweet - Matching Tracks
        /// </summary>
        void StartStream(IToken token, Action<ITweet, List<string>> processTweetDelegate);

        /// <summary>
        /// Start a stream that you can stop from the processTweetDelegate
        /// Foreach tweet received, you will get the following information : 
        /// Tweet - Matching Tracks
        /// </summary>
        void StartStream(IToken token, Func<ITweet, List<string>, bool> processTweetDelegate);
    }
}
