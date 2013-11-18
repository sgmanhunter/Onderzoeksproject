using System;
using System.Collections.Generic;

namespace TweetinCore.Interfaces.StreamInvi
{
    public interface IStreamTrackManager<T> : ITrackManager<T>, ITrackStringAnalyzer
    {
        List<string> MatchingTracks(string input, out List<Action<T>> actions);
    }
}
