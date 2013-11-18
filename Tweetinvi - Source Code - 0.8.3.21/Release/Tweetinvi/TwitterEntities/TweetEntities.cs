using System;
using System.Collections.Generic;
using System.Linq;
using TweetinCore.Extensions;
using TweetinCore.Interfaces;
using Tweetinvi.Helpers;

namespace Tweetinvi.TwitterEntities
{
    /// <summary>
    /// Class storing multiple types of TweetEntities
    /// https://dev.twitter.com/docs/tweet-entities
    /// </summary>
    public class TweetEntities : ITweetEntities
    {
        #region Private Attributes

        /// <summary>
        /// Collection of urls associated with a tweet
        /// </summary>
        private List<IUrlEntity> _urls;

        /// <summary>
        /// Collection of medias associated with a tweet
        /// </summary>
        private List<IMediaEntity> _medias;

        /// <summary>
        /// Collection of tweets mentioning this tweet
        /// </summary>
        private List<IUserMentionEntity> _userMentions;

        /// <summary>
        /// Collection of hashtags associated with a Tweet
        /// </summary>
        private List<IHashTagEntity> _hashtags;
        
        #endregion

        #region Public Attributes
        public List<IUrlEntity> Urls
        {
            get { return _urls; }
            set
            {
                _urls = value;
            }
        }

        public List<IMediaEntity> Medias
        {
            get { return _medias; }
            set
            {
                _medias = value;
            }
        }

        public List<IUserMentionEntity> UserMentions
        {
            get { return _userMentions; }
            set { _userMentions = value; }
        }

        public List<IHashTagEntity> Hashtags
        {
            get { return _hashtags; }
            set { _hashtags = value; }
        }
        #endregion

        #region Constructors

        /// <summary>
        /// Creates an empty store of Entities
        /// </summary>
        public TweetEntities() { }

        /// <summary>
        /// Creates a store of entities based on information retrieved from Twitter
        /// </summary>
        /// <param name="entities">Twitter information</param>
        public TweetEntities(Dictionary<String, object> entities)
        {
            Urls = (from x in entities.GetPropCollection("urls")
                    select new UrlEntity(x) as IUrlEntity)
                    .ToList();

            Medias = (from x in entities.GetPropCollection("media")
                     select new MediaEntity(x) as IMediaEntity)
                     .ToList();

            UserMentions = (from x in entities.GetPropCollection("user_mentions")
                            select new UserMentionEntity(x) as IUserMentionEntity)
                            .ToList();

            Hashtags = (from x in entities.GetPropCollection("hashtags")
                         select new HashTagEntity(x) as IHashTagEntity)
                         .ToList();
        } 
        #endregion
    }
}
