using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Streaminvi
{
    /// <summary>
    /// Provide a list of events for a specific user
    /// </summary>
    public class UserStream : BaseStream<object>, IUserStream
    {
        private readonly StreamTrackManager<ITweet> _trackManager;
        protected readonly Dictionary<string, Tuple<Action<object>, Func<Dictionary<string, object>, object>>> _eventActions;

        private ITokenUser _tokenUser;
        public ITokenUser TokenUser
        {
            get { return _tokenUser; }
            set
            {
                if (StreamState == StreamState.Stop)
                {
                    _tokenUser = value;
                }
            }
        }

        // Events
        public event EventHandler<GenericEventArgs<ITweet, ITokenUser>> TweetCreatedByMe;
        public event EventHandler<GenericEventArgs<ITweet>> TweetCreatedByAnyone;
        public event EventHandler<GenericEventArgs<ITweet>> TweetCreatedByAnyoneButMe;

        public event EventHandler<GenericEventArgs<ITweet, ITokenUser, List<string>, bool>> TrackedTweetCreatedByMe;
        public event EventHandler<GenericEventArgs<ITweet, List<string>, bool>> TrackedTweetCreatedByAnyone;
        public event EventHandler<GenericEventArgs<ITweet, List<string>, bool>> TrackedTweetCreatedByAnyoneButMe;

        public event EventHandler<GenericEventArgs<long?, ITokenUser>> TweetDeletedByMe;
        public event EventHandler<GenericEventArgs<long?, long?>> TweetDeletedByAnyone;
        public event EventHandler<GenericEventArgs<long?, long?>> TweetDeletedByAnyoneButMe;

        public event EventHandler<GenericEventArgs<ITokenUser, IUser>> FollowUser;
        public event EventHandler<GenericEventArgs<ITokenUser, IUser>> FollowedByUser;
        public event EventHandler<GenericEventArgs<ITokenUser, IUser>> UnFollowUser;
        public event EventHandler<GenericEventArgs<ITokenUser, IUser>> UnFollowedByUser;

        public event EventHandler<GenericEventArgs<IMessage>> MessageSentOrReceived;
        public event EventHandler<GenericEventArgs<IMessage, ITokenUser, IUser>> MessageSentByMeToX;
        public event EventHandler<GenericEventArgs<IMessage, ITokenUser, IUser>> MessageReceivedFromX;

        public event EventHandler<GenericEventArgs<List<long>>> FriendIdsReceived;
        public event EventHandler<GenericEventArgs<string, string, short>> WarningReceived;

        public UserStream()
        {
            _trackManager = new StreamTrackManager<ITweet>();

            _eventActions = new Dictionary<string, Tuple<Action<object>, Func<Dictionary<string, object>, object>>>();

            _eventActions.Add("event", new Tuple<Action<object>, Func<Dictionary<string, object>, object>>(
                               EventAction, p => p));
            _eventActions.Add("delete", new Tuple<Action<object>, Func<Dictionary<string, object>, object>>(
                               DeleteAction, p => p.Values.ElementAt(0)));
            _eventActions.Add("direct_message", new Tuple<Action<object>, Func<Dictionary<string, object>, object>>(
                               DirectMessageAction, p => p.Values.ElementAt(0)));
            _eventActions.Add("friends", new Tuple<Action<object>, Func<Dictionary<string, object>, object>>(
                               UserFriends, p => p.Values.ElementAt(0)));
            _eventActions.Add("warning", new Tuple<Action<object>, Func<Dictionary<string, object>, object>>(
                               WarningAction, p => p.Values.ElementAt(0)));
        }

        public UserStream(IToken token)
            : this()
        {
            _tokenUser = new TokenUser(token);
        }

        public UserStream(ITokenUser tokenUser)
            : this()
        {
            _tokenUser = tokenUser;
        }

        public void StartStream(IToken token)
        {
            _tokenUser = new TokenUser(token);
            StartStream();
        }

        public override void StartStream(IToken token, Func<object, bool> processObjectDelegate)
        {
            _tokenUser = new TokenUser(token);
            StartStream(processObjectDelegate);
        }

        public void StartStream()
        {
            StartStream((Func<object, bool>)null);
        }

        public void StartStream(Func<object, bool> processObjectDelegate)
        {
            if (_tokenUser == null)
            {
                return;
            }

            Func<HttpWebRequest> generateWebRequest = delegate
            {
                StringBuilder queryBuilder = new StringBuilder(Resources.Stream_UserStream);

                var track = QueryGeneratorHelper.GenerateFilterTrackRequest(_trackManager.Tracks.Keys.ToList());
                queryBuilder.Append(String.IsNullOrEmpty(track) ? "" : String.Format("&{0}", track));

                if (String.IsNullOrEmpty(track))
                {
                    return _tokenUser.ObjectToken.GetQueryWebRequest(queryBuilder.ToString(), HttpMethod.GET);
                }

                return _tokenUser.ObjectToken.GetQueryWebRequest(queryBuilder.ToString(), HttpMethod.POST);
            };

            Func<string, bool> generateTweetDelegate = x =>
            {
                var obj = _jsSerializer.Deserialize<Dictionary<string, object>>(x);
                if (obj == null)
                {
                    return true;
                }

                if (processObjectDelegate != null)
                {
                    processObjectDelegate(x);
                }

                if (_eventActions.ContainsKey(obj.Keys.ElementAt(0)))
                {
                    _eventActions[obj.Keys.ElementAt(0)].Item1.Invoke(
                        _eventActions[obj.Keys.ElementAt(0)].Item2(obj));
                }
                else
                {
                    // Retweeted is unique to Tweet
                    if (obj.ContainsKey("retweeted"))
                    {
                        TweetCreatedAction(obj);
                    }
                }

                // The information sent from Twitter was not the expected object
                return true;
            };

            _streamResultGenerator.StartStream(generateTweetDelegate, generateWebRequest);
        }

        protected virtual void TweetCreatedAction(object tweetObject)
        {
            Tweet t = new Tweet(tweetObject as Dictionary<string, object>);
            Debug.WriteLine(t.Text);

            List<string> matchingTracks = null;
            bool matchesAll = false;
            var tracked = _trackManager.TracksCount != 0;
            if (tracked)
            {
                matchingTracks = _trackManager.MatchingTracks(t.Text);
                matchesAll = matchingTracks.Count == Tracks.Count;
                tracked = matchingTracks.Any();
            }

            this.Raise(TweetCreatedByAnyone, t);
            if (tracked)
            {
                this.Raise(TrackedTweetCreatedByAnyone, t, matchingTracks, matchesAll);
            }

            if (t.Creator.Id == _tokenUser.Id)
            {
                this.Raise(TweetCreatedByMe, t, _tokenUser);
                if (tracked)
                {
                    this.Raise(TrackedTweetCreatedByMe, t, _tokenUser, matchingTracks, matchesAll);
                }
                return;
            }

            this.Raise(TweetCreatedByAnyoneButMe, t);
            if (tracked)
            {
                this.Raise(TrackedTweetCreatedByAnyoneButMe, t, matchingTracks, matchesAll);
            }
        }

        protected virtual void EventAction(object eventObject)
        {
            var eventInfo = eventObject as Dictionary<string, object>;

            if (eventInfo == null)
            {
                return;
            }

            if (eventInfo["event"] as string == "follow")
            {
                EventFollow(eventInfo);
            }

            if (eventInfo["event"] as string == "unfollow")
            {
                EventUnFollow(eventInfo);
            }
        }

        protected virtual void EventFollow(Dictionary<string, object> eventInfo)
        {
            IUser source = new User(eventInfo["source"] as Dictionary<string, object>);
            IUser target = new User(eventInfo["target"] as Dictionary<string, object>);

            if (source.Id == _tokenUser.Id)
            {
                this.Raise(FollowUser, _tokenUser, target);

                if (target.Id != null)
                {
                    _tokenUser.FriendIds.Add((long)target.Id);
                }
            }

            if (target.Id == _tokenUser.Id)
            {
                this.Raise(FollowedByUser, _tokenUser, source);
                if (source.Id != null)
                {
                    _tokenUser.FollowerIds.Add((long)source.Id);
                }
            }
        }

        protected virtual void EventUnFollow(Dictionary<string, object> eventInfo)
        {
            IUser source = new User(eventInfo["source"] as Dictionary<string, object>);
            IUser target = new User(eventInfo["target"] as Dictionary<string, object>);

            if (source.Id == _tokenUser.Id)
            {
                this.Raise(UnFollowUser, _tokenUser, target);

                if (target.Id != null)
                {
                    _tokenUser.FriendIds.Remove((long)target.Id);
                }
            }

            if (target.Id == _tokenUser.Id)
            {
                this.Raise(UnFollowedByUser, _tokenUser, source);

                if (source.Id != null)
                {
                    _tokenUser.FollowerIds.Remove((long)source.Id);
                }
            }
        }

        protected virtual void DeleteAction(object deleteInfo)
        {
            var deleteType = deleteInfo as Dictionary<string, object>;
            if (deleteType != null && deleteType.ContainsKey("status"))
            {
                var deletedTweetDetails = deleteType["status"] as Dictionary<string, object>;
                DeletedTweet(deletedTweetDetails);
                return;
            }
            Console.WriteLine(deleteInfo);
        }

        protected virtual void DeletedTweet(Dictionary<string, object> deletedTweet)
        {
            string idStr = deletedTweet["id_str"] as string;
            string userIdStr = deletedTweet["user_id_str"] as string;

            if (idStr == null || userIdStr == null)
            {
                return;
            }

            long tweetId = Int64.Parse(idStr);
            long userId = Int64.Parse(userIdStr);

            this.Raise(TweetDeletedByAnyone, tweetId, userId);
            if (userId == _tokenUser.Id)
            {
                this.Raise(TweetDeletedByMe, tweetId, _tokenUser);
                return;
            }

            this.Raise(TweetDeletedByAnyoneButMe, tweetId, userId);
        }

        protected virtual void DirectMessageAction(object directMessage)
        {
            Message message = new Message(directMessage as Dictionary<string, object>);

            this.Raise(MessageSentOrReceived, message);
            if (message.Sender.Id == _tokenUser.Id)
            {
                this.Raise(MessageSentByMeToX, message, _tokenUser, message.Receiver);
            }

            if (message.Receiver.Id == _tokenUser.Id)
            {
                this.Raise(MessageReceivedFromX, message, _tokenUser, message.Sender);
            }
        }

        protected virtual void UserFriends(object friends)
        {
            if (friends == null || (friends as ArrayList) == null)
            {
                return;
            }
            
            var followingIds = friends as ArrayList;

            _tokenUser.FriendIds = new List<long>();
            foreach (var followingId in followingIds)
            {
                _tokenUser.FriendIds.Add((int)followingId);
            }

            this.Raise(FriendIdsReceived, new GenericEventArgs<List<long>>(_tokenUser.FriendIds));
        }

        private void WarningAction(object warningObject)
        {
            var warning = warningObject as Dictionary<string, object>;
            if (warning == null)
            {
                return;
            }

            this.Raise(WarningReceived, 
                warning["code"] as string, 
                warning["message"] as string,
                (short)(int)warning["percent_full"]);
        }

        public int TracksCount
        {
            get { return _trackManager.TracksCount; }
        }

        public int MaxTracks
        {
            get { return _trackManager.MaxTracks; }
        }

        public Dictionary<string, Action<ITweet>> Tracks
        {
            get { return _trackManager.Tracks; }
        }

        public void AddTrack(string track, Action<ITweet> trackReceived = null)
        {
            _trackManager.AddTrack(track, trackReceived);
        }

        public void RemoveTrack(string track)
        {
            _trackManager.RemoveTrack(track);
        }

        public bool ContainsTrack(string track)
        {
            return _trackManager.ContainsTrack(track);
        }

        public void ClearTracks()
        {
            _trackManager.ClearTracks();
        } 
    }
}