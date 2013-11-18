using System;
using System.Linq;
using System.Net;
using TweetinCore.Exception;

namespace Tweetinvi
{
    public interface ITwitterContext
    {
        /// <summary>
        /// Return the last Exception if it is either a 
        /// TwitterException or a TweetinviException
        /// </summary>
        Exception LastActionException { get; }

        /// <summary>
        /// Exception from Twitter or the connection
        /// </summary>
        TwitterException LastActionTwitterException { get; }

        /// <summary>
        /// Exception from Tweetinvi
        /// </summary>
        Exception LastActionTweetinviException { get; }
        
        /// <summary>
        /// Invoke an Action and register information about this action
        /// </summary>
        /// <returns>Has the action been successfully performed</returns>
        bool TryInvokeAction(Action action);

        /// <summary>
        /// Invoke a Func that returns a value
        /// </summary>
        /// <param name="action">Action to perform</param>
        /// <param name="result">Result</param>
        /// <returns>Has the action been successfully performed</returns>
        bool TryInvokeAction<T>(Func<T> action, out T result);
    }

    public class TwitterContext : ITwitterContext
    {
        private readonly ITwitterExceptionTracker _twitterExceptionTracker;

        public TwitterContext()
        {
            _twitterExceptionTracker = new TwitterExceptionTracker();
        }

        public Exception LastActionException
        {
            get
            {
                return LastActionTwitterException ?? LastActionTweetinviException;
            }
        }
        public Exception LastActionTweetinviException
        {
            get
            {
                var lastTweetinviException = _twitterExceptionTracker.TweetinviExceptions.LastOrDefault();

                if (lastTweetinviException != null)
                {
                    return lastTweetinviException.Item2;
                }

                return null;
            }
        }
        public TwitterException LastActionTwitterException
        {
            get
            {
                var lastExceptionInfo = _twitterExceptionTracker.TwitterExceptions.LastOrDefault();

                if (lastExceptionInfo != null)
                {
                    return _twitterExceptionTracker.TwitterExceptions.Last().Item2;
                }

                return null;
            }
        }

        public bool TryInvokeAction(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                return OperationFailed(action, ex);
            }

            return OperationSucceeded(action);
        }

        public bool TryInvokeAction<T>(Func<T> action, out T result)
        {
            try
            {
                result = action();
            }
            catch (Exception ex)
            {
                return OperationFailed(action, ex, out result);
            }

            return OperationSucceeded(action);
        }

        private bool OperationSucceeded(object action)
        {
            // Register all the operation and their "related exception"
            // If an operation has been a success we specify that there 
            // was no exception in the last action

            _twitterExceptionTracker.ActionSucceed(action);
            return true;
        }
        private bool OperationFailed(object action, Exception ex)
        {
            if (ex is WebException)
            {
                _twitterExceptionTracker.ActionFailed(action, ex as WebException);
            }
            else
            {
                _twitterExceptionTracker.ActionFailed(action, ex);
            }

            return false;
        }
        private bool OperationFailed<T>(object action, Exception ex, out T expectedResult)
        {
            expectedResult = default(T);
            return OperationFailed(action, ex);
        }

        // For later -- Transactions : when an operation is performed we want to do the opposite 
        // If the global scope failed, we consequently need to keep track of the arguments
        private bool TryInvokeAction<P1>(Action<P1> action, P1 param1)
        {
            try
            {
                action(param1);
            }
            catch (Exception ex)
            {
                _twitterExceptionTracker.ActionFailed(action, ex);
                return false;
            }

            return OperationSucceeded(action);
        }
        private bool TryInvokeAction<P1, T>(Func<P1, T> action, P1 param1, out T result)
        {
            try
            {
                result = action(param1);
            }
            catch (Exception ex)
            {
                return OperationFailed(action, ex, out result);
            }

            return OperationSucceeded(action);
        }
    }
}
