using Microsoft.VisualStudio.TestTools.UnitTesting;
using Streaminvi.Helpers;
using TweetinCore.Interfaces;

namespace Testinvi.Streaminvi
{
    [TestClass]
    public class StreamTrackManagerTests
    {
        private StreamTrackManager<ITweet> _streamTrackManager;

        [TestInitialize]
        public void TestInitialise()
        {
            _streamTrackManager = new StreamTrackManager<ITweet>();
        }

        #region Add/Remove/Contains 

        [TestMethod]
        public void AddAndContains()
        {
            // Arrange
            string expectedTrack = "hello piloupe";
            _streamTrackManager.AddTrack(expectedTrack);

            // Act
            var exists = _streamTrackManager.ContainsTrack(expectedTrack);

            // Assert
            Assert.IsTrue(exists);
        }

        [TestMethod]
        public void AddThenRemoveSameRefAndContains()
        {
            // Arrange
            string expectedTrack = "hello piloupe";
            _streamTrackManager.AddTrack(expectedTrack);
            _streamTrackManager.RemoveTrack(expectedTrack);

            // Act
            var exists = _streamTrackManager.ContainsTrack(expectedTrack);

            // Assert
            Assert.IsFalse(exists);
            Assert.AreEqual(_streamTrackManager.TracksCount, 0);
        }

        [TestMethod]
        public void AddThenRemoveAndContains()
        {
            // Arrange
            string expectedTrack = "hello piloupe";
            _streamTrackManager.AddTrack(expectedTrack);
            _streamTrackManager.RemoveTrack("hello piloupe");

            // Act
            var exists = _streamTrackManager.ContainsTrack(expectedTrack);

            // Assert
            Assert.IsFalse(exists);
        }

        #endregion

        #region GenerateTrackStringRequest

        #endregion

        #region Matches
        [TestMethod]
        public void Matches_ExistingAsPartOfAWordSimpleWord_ReturnsFalse()
        {
            // Arrange
            _streamTrackManager.AddTrack("hello boy");
            _streamTrackManager.AddTrack("kikou");
            _streamTrackManager.AddTrack("piloupe gerard");

            // Act
            var result = _streamTrackManager.Matches("foqnfuqkikoufqfjnq");

            // Assert
            Assert.AreEqual(result, false);
        }

        [TestMethod]
        public void Matches_ExistingAsAWordSimpleWord_ReturnsTrue()
        {
            // Arrange
            _streamTrackManager.AddTrack("hello boy");
            _streamTrackManager.AddTrack("kikou");
            _streamTrackManager.AddTrack("piloupe gerard");

            // Act
            var result = _streamTrackManager.Matches("foqnfuq kikou fqfjnq");

            // Assert
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void Matches_ExistingAsPartof2WordsTouchingNoSpace_ReturnsFalse()
        {
            _streamTrackManager.AddTrack("hello boy");
            _streamTrackManager.AddTrack("kikou");
            _streamTrackManager.AddTrack("piloupe gerard");

            var result = _streamTrackManager.Matches("foqnfuqhelloboyfqfjnq");

            Assert.AreEqual(result, false);
        }

        [TestMethod]
        public void Matches_ExistingAs2WordsTouching_ReturnsTrue()
        {
            _streamTrackManager.AddTrack("hello boy");
            _streamTrackManager.AddTrack("kikou");
            _streamTrackManager.AddTrack("piloupe gerard");

            var result = _streamTrackManager.Matches("foqnfuq hello boy jnq");

            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void Matches_ExistingAsPartof2WordsTouchingAndInvertedNoSpace_ReturnsFalse()
        {
            _streamTrackManager.AddTrack("hello boy");
            _streamTrackManager.AddTrack("kikou");
            _streamTrackManager.AddTrack("piloupe gerard");

            var result = _streamTrackManager.Matches("foqnfuqboyfqfhellofqfjnq");

            Assert.AreEqual(result, false);
        }

        [TestMethod]
        public void Matches_ExistingAs2WordsTouchingAndInverted_ReturnsTrue()
        {
            _streamTrackManager.AddTrack("hello boy");
            _streamTrackManager.AddTrack("kikou");
            _streamTrackManager.AddTrack("piloupe gerard");

            var result = _streamTrackManager.Matches("foqnfuq boy hello fqfjnq");

            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void Matches_Existing2WordsSeparatedNoSpace_ReturnsFalse()
        {
            _streamTrackManager.AddTrack("hello boy");
            _streamTrackManager.AddTrack("kikou");
            _streamTrackManager.AddTrack("piloupe gerard");

            var result = _streamTrackManager.Matches("foqboynfuqhellofqfjnq");

            Assert.AreEqual(result, false);
        }

        [TestMethod]
        public void Matches_Existing2WordsInvertedAndSeparatedNoSpace_ReturnsFalse()
        {
            _streamTrackManager.AddTrack("hello boy");
            _streamTrackManager.AddTrack("kikou");
            _streamTrackManager.AddTrack("piloupe gerard");

            var result = _streamTrackManager.Matches("foqboynfuqhellofqfjnq");

            Assert.AreEqual(result, false);
        }

        [TestMethod]
        public void Matches_Existing2WordsInvertedAndSeparated_ReturnsTrue()
        {
            _streamTrackManager.AddTrack("hello boy");
            _streamTrackManager.AddTrack("kikou");
            _streamTrackManager.AddTrack("piloupe gerard");

            var result = _streamTrackManager.Matches("foq boy nfuq hello fqfjnq");

            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void Matches_1WordMatchingInsteadOf2_ReturnsFalse()
        {
            _streamTrackManager.AddTrack("hello boy");
            _streamTrackManager.AddTrack("kikou");
            _streamTrackManager.AddTrack("piloupe gerard");

            var result = _streamTrackManager.Matches("foq boy nfuq fqfjnq");

            Assert.AreEqual(result, false);
        }

        [TestMethod]
        public void Matches_NoWordMatching_ReturnsFalse()
        {
            _streamTrackManager.AddTrack("hello boy");
            _streamTrackManager.AddTrack("kikou");
            _streamTrackManager.AddTrack("piloupe gerard");

            var result = _streamTrackManager.Matches("foq nfuq fqfjnq");

            Assert.AreEqual(result, false);
        }
        #endregion

        #region MatchesTracks

        // No Tracking Keyword
        [TestMethod]
        public void MatchesTracks_NoTrackAndNull_ReturnsEmptyCollection()
        {
            // Arrange
            var tracker = new StreamTrackManager<ITweet>();

            // Act
            var result = tracker.MatchingTracks(null);

            // Assert
            Assert.AreNotEqual(result, null);
            Assert.AreEqual(result.Count, 0);
        }

        [TestMethod]
        public void MatchesTracks_NoTrackAndValidInput_ReturnsEmptyCollection()
        {
            // Arrange
            var tracker = new StreamTrackManager<ITweet>();

            // Act
            var result = tracker.MatchingTracks("hello");

            // Assert
            Assert.AreNotEqual(result, null);
            Assert.AreEqual(result.Count, 0);
        }

        // Existing Tracking Keywords
        [TestMethod]
        public void MatchesTracks_TrackAndNull_ReturnsEmptyCollection()
        {
            // Arrange
            var tracker = new StreamTrackManager<ITweet>();
            tracker.AddTrack("hello");

            // Act
            var result = tracker.MatchingTracks(null);

            // Assert
            Assert.AreNotEqual(result, null);
            Assert.AreEqual(result.Count, 0);
        }

        [TestMethod]
        public void MatchesTracks_TrackAndEmptyInput_ReturnsEmptyCollection()
        {
            // Arrange
            var tracker = new StreamTrackManager<ITweet>();
            tracker.AddTrack("hello");

            // Act
            var result = tracker.MatchingTracks("");

            // Assert
            Assert.AreNotEqual(result, null);
            Assert.AreEqual(result.Count, 0);
        }

        [TestMethod]
        public void MatchesTracks_TrackAndSingleCharInput_ReturnsEmptyCollection()
        {
            // Arrange
            var tracker = new StreamTrackManager<ITweet>();
            tracker.AddTrack("hello");

            // Act
            var result = tracker.MatchingTracks("h");

            // Assert
            Assert.AreNotEqual(result, null);
            Assert.AreEqual(result.Count, 0);
        }

        [TestMethod]
        public void MatchesTracks_TrackAndMatchingInput_ReturnsMatchingElement()
        {
            // Arrange
            var tracker = new StreamTrackManager<ITweet>();
            tracker.AddTrack("hello");

            // Act
            var result = tracker.MatchingTracks("hello");

            // Assert
            Assert.AreNotEqual(result, null);
            Assert.AreEqual(result.Count, 1);
            Assert.AreEqual(result[0], "hello");
        }

        [TestMethod]
        public void MatchesTracks_TrackAndMatchingInputTwice_ReturnsMatchingElement()
        {
            // Arrange
            var tracker = new StreamTrackManager<ITweet>();
            tracker.AddTrack("hello");

            // Act
            var result = tracker.MatchingTracks("hello hello");

            // Assert
            Assert.AreNotEqual(result, null);
            Assert.AreEqual(result.Count, 1);
            Assert.AreEqual(result[0], "hello");
        }

        [TestMethod]
        public void MatchesTracks_TrackAndMatchingAroundOtherWords_ReturnsMatchingElement()
        {
            // Arrange
            var tracker = new StreamTrackManager<ITweet>();
            tracker.AddTrack("hello");

            // Act
            var result = tracker.MatchingTracks("w1 hello, w12");

            // Assert
            Assert.AreNotEqual(result, null);
            Assert.AreEqual(result.Count, 1);
            Assert.AreEqual(result[0], "hello");
        }

        [TestMethod]
        public void MatchesTracks_2TracksAnd1Matching_ReturnsMatchingElement()
        {
            // Arrange
            var tracker = new StreamTrackManager<ITweet>();
            tracker.AddTrack("hello");
            tracker.AddTrack("world");

            // Act
            var result = tracker.MatchingTracks("w1 #hello w12");

            // Assert
            Assert.AreNotEqual(result, null);
            Assert.AreEqual(result.Count, 1);
            Assert.AreEqual(result[0], "hello");
        }

        [TestMethod]
        public void MatchesTracks_2TracksAnd2Matching_ReturnsMatchingElements()
        {
            // Arrange
            var tracker = new StreamTrackManager<ITweet>();
            tracker.AddTrack("hello");
            tracker.AddTrack("world");

            // Act
            var result = tracker.MatchingTracks("world w1 hello w12");

            // Assert
            Assert.AreNotEqual(result, null);
            Assert.AreEqual(result.Count, 2);
            Assert.AreEqual(result[0], "hello");
            Assert.AreEqual(result[1], "world");
        }

        [TestMethod]
        public void MatchesTracks_TextHasSymbolInFrontOfItWhichWasRegistered_ReturnsTrack()
        {
            // Arrange
            var tracker = new StreamTrackManager<ITweet>();
            tracker.AddTrack("$APPL");

            // Act
            var result = tracker.MatchingTracks("$APPL salut");

            // Assert
            Assert.AreNotEqual(result, null);
            Assert.AreEqual(result.Count, 1);
        }

        [TestMethod]
        public void MatchesTracks_TextHasSymbolInFrontOfItWhichWasNotRegistered_ReturnsEmptyCollection()
        {
            // Arrange
            var tracker = new StreamTrackManager<ITweet>();
            tracker.AddTrack("APPL");

            // Act
            var result = tracker.MatchingTracks("$APPL salut");

            // Assert
            Assert.AreNotEqual(result, null);
            Assert.AreEqual(result.Count, 0);
        }

        #endregion
    }
}
