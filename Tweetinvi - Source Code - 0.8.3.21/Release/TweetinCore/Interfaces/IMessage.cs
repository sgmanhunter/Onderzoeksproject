using System;
using System.Collections.Generic;
using TweetinCore.Interfaces.TwitterToken;

namespace TweetinCore.Interfaces
{
    /// <summary>
    /// Message that can be sent privately between Twitter users
    /// </summary>
    public interface IMessage : ITwitterObject, IEquatable<IMessage>
    {
        #region IMessage Properties

        /// <summary>
        /// Id of the Message
        /// </summary>
        long? Id { get; set; }

        /// <summary>
        /// Text contained in the message
        /// </summary>
        string Text { get; set; }

        /// <summary>
        /// Creation date of the message
        /// </summary>
        DateTime CreatedAt { get; set; }

        /// <summary>
        /// User who sent the message
        /// </summary>
        IUser Sender { get; set; }

        /// <summary>
        /// Receiver of the message
        /// </summary>
        IUser Receiver { get; set; }

        #endregion

        #region IMessage Methods

        bool Publish(IToken token);

        /// <summary>
        /// Send a message to a user
        /// </summary>
        /// <param name="receiver">Follower to send the message to</param>
        /// <param name="token">Token used to send the message</param>
        /// <returns>If the message has successfully be sent</returns>
        bool Publish(IUser receiver = null, IToken token = null);

        /// <summary>
        /// Send a message to a user
        /// </summary>
        /// <param name="receiverId">The user id of a Follower to send the message to</param>
        /// <param name="token">Token used to send the message</param>
        /// <returns>If the message has successfully be sent</returns>
        bool Publish(long receiverId, IToken token = null);

        bool Destroy(IToken token = null);

        #endregion
    }
}
