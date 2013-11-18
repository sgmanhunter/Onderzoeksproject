using System;
using System.Collections.Generic;
using Streaminvi.Helpers;
using TweetinCore.Interfaces;
using TweetinCore.Interfaces.StreamInvi;
using TweetinCore.Interfaces.TwitterToken;

namespace Streaminvi
{
    public abstract class BaseTrackedStream : BaseStream<ITweet>, ITrackManager<ITweet>, ITrackedStream
    {
        protected readonly IStreamTrackManager<ITweet> _trackManager;

        protected BaseTrackedStream(int maxTrack = Int32.MaxValue)
        {
            _trackManager = new StreamTrackManager<ITweet>(maxTrack);
        }

        public virtual void StartStream(IToken token, Action<ITweet, List<string>> processTweetDelegate)
        {
            StartStream(token, (tweet, matchingTracks) =>
            {
                processTweetDelegate(tweet, matchingTracks);
                return true;
            });
        }

        public abstract void StartStream(IToken token, Func<ITweet, List<string>, bool> processTweetDelegate);

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