using System;
using System.Web.Script.Serialization;
using Streaminvi.Helpers;
using TweetinCore.Enum;
using TweetinCore.Events;
using TweetinCore.Interfaces.StreamInvi;
using TweetinCore.Interfaces.TwitterToken;

namespace Streaminvi
{
    /// <summary>
    /// Base behavior of a Stream
    /// </summary>
    public abstract class BaseStream<T> : IStream<T>
    {
        
        public event EventHandler StreamStarted
        {
            add { _streamResultGenerator.StreamStarted += value; }
            remove { _streamResultGenerator.StreamStarted -= value; }
        }
        public event EventHandler StreamResumed
        {
            add { _streamResultGenerator.StreamResumed += value; }
            remove { _streamResultGenerator.StreamResumed -= value; }
        }
        public event EventHandler StreamPaused
        {
            add { _streamResultGenerator.StreamPaused += value; }
            remove { _streamResultGenerator.StreamPaused -= value; }
        }
        public event EventHandler<GenericEventArgs<Exception>> StreamStopped
        {
            add { _streamResultGenerator.StreamStopped += value; }
            remove { _streamResultGenerator.StreamStopped -= value; }
        }
        
        protected readonly JavaScriptSerializer _jsSerializer;
        protected readonly StreamResultGenerator _streamResultGenerator;
        
        public StreamState StreamState
        {
            get { return _streamResultGenerator.StreamState; }
        }

        protected BaseStream()
        {
            _jsSerializer = new JavaScriptSerializer();
            _streamResultGenerator = new StreamResultGenerator();
        }

        #region IStream Members
        
        public virtual void StartStream(IToken token, Action<T> processObjectDelegate)
        {
            StartStream(token, x =>
            {
                processObjectDelegate(x);
                return true;
            });
        }

        public abstract void StartStream(IToken token, Func<T, bool> processObjectDelegate);

        public void ResumeStream()
        {
            _streamResultGenerator.ResumeStream();
        }

        public void PauseStream()
        {
            _streamResultGenerator.PauseStream();
        }

        public void StopStream()
        {
            _streamResultGenerator.StopStream();
        } 

        #endregion
    }
}
