using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using TweetinCore.Exception;

namespace Tweetinvi
{
    public interface ITwitterExceptionTracker
    {
        void ActionSucceed(object action);
        void ActionFailed(object action, Exception ex);
        TwitterException ActionFailed(object action, WebException wex);

        /// <summary>
        /// Exception that should not be related with Twitter 
        /// </summary>
        List<Tuple<object, Exception, DateTime>> TweetinviExceptions { get; }

        /// <summary>
        /// Exception sent by Twitter
        /// </summary>
        List<Tuple<object, TwitterException>> TwitterExceptions { get; }
    }

    public class TwitterExceptionTracker : ITwitterExceptionTracker
    {
        public List<Tuple<object, Exception, DateTime>> TweetinviExceptions { get; private set; }
        public List<Tuple<object, TwitterException>> TwitterExceptions { get; private set; }
        
        // Constructor
        public TwitterExceptionTracker()
        {
            TweetinviExceptions = new List<Tuple<object, Exception, DateTime>>();
            TwitterExceptions = new List<Tuple<object, TwitterException>>();
        }

        public void ActionSucceed(object action)
        {
            TweetinviExceptions.Add(new Tuple<object, Exception, DateTime>(action, null, DateTime.Now));
        }

        public virtual void ActionFailed(object action, Exception ex)
        {
            TweetinviExceptions.Add(new Tuple<object, Exception, DateTime>(action, ex, DateTime.Now));

            // This is the place to put a logger in your application
            // If you need to know why your application has received
            // An error in release mode
#if DEBUG
            Debug.WriteLine(ex.Message);
            Debug.WriteLine(ex.StackTrace);
#endif
        }

        public virtual TwitterException ActionFailed(object action, WebException wex)
        {
            var twitterException = new TwitterException(wex);
            TwitterExceptions.Add(new Tuple<object, TwitterException>(action, twitterException));
            
            return twitterException;
        }
    }
}
