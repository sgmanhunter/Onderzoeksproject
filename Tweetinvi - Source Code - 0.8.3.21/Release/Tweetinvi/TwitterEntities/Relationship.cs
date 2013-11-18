using System;
using System.Collections.Generic;
using TweetinCore.Extensions;
using TweetinCore.Interfaces;
using TweetinCore.Interfaces.TwitterToken;
using Tweetinvi.Helpers;

namespace Tweetinvi.TwitterEntities
{
    class Relationship : IRelationship
    {
        public IUser Source { get; private set; }
        public IUser Target { get; private set; }
        public bool Following { get; private set; }
        public bool FollowedBy { get; private set; }
        public bool NotificationsEnabled { get; private set; }
        public bool Blocking { get; private set; }
        public bool WantRetweets { get; private set; }
        public bool AllReplies { get; private set; }
        public bool MarkedSpam { get; private set; }
        public bool CanDm { get; private set; }

        public Relationship(Dictionary<string, object> relationshipObject, IToken token = null)
        {
            Populate(relationshipObject, token);
        }

        protected void Populate(Dictionary<string, object> relationshipObject, IToken token)
        {
            var relationship = relationshipObject.GetProp<Dictionary<string, object>>("relationship");

            var source = relationship.GetProp<Dictionary<string, object>>("source");
            long? sourceId = Int64.Parse(source.GetProp<string>("id_str"));

            Source = new User(sourceId);

            var target = relationship.GetProp<Dictionary<string, object>>("target");
            long? targetId = Int64.Parse(target.GetProp<string>("id_str"));

            Target = new User(targetId);

            Following = source.GetProp<bool>("following");
            FollowedBy = source.GetProp<bool>("followed_by");
            NotificationsEnabled = source.GetProp<bool>("notifications_enabled");
            Blocking = source.GetProp<bool>("blocking");
            WantRetweets = source.GetProp<bool>("want_retweets");
            AllReplies = source.GetProp<bool>("all_replies");
            MarkedSpam = source.GetProp<bool>("marked_spam");
            CanDm = source.GetProp<bool>("can_dm");
        }
    }
}
