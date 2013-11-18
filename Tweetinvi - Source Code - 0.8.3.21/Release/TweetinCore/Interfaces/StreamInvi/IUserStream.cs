using System;
using System.Collections.Generic;
using TweetinCore.Events;
using TweetinCore.Interfaces.TwitterToken;

namespace TweetinCore.Interfaces.StreamInvi
{
    /// <summary>
    /// Methods and Events tracking any action related with a User
    /// </summary>
    public interface IUserStream : IStream<object>, ITrackManager<ITweet>
    {
        /// <summary>
        /// Current TokenUser analyzed by the UserStream
        /// </summary>
        ITokenUser TokenUser { get; set; }

        // Any Tweet Received
        /// <summary>
        /// Event informing that a Tweet has been created by the TokenUser
        /// </summary>
        event EventHandler<GenericEventArgs<ITweet, ITokenUser>> TweetCreatedByMe;
        /// <summary>
        /// Event informing that a Tweet has been created by a user who is not the TokenUser
        /// </summary>
        event EventHandler<GenericEventArgs<ITweet>> TweetCreatedByAnyone;
        /// <summary>
        /// Event informing that a Tweet has been created by any user (TokenUser or someone else)
        /// </summary>
        event EventHandler<GenericEventArgs<ITweet>> TweetCreatedByAnyoneButMe;

        // Tracked Tweet Received
        /// <summary>
        /// Event informing that a Tracked Tweet has been created by the TokenUser
        /// 3rd parameter is the list of keywords that matched
        /// 4th parameter inform whether all the tracks have been matched
        /// </summary>
        event EventHandler<GenericEventArgs<ITweet, ITokenUser, List<string>, bool>> TrackedTweetCreatedByMe;
        /// <summary>
        /// Event informing that a Tracked Tweet has been created by a user who is not the TokenUser
        /// 2nd parameter is the list of keywords that matched
        /// 3rd parameter inform whether all the tracks have been matched
        /// </summary>
        event EventHandler<GenericEventArgs<ITweet, List<string>, bool>> TrackedTweetCreatedByAnyone;
        /// <summary>
        /// Event informing that a Tracked Tweet has been created by any user (TokenUser or someone else)
        /// 2nd parameter is the list of keywords that matched
        /// 3rd parameter inform whether all the tracks have been matched
        /// </summary>
        event EventHandler<GenericEventArgs<ITweet, List<string>, bool>> TrackedTweetCreatedByAnyoneButMe;

        // Any Tweet Deleted
        /// <summary>
        /// Event informing that a Tweet has been deleted by the TokenUser
        /// </summary>
        event EventHandler<GenericEventArgs<long?, ITokenUser>> TweetDeletedByMe;
        /// <summary>
        /// Event informing that a Tweet has been deleted by a user who is not the TokenUser
        /// </summary>
        event EventHandler<GenericEventArgs<long?, long?>> TweetDeletedByAnyone;
        /// <summary>
        /// Event informing that a Tweet has been deleted by any user (TokenUser or someone else)
        /// </summary>
        event EventHandler<GenericEventArgs<long?, long?>> TweetDeletedByAnyoneButMe;

        // Follow - Unfollow
        /// <summary>
        /// Event informing that the TokenUser is following a User
        /// </summary>
        event EventHandler<GenericEventArgs<ITokenUser, IUser>> FollowUser;
        /// <summary>
        /// Event informing that the TokenUser is being followed by another user
        /// </summary>
        event EventHandler<GenericEventArgs<ITokenUser, IUser>> FollowedByUser;
        /// <summary>
        /// Event informing that the Token user is not following another user
        /// </summary>
        event EventHandler<GenericEventArgs<ITokenUser, IUser>> UnFollowUser;
        /// <summary>
        /// Event informing that the TokenUser is not being followed by another user anymore
        /// </summary>
        [Obsolete("This is currently not managed by the Twitter UserStream API 1.1")]
        event EventHandler<GenericEventArgs<ITokenUser, IUser>> UnFollowedByUser;

        // Messages
        /// <summary>
        /// Event informing that a Message has been sent or received
        /// </summary>
        event EventHandler<GenericEventArgs<IMessage>> MessageSentOrReceived;
        /// <summary>
        /// Event informing that a Message has been sent
        /// </summary>
        event EventHandler<GenericEventArgs<IMessage, ITokenUser, IUser>> MessageSentByMeToX;
        /// <summary>
        /// Event informing that a Message has been received
        /// </summary>
        event EventHandler<GenericEventArgs<IMessage, ITokenUser, IUser>> MessageReceivedFromX;

        // Other
        /// <summary>
        /// Event informing that the list of friends has been received
        /// </summary>
        event EventHandler<GenericEventArgs<List<long>>> FriendIdsReceived;

        // Warnings
        /// <summary>
        /// Event informing that a Warning has been emitted
        /// </summary>
        event EventHandler<GenericEventArgs<string, string, short>> WarningReceived;

        /// <summary>
        /// Start the stream with the current TokenUser and does not send the response
        /// back to any delegate
        /// </summary>
        void StartStream();

        /// <summary>
        /// Start a stream with a renewed Token and TokenUser
        /// </summary>
        /// <param name="token"></param>
        void StartStream(IToken token);

        /// <summary>
        /// Start the stream with the current TokenUser and start
        /// the delegate with the retrieved response
        /// </summary>
        void StartStream(Func<object, bool> processObjectDelegate);
    }
}
