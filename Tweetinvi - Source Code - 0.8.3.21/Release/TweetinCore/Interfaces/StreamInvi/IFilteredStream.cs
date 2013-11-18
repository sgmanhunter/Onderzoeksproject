using System;
using System.Collections.Generic;
using TweetinCore.Events;
using TweetinCore.Interfaces.TwitterToken;

namespace TweetinCore.Interfaces.StreamInvi
{
    /// <summary>
    /// Set of methods to add filters to a Stream
    /// </summary>
    public interface IFilteredStream : IStream<ITweet>, ITrackManager<ITweet>, ITrackedStream
    {
        /// <summary>
        /// When a filtered stream is able to capture more than 1% of the Total Tweets
        /// the LimitReached event is raised and give you the number of Tweets that you 
        /// have not been able to collect.
        /// </summary>
        event EventHandler<GenericEventArgs<int>> LimitReached;

        #region Follow
        /// <summary>
        /// List of UserId followed by the stream
        /// </summary>
        Dictionary<long?, Action<ITweet>> FollowingUserIds { get; }

        /// <summary>
        /// Follow a specific userId
        /// </summary>
        void AddFollow(long? userId, Action<ITweet> userPublishedTweet = null);

        /// <summary>
        /// Follow a specific user
        /// </summary>
        void AddFollow(IUser user, Action<ITweet> userPublishedTweet = null);

        /// <summary>
        /// Unfollow a specific userId
        /// </summary>
        void RemoveFollow(long? userId);

        /// <summary>
        /// Unfollow a specific user
        /// </summary>
        void RemoveFollow(IUser user);

        /// <summary>
        /// Tells you whether you are following a userId
        /// </summary>
        bool ContainsFollow(long? userId);

        /// <summary>
        /// Tells you whether you are following a user
        /// </summary>
        bool ContainsFollow(IUser user);

        /// <summary>
        /// Unfollow all the currently followed users
        /// </summary>
        void ClearFollows(); 
        #endregion

        #region Location

        /// <summary>
        /// List of locations analyzed by the stream
        /// </summary>
        Dictionary<ILocation, Action<ITweet>> Locations { get; }

        /// <summary>
        /// Add a location for the stream to analyze
        /// </summary>
        void AddLocation(ILocation location, Action<ITweet> locationDetected = null);

        /// <summary>
        /// Add a location for the stream to analyze
        /// </summary>
        ILocation AddLocation(ICoordinates coordinate1, ICoordinates coordinate2, Action<ITweet> locationDetected = null);

        /// <summary>
        /// Remove a location for the stream to analyze
        /// </summary>
        void RemoveLocation(ILocation location);

        /// <summary>
        /// Remove a location for the stream to analyze
        /// </summary>
        void RemoveLocation(ICoordinates coordinate1, ICoordinates coordinate2);

        /// <summary>
        /// Tells you whether you are analyzing a specific location
        /// </summary>
        bool ContainsLocation(ILocation location);

        /// <summary>
        /// Tells you whether you are analyzing a specific location
        /// </summary>
        bool ContainsLocation(ICoordinates coordinate1, ICoordinates coordinate2);

        /// <summary>
        /// Remove all the currently analyzed locations
        /// </summary>
        void ClearLocations();

        #endregion

        #region Start Stream

        #region StartStream

        /// <summary>
        /// Start an infinite stream (can be stopped from PauseStream/StopStream)
        /// Foreach tweet received, you will get the following information : 
        /// Tweet - Matching Locations
        /// </summary>
        void StartStream(IToken token,
            Action<ITweet, List<ILocation>> processTweetDelegate);

        /// <summary>
        /// Default behavior provided by Twitter. Gets you Filtered Stream that match either the 
        /// Follow, Track and Location conditions. 
        /// Foreach tweet received, you will get the following information : 
        /// Tweet - Matching Locations
        /// </summary>
        void StartStream(IToken token,
            Func<ITweet, List<ILocation>, bool> processTweetDelegate);

        /// <summary>
        /// Start an infinite stream (can be stopped from PauseStream/StopStream)
        /// Foreach tweet received, you will get the following information : 
        /// Tweet - Matching Tracks - Matching Locations
        /// </summary>
        void StartStream(IToken token,
            Action<ITweet, List<string>, List<ILocation>> processTweetDelegate);

        /// <summary>
        /// Default behavior provided by Twitter. Gets you Filtered Stream that match either the 
        /// Follow, Track and Location conditions. 
        /// Foreach tweet received, you will get the following information : 
        /// Tweet - Matching Tracks - Matching Locations
        /// </summary>
        void StartStream(IToken token,
            Func<ITweet, List<string>, List<ILocation>, bool> processTweetDelegate); 

        #endregion

        #region StartStream Matching All Conditions

        /// <summary>
        /// Start an infinite stream that retrieve tweets from the filtered stream only if they comply with all the
        /// Follow, Track and Location conditions.
        /// Foreach tweet received, you will get the following information : 
        /// Tweet
        /// </summary>
        void StartStreamMatchingAllConditions(IToken token,
            Action<ITweet> processTweetDelegate);

        /// <summary>
        /// Gets you tweets from the filtered stream only if they comply with all the
        /// Follow, Track and Location conditions.
        /// Foreach tweet received, you will get the following information : 
        /// Tweet
        /// </summary>
        void StartStreamMatchingAllConditions(IToken token,
            Func<ITweet, bool> processTweetDelegate);

        /// <summary>
        /// Start an infinite stream that retrieve tweets from the filtered stream only if they comply with all the
        /// Follow, Track and Location conditions. 
        /// Foreach tweet received, you will get the following information : 
        /// Tweet - Matching Tracks
        /// </summary>
        void StartStreamMatchingAllConditions(IToken token,
            Action<ITweet, List<string>> processTweetDelegate);

        /// <summary>
        /// Gets you tweets from the filtered stream only if they comply with all the
        /// Follow, Track and Location conditions. 
        /// Foreach tweet received, you will get the following information : 
        /// Tweet - Matching Keywords
        /// </summary>
        void StartStreamMatchingAllConditions(IToken token,
            Action<ITweet, List<ILocation>> processTweetDelegate);

        /// <summary>
        /// Gets you tweets from the filtered stream only if they comply with all the
        /// Follow, Track and Location conditions. 
        /// Foreach tweet received, you will get the following information : 
        /// Tweet - Matching Location
        /// </summary>
        void StartStreamMatchingAllConditions(IToken token,
            Func<ITweet, List<ILocation>, bool> processTweetDelegate);

        /// <summary>
        /// Gets you tweets from the filtered stream only if they comply with all the
        /// Follow, Track and Location conditions. 
        /// Foreach tweet received, you will get the following information : 
        /// Tweet - Matching Tracks
        /// </summary>
        void StartStreamMatchingAllConditions(IToken token,
            Func<ITweet, List<string>, bool> processTweetDelegate);

        /// <summary>
        /// Start an infinite stream that retrieve tweets from the filtered stream only if they comply with all the
        /// Follow, Track and Location conditions. 
        /// Foreach tweet received, you will get the following information : 
        /// Tweet - Matching Tracks - Matching Locations
        /// </summary>
        void StartStreamMatchingAllConditions(IToken token,
            Action<ITweet, List<string>, List<ILocation>> processTweetDelegate);

        /// <summary>
        /// Gets you tweets from the filtered stream only if they comply with all the
        /// Follow, Track and Location conditions. 
        /// Foreach tweet received, you will get the following information : 
        /// Tweet - Matching Tracks - Matching Locations
        /// </summary>
        void StartStreamMatchingAllConditions(IToken token, 
            Func<ITweet, List<string>, List<ILocation>, bool> processTweetDelegate);

        #endregion

        #endregion
    }
}
