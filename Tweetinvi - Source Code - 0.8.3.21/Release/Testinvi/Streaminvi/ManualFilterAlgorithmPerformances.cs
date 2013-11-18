using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Streaminvi.Helpers;
using TweetinCore.Events;
using TweetinCore.Interfaces;

namespace Testinvi.Streaminvi
{
    [TestClass]
    public class ManualFilterAlgorithmPerformances
    {
        #region Algorithm SpeedTests

        [TestMethod]
        public void CompareSpeedTestBetweenFirstAndLaterQueries()
        {
            var trackManager = new StreamTrackManager<ITweet>();

            // ReSharper disable once NotAccessedVariable
            int strDetected = 0;
            Action<ITweet> found = str =>
            {
                ++strDetected;
            };

            trackManager.AddTrack("hello boy", found);
            trackManager.AddTrack("#plop", found);
            trackManager.AddTrack("hello #boy", found);
            trackManager.AddTrack("hello", found);

            string input = Regex.Escape("#hello I am pretty happy with what you’ve #plop just told me concerning syria " +
                                        "and other topics like obame, linvi, Tweetinvi, c#, api, and streaming as well");

            string[] inputKeywords = input.Split(' ');
            for (int i = 0; i < inputKeywords.Length; ++i)
            {
                trackManager.AddTrack(inputKeywords[i]);

                if (i + 3 < inputKeywords.Length)
                {
                    trackManager.AddTrack(String.Format("{0} {1}", inputKeywords[i], inputKeywords[i + 2]), found);
                }
            }

            for (int i = 0; i < 400; ++i)
            {
                trackManager.AddTrack(String.Format("plop {0}", i), found);
            }

            Stopwatch s = new Stopwatch();
            s.Start();
            var result = trackManager.MatchingTracks(input);
            s.Stop();

            Stopwatch s2 = new Stopwatch();
            s2.Start();
            var result2 = trackManager.MatchingTracks(input);
            s2.Stop();

            trackManager.AddTrack("JustResetMe!");
            trackManager.RemoveTrack("JustResetMe!");

            Stopwatch s3 = new Stopwatch();
            s3.Start();
            var result3 = trackManager.MatchingCharacters(input);
            s3.Stop();

            Stopwatch s4 = new Stopwatch();
            s4.Start();
            var result4 = trackManager.MatchingCharacters(input);
            s4.Stop();

            Console.WriteLine(s.Elapsed);
            Console.WriteLine(s2.Elapsed);
            Console.WriteLine(s3.Elapsed);
            Console.WriteLine(s4.Elapsed);

            Assert.AreEqual(result.Count, result2.Count);
            Assert.AreEqual(result3.Count, result4.Count);
        }

        [TestMethod]
        public void EventVsDelegate()
        {
            // Test events
            int x = 0;
            EventHandler<GenericEventArgs<string>> e = (sender, args) =>
            {
                // Do nothing and test!
                ++x;
            };

            Stopwatch s = new Stopwatch();
            s.Start();
            for (int i = 0; i < 100000; ++i)
            {
                e(null, new GenericEventArgs<string>("hello"));
            }
            s.Stop();

            int y = 0;
            Action<string> action = s1 =>
            {
                ++y;
            };

            Stopwatch s2 = new Stopwatch();
            s2.Start();
            for (int i = 0; i < 100000; ++i)
            {
                action("hello");
            }
            s2.Stop();

            Assert.AreEqual(x, y);
            Console.WriteLine(s.Elapsed);
            Console.WriteLine(s2.Elapsed);
        }


        public static string CreateRegexGroupName(string str)
        {
            string res = Regex.Replace(str, @"^[^a-zA-Z]", match => String.Format("special{0}", (int)match.Value[0]));
            string res2 = Regex.Replace(res, @"[^a-zA-Z0-9]", match => String.Format("special{0}", (int)match.Value[0]));
            return res2;
        }

        public static string CleanForRegex(string str)
        {
            string res = Regex.Replace(str, @"[.^$*+?()[{\|#]", match => String.Format(@"\{0}", match));
            return res;
        }

        private void AddTrackForAnalysis(
            string track,
            HashSet<string> uniqueKeywords,
            List<string> tracksList,
            List<string[]> trackedKeywords)
        {
            string[] trackKeywords = track.Split(' ');
            uniqueKeywords.UnionWith(trackKeywords);
            tracksList.Add(track);
            trackedKeywords.Add(trackKeywords);
        }

        private bool CompareFirstWords(string input, string keyword)
        {
            if (input.Length < keyword.Length)
            {
                return false;
            }

            int i = 0;
            while (i < keyword.Length && keyword[i] == input[i])
            {
                ++i;
            }

            return i == keyword.Length && (input.Length == keyword.Length || input[i] == ' ');
        }

        [TestMethod]
        public void CompareFirstWord()
        {
            Assert.IsTrue(CompareFirstWords("hello", "hello"));
            Assert.IsTrue(CompareFirstWords("hello ", "hello"));
            Assert.IsTrue(CompareFirstWords("hell o", "hell"));
            Assert.IsFalse(CompareFirstWords("hell", "hello"));
            Assert.IsFalse(CompareFirstWords("hello", "hell"));
        }

        private bool IsWordComplete(string input, int pos)
        {
            if (pos == input.Length)
            {
                return true;
            }

            return input[pos] == ' ' ||
                   input[pos] == '.' ||
                   input[pos] == ',' ||
                   input[pos] == '-' ||
                   input[pos] == '?' ||
                   input[pos] == ':' ||
                   input[pos] == '!' ||
                   input[pos] == ';' ||
                   input[pos] == '(' ||
                   input[pos] == ')' ||
                   input[pos] == '[' ||
                   input[pos] == ']' ||
                   input[pos] == '{' ||
                   input[pos] == '}' ||
                   input[pos] == '_' ||
                   input[pos] == '=' ||
                   input[pos] == '+' ||
                   input[pos] == '%' ||
                   input[pos] == '*' ||
                   input[pos] == '<' ||
                   input[pos] == '>';

        }

        private bool IsInputContainsKeyword(string input, string keyword)
        {
            if (input.Length + 1 < keyword.Length)
            {
                return false;
            }

            bool result = false;

            int i = 0;

            while (i <= input.Length - keyword.Length + 1)
            {
                // Do not care about special chars
                while (i < input.Length && (input[i] == ' '))
                {
                    ++i;
                }

                // Store the begining of the word
                int startWordPos = i;

                // Match the whole word
                while (i < input.Length && input[i] != ' ')
                {
                    ++i;
                }

                // Is the word big enough to be analyzed
                if (i - startWordPos < keyword.Length)
                {
                    continue;
                }

                // Analyze the current keyword
                int currentWordPos = startWordPos;
                int keywordPos = 0;

                if (input[currentWordPos] == '#' && keyword[0] != '#')
                {
                    ++currentWordPos;
                    ++startWordPos;
                }

                while (currentWordPos < input.Length && keywordPos < keyword.Length && input[currentWordPos] == keyword[keywordPos])
                {
                    ++currentWordPos;
                    ++keywordPos;
                }

                if (currentWordPos - startWordPos == keyword.Length && IsWordComplete(input, currentWordPos))
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        [TestMethod]
        public void CompareSentencesWithHash()
        {
            // First and Last
            Assert.IsTrue(IsInputContainsKeyword("#hello", "hello")); // Equal
            Assert.IsTrue(IsInputContainsKeyword("#hello ", "hello")); // 1st word with space
            Assert.IsTrue(IsInputContainsKeyword("#hello how are you?", "hello")); // 1st word with multiple words
            Assert.IsTrue(IsInputContainsKeyword("#hello?", "hello")); // 1st word with punctuation
            Assert.IsFalse(IsInputContainsKeyword("#hell", "hello")); // 1st word start
            Assert.IsFalse(IsInputContainsKeyword("#hello", "hell")); // 1st word more than required

            // Middle
            Assert.IsTrue(IsInputContainsKeyword("first #hello last", "hello")); // 1st word with space
            Assert.IsTrue(IsInputContainsKeyword("first #hello how are you?", "hello")); // 1st word with multiple words
            Assert.IsTrue(IsInputContainsKeyword("first #hello? are you?", "hello")); // 1st word with punctuation
            Assert.IsFalse(IsInputContainsKeyword("first #hell last", "hello")); // 1st word start
            Assert.IsFalse(IsInputContainsKeyword("first #hello last", "hell")); // 1st word more than required

            // Last
            Assert.IsTrue(IsInputContainsKeyword("first #hello ", "hello")); // 1st word with space
            Assert.IsTrue(IsInputContainsKeyword("first #hello?", "hello")); // 1st word with punctuation
            Assert.IsFalse(IsInputContainsKeyword("first #hell", "hello")); // 1st word start
            Assert.IsFalse(IsInputContainsKeyword("first #hello", "hell")); // 1st word more than required

            // Inside
            Assert.IsFalse(IsInputContainsKeyword("#hellok", "hello")); // Equal
            Assert.IsFalse(IsInputContainsKeyword("#hellok ", "hello")); // 1st word with space
            Assert.IsFalse(IsInputContainsKeyword("#hellok how are you?", "hello")); // 1st word with multiple words
            Assert.IsFalse(IsInputContainsKeyword("#hellok?", "hello")); // 1st word with punctuation
            Assert.IsFalse(IsInputContainsKeyword("first #khello last", "hello")); // 1st word with space
            Assert.IsFalse(IsInputContainsKeyword("first #khello how are you?", "hello")); // 1st word with multiple words
            Assert.IsFalse(IsInputContainsKeyword("first #khello? are you?", "hello")); // 1st word with punctuation
            Assert.IsFalse(IsInputContainsKeyword("first #khello ", "hello")); // 1st word with space
            Assert.IsFalse(IsInputContainsKeyword("first #khello?", "hello")); // 1st word with punctuation
        }

        [TestMethod]
        public void CompareSentences()
        {
            // First and Last
            Assert.IsTrue(IsInputContainsKeyword("hello", "hello")); // Equal
            Assert.IsTrue(IsInputContainsKeyword("hello ", "hello")); // 1st word with space
            Assert.IsTrue(IsInputContainsKeyword("hello how are you?", "hello")); // 1st word with multiple words
            Assert.IsTrue(IsInputContainsKeyword("hello?", "hello")); // 1st word with punctuation
            Assert.IsFalse(IsInputContainsKeyword("hell", "hello")); // 1st word start
            Assert.IsFalse(IsInputContainsKeyword("hello", "hell")); // 1st word more than required

            // Middle
            Assert.IsTrue(IsInputContainsKeyword("first hello last", "hello")); // 1st word with space
            Assert.IsTrue(IsInputContainsKeyword("first hello how are you?", "hello")); // 1st word with multiple words
            Assert.IsTrue(IsInputContainsKeyword("first hello? are you?", "hello")); // 1st word with punctuation
            Assert.IsFalse(IsInputContainsKeyword("first hell last", "hello")); // 1st word start
            Assert.IsFalse(IsInputContainsKeyword("first hello last", "hell")); // 1st word more than required

            // Last
            Assert.IsTrue(IsInputContainsKeyword("first hello ", "hello")); // 1st word with space
            Assert.IsTrue(IsInputContainsKeyword("first hello?", "hello")); // 1st word with punctuation
            Assert.IsFalse(IsInputContainsKeyword("first hell", "hello")); // 1st word start
            Assert.IsFalse(IsInputContainsKeyword("first hello", "hell")); // 1st word more than required

            // Inside
            Assert.IsFalse(IsInputContainsKeyword("hellok", "hello")); // Equal
            Assert.IsFalse(IsInputContainsKeyword("hellok ", "hello")); // 1st word with space
            Assert.IsFalse(IsInputContainsKeyword("hellok how are you?", "hello")); // 1st word with multiple words
            Assert.IsFalse(IsInputContainsKeyword("hellok?", "hello")); // 1st word with punctuation
            Assert.IsFalse(IsInputContainsKeyword("first khello last", "hello")); // 1st word with space
            Assert.IsFalse(IsInputContainsKeyword("first khello how are you?", "hello")); // 1st word with multiple words
            Assert.IsFalse(IsInputContainsKeyword("first khello? are you?", "hello")); // 1st word with punctuation
            Assert.IsFalse(IsInputContainsKeyword("first khello ", "hello")); // 1st word with space
            Assert.IsFalse(IsInputContainsKeyword("first khello?", "hello")); // 1st word with punctuation
        }

        readonly Regex _regexToGetAllInputWords = new Regex(@"\#\w+|\w+", RegexOptions.Compiled);

        [TestMethod]
        public void AlgorithmSpeedTests()
        {
            #region Initialize
            // Get list of unique keywords
            HashSet<string> uniqueKeywordHash = new HashSet<string>();
            List<string> trackList = new List<string>();
            List<string[]> trackKeywordsList = new List<string[]>();

            //AddTrackForAnalysis("#plop", uniqueKeywordHash, trackList, trackKeywordsList);
            //AddTrackForAnalysis("hello #boy", uniqueKeywordHash, trackList, trackKeywordsList);
            //AddTrackForAnalysis("hello", uniqueKeywordHash, trackList, trackKeywordsList);

            string input = "dqdmhellodq #salut, #plop I! am? pretty-happy with what you’ve #plop just told me concerning syria " +
                           "and other topics like obame linvi Tweetinvi c# api, and streaming as well";

            string[] inputKeywords = input.Split(' ');
            for (int i = 0; i < inputKeywords.Length; ++i)
            {
                AddTrackForAnalysis(inputKeywords[i], uniqueKeywordHash, trackList, trackKeywordsList);
                AddTrackForAnalysis(inputKeywords[i] + i, uniqueKeywordHash, trackList, trackKeywordsList);

                if (i + 3 < inputKeywords.Length)
                {
                    AddTrackForAnalysis(String.Format("{0} {1}", inputKeywords[i], inputKeywords[i + 2]), uniqueKeywordHash, trackList, trackKeywordsList);
                }
            }

            for (int i = 0; i < 399; ++i)
            {
                AddTrackForAnalysis(Guid.NewGuid().ToString(), uniqueKeywordHash, trackList, trackKeywordsList);
            }

            // Done before starting the analysis -- arrays are used for improved performances
            string[] uniqueKeywords = uniqueKeywordHash.ToArray();
            string[] tracks = trackList.ToArray();
            string[][] tracksKeywords = new string[trackKeywordsList.Count][];

            for (int i = 0; i < trackKeywordsList.Count; ++i)
            {
                tracksKeywords[i] = trackKeywordsList[i];
            }
            #endregion
            Thread.Sleep(1000);

            #region Solution 1
            StringBuilder patternBuilder = new StringBuilder();

            // Matching a Tweet keyword should never exceed : 20ms (4M tweets/day)
            foreach (var keywordPattern in uniqueKeywords)
            {
                // -- Time to proceed 400 elements : 00:00:00.0057224
                patternBuilder.Append(String.Format(@"(?=.*(?<{0}>(?:^|\s+){1}(?:\s+|$)))?",
                    CreateRegexGroupName(keywordPattern), CleanForRegex(keywordPattern)));
                // -- Time to proceed 400 elements : 00:00:00.0029869
                //patternBuilder.Append(String.Format(@"(?=.*(?<{0}>{0}))?", keywordPattern)); use contains instead for performances
            }

            string pattern = patternBuilder.ToString();
            // Done during analysis
            Stopwatch s = new Stopwatch();
            s.Start();
            MatchCollection matches = Regex.Matches(input, pattern, RegexOptions.IgnoreCase);
            GroupCollection groups = matches[0].Groups;
            List<string> matchingKeywords = new List<string>();

            for (int i = 0; i < uniqueKeywords.Length; ++i)
            {
                if (groups[CreateRegexGroupName(uniqueKeywords[i])].Success)
                {
                    matchingKeywords.Add(uniqueKeywords[i]);
                }
            }

            // Matched keywords are now ready!
            List<string> result = new List<string>();
            for (int i = 0; i < tracksKeywords.Length; ++i)
            {
                bool trackIsMatching = true;
                for (int j = 0; j < tracksKeywords[i].Length && trackIsMatching; ++j)
                {
                    trackIsMatching = matchingKeywords.Contains(tracksKeywords[i][j]);
                }

                if (trackIsMatching)
                {
                    result.Add(tracks[i]);
                }
            }

            s.Stop();
            Debug.WriteLine("Solution 1 = {0}ms", s.Elapsed.TotalMilliseconds);
            #endregion

            #region Solution 2

            // Initialize
            var uniqueKeywordsRegex = new string[uniqueKeywords.Length];
            for (int i = 0; i < uniqueKeywords.Length; ++i)
            {
                uniqueKeywordsRegex[i] = uniqueKeywords[i][0] == '#'
                                          ? String.Format(@"\#\b{0}\b", uniqueKeywords[i].Substring(1))
                                          : String.Format(@"\b{0}\b", uniqueKeywords[i]);
            }
            Stopwatch s2 = new Stopwatch();
            s2.Start();
            List<string> matchingKeywords2 = new List<string>();
            for (int i = 0; i < uniqueKeywords.Length; ++i)
            {
                // Match keyword -- Or match hashtag followed by keyword
                if (Regex.IsMatch(input, String.Format(uniqueKeywordsRegex[i])))
                {
                    matchingKeywords2.Add(uniqueKeywords[i]);
                }
            }

            List<string> result2 = new List<string>();

            for (int i = 0; i < tracksKeywords.Length; ++i)
            {
                bool trackIsMatching = true;
                for (int j = 0; j < tracksKeywords[i].Length && trackIsMatching; ++j)
                {
                    trackIsMatching = matchingKeywords2.Contains(tracksKeywords[i][j]);
                }

                if (trackIsMatching)
                {
                    string keyword = tracks[i];
                    result2.Add(keyword);
                }
            }
            s2.Stop();
            Debug.WriteLine("Solution 2 = {0}ms", s2.Elapsed.TotalMilliseconds);
            #endregion

            #region Solution 3

            Stopwatch s3 = new Stopwatch();
            s3.Start();
            List<string> matchingKeywords3 = new List<string>();
            for (int i = 0; i < uniqueKeywords.Length; ++i)
            {
                bool isMatching = false;

                if (uniqueKeywords[i][0] != '#')
                {
                    // Check uniqueKeywords[i] alone
                    isMatching = input.Contains(String.Format(" {0} ", uniqueKeywords[i])) ||
                                   input == uniqueKeywords[i] ||
                                   (input.Length > uniqueKeywords[i].Length + 1 &&
                                    (input.Substring(0, uniqueKeywords[i].Length + 1) == String.Format("{0} ", uniqueKeywords[i]) ||
                                     input.Substring(input.Length - uniqueKeywords[i].Length - 1, uniqueKeywords[i].Length + 1) == String.Format(" {0}", uniqueKeywords[i])));
                }

                if (!isMatching)
                {
                    var keywordwithHash = uniqueKeywords[i][0] == '#' ? uniqueKeywords[i] : String.Format("#{0}", uniqueKeywords[i]);
                    isMatching = input.Contains(String.Format(" {0} ", keywordwithHash)) ||
                                 input == keywordwithHash ||
                                 (input.Length > keywordwithHash.Length + 1 &&
                                  (input.Substring(0, keywordwithHash.Length + 1) == String.Format("{0} ", keywordwithHash) ||
                                   input.Substring(input.Length - keywordwithHash.Length - 1, keywordwithHash.Length + 1) == String.Format(" {0}", keywordwithHash)));
                }

                if (isMatching)
                {
                    matchingKeywords3.Add(uniqueKeywords[i]);
                }
            }

            List<string> result3 = new List<string>();
            for (int i = 0; i < tracksKeywords.Length; ++i)
            {
                bool trackIsMatching = true;
                for (int j = 0; j < tracksKeywords[i].Length && trackIsMatching; ++j)
                {
                    trackIsMatching = matchingKeywords3.Contains(tracksKeywords[i][j]);
                }

                if (trackIsMatching)
                {
                    string keyword = tracks[i];
                    result3.Add(keyword);
                }
            }

            s3.Stop();
            Debug.WriteLine("Solution 3 = {0}ms", s3.Elapsed.TotalMilliseconds);
            #endregion

            #region Solution 4

            Stopwatch s4 = new Stopwatch();
            s4.Start();
            List<string> matchingKeywords4 = new List<string>();
            for (int i = 0; i < uniqueKeywords.Length; ++i)
            {
                if (IsInputContainsKeyword(input, uniqueKeywords[i]))
                {
                    matchingKeywords4.Add(uniqueKeywords[i]);
                }
            }

            List<string> result4 = new List<string>();
            for (int i = 0; i < tracksKeywords.Length; ++i)
            {
                bool trackIsMatching = true;
                for (int j = 0; j < tracksKeywords[i].Length && trackIsMatching; ++j)
                {
                    trackIsMatching = matchingKeywords4.Contains(tracksKeywords[i][j]);
                }

                if (trackIsMatching)
                {
                    string keyword = tracks[i];
                    result4.Add(keyword);
                }
            }

            s4.Stop();
            Debug.WriteLine("Solution 4 = {0}ms", s4.Elapsed.TotalMilliseconds);
            #endregion

            #region Solution 5

            Stopwatch s5 = new Stopwatch();
            s5.Start();
            var inputWordsMatchCollection5 = _regexToGetAllInputWords.Matches(input.ToLower());
            string[] inputWords = new string[inputWordsMatchCollection5.Count];
            for (int i = 0; i < inputWordsMatchCollection5.Count; ++i)
            {
                inputWords[i] = inputWordsMatchCollection5[i].Value;
            }

            var matchingKeywords5 = inputKeywords.Intersect(inputWords).ToArray();

            List<string> result5 = new List<string>();
            for (int i = 0; i < tracksKeywords.Length; ++i)
            {
                bool trackIsMatching = true;
                for (int j = 0; j < tracksKeywords[i].Length && trackIsMatching; ++j)
                {
                    trackIsMatching = matchingKeywords5.Contains(tracksKeywords[i][j]);
                }

                if (trackIsMatching)
                {
                    string keyword = tracks[i];
                    result5.Add(keyword);
                }
            }

            s5.Stop();
            Debug.WriteLine("Solution 5 = {0}ms", s5.Elapsed.TotalMilliseconds);

            #endregion

            #region Solution 6

            // Perform the regex for memory management equivalent performances

            for (int i = 0; i < 3; ++i)
            {
                var k = inputKeywords.Intersect(_regexToGetAllInputWords.Matches(input.ToLower()).OfType<Match>().Select(x => x.Value)).ToList();
                k.Clear();
            }

            Stopwatch s6 = new Stopwatch();
            s6.Start();
            var inputWordsMatchCollection6 = _regexToGetAllInputWords.Matches(input.ToLower()).OfType<Match>().Select(x => x.Value);
            var matchingKeywords6 = inputKeywords.Intersect(inputWordsMatchCollection6).ToArray();

            List<string> result6 = new List<string>();
            for (int i = 0; i < tracksKeywords.Length; ++i)
            {
                bool trackIsMatching = true;
                for (int j = 0; j < tracksKeywords[i].Length && trackIsMatching; ++j)
                {
                    trackIsMatching = matchingKeywords6.Contains(tracksKeywords[i][j]);
                }

                if (trackIsMatching)
                {
                    string keyword = tracks[i];
                    result6.Add(keyword);
                }
            }

            s6.Stop();
            Debug.WriteLine("Solution 6 = {0}ms", s6.Elapsed.TotalMilliseconds);

            #endregion

            #region Solution 7

            Stopwatch s7 = new Stopwatch();
            //Stopwatch s71 = new Stopwatch();
            //Stopwatch s72 = new Stopwatch();
            //Stopwatch s73 = new Stopwatch();
            //Stopwatch s74 = new Stopwatch();
            //Stopwatch s75 = new Stopwatch();
            s7.Start();
            //s71.Start();
            var inputWordsMatchCollection7 = _regexToGetAllInputWords.Matches(input.ToLower()).OfType<Match>().Select(x => x.Value).ToArray();

            //s71.Stop();
            //s72.Start();

            var matchingKeywords7 = uniqueKeywords.Intersect(inputWordsMatchCollection7).ToArray();

            //s72.Stop();
            //s73.Start();

            List<string> result7 = new List<string>();
            for (int i = 0; i < tracksKeywords.Length; ++i)
            {
                bool trackIsMatching = true;
                //s74.Start();
                for (int j = 0; j < tracksKeywords[i].Length && trackIsMatching; ++j)
                {
                    trackIsMatching = matchingKeywords7.Contains(tracksKeywords[i][j]);
                }

                //s74.Stop();
                //s75.Start();

                if (trackIsMatching)
                {
                    string keyword = tracks[i];
                    result7.Add(keyword);
                }
                //s75.Stop();
            }
            //s73.Stop();

            s7.Stop();
            Debug.WriteLine("Solution 7 = {0}ms", s7.Elapsed.TotalMilliseconds);
            //Debug.WriteLine("Solution 7[1] = {0}ms", s71.Elapsed.TotalMilliseconds);
            //Debug.WriteLine("Solution 7[2] = {0}ms", s72.Elapsed.TotalMilliseconds);
            //Debug.WriteLine("Solution 7[3] = {0}ms", s73.Elapsed.TotalMilliseconds);
            //Debug.WriteLine("Solution 7[4] = {0}ms", s74.Elapsed.TotalMilliseconds);
            //Debug.WriteLine("Solution 7[5] = {0}ms", s75.Elapsed.TotalMilliseconds);

            #endregion

            #region Solution 8

            // Perform the regex for equivalent performances

            Stopwatch s8 = new Stopwatch();
            s8.Start();
            var inputWordsMatchCollection8 = _regexToGetAllInputWords.Matches(input.ToLower()).OfType<Match>().Select(x => x.Value);
            var matchingKeywords8 = inputKeywords.Intersect(inputWordsMatchCollection8).ToArray();

            List<string> result8 = new List<string>();
            for (int i = 0; i < tracksKeywords.Length; ++i)
            {
                //bool trackIsMatching = true;
                //for (int j = 0; j < tracksKeywords[i].Length && trackIsMatching; ++j)
                //{
                //    trackIsMatching = matchingKeywords8.Contains(tracksKeywords[i][j]);
                //}

                if (matchingKeywords8.Intersect(tracksKeywords[i]).Any())
                {
                    string keyword = tracks[i];
                    result8.Add(keyword);
                }
            }

            s8.Stop();
            Debug.WriteLine("Solution 8 = {0}ms", s8.Elapsed.TotalMilliseconds);

            #endregion

            #region Solution 8

            const int nbItemsPerThread = 5;

            Stopwatch s9 = new Stopwatch();
            Stopwatch s91 = new Stopwatch();
            Stopwatch s92 = new Stopwatch();
            Stopwatch s93 = new Stopwatch();
            s9.Start();
            s91.Start();
            var inputWordsMatchCollection9 = _regexToGetAllInputWords.Matches(input.ToLower()).OfType<Match>().Select(x => x.Value);
            s91.Stop();
            s92.Start();
            var matchingKeywords9 = inputKeywords.Intersect(inputWordsMatchCollection9).ToArray();
            s92.Stop();

            // Separate simple words from multiple words

            s93.Start();
            List<string> result9 = new List<string>();

            var nbThreads = (tracksKeywords.Length / nbItemsPerThread) + 1;
            Thread[] threads = new Thread[nbThreads];

            for (int i = 0; i < nbThreads; ++i)
            {
                Thread t = new Thread(x =>
                {
                    int lastItemToComparePosition = (int)x + 10;
                    for (int j = (int)x; j < lastItemToComparePosition; ++j)
                    {
                        bool trackIsMatching = true;
                        for (int k = 0; k < tracksKeywords[j].Length && trackIsMatching; ++k)
                        {
                            trackIsMatching = matchingKeywords9.Contains(tracksKeywords[j][k]);
                        }

                        if (trackIsMatching)
                        {
                            string keyword = tracks[j];
                            result9.Add(keyword);
                        }
                    }
                });

                threads[i] = t;
                t.Start(i);
            }

            for (int i = 0; i < nbThreads; ++i)
            {
                threads[i].Join();
            }
            s93.Stop();

            s9.Stop();
            Debug.WriteLine("Solution 9 = {0}ms", s9.Elapsed.TotalMilliseconds);
            Debug.WriteLine("Solution 9[1] = {0}ms", s91.Elapsed.TotalMilliseconds);
            Debug.WriteLine("Solution 9[2] = {0}ms", s92.Elapsed.TotalMilliseconds);
            Debug.WriteLine("Solution 9[2] = {0}ms", s93.Elapsed.TotalMilliseconds);

            #endregion

            Debug.WriteLine("Solution 1 : {0}", result.Count);
            Debug.WriteLine("Solution 2 : {0}", result2.Count);
            Debug.WriteLine("Solution 3 : {0}", result3.Count);
            Debug.WriteLine("Solution 4 : {0}", result4.Count);
            Debug.WriteLine("Solution 5 : {0}", result5.Count);
            Debug.WriteLine("Solution 6 : {0}", result6.Count);
            Debug.WriteLine("Solution 7 : {0}", result7.Count);
            Debug.WriteLine("Solution 8 : {0}", result8.Count);
            Debug.WriteLine("Solution 9 : {0}", result9.Count);
        }

        private void Iteration(string input, string[] uniqueKeywords, string[][] tracksKeywords, string[] tracks)
        {
            Stopwatch s = new Stopwatch();
            s.Start();

            var inputWordsMatchCollection = _regexToGetAllInputWords.Matches(input.ToLower()).OfType<Match>().Select(x => x.Value).ToArray();
            var matchingKeywords = uniqueKeywords.Intersect(inputWordsMatchCollection).ToArray();

            List<string> result = new List<string>();
            for (int i = 0; i < tracksKeywords.Length; ++i)
            {
                bool trackIsMatching = true;
                for (int j = 0; j < tracksKeywords[i].Length && trackIsMatching; ++j)
                {
                    trackIsMatching = matchingKeywords.Contains(tracksKeywords[i][j]);
                }

                if (trackIsMatching)
                {
                    string keyword = tracks[i];
                    result.Add(keyword);
                }
            }

            s.Stop();

            //Debug.WriteLine("Solution 7 = {0}ms ({1} keywords)", s.Elapsed.TotalMilliseconds, result.Count);
        }

        [TestMethod]
        public void BestSolution()
        {
            string input = "dqdmhellodq #salut, #plop I! am? pretty-happy with what you’ve #plop just told me concerning syria " +
                           "and other topics like obame linvi Tweetinvi c# api, and streaming as well";

            var tracks = _regexToGetAllInputWords.Matches(input.ToLower()).OfType<Match>().Select(x => x.Value).ToList();
            int initialTracksCount = tracks.Count;
            for (int i = 0; i < initialTracksCount; ++i)
            {
                if (i + 3 < tracks.Count)
                {
                    tracks.Add(String.Format("{0} {1}", tracks[i], tracks[i + 2]));
                }
            }

            for (int i = 0; i < 399; ++i)
            {
                tracks.Add(Guid.NewGuid().ToString());
            }

            List<string[]> tracksKeywordsList = new List<string[]>();
            for (int i = 0; i < tracks.Count(); ++i)
            {
                tracksKeywordsList.Add(tracks[i].Split(' '));
            }

            HashSet<string> uniqueKeywordsHashSet = new HashSet<string>();
            for (int i = 0; i < tracks.Count; ++i)
            {
                uniqueKeywordsHashSet.Add(tracks[i]);
            }

            var tracksKeywords = tracksKeywordsList.ToArray();
            var uniqueKeywords = uniqueKeywordsHashSet.ToArray();
            var trackArray = tracks.ToArray();

            Stopwatch s = new Stopwatch();
            //Stopwatch s1 = new Stopwatch();
            //Stopwatch s2 = new Stopwatch();
            //Stopwatch s3 = new Stopwatch();
            //Stopwatch s4 = new Stopwatch();
            s.Start();
            //s1.Start();
            Iteration(input, uniqueKeywords, tracksKeywords, trackArray);
            //s1.Stop();
            //s2.Start();
            Iteration(input, uniqueKeywords, tracksKeywords, trackArray);
            //s2.Stop();
            //s3.Start();
            Iteration(input, uniqueKeywords, tracksKeywords, trackArray);
            //s3.Stop();
            //s4.Start();
            Iteration(input, uniqueKeywords, tracksKeywords, trackArray);
            //s4.Stop();
            s.Stop();
            //Debug.WriteLine("1 = {0}ms", s1.Elapsed.TotalMilliseconds);
            //Debug.WriteLine("2 = {0}ms", s2.Elapsed.TotalMilliseconds);
            //Debug.WriteLine("3 = {0}ms", s3.Elapsed.TotalMilliseconds);
            //Debug.WriteLine("4 = {0}ms", s4.Elapsed.TotalMilliseconds);
            Debug.WriteLine("total = {0}", s.Elapsed.TotalMilliseconds);

            Stopwatch st = new Stopwatch();
            st.Start();
            Thread t = new Thread(() => Iteration(input, uniqueKeywords, tracksKeywords, trackArray));
            Thread t2 = new Thread(() => Iteration(input, uniqueKeywords, tracksKeywords, trackArray));
            Thread t3 = new Thread(() => Iteration(input, uniqueKeywords, tracksKeywords, trackArray));
            Thread t4 = new Thread(() => Iteration(input, uniqueKeywords, tracksKeywords, trackArray));
            t.Start();
            t2.Start();
            t3.Start();
            t4.Start();
            t.Join();
            t2.Join();
            t3.Join();
            t4.Join();
            st.Stop();
            Debug.WriteLine("total = {0}", st.Elapsed.TotalMilliseconds);
        }

        #endregion
    }
}
