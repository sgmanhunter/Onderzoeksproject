using System.Collections.Generic;
using TweetinCore.Extensions;

namespace TweetinCore.Exception
{
    /// <summary>
    /// Host information sent by Twitter concerning an error
    /// </summary>
    public interface ITwitterExceptionInfo
    {
        /// <summary>
        /// Error code sent by Twitter -- https://dev.twitter.com/docs/error-codes-responses
        /// </summary>
        int Code { get; }

        /// <summary>
        /// Message sent by Twitter
        /// </summary>
        string Message { get; }
    }

    public class TwitterExceptionInfo : ITwitterExceptionInfo
    {
        public int Code { get; private set; }
        public string Message { get; private set; }

        public TwitterExceptionInfo(Dictionary<string, object> error)
        {
            Code = error.GetProp<int>("code");
            Message = error.GetProp<string>("message");
        }
    }
}
