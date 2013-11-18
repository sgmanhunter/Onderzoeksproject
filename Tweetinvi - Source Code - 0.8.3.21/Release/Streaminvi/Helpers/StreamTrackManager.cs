using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TweetinCore.Interfaces.StreamInvi;

namespace Streaminvi.Helpers
{
    /// <summary>
    /// List of methods to be used to Track keywords
    /// </summary>
    public class StreamTrackManager<T> : IStreamTrackManager<T>
    {
        protected bool _refreshTracking;
        protected readonly int _maxTracks;
        readonly Regex _getStringWordsRegex = new Regex(@"(?:[^\s\p{P}]|\#|\w)+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        // Stores the entire track

        private readonly Dictionary<string, Action<T>> _tracks;
        public Dictionary<string, Action<T>> Tracks
        {
            get { return _tracks; }
        }

        // Stores the keywords included in a track
        protected readonly List<string[]> _tracksKeywords;

        public int TracksCount
        {
            get { return _tracks.Count; }
        }

        public int MaxTracks
        {
            get { return _maxTracks; }
        }

        public StreamTrackManager()
            : this(Int32.MaxValue)
        {
        }

        public StreamTrackManager(int maxTrack)
        {
            _maxTracks = maxTrack;

            _tracks = new Dictionary<string, Action<T>>();
            _tracksKeywords = new List<string[]>();
        }

        // Twitter API Tracking
        public void AddTrack(string track, Action<T> trackReceived = null)
        {
            if (_tracks.Count < MaxTracks)
            {
                string lowerTrack = track.ToLower();
                string[] trackSplit = lowerTrack.Split(' ');

                lock (this) // Not allowed to add multiple at the same time
                {
                    if (!_tracks.Keys.Contains(lowerTrack))
                    {
                        _tracks.Add(lowerTrack, trackReceived);
                        _tracksKeywords.Add(trackSplit);
                    }
                }

                _refreshTracking = true;
            }
        }

        public void RemoveTrack(string track)
        {
            string lowerTrack = track.ToLower();

            lock (this) // Not allowed to remove multiple at the same time
            {
                if (_tracks.Keys.Contains(lowerTrack))
                {
                    string[] trackSplit = lowerTrack.Split(' ');

                    _tracksKeywords.RemoveAll(x => x.Length == trackSplit.Length &&
                                                  !x.Except(trackSplit).Any());
                    _tracks.Remove(lowerTrack);
                }
            }

            _refreshTracking = true;
        }

        public bool ContainsTrack(string track)
        {
            return _tracks.Keys.Contains(track.ToLower());
        }

        public void ClearTracks()
        {
            _tracks.Clear();
            _tracksKeywords.Clear();
            _refreshTracking = true;
        }

        // Manual Tracking
        // private string[] _uniqueKeywordsArray;
        private HashSet<string> _uniqueKeywordsHashSet;
        private string[][] _tracksKeywordsArray;

        /// <summary>
        /// Creates Arrays of string that cache information for later comparisons
        /// This is required for performances improvement
        /// </summary>
        private void RefreshTracking()
        {
            // List of keywords associated with a track
            _tracksKeywordsArray = _tracksKeywords.ToArray();
            _uniqueKeywordsHashSet = new HashSet<string>();
            for (int i = 0; i < _tracksKeywordsArray.Length; ++i)
            {
                _uniqueKeywordsHashSet.UnionWith(_tracksKeywordsArray[i]);
            }

            // _uniqueKeywordsArray = _uniqueKeywordsHashSet.ToArray();
        }

        public bool Matches(string input)
        {
            lock (this)
            {
                return _matchingTracks(input).Any();
            }
        }

        public bool MatchesAll(string input)
        {
            lock (this)
            {
                return _matchingTracks(input).Count == _tracks.Count;
            }
        }

        private List<string> _matchingCharacters(string input)
        {
            // This behavior allows live refresh of the tracking
            // But reduces considerably the performances of the first test
            // First attempt ~= 10 x Later Attemps
            if (_refreshTracking)
            {
                RefreshTracking();
                _refreshTracking = false;
            }

            List<string> matchingKeywords = new List<string>();
            for (int i = 0; i < _uniqueKeywordsHashSet.Count; ++i)
            {
                if (input.Contains(String.Format("{0}", _uniqueKeywordsHashSet.ElementAt(i))))
                {
                    matchingKeywords.Add(_uniqueKeywordsHashSet.ElementAt(i));
                }
            }

            List<string> result = new List<string>();
            for (int i = 0; i < _tracksKeywordsArray.Length; ++i)
            {
                bool trackIsMatching = true;
                for (int j = 0; j < _tracksKeywordsArray[i].Length && trackIsMatching; ++j)
                {
                    trackIsMatching = matchingKeywords.Contains(_tracksKeywordsArray[i][j]);
                }

                if (trackIsMatching)
                {
                    result.Add(_tracks.Keys.ElementAt(i));
                }
            }

            return result;
        }
        public List<string> MatchingCharacters(string input)
        {
            lock (this)
            {
                return _matchingCharacters(input);
            }
        }

        private List<string> _matchingTracks(string input, List<Action<T>> actions = null)
        {
            // Missing match of # for simple tracked keywords
            if (String.IsNullOrEmpty(input) || _tracks.Count == 0)
            {
                return new List<string>();
            }

            // This behavior allows live refresh of the tracking
            // But reduces considerably the performances of the first test
            if (_refreshTracking)
            {
                RefreshTracking();
                _refreshTracking = false;
            }

            var matchingKeywords = _getStringWordsRegex.Matches(input.ToLower())
                                    .OfType<Match>()
                                    .Where(match =>
                                    {
                                        if (match.Value[0] == '#')
                                        {
                                            return _uniqueKeywordsHashSet.Contains(match.Value) ||
                                                   _uniqueKeywordsHashSet.Contains(match.Value.Substring(1, match.Value.Length - 1));
                                        }

                                        return _uniqueKeywordsHashSet.Contains(match.Value);
                                    })
                                    .Select(x => x.Value).ToArray();

            List<string> result = new List<string>();
            for (int i = 0; i < _tracksKeywordsArray.Length; ++i)
            {
                var isMatching = true;
                for (int j = 0; j < _tracksKeywordsArray[i].Length && isMatching; ++j)
                {
                    if (_tracksKeywordsArray[i][j][0] != '#')
                    {
                        isMatching = matchingKeywords.Contains(_tracksKeywordsArray[i][j]) ||
                                     matchingKeywords.Contains(String.Format("#{0}", _tracksKeywordsArray[i][j]));
                    }
                    else
                    {
                        isMatching = matchingKeywords.Contains(_tracksKeywordsArray[i][j]);
                    }
                }

                if (isMatching)
                {
                    string keyword = _tracks.Keys.ElementAt(i);
                    result.Add(keyword);

                    if (actions != null && _tracks.ElementAt(i).Value != null)
                    {
                        actions.Add(_tracks.ElementAt(i).Value);
                    }
                }
            }

            return result;
        }
        
        public List<string> MatchingTracks(string input)
        {
            lock (this)
            {
                return _matchingTracks(input);
            }
        }
        
        public List<string> MatchingTracks(string input, out List<Action<T>> actions)
        {
            lock (this)
            {
                actions = new List<Action<T>>();
                return _matchingTracks(input, actions);
            }
        }
    }
}