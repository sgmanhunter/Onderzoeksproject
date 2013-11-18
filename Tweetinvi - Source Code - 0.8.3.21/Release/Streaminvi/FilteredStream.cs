using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Streaminvi.Helpers;
using Streaminvi.Properties;
using TweetinCore.Enum;
using TweetinCore.Events;
using TweetinCore.Interfaces;
using TweetinCore.Interfaces.StreamInvi;
using TweetinCore.Interfaces.TwitterToken;
using Tweetinvi;
using Tweetinvi.Model;

namespace Streaminvi
{
    /// <summary>
    /// Stream filtering the objects from a stream
    /// </summary>
    public class FilteredStream : BaseTrackedStream, IFilteredStream
    {
        // Const
        private const int MAXIMUM_TRACKED_LOCATIONS_AUTHORIZED = 25;
        private const int MAXIMUM_TRACKED_USER_ID_AUTHORIZED = 5000;


        // Events
        public event EventHandler<GenericEventArgs<int>> LimitReached;

        // Properties
        private readonly Dictionary<long?, Action<ITweet>> _followingUserIds;
        public Dictionary<long?, Action<ITweet>> FollowingUserIds
        {
            get { return _followingUserIds; }
        }

        private readonly Dictionary<ILocation, Action<ITweet>> _locations;
        public Dictionary<ILocation, Action<ITweet>> Locations
        {
            get { return _locations; }
        }

        // Constructor
        public FilteredStream()
        {
            _followingUserIds = new Dictionary<long?, Action<ITweet>>();
            _locations = new Dictionary<ILocation, Action<ITweet>>();
        }

        #region Follow

        public void AddFollow(long? userId, Action<ITweet> userPublishedTweet = null)
        {
            if (userId != null && _followingUserIds.Count < MAXIMUM_TRACKED_USER_ID_AUTHORIZED)
            {
                _followingUserIds.Add(userId, userPublishedTweet);
            }
        }

        public void AddFollow(IUser user, Action<ITweet> userPublishedTweet = null)
        {
            if (user != null && user.Id != null)
            {
                AddFollow(user.Id, userPublishedTweet);
            }
        }

        public void RemoveFollow(long? userId)
        {
            if (userId != null)
            {
                _followingUserIds.Remove(userId);
            }
        }

        public void RemoveFollow(IUser user)
        {
            if (user != null)
            {
                RemoveFollow(user.Id);
            }
        }

        public bool ContainsFollow(long? userId)
        {
            if (userId != null)
            {
                return _followingUserIds.Keys.Contains(userId);
            }

            return false;
        }

        public bool ContainsFollow(IUser user)
        {
            if (user != null)
            {
                ContainsFollow(user.Id);
            }

            return false;
        }

        public void ClearFollows()
        {
            _followingUserIds.Clear();
        }

        #endregion

        #region Location

        public ILocation AddLocation(ICoordinates coordinate1, ICoordinates coordinate2, Action<ITweet> locationDetected = null)
        {
            ILocation location = new Location(coordinate1, coordinate2);
            AddLocation(location, locationDetected);

            return location;
        }

        public void AddLocation(ILocation location, Action<ITweet> locationDetected = null)
        {
            if (!ContainsLocation(location) && _locations.Count < MAXIMUM_TRACKED_LOCATIONS_AUTHORIZED)
            {
                Locations.Add(location, locationDetected);
            }
        }

        public void RemoveLocation(ICoordinates coordinate1, ICoordinates coordinate2)
        {
            var location = Locations.Keys.FirstOrDefault(x => (x.Coordinate1 == coordinate1 && x.Coordinate2 == coordinate2) ||
                                                              (x.Coordinate1 == coordinate2 && x.Coordinate2 == coordinate1));

            if (location != null)
            {
                Locations.Remove(location);
            }
        }

        public void RemoveLocation(ILocation location)
        {
            RemoveLocation(location.Coordinate1, location.Coordinate2);
        }

        public bool ContainsLocation(ICoordinates coordinate1, ICoordinates coordinate2)
        {
            return Locations.Keys.Any(x => (x.Coordinate1 == coordinate1 && x.Coordinate2 == coordinate2) ||
                                           (x.Coordinate1 == coordinate2 && x.Coordinate2 == coordinate1));
        }

        public bool ContainsLocation(ILocation location)
        {
            return ContainsLocation(location.Coordinate1, location.Coordinate2);
        }

        public void ClearLocations()
        {
            Locations.Clear();
        }

        private bool LocationMatchesTrackedLocations(ICoordinates coordinates)
        {
            if (!_locations.Any() || coordinates == null)
            {
                return false;
            }

            bool matches = false;
            for (int i = 0; i < _locations.Count && !matches; ++i)
            {
                matches = Location.CoordinatesLocatedIn(coordinates, _locations.Keys.ElementAt(i));
            }

            return matches;
        }

        private List<ILocation> GetMatchedLocations(ICoordinates coordinates)
        {
            if (!_locations.Any() || coordinates == null)
            {
                return new List<ILocation>();
            }

            return _locations.Keys.Where(x => Location.CoordinatesLocatedIn(coordinates, x)).ToList();
        }

        #endregion

        #region Start Stream

        private string GenerateORFilterQuery()
        {
            StringBuilder queryBuilder = new StringBuilder(Resources.Stream_Filter);

            var followPostRequest = QueryGeneratorHelper.GenerateFilterFollowRequest(FollowingUserIds.Keys.ToList());
            var trackPostRequest = QueryGeneratorHelper.GenerateFilterTrackRequest(_trackManager.Tracks.Keys.ToList());
            var locationPostRequest = QueryGeneratorHelper.GenerateFilterLocationRequest(Locations.Keys.ToList());

            if (!String.IsNullOrEmpty(trackPostRequest))
            {
                queryBuilder.Append(trackPostRequest);
            }

            if (!String.IsNullOrEmpty(followPostRequest))
            {
                queryBuilder.Append(queryBuilder.Length == 0 ? followPostRequest : String.Format("&{0}", followPostRequest));
            }

            if (!String.IsNullOrEmpty(locationPostRequest))
            {
                queryBuilder.Append(queryBuilder.Length == 0 ? locationPostRequest : String.Format("&{0}", locationPostRequest));
            }

            return queryBuilder.ToString();
        }

        private string GenerateANDFilterQuery()
        {
            StringBuilder queryBuilder = new StringBuilder(Resources.Stream_Filter);

            var followPostRequest = QueryGeneratorHelper.GenerateFilterFollowRequest(FollowingUserIds.Keys.ToList());
            var trackPostRequest = QueryGeneratorHelper.GenerateFilterTrackRequest(_trackManager.Tracks.Keys.ToList());
            var locationPostRequest = QueryGeneratorHelper.GenerateFilterLocationRequest(Locations.Keys.ToList());

            if (!String.IsNullOrEmpty(followPostRequest))
            {
                queryBuilder.Append(followPostRequest);
            }
            else if (!String.IsNullOrEmpty(trackPostRequest))
            {
                queryBuilder.Append(trackPostRequest);
            }
            else if (!String.IsNullOrEmpty(locationPostRequest))
            {
                queryBuilder.Append(locationPostRequest);
            }

            return queryBuilder.ToString();
        }

        private bool TryGetMatchingKeywordsAndRelatedActions(ITweet tweet,
            out List<string> matchingKeywords,
            out List<Action<ITweet>> matchingKeywordsActions)
        {
            matchingKeywords = _trackManager.MatchingTracks(tweet.Text, out matchingKeywordsActions);

            return matchingKeywords.Any();
        }

        private void CallMultipleActions<T>(T tweet, List<Action<T>> tracksActionsIdenfied)
        {
            if (tracksActionsIdenfied != null && tracksActionsIdenfied.Any())
            {
                for (int i = 0; i < tracksActionsIdenfied.Count; ++i)
                {
                    tracksActionsIdenfied[i](tweet);
                }
            }
        }
        private ITweet GetTweetFromJson(string jsonTweet)
        {
            var tweetObject = _jsSerializer.Deserialize<Dictionary<string, object>>(jsonTweet);

            if (!String.IsNullOrEmpty(jsonTweet))
            {
                if (tweetObject.Count() == 1)
                {
                    if (tweetObject.Keys.ElementAt(0) == "limit")
                    {
                        var limitInfo = (tweetObject["limit"] as Dictionary<string, object>);
                        if (limitInfo == null)
                        {
                            return null;
                        }

                        this.Raise(LimitReached, (int)limitInfo["track"]);
                    }

                    return null;
                }

                return new Tweet(tweetObject);
            }

            return null;
        }

        private bool MatchesAllConditions(ITweet tweet)
        {
            if (tweet.Creator.Id == null)
            {
                return false;
            }

            bool followMatches = !FollowingUserIds.Any() || ContainsFollow(tweet.Creator.Id);
            bool tracksMatches = !Tracks.Any() || _trackManager.Matches(tweet.Text);
            bool locationMatches = !Locations.Any() || LocationMatchesTrackedLocations(tweet.LocationCoordinates);

            bool result;
            if (FollowingUserIds.Any())
            {
                result = followMatches && tracksMatches && locationMatches;
            }
            else if (Tracks.Any())
            {
                result = tracksMatches && locationMatches;
            }
            else if (Locations.Any())
            {
                result = locationMatches;
            }
            else
            {
                result = true;
            }

            return result;
        }

        // StartStream

        #region Return Tweet only

        public override void StartStream(IToken token, Func<ITweet, bool> processTweetDelegate)
        {
            Func<HttpWebRequest> generateWebRequest = () => token.GetQueryWebRequest(GenerateORFilterQuery(), HttpMethod.POST);

            StartSimpleStream(processTweetDelegate, tweet => true, generateWebRequest);
        }

        public void StartStreamMatchingAllConditions(IToken token, Action<ITweet> processTweetDelegate)
        {
            StartStreamMatchingAllConditions(token, tweet =>
            {
                processTweetDelegate(tweet);
                return true;
            });
        }

        public void StartStreamMatchingAllConditions(IToken token, Func<ITweet, bool> processTweetDelegate)
        {
            Func<HttpWebRequest> generateWebRequest = () => token.GetQueryWebRequest(GenerateANDFilterQuery(), HttpMethod.POST);

            StartSimpleStream(processTweetDelegate, MatchesAllConditions, generateWebRequest);
        }

        // Common Stream starter
        private void StartSimpleStream(
            Func<ITweet, bool> processTweetDelegate,
            Func<ITweet, bool> tweetIsValid,
            Func<HttpWebRequest> generateWebRequest)
        {
            _streamResultGenerator.StartStream(TweetObjectProcessor(tweetIsValid, processTweetDelegate), generateWebRequest);
        }

        #endregion

        #region Return Tweet and Matching Keywords

        public override void StartStream(IToken token, Func<ITweet, List<string>, bool> processTweetDelegate)
        {
            Func<HttpWebRequest> generateWebRequest = () => token.GetQueryWebRequest(GenerateORFilterQuery(), HttpMethod.POST);

            StartStreamReportingMatchingKeywords(processTweetDelegate, tweet => true, generateWebRequest);
        }

        // Match if all condition are valid
        public void StartStreamMatchingAllConditions(IToken token, Action<ITweet, List<string>> processTweetDelegate)
        {
            StartStreamMatchingAllConditions(token, (tweet, matchingKeywords) =>
            {
                processTweetDelegate(tweet, matchingKeywords);
                return true;
            });
        }

        public void StartStreamMatchingAllConditions(IToken token, Func<ITweet, List<string>, bool> processTweetDelegate)
        {
            Func<HttpWebRequest> generateWebRequest = () => token.GetQueryWebRequest(GenerateANDFilterQuery(), HttpMethod.POST);
            StartStreamReportingMatchingKeywords(processTweetDelegate, MatchesAllConditions, generateWebRequest);
        }

        // Common starter
        private void StartStreamReportingMatchingKeywords(
           Func<ITweet, List<string>, bool> processTweetDelegate,
           Func<ITweet, bool> tweetIsValid,
           Func<HttpWebRequest> generateWebRequest)
        {
            _streamResultGenerator.StartStream(TweetObjectProcessor(tweetIsValid, processTweetDelegate), generateWebRequest);
        }

        #endregion

        #region Return Tweet and Matching Locations

        public void StartStream(IToken token, Action<ITweet, List<ILocation>> processTweetDelegate)
        {
            StartStream(token, (tweet, matchingTracks) =>
            {
                processTweetDelegate(tweet, matchingTracks);
                return true;
            });
        }

        public void StartStream(IToken token, Func<ITweet, List<ILocation>, bool> processTweetDelegate)
        {
            Func<HttpWebRequest> generateWebRequest = () => token.GetQueryWebRequest(GenerateORFilterQuery(), HttpMethod.POST);

            StartStreamReportingMatchingKeywords(processTweetDelegate, tweet => true, generateWebRequest);
        }

        // Match if all condition are valid
        public void StartStreamMatchingAllConditions(IToken token, Action<ITweet, List<ILocation>> processTweetDelegate)
        {
            StartStreamMatchingAllConditions(token, (tweet, matchingKeywords) =>
            {
                processTweetDelegate(tweet, matchingKeywords);
                return true;
            });
        }

        public void StartStreamMatchingAllConditions(IToken token, Func<ITweet, List<ILocation>, bool> processTweetDelegate)
        {
            Func<HttpWebRequest> generateWebRequest = () => token.GetQueryWebRequest(GenerateANDFilterQuery(), HttpMethod.POST);
            StartStreamReportingMatchingKeywords(processTweetDelegate, MatchesAllConditions, generateWebRequest);
        }

        // Common starter
        private void StartStreamReportingMatchingKeywords(
           Func<ITweet, List<ILocation>, bool> processTweetDelegate,
           Func<ITweet, bool> tweetIsValid,
           Func<HttpWebRequest> generateWebRequest)
        {
            _streamResultGenerator.StartStream(TweetObjectProcessor(tweetIsValid, processTweetDelegate), generateWebRequest);
        }

        #endregion

        #region Return Tweet, Matching Keywords and Matching Locations

        // Match if either condition is valid

        public void StartStream(IToken token, Action<ITweet, List<string>, List<ILocation>> processTweetDelegate)
        {
            StartStream(token, (tweet, matchingTracks, matchingLocations) =>
            {
                processTweetDelegate(tweet, matchingTracks, matchingLocations);
                return true;
            });
        }

        public void StartStream(IToken token, Func<ITweet, List<string>, List<ILocation>, bool> processTweetDelegate)
        {
            Func<HttpWebRequest> generateWebRequest = () => token.GetQueryWebRequest(GenerateORFilterQuery(), HttpMethod.POST);
            StartStreamReportingMatchingKeywordsAndLocations(processTweetDelegate, tweet => true, generateWebRequest);
        }

        // Match if all condition are valid

        public void StartStreamMatchingAllConditions(IToken token, Action<ITweet, List<string>, List<ILocation>> processTweetDelegate)
        {
            StartStreamMatchingAllConditions(token, (tweet, matchingKeywords, matchingLocations) =>
            {
                processTweetDelegate(tweet, matchingKeywords, matchingLocations);
                return true;
            });
        }

        public void StartStreamMatchingAllConditions(IToken token, Func<ITweet, List<string>, List<ILocation>, bool> processTweetDelegate)
        {
            Func<HttpWebRequest> generateWebRequest = () => token.GetQueryWebRequest(GenerateANDFilterQuery(), HttpMethod.POST);
            StartStreamReportingMatchingKeywordsAndLocations(processTweetDelegate, MatchesAllConditions, generateWebRequest);
        }

        // Common starter
        private void StartStreamReportingMatchingKeywordsAndLocations(
           Func<ITweet, List<string>, List<ILocation>, bool> processTweetDelegate,
           Func<ITweet, bool> tweetIsValid,
           Func<HttpWebRequest> generateWebRequest)
        {
            _streamResultGenerator.StartStream(TweetObjectProcessor(tweetIsValid, processTweetDelegate), generateWebRequest);
        }

        #endregion

        private void ProcessTrackedActions(ITweet tweet, out List<string> matchingKeywords, out List<ILocation> matchingLocations)
        {
            List<Action<ITweet>> matchingKeywordsActions;

            if (TryGetMatchingKeywordsAndRelatedActions(tweet, out matchingKeywords, out matchingKeywordsActions))
            {
                CallMultipleActions(tweet, matchingKeywordsActions);
            }

            matchingLocations = GetMatchedLocations(tweet.LocationCoordinates);

            if (matchingLocations != null)
            {
                for (int i = 0; i < matchingLocations.Count; ++i)
                {
                    if (_locations[matchingLocations[i]] != null)
                    {
                        _locations[matchingLocations[i]](tweet);
                    }
                }
            }

            // ReSharper disable AssignNullToNotNullAttribute
            if (tweet.Creator != null && ContainsFollow(tweet.Creator.Id) && _followingUserIds[tweet.Creator.Id] != null)
            {
                _followingUserIds[tweet.Creator.Id](tweet);
            }
            // ReSharper restore AssignNullToNotNullAttribute
        }

        private Func<string, bool> TweetObjectProcessor(
            Func<ITweet, bool> tweetIsValid,
            Func<ITweet, bool> processTweetDelegate)
        {
            return delegate(string jsonTweet)
            {
                ITweet tweet = GetTweetFromJson(jsonTweet);

                if (tweet == null || !tweetIsValid(tweet))
                {
                    return true;
                }

                List<string> matchingKeywords;
                List<ILocation> matchingLocations;
                ProcessTrackedActions(tweet, out matchingKeywords, out matchingLocations);

                return processTweetDelegate(tweet);
            };
        }

        private Func<string, bool> TweetObjectProcessor(
            Func<ITweet, bool> tweetIsValid,
            Func<ITweet, List<string>, bool> processTweetDelegate)
        {
            return delegate(string jsonTweet)
            {
                ITweet tweet = GetTweetFromJson(jsonTweet);

                if (tweet == null || !tweetIsValid(tweet))
                {
                    return true;
                }

                List<string> matchingKeywords;
                List<ILocation> matchingLocations;
                ProcessTrackedActions(tweet, out matchingKeywords, out matchingLocations);

                return processTweetDelegate(tweet, matchingKeywords);
            };
        }

        private Func<string, bool> TweetObjectProcessor(
            Func<ITweet, bool> tweetIsValid,
            Func<ITweet, List<ILocation>, bool> processTweetDelegate)
        {
            return delegate(string jsonTweet)
            {
                ITweet tweet = GetTweetFromJson(jsonTweet);

                if (tweet == null || !tweetIsValid(tweet))
                {
                    return true;
                }

                List<string> matchingKeywords;
                List<ILocation> matchingLocations;
                ProcessTrackedActions(tweet, out matchingKeywords, out matchingLocations);

                return processTweetDelegate(tweet, matchingLocations);
            };
        }

        private Func<string, bool> TweetObjectProcessor(
             Func<ITweet, bool> tweetIsValid,
             Func<ITweet, List<string>, List<ILocation>, bool> processTweetDelegate)
        {
            return delegate(string jsonTweet)
            {
                ITweet tweet = GetTweetFromJson(jsonTweet);

                if (tweet == null || !tweetIsValid(tweet))
                {
                    return true;
                }

                List<string> matchingKeywords;
                List<ILocation> matchingLocations;
                ProcessTrackedActions(tweet, out matchingKeywords, out matchingLocations);

                return processTweetDelegate(tweet, matchingKeywords, matchingLocations);
            };
        }

        #endregion
    }
}