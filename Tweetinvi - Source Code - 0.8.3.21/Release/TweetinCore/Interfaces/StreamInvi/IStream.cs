using System;
using TweetinCore.Enum;
using TweetinCore.Events;
using TweetinCore.Interfaces.TwitterToken;

namespace TweetinCore.Interfaces.StreamInvi
{
    /// <summary>
    /// Set of methods to control and manage a stream
    /// </summary>
    public interface IStream<T>
    {
        event EventHandler StreamStarted;
        event EventHandler StreamResumed;
        event EventHandler StreamPaused;
        event EventHandler<GenericEventArgs<System.Exception>> StreamStopped;

        /// <summary>
        /// Get the current state of the stream
        /// </summary>
        StreamState StreamState { get; }

        /// <summary>
        /// Start an infinite stream (can be stopped from PauseStream/StopStream)
        /// Foreach tweet received, you will get the following information : 
        /// Tweet
        /// </summary>
        /// <param name="token">Token against which the stream is using the API</param>
        /// <param name="processObjectDelegate">Method that will be called foreach object</param>
        void StartStream(IToken token, Action<T> processObjectDelegate);

        /// <summary>
        /// Start a stream that you can stop from the processTweetDelegate
        /// Foreach tweet received, you will get the following information : 
        /// Tweet
        /// </summary>
        /// <param name="token">Token against which the stream is using the API</param>
        /// <param name="processObjectDelegate">
        /// Method that will be called foreach object 
        /// returns whether the stream should continue
        /// </param>
        void StartStream(IToken token, Func<T, bool> processObjectDelegate);
     
        /// <summary>
        /// Resume a stopped Stream
        /// </summary>
        void ResumeStream();

        /// <summary>
        /// Pause a running Stream
        /// </summary>
        void PauseStream();

        /// <summary>
        /// Stop a running or paused stream
        /// </summary>
        void StopStream();
    }
}
