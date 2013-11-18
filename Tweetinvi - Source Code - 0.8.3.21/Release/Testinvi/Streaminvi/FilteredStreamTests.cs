using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Streaminvi;
using Testinvi.Helpers;
using TweetinCore.Enum;
using TweetinCore.Interfaces;
using TweetinCore.Interfaces.StreamInvi;
using System.Threading;
using TweetinCore.Interfaces.TwitterToken;
using Tweetinvi;
using Tweetinvi.Model;
using TwitterToken;

namespace Testinvi.Streaminvi
{
    [TestClass]
    public class FilteredStreamTests
    {
        // This is the delay between performing an action
        // and detecting the action in the UserStream
        private const int STREAM_DELAY = 6000;

        // Change this const based on your connection speed
        // This is the time required for your connection to be 
        // ready to listen to the stream
        private const int STREAM_START_DELAY = 2000;

        private IToken _token1;
        private IToken _token2;

        [TestInitialize]
        public void Initialize()
        {
            TokenTestSingleton.Initialize(true);

            _token1 = new Token(
                       ConfigurationManager.AppSettings["token_AccessToken"],
                       ConfigurationManager.AppSettings["token_AccessTokenSecret"],
                       ConfigurationManager.AppSettings["token_ConsumerKey"],
                       ConfigurationManager.AppSettings["token_ConsumerSecret"]);

            _token2 = new Token(
                        ConfigurationManager.AppSettings["token2_AccessToken"],
                        ConfigurationManager.AppSettings["token2_AccessTokenSecret"],
                        ConfigurationManager.AppSettings["token2_ConsumerKey"],
                        ConfigurationManager.AppSettings["token2_ConsumerSecret"]);
        }

        #region Simple Tracking from Twitter API

        [TestMethod]
        public void StartStreamTrackRandomUniqueWord()
        {
            // Arrange
            var randomWord = String.Format("Tweetinvi{0}", new Random().Next());
            var expectedMessage = String.Format("Hello {0} Random", randomWord);
            bool result = false;
            bool tracked = false;

            IFilteredStream stream = new FilteredStream();
            stream.AddTrack(randomWord, tweet => tracked = true);

            Func<ITweet, bool> listen = delegate(ITweet tweet)
            {
                if (tweet != null)
                {
                    result = tweet.Text == expectedMessage;
                }

                // End the stream
                return false;
            };

            Thread t = ThreadTestHelper.StartLifetimedThread(
               () => stream.StartStream(TokenSingleton.Token, listen),
               stream.StopStream,
               STREAM_DELAY, STREAM_START_DELAY);

            // Act
            ITweet newTweet = new Tweet(expectedMessage, TokenTestSingleton.Instance);
            newTweet.Publish();

            t.Join();

            // Cleanup
            newTweet.Destroy();

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(tracked);
        }

        [TestMethod]
        public void StartStreamTrackWord1ORWord2InTheSameTweet()
        {
            // Arrange
            var randomWord1 = String.Format("Tweetinvi{0}", new Random().Next());
            var randomWord2 = String.Format("Tweetinvi2{0}", new Random().Next());

            var expectedMessage1 = String.Format("Hello {0}", randomWord1);
            var expectedMessage2 = String.Format("Hello {0}", randomWord2);
            var expectedMessage3 = String.Format("Hello {0} {1}", randomWord1, randomWord2);

            int i = 0;

            IFilteredStream stream = new FilteredStream();
            stream.AddTrack(randomWord1);
            stream.AddTrack(randomWord2);

            Func<ITweet, bool> listen = delegate(ITweet tweet)
                {
                    Debug.WriteLine(tweet != null ? tweet.Text : "Tweet is null");
                    if (tweet != null)
                    {
                        bool result = tweet.Text == expectedMessage1 ||
                                      tweet.Text == expectedMessage2 ||
                                      tweet.Text == expectedMessage3;

                        if (result)
                        {
                            Debug.WriteLine(tweet.Text);
                            ++i;
                        }
                    }

                    // End the stream
                    return true;
                };

            Thread t = ThreadTestHelper.StartLifetimedThread(
                () => stream.StartStream(TokenSingleton.Token, listen),
                stream.StopStream,
                STREAM_DELAY, STREAM_START_DELAY);

            // Act
            ITweet newTweet1 = new Tweet(expectedMessage1, TokenTestSingleton.Instance);
            newTweet1.Publish();

            ITweet newTweet2 = new Tweet(expectedMessage2, TokenTestSingleton.Instance);
            newTweet2.Publish();

            ITweet newTweet3 = new Tweet(expectedMessage3, TokenTestSingleton.Instance);
            newTweet3.Publish();

            t.Join();

            // Cleanup
            newTweet1.Destroy();
            newTweet2.Destroy();
            newTweet3.Destroy();

            // Assert
            Assert.AreEqual(i, 3);
        }

        [TestMethod]
        public void StartStreamTrackWord1ANDWord2InTheSameTweet()
        {
            // Arrange
            var randomWord1 = String.Format("Tweetinvi{0}", new Random().Next());
            var randomWord2 = String.Format("Tweetinvi2{0}", new Random().Next());

            var expectedMessage1 = String.Format("Hello {0}", randomWord1);
            var expectedMessage2 = String.Format("Hello {0}", randomWord2);
            var expectedMessage3 = String.Format("Hello {0} and {1}", randomWord1, randomWord2);

            int i = 0;

            IFilteredStream stream = new FilteredStream();

            stream.AddTrack(String.Format("{0} {1}", randomWord1, randomWord2));

            Func<ITweet, bool> listen = delegate(ITweet tweet)
            {
                if (tweet != null)
                {
                    bool result = tweet.Text == expectedMessage1 ||
                                  tweet.Text == expectedMessage2 ||
                                  tweet.Text == expectedMessage3;

                    if (result)
                    {
                        Debug.WriteLine(tweet.Text);
                        ++i;
                    }
                }

                // End the stream
                return true;
            };

            Thread t = ThreadTestHelper.StartLifetimedThread(
                () => stream.StartStream(TokenSingleton.Token, listen),
                stream.StopStream,
                STREAM_DELAY, STREAM_START_DELAY);

            // Act
            ITweet newTweet1 = new Tweet(expectedMessage1, TokenTestSingleton.Instance);
            newTweet1.Publish();

            ITweet newTweet2 = new Tweet(expectedMessage2, TokenTestSingleton.Instance);
            newTweet2.Publish();

            ITweet newTweet3 = new Tweet(expectedMessage3, TokenTestSingleton.Instance);
            newTweet3.Publish();

            t.Join();

            // Cleanup
            newTweet1.Destroy();
            newTweet2.Destroy();
            newTweet3.Destroy();

            // Assert
            Assert.AreEqual(i, 1);
        }

        #endregion

        #region TrackKeywords Events

        [TestMethod]
        public void TrackKewyordsEvents_EventRaised()
        {
            // Arrange
            var randomWord = String.Format("Tweetinvi{0}", new Random().Next());
            var expectedMessage = String.Format("Hello {0} Random", randomWord);
            bool result = false;

            IFilteredStream stream = new FilteredStream();
            stream.AddTrack(randomWord, tweet => { result = true; });

            Thread t = ThreadTestHelper.StartLifetimedThread(
               () => stream.StartStream(TokenSingleton.Token, tweet => true),
               stream.StopStream,
               STREAM_DELAY, STREAM_START_DELAY);

            // Act
            ITweet newTweet = new Tweet(expectedMessage, TokenTestSingleton.Instance);
            newTweet.Publish();

            t.Join();

            // Cleanup
            newTweet.Destroy();

            // Assert
            Assert.IsTrue(result);
        }

       
        #endregion

        #region Follow

        [TestMethod]
        public void FilterFollowSpecificUser()
        {
            // Arrange
            IFilteredStream stream = new FilteredStream();
            ITokenUser user2 = new TokenUser(_token2);

            var randomWord = String.Format("Tweetinvi{0}", new Random().Next());
            var expectedMessage = String.Format("Hello {0} Random", randomWord);

            stream.AddFollow(user2);

            ITweet newTweet1 = new Tweet(expectedMessage, _token1);
            ITweet newTweet2 = new Tweet(expectedMessage + " 2", _token2);

            bool tweet1Detected = false;
            bool tweet2Detected = false;
            Func<ITweet, bool> listen = delegate(ITweet tweet)
            {
                tweet1Detected = tweet1Detected || tweet.Text == newTweet1.Text;
                tweet2Detected = tweet2Detected || tweet.Text == newTweet2.Text;

                // End the stream
                return false;
            };

            Thread t = ThreadTestHelper.StartLifetimedThread(
               () => stream.StartStream(_token1, listen),
               stream.StopStream,
               STREAM_DELAY, STREAM_START_DELAY);

            // Act

            newTweet1.Publish();
            newTweet2.Publish();

            t.Join();

            // Cleanup
            newTweet1.Destroy();
            newTweet2.Destroy();

            // Assert
            Assert.IsFalse(tweet1Detected);
            Assert.IsTrue(tweet2Detected);
        }

        [TestMethod]
        public void FilterFollowSpecificUser_FollowTrackEventCalled()
        {
            // Arrange
            IFilteredStream stream = new FilteredStream();
            ITokenUser user2 = new TokenUser(_token2);

            var randomWord = String.Format("Tweetinvi{0}", new Random().Next());
            var expectedMessage = String.Format("Hello {0} Random", randomWord);

            bool followEventCalled = false;
            stream.AddFollow(user2, tweet =>
            {
                followEventCalled = true;
            });

            ITweet newTweet1 = new Tweet(expectedMessage, _token1);
            ITweet newTweet2 = new Tweet(expectedMessage + " 2", _token2);

            bool tweet1Detected = false;
            bool tweet2Detected = false;
            Func<ITweet, bool> listen = delegate(ITweet tweet)
            {
                tweet1Detected = tweet1Detected || tweet.Text == newTweet1.Text;
                tweet2Detected = tweet2Detected || tweet.Text == newTweet2.Text;

                // End the stream
                return false;
            };

            Thread t = ThreadTestHelper.StartLifetimedThread(
               () => stream.StartStream(_token1, listen),
               stream.StopStream,
               STREAM_DELAY, STREAM_START_DELAY);

            // Act

            newTweet1.Publish();
            newTweet2.Publish();

            t.Join();

            // Cleanup
            newTweet1.Destroy();
            newTweet2.Destroy();

            // Assert
            Assert.IsFalse(tweet1Detected);
            Assert.IsTrue(tweet2Detected);
            Assert.IsTrue(followEventCalled);
        }

        #endregion

        #region Location

        [TestMethod]
        public void FilterByLocation()
        {
            // Arrange
            var tweet1 = new Tweet(String.Format("tweetinvi : {0}", Guid.NewGuid()), _token1);
            var tweet2 = new Tweet(String.Format("tweetinvi 2 : {0}", Guid.NewGuid()), _token1);
            var tweet3 = new Tweet(String.Format("tweetinvi 3 : {0}", Guid.NewGuid()), _token1);
            var tweet4 = new Tweet(String.Format("tweetinvi 4 : {0}", Guid.NewGuid()), _token1);

            const int boxSize = 1;

            var topLeft = new Coordinates(-124.75, 36.8);
            var bottomRight = new Coordinates(-123.75 + boxSize, 37.8 + boxSize);

            var inTheSea = new Location(topLeft, bottomRight);
            
            var stream = new FilteredStream();
            stream.AddLocation(inTheSea);

            bool tweet1Detected = false;
            bool tweet2Detected = false;
            bool tweet3Detected = false;
            bool tweet4Detected = false;

            Action<ITweet> tweetReceived = tweet =>
            {
                tweet1Detected = tweet1Detected || tweet.Text == tweet1.Text;
                tweet2Detected = tweet2Detected || tweet.Text == tweet2.Text;
                tweet3Detected = tweet3Detected || tweet.Text == tweet3.Text;
                tweet4Detected = tweet4Detected || tweet.Text == tweet4.Text;
            };

            var t = ThreadTestHelper.StartLifetimedThread(
                () => stream.StartStream(_token1, tweetReceived),
                stream.StopStream,
                STREAM_DELAY + 1000, STREAM_START_DELAY + 1000);

            // Act
            tweet1.PublishWithGeo(-124.75, 36.95);
            tweet2.PublishWithGeo(-124.75, 10.55);
            tweet3.PublishWithGeo(-100.75, 36.95);
            tweet4.PublishWithGeo(-100.75, 10.75);

            t.Join();

            // Cleanup
            tweet1.Destroy();
            tweet2.Destroy();
            tweet3.Destroy();
            tweet4.Destroy();

            // Assert
            Assert.IsTrue(tweet1Detected);
            Assert.IsFalse(tweet2Detected);
            Assert.IsFalse(tweet3Detected);
            Assert.IsFalse(tweet4Detected);
        }

        [TestMethod]
        public void FilterByCoordinates()
        {
            // Arrange
            var tweet1 = new Tweet(String.Format("tweetinvi : {0}", Guid.NewGuid()), _token1);
            var tweet2 = new Tweet(String.Format("tweetinvi 2 : {0}", Guid.NewGuid()), _token1);
            var tweet3 = new Tweet(String.Format("tweetinvi 3 : {0}", Guid.NewGuid()), _token1);
            var tweet4 = new Tweet(String.Format("tweetinvi 4 : {0}", Guid.NewGuid()), _token1);

            const int boxSize = 1;
            var topLeft = new Coordinates(-124.75, 36.8);
            var bottomRight = new Coordinates(-123.75 + boxSize, 37.8 + boxSize);

            var stream = new FilteredStream();
            stream.AddLocation(topLeft, bottomRight);

            bool tweet1Detected = false;
            bool tweet2Detected = false;
            bool tweet3Detected = false;
            bool tweet4Detected = false;

            Action<ITweet> tweetReceived = tweet =>
            {
                tweet1Detected = tweet1Detected || tweet.Text == tweet1.Text;
                tweet2Detected = tweet2Detected || tweet.Text == tweet2.Text;
                tweet3Detected = tweet3Detected || tweet.Text == tweet3.Text;
                tweet4Detected = tweet4Detected || tweet.Text == tweet4.Text;
            };

            var t = ThreadTestHelper.StartLifetimedThread(
                () => stream.StartStream(_token1, tweetReceived),
                stream.StopStream,
                STREAM_DELAY, STREAM_START_DELAY);

            // Act
            tweet1.PublishWithGeo(-124.75, 36.95);
            tweet2.PublishWithGeo(-124.75, 10.55);
            tweet3.PublishWithGeo(-100.75, 36.95);
            tweet4.PublishWithGeo(-100.75, 10.75);

            t.Join();

            // Cleanup
            tweet1.Destroy();
            tweet2.Destroy();
            tweet3.Destroy();
            tweet4.Destroy();

            // Assert
            Assert.IsTrue(tweet1Detected);
            Assert.IsFalse(tweet2Detected);
            Assert.IsFalse(tweet3Detected);
            Assert.IsFalse(tweet4Detected);
        }

        [TestMethod]
        public void TrackWithManualLocationTracking()
        {
            // Arrange
            var tweet1 = new Tweet(String.Format("tweetinvi : {0}", Guid.NewGuid()), _token1);
            var tweet2 = new Tweet(String.Format("tweetinvi 2 : {0}", Guid.NewGuid()), _token1);
            var tweet3 = new Tweet(String.Format("tweetinvi 3 : {0}", Guid.NewGuid()), _token1);
            var tweet4 = new Tweet(String.Format("tweetinvi 4 : {0}", Guid.NewGuid()), _token1);

            const int boxSize = 1;

            var topLeft = new Coordinates(-124.75, 36.8);
            var bottomRight = new Coordinates(-123.75 + boxSize, 37.8 + boxSize);

            var inTheSea = new Location(topLeft, bottomRight);

            var stream = new FilteredStream();
            stream.AddLocation(inTheSea);

            bool tweet1Detected = false;
            bool tweet2Detected = false;
            bool tweet3Detected = false;
            bool tweet4Detected = false;

            Action<ITweet> tweetReceived = tweet =>
            {
                tweet1Detected = tweet1Detected || tweet.Text == tweet1.Text;
                tweet2Detected = tweet2Detected || tweet.Text == tweet2.Text;
                tweet3Detected = tweet3Detected || tweet.Text == tweet3.Text;
                tweet4Detected = tweet4Detected || tweet.Text == tweet4.Text;
            };

            var t = ThreadTestHelper.StartLifetimedThread(
                () => stream.StartStream(_token1, tweetReceived),
                stream.StopStream,
                STREAM_DELAY, STREAM_START_DELAY);

            // Act
            tweet1.PublishWithGeo(-124.75, 36.95);
            tweet2.PublishWithGeo(-124.75, 10.55);
            tweet3.PublishWithGeo(-100.75, 36.95);
            tweet4.PublishWithGeo(-100.75, 10.75);

            t.Join();

            // Cleanup
            tweet1.Destroy();
            tweet2.Destroy();
            tweet3.Destroy();
            tweet4.Destroy();

            // Assert
            Assert.IsTrue(tweet1Detected);
            Assert.IsFalse(tweet2Detected);
            Assert.IsFalse(tweet3Detected);
            Assert.IsFalse(tweet4Detected);
        }

        [TestMethod]
        public void FilterByLocationAnd2Valid()
        {
            // Arrange
            var tweet1 = new Tweet(String.Format("tweetinvi : {0}", Guid.NewGuid()), _token1);
            var tweet2 = new Tweet(String.Format("tweetinvi 2 : {0}", Guid.NewGuid()), _token1);
            var tweet3 = new Tweet(String.Format("tweetinvi 3 : {0}", Guid.NewGuid()), _token1);
            var tweet4 = new Tweet(String.Format("tweetinvi 4 : {0}", Guid.NewGuid()), _token1);

            int boxSize = 2;
            const double centerLongitude = -155.75;
            const double centerLatitude = 30.75;

            var inTheSeaTopLeft = new Coordinates(centerLongitude - boxSize, centerLatitude - boxSize);
            var inTheSeaBottomRight = new Coordinates(centerLongitude + boxSize, centerLatitude + boxSize);

            boxSize = 4;
            var inTheSeaLargerTopLeft = new Coordinates(centerLongitude - boxSize, centerLatitude - boxSize);
            var inTheSeaLargerBottomRight = new Coordinates(centerLongitude + boxSize, centerLatitude + boxSize);

            var inTheSea = new Location(inTheSeaTopLeft, inTheSeaBottomRight);
            var inTheSeaLarger = new Location(inTheSeaLargerTopLeft, inTheSeaLargerBottomRight);

            var stream = new FilteredStream();
            stream.AddLocation(inTheSea);
            stream.AddLocation(inTheSeaLarger);

            bool tweet1Detected = false;
            int tweet1DetectinXLocations = 0;
            bool tweet2Detected = false;
            bool tweet3Detected = false;
            bool tweet4Detected = false;

            int i = 0;
            Action<ITweet, List<ILocation>> tweetReceived = (tweet, locations) =>
            {
                ++i;
                tweet1Detected = tweet1Detected || tweet.Text == tweet1.Text;

                if (tweet.Text == tweet1.Text)
                {
                    tweet1DetectinXLocations = locations.Count;
                }

                tweet2Detected = tweet2Detected || tweet.Text == tweet2.Text;
                tweet3Detected = tweet3Detected || tweet.Text == tweet3.Text;
                tweet4Detected = tweet4Detected || tweet.Text == tweet4.Text;
            };

            var t = ThreadTestHelper.StartLifetimedThread(
                () => stream.StartStream(_token1, tweetReceived),
                stream.StopStream,
                STREAM_DELAY + 1000, STREAM_START_DELAY + 1000);

            // Act
            boxSize = 10;
            tweet1.PublishWithGeo(centerLongitude, centerLatitude);
            tweet2.PublishWithGeo(centerLongitude + boxSize, centerLatitude);
            tweet3.PublishWithGeo(centerLongitude, centerLatitude + boxSize);
            tweet4.PublishWithGeo(centerLongitude + boxSize, centerLatitude + boxSize);

            t.Join();

            // Cleanup
            tweet1.Destroy();
            tweet2.Destroy();
            tweet3.Destroy();
            tweet4.Destroy();

            // Assert
            Assert.IsTrue(tweet1Detected);
            Assert.IsFalse(tweet2Detected);
            Assert.IsFalse(tweet3Detected);
            Assert.IsFalse(tweet4Detected);
            Assert.AreEqual(i, 1);
            Assert.AreEqual(tweet1DetectinXLocations, 2);
        }

        [TestMethod]
        public void FilterByLocationAnd2ValidAndTracked()
        {
            // Arrange
            var tweet1 = new Tweet(String.Format("tweetinvi : {0}", Guid.NewGuid()), _token1);
            var tweet2 = new Tweet(String.Format("tweetinvi 2 : {0}", Guid.NewGuid()), _token1);
            var tweet3 = new Tweet(String.Format("tweetinvi 3 : {0}", Guid.NewGuid()), _token1);
            var tweet4 = new Tweet(String.Format("tweetinvi 4 : {0}", Guid.NewGuid()), _token1);

            int boxSize = 2;
            const double centerLongitude = -155.75;
            const double centerLatitude = 30.75;

            var inTheSeaTopLeft = new Coordinates(centerLongitude - boxSize, centerLatitude - boxSize);
            var inTheSeaBottomRight = new Coordinates(centerLongitude + boxSize, centerLatitude + boxSize);

            boxSize = 4;
            var inTheSeaLargerTopLeft = new Coordinates(centerLongitude - boxSize, centerLatitude - boxSize);
            var inTheSeaLargerBottomRight = new Coordinates(centerLongitude + boxSize, centerLatitude + boxSize);

            var inTheSea = new Location(inTheSeaTopLeft, inTheSeaBottomRight);
            var inTheSeaLarger = new Location(inTheSeaLargerTopLeft, inTheSeaLargerBottomRight);

            var stream = new FilteredStream();

            bool inTheSeaTracked = false;
            bool inTheSeaLargerTracked = false;

            stream.AddLocation(inTheSea, tweet => inTheSeaTracked = true);
            stream.AddLocation(inTheSeaLarger, tweet => inTheSeaLargerTracked = true);

            bool tweet1Detected = false;
            int tweet1DetectinXLocations = 0;
            bool tweet2Detected = false;
            bool tweet3Detected = false;
            bool tweet4Detected = false;

            int i = 0;
            Action<ITweet, List<ILocation>> tweetReceived = (tweet, locations) =>
            {
                ++i;
                tweet1Detected = tweet1Detected || tweet.Text == tweet1.Text;

                if (tweet.Text == tweet1.Text)
                {
                    tweet1DetectinXLocations = locations.Count;
                }

                tweet2Detected = tweet2Detected || tweet.Text == tweet2.Text;
                tweet3Detected = tweet3Detected || tweet.Text == tweet3.Text;
                tweet4Detected = tweet4Detected || tweet.Text == tweet4.Text;
            };

            var t = ThreadTestHelper.StartLifetimedThread(
                () => stream.StartStream(_token1, tweetReceived),
                stream.StopStream,
                STREAM_DELAY + 1000, STREAM_START_DELAY + 1000);

            // Act
            boxSize = 10;
            tweet1.PublishWithGeo(centerLongitude, centerLatitude);
            tweet2.PublishWithGeo(centerLongitude + boxSize, centerLatitude);
            tweet3.PublishWithGeo(centerLongitude, centerLatitude + boxSize);
            tweet4.PublishWithGeo(centerLongitude + boxSize, centerLatitude + boxSize);

            t.Join();

            // Cleanup
            tweet1.Destroy();
            tweet2.Destroy();
            tweet3.Destroy();
            tweet4.Destroy();

            // Assert
            Assert.IsTrue(inTheSeaTracked);
            Assert.IsTrue(inTheSeaLargerTracked);

            Assert.IsTrue(tweet1Detected);
            Assert.IsFalse(tweet2Detected);
            Assert.IsFalse(tweet3Detected);
            Assert.IsFalse(tweet4Detected);
            Assert.AreEqual(i, 1);
            Assert.AreEqual(tweet1DetectinXLocations, 2);
        }

        #endregion

        #region StartStreamMatchingAllConditions

        [TestMethod]
        public void StartStreamMatchingAllConditionsWithLocationOnly()
        {
            // Arrange
            var tweet1 = new Tweet(String.Format("tweetinvi : {0}", Guid.NewGuid()), _token1);
            var tweet2 = new Tweet(String.Format("tweetinvi 2 : {0}", Guid.NewGuid()), _token1);
            var tweet3 = new Tweet(String.Format("tweetinvi 3 : {0}", Guid.NewGuid()), _token1);
            var tweet4 = new Tweet(String.Format("tweetinvi 4 : {0}", Guid.NewGuid()), _token1);

            const int boxSize = 1;
            var topLeft = new Coordinates(-124.75, 36.8);
            var bottomRight = new Coordinates(-123.75 + boxSize, 37.8 + boxSize);

            var stream = new FilteredStream();
            stream.AddLocation(topLeft, bottomRight);

            bool tweet1Detected = false;
            bool tweet2Detected = false;
            bool tweet3Detected = false;
            bool tweet4Detected = false;

            Func<ITweet, bool> tweetReceived = tweet =>
            {
                tweet1Detected = tweet1Detected || tweet.Text == tweet1.Text;
                tweet2Detected = tweet2Detected || tweet.Text == tweet2.Text;
                tweet3Detected = tweet3Detected || tweet.Text == tweet3.Text;
                tweet4Detected = tweet4Detected || tweet.Text == tweet4.Text;

                return true;
            };

            var t = ThreadTestHelper.StartLifetimedThread(
                () => stream.StartStreamMatchingAllConditions(_token1, tweetReceived),
                stream.StopStream,
                STREAM_DELAY, STREAM_START_DELAY);

            // Act
            tweet1.PublishWithGeo(-124.75, 36.95);
            tweet2.PublishWithGeo(-124.75, 10.55);
            tweet3.PublishWithGeo(-100.75, 36.95);
            tweet4.PublishWithGeo(-100.75, 10.75);

            t.Join();

            // Cleanup
            tweet1.Destroy();
            tweet2.Destroy();
            tweet3.Destroy();
            tweet4.Destroy();

            // Assert
            Assert.IsTrue(tweet1Detected);
            Assert.IsFalse(tweet2Detected);
            Assert.IsFalse(tweet3Detected);
            Assert.IsFalse(tweet4Detected);
        }

        [TestMethod]
        public void StartStreamMatchingAllConditionsWithLocationAndTrack()
        {
            // Arrange
            var tweet1 = new Tweet(String.Format("tweetinvi : {0}", Guid.NewGuid()), _token1);
            var tweet2 = new Tweet(String.Format("tweetinvi 2 : {0}", Guid.NewGuid()), _token1);
            var tweet3 = new Tweet(String.Format("tweetinvi 3 : {0}", Guid.NewGuid()), _token1);
            var tweet4 = new Tweet(String.Format("tweetinvi 4 : {0}", Guid.NewGuid()), _token1);

            var tweet5 = new Tweet(String.Format("linvi : {0}", Guid.NewGuid()), _token1);
            var tweet6 = new Tweet(String.Format("linvi 2 : {0}", Guid.NewGuid()), _token1);
            var tweet7 = new Tweet(String.Format("linvi 3 : {0}", Guid.NewGuid()), _token1);
            var tweet8 = new Tweet(String.Format("linvi 4 : {0}", Guid.NewGuid()), _token1);

            const int boxSize = 1;
            var topLeft = new Coordinates(-124.75, 36.8);
            var bottomRight = new Coordinates(-123.75 + boxSize, 37.8 + boxSize);

            var stream = new FilteredStream();
            stream.AddLocation(topLeft, bottomRight);
            stream.AddTrack("tweetinvi");

            bool tweet1Detected = false;
            bool tweet2Detected = false;
            bool tweet3Detected = false;
            bool tweet4Detected = false;

            bool tweet5Detected = false;
            bool tweet6Detected = false;
            bool tweet7Detected = false;
            bool tweet8Detected = false;

            Func<ITweet, bool> tweetReceived = tweet =>
            {
                tweet1Detected = tweet1Detected || tweet.Text == tweet1.Text;
                tweet2Detected = tweet2Detected || tweet.Text == tweet2.Text;
                tweet3Detected = tweet3Detected || tweet.Text == tweet3.Text;
                tweet4Detected = tweet4Detected || tweet.Text == tweet4.Text;

                tweet5Detected = tweet5Detected || tweet.Text == tweet5.Text;
                tweet6Detected = tweet6Detected || tweet.Text == tweet6.Text;
                tweet7Detected = tweet7Detected || tweet.Text == tweet7.Text;
                tweet8Detected = tweet8Detected || tweet.Text == tweet8.Text;

                return true;
            };

            var t = ThreadTestHelper.StartLifetimedThread(
                () => stream.StartStreamMatchingAllConditions(_token1, tweetReceived),
                stream.StopStream,
                STREAM_DELAY + 2000, STREAM_START_DELAY);

            // Act
            tweet1.PublishWithGeo(-124.75, 36.95);
            tweet2.PublishWithGeo(-124.75, 10.55);
            tweet3.PublishWithGeo(-100.75, 36.95);
            tweet4.PublishWithGeo(-100.75, 10.75);

            tweet5.PublishWithGeo(-124.75, 36.95);
            tweet6.PublishWithGeo(-124.75, 10.55);
            tweet7.PublishWithGeo(-100.75, 36.95);
            tweet8.PublishWithGeo(-100.75, 10.75);

            t.Join();

            // Cleanup
            tweet1.Destroy();
            tweet2.Destroy();
            tweet3.Destroy();
            tweet4.Destroy();
            tweet5.Destroy();
            tweet6.Destroy();
            tweet7.Destroy();
            tweet8.Destroy();

            // Assert
            Assert.IsTrue(tweet1Detected);
            Assert.IsFalse(tweet2Detected);
            Assert.IsFalse(tweet3Detected);
            Assert.IsFalse(tweet4Detected);
            Assert.IsFalse(tweet5Detected);
            Assert.IsFalse(tweet6Detected);
            Assert.IsFalse(tweet7Detected);
            Assert.IsFalse(tweet8Detected);
        }

        [TestMethod]
        public void StartStreamMatchingAllConditionsWithLocationAndTrackAndFollow()
        {
            // Arrange
            var tweet1 = new Tweet(String.Format("tweetinvi : {0}", Guid.NewGuid()), _token1);
            var tweet2 = new Tweet(String.Format("tweetinvi 2 : {0}", Guid.NewGuid()), _token1);
            var tweet3 = new Tweet(String.Format("tweetinvi 3 : {0}", Guid.NewGuid()), _token1);
            var tweet4 = new Tweet(String.Format("tweetinvi 4 : {0}", Guid.NewGuid()), _token1);

            var tweet5 = new Tweet(String.Format("linvi : {0}", Guid.NewGuid()), _token1);
            var tweet6 = new Tweet(String.Format("linvi 2 : {0}", Guid.NewGuid()), _token1);
            var tweet7 = new Tweet(String.Format("linvi 3 : {0}", Guid.NewGuid()), _token1);
            var tweet8 = new Tweet(String.Format("linvi 4 : {0}", Guid.NewGuid()), _token1);

            var tweet9 = new Tweet(String.Format("tweetinvi : {0}", Guid.NewGuid()), _token2);
            var tweet10 = new Tweet(String.Format("tweetinvi 2 : {0}", Guid.NewGuid()), _token2);
            var tweet11 = new Tweet(String.Format("tweetinvi 3 : {0}", Guid.NewGuid()), _token2);
            var tweet12 = new Tweet(String.Format("tweetinvi 4 : {0}", Guid.NewGuid()), _token2);

            const int boxSize = 1;
            var topLeft = new Coordinates(-124.75, 36.8);
            var bottomRight = new Coordinates(-123.75 + boxSize, 37.8 + boxSize);

            var stream = new FilteredStream();
            stream.AddLocation(topLeft, bottomRight);
            stream.AddTrack("tweetinvi");
            stream.AddFollow(new TokenUser(_token1));

            bool tweet1Detected = false;
            bool tweet2Detected = false;
            bool tweet3Detected = false;
            bool tweet4Detected = false;

            bool tweet5Detected = false;
            bool tweet6Detected = false;
            bool tweet7Detected = false;
            bool tweet8Detected = false;

            bool tweet9Detected = false;
            bool tweet10Detected = false;
            bool tweet11Detected = false;
            bool tweet12Detected = false;

            Func<ITweet, bool> tweetReceived = tweet =>
            {
                tweet1Detected = tweet1Detected || tweet.Text == tweet1.Text;
                tweet2Detected = tweet2Detected || tweet.Text == tweet2.Text;
                tweet3Detected = tweet3Detected || tweet.Text == tweet3.Text;
                tweet4Detected = tweet4Detected || tweet.Text == tweet4.Text;

                tweet5Detected = tweet5Detected || tweet.Text == tweet5.Text;
                tweet6Detected = tweet6Detected || tweet.Text == tweet6.Text;
                tweet7Detected = tweet7Detected || tweet.Text == tweet7.Text;
                tweet8Detected = tweet8Detected || tweet.Text == tweet8.Text;

                tweet9Detected = tweet9Detected || tweet.Text == tweet9.Text;
                tweet10Detected = tweet10Detected || tweet.Text == tweet10.Text;
                tweet11Detected = tweet11Detected || tweet.Text == tweet11.Text;
                tweet12Detected = tweet12Detected || tweet.Text == tweet12.Text;

                return true;
            };

            var t = ThreadTestHelper.StartLifetimedThread(
                () => stream.StartStreamMatchingAllConditions(_token1, tweetReceived),
                stream.StopStream,
                STREAM_DELAY + 2000, STREAM_START_DELAY);

            // Act
            tweet1.PublishWithGeo(-124.75, 36.95);
            tweet2.PublishWithGeo(-124.75, 10.55);
            tweet3.PublishWithGeo(-100.75, 36.95);
            tweet4.PublishWithGeo(-100.75, 10.75);

            tweet5.PublishWithGeo(-124.75, 36.95);
            tweet6.PublishWithGeo(-124.75, 10.55);
            tweet7.PublishWithGeo(-100.75, 36.95);
            tweet8.PublishWithGeo(-100.75, 10.75);

            tweet9.PublishWithGeo(-124.75, 36.95);
            tweet10.PublishWithGeo(-124.75, 10.55);
            tweet11.PublishWithGeo(-100.75, 36.95);
            tweet12.PublishWithGeo(-100.75, 10.75);

            t.Join();

            // Cleanup
            tweet1.Destroy();
            tweet2.Destroy();
            tweet3.Destroy();
            tweet4.Destroy();
            tweet5.Destroy();
            tweet6.Destroy();
            tweet7.Destroy();
            tweet8.Destroy();
            tweet9.Destroy();
            tweet10.Destroy();
            tweet11.Destroy();
            tweet12.Destroy();

            // Assert
            Assert.IsTrue(tweet1Detected);
            Assert.IsFalse(tweet2Detected);
            Assert.IsFalse(tweet3Detected);
            Assert.IsFalse(tweet4Detected);
            Assert.IsFalse(tweet5Detected);
            Assert.IsFalse(tweet6Detected);
            Assert.IsFalse(tweet7Detected);
            Assert.IsFalse(tweet8Detected);
            Assert.IsFalse(tweet9Detected);
            Assert.IsFalse(tweet10Detected);
            Assert.IsFalse(tweet11Detected);
            Assert.IsFalse(tweet12Detected);
        }

        // Track delegate

        [TestMethod]
        public void MatchingAllConditions_KeywordActionsCalledOnlyIfAllConditionsMatched()
        {
            // Arrange
            var tweet1 = new Tweet(String.Format("tweetinvi : {0}", Guid.NewGuid()), _token1);
            var tweet2 = new Tweet(String.Format("tweetinvi 2 : {0}", Guid.NewGuid()), _token1);

            const int boxSize = 1;
            var topLeft = new Coordinates(-124.75, 36.8);
            var bottomRight = new Coordinates(-123.75 + boxSize, 37.8 + boxSize);

            int tweetinviNbTracked = 0;
            bool tweet1PerformedAction = false;
            bool tweet1Detected = false;
            bool tweet2Detected = false;

            var stream = new FilteredStream();
            stream.AddLocation(topLeft, bottomRight);
            stream.AddTrack("tweetinvi", tweet =>
            {
                ++tweetinviNbTracked;
                tweet1PerformedAction = tweet1.Text == tweet.Text;
            });

            Func<ITweet, bool> tweetReceived = tweet =>
            {
                tweet1Detected = tweet1Detected || tweet.Text == tweet1.Text;
                tweet2Detected = tweet2Detected || tweet.Text == tweet2.Text;

                return true;
            };

            var t = ThreadTestHelper.StartLifetimedThread(
                () => stream.StartStreamMatchingAllConditions(_token1, tweetReceived),
                stream.StopStream,
                STREAM_DELAY, STREAM_START_DELAY + 1000);

            // Act
            tweet1.PublishWithGeo(-124.75, 36.95);
            tweet2.PublishWithGeo(-124.75, 10.55);

            t.Join();

            // Cleanup
            tweet1.Destroy();
            tweet2.Destroy();

            // Assert
            Assert.IsTrue(tweet1Detected);
            Assert.IsTrue(tweet1PerformedAction);
            Assert.IsFalse(tweet2Detected);
            Assert.AreEqual(tweetinviNbTracked, 1);
        }

        [TestMethod]
        public void MatchingAllConditions_LocationActionsCalledOnlyIfAllConditionsMatched()
        {
            // Arrange
            var tweet1 = new Tweet(String.Format("tweetinvi : {0}", Guid.NewGuid()), _token1);
            var tweet2 = new Tweet(String.Format("tweetinvi 2 : {0}", Guid.NewGuid()), _token1);

            const int boxSize = 1;
            var topLeft = new Coordinates(-124.75, 36.8);
            var bottomRight = new Coordinates(-123.75 + boxSize, 37.8 + boxSize);

            int tweetinviNbTracked = 0;
            bool tweet1PerformedAction = false;
            bool tweet1Detected = false;
            bool tweet2Detected = false;

            var stream = new FilteredStream();
            stream.AddLocation(topLeft, bottomRight, tweet =>
            {
                ++tweetinviNbTracked;
                tweet1PerformedAction = tweet1.Text == tweet.Text;
            });
            stream.AddTrack("tweetinvi");

            Func<ITweet, bool> tweetReceived = tweet =>
            {
                tweet1Detected = tweet1Detected || tweet.Text == tweet1.Text;
                tweet2Detected = tweet2Detected || tweet.Text == tweet2.Text;

                return true;
            };

            var t = ThreadTestHelper.StartLifetimedThread(
                () => stream.StartStreamMatchingAllConditions(_token1, tweetReceived),
                stream.StopStream,
                STREAM_DELAY, STREAM_START_DELAY + 1000);

            // Act
            tweet1.PublishWithGeo(-124.75, 36.95);
            tweet2.PublishWithGeo(-124.75, 10.55);

            t.Join();

            // Cleanup
            tweet1.Destroy();
            tweet2.Destroy();

            // Assert
            Assert.IsTrue(tweet1Detected);
            Assert.IsTrue(tweet1PerformedAction);
            Assert.IsFalse(tweet2Detected);
            Assert.AreEqual(tweetinviNbTracked, 1);
        }

        #endregion

        #region Manual Keywords Tracking

        [TestMethod]
        public void TrackKeyword_1ORWord2InTheSameTweet_TweetsValidates()
        {
            // Arrange
            var randomWord1 = String.Format("Tweetinvi{0}", new Random().Next());
            var randomWord2 = String.Format("Tweetinvi2{0}", new Random().Next());

            var expectedMessage1 = String.Format("Hello {0}", randomWord1);
            var expectedMessage2 = String.Format("Hello {0}", randomWord2);
            var expectedMessage3 = String.Format("Hello {0} {1}", randomWord1, randomWord2);

            int i = 0;

            IFilteredStream stream = new FilteredStream();
            stream.AddTrack(randomWord1);
            stream.AddTrack(randomWord2);

            for (int j = 0; j < 398; ++j)
            {
                stream.AddTrack(Guid.NewGuid().ToString());
            }

            Func<ITweet, List<string>, bool> listen = delegate(ITweet tweet, List<string> matches)
            {
                Debug.WriteLine(tweet != null ? tweet.Text : "Tweet is null");
                if (tweet != null)
                {
                    bool result = tweet.Text == expectedMessage1 && matches[0] == randomWord1.ToLower() ||
                                  tweet.Text == expectedMessage2 && matches[0] == randomWord2.ToLower() ||
                                  tweet.Text == expectedMessage3 && matches[0] == randomWord1.ToLower()
                                                                 && matches[1] == randomWord2.ToLower();

                    if (result)
                    {
                        Debug.WriteLine(tweet.Text);
                        ++i;
                    }
                }

                // End the stream
                return true;
            };

            Thread t = ThreadTestHelper.StartLifetimedThread(
                () => stream.StartStream(TokenSingleton.Token, listen),
                stream.StopStream,
                STREAM_DELAY + 1000, STREAM_START_DELAY + 1000);

            // Act
            ITweet newTweet1 = new Tweet(expectedMessage1, TokenTestSingleton.Instance);
            newTweet1.Publish();

            ITweet newTweet2 = new Tweet(expectedMessage2, TokenTestSingleton.Instance);
            newTweet2.Publish();

            ITweet newTweet3 = new Tweet(expectedMessage3, TokenTestSingleton.Instance);
            newTweet3.Publish();

            ITweet newTweet4 = new Tweet(expectedMessage3 + expectedMessage1, TokenTestSingleton.Instance);
            newTweet4.Publish();

            ITweet newTweet5 = new Tweet(expectedMessage3 + expectedMessage2, TokenTestSingleton.Instance);
            newTweet5.Publish();

            ITweet newTweet6 = new Tweet(expectedMessage3 + expectedMessage3, TokenTestSingleton.Instance);
            newTweet6.Publish();

            t.Join();

            // Cleanup
            newTweet1.Destroy();
            newTweet2.Destroy();
            newTweet3.Destroy();
            newTweet4.Destroy();
            newTweet5.Destroy();
            newTweet6.Destroy();

            // Assert
            Assert.AreEqual(i, 3);
        }

        [TestMethod]
        public void TrackKeywords_PerformancesCalculator()
        {
            // Arrange
            var randomWord1 = String.Format("Tweetinvi{0}", new Random().Next());
            var randomWord2 = String.Format("Tweetinvi2{0}", new Random().Next());

            var expectedMessage1 = String.Format("Hello {0}", randomWord1);
            var expectedMessage2 = String.Format("Hello {0}", randomWord2);
            var expectedMessage3 = String.Format("Hello {0} {1}", randomWord1, randomWord2);

            int i = 0;

            IFilteredStream stream = new FilteredStream();
            stream.AddTrack(randomWord1);
            stream.AddTrack(randomWord2);

            for (int j = 0; j < 350; ++j)
            {
                var trackKeywords = j % 2 == 0 ? "piloupe" : "#piloupe";
                trackKeywords += new Random().Next();

                stream.AddTrack(String.Format("{0}1 {0}2", trackKeywords));
            }

            Func<ITweet, List<string>, bool> listen = delegate(ITweet tweet, List<string> matches)
            {
                Debug.WriteLine(tweet != null ? tweet.Text : "Tweet is null");
                if (tweet != null)
                {
                    bool result = tweet.Text == expectedMessage1 && matches[0] == randomWord1.ToLower() ||
                                  tweet.Text == expectedMessage2 && matches[0] == randomWord2.ToLower() ||
                                  tweet.Text == expectedMessage3 && matches[0] == randomWord1.ToLower()
                                                                 && matches[1] == randomWord2.ToLower();

                    if (result)
                    {
                        Debug.WriteLine(tweet.Text);
                        ++i;
                    }
                }

                // End the stream
                return i < 6;
            };

            Thread t = ThreadTestHelper.StartLifetimedThread(
                () => stream.StartStream(TokenSingleton.Token, listen),
                stream.StopStream,
                STREAM_DELAY, STREAM_START_DELAY);

            // Act
            ITweet newTweet1 = new Tweet(expectedMessage1, TokenTestSingleton.Instance);
            newTweet1.Publish();

            ITweet newTweet2 = new Tweet(expectedMessage2, TokenTestSingleton.Instance);
            newTweet2.Publish();

            ITweet newTweet3 = new Tweet(expectedMessage3, TokenTestSingleton.Instance);
            newTweet3.Publish();

            ITweet newTweet4 = new Tweet(expectedMessage3 + expectedMessage1, TokenTestSingleton.Instance);
            newTweet4.Publish();

            ITweet newTweet5 = new Tweet(expectedMessage3 + expectedMessage2, TokenTestSingleton.Instance);
            newTweet5.Publish();

            ITweet newTweet6 = new Tweet(expectedMessage3 + expectedMessage3, TokenTestSingleton.Instance);
            newTweet6.Publish();

            t.Join();

            // Cleanup
            newTweet1.Destroy();
            newTweet2.Destroy();
            newTweet3.Destroy();
            newTweet4.Destroy();
            newTweet5.Destroy();
            newTweet6.Destroy();

            // Assert
            Assert.AreEqual(i, 3);
        }

        [TestMethod]
        public void TrackedKeywords_MatchesMultipleTracks()
        {
            // Arrange
            var randomWord1 = String.Format("Tweetinvi{0}", new Random().Next());
            var randomWord2 = String.Format("Tweetinvi2{0}", new Random().Next());

            var expectedMessage1 = String.Format("Hello {0}", randomWord1);
            var expectedMessage2 = String.Format("Hello {0}", randomWord2);
            var expectedMessage3 = String.Format("Hello {0} {1}", randomWord1, randomWord2);

            int success = 0;
            int failure = 0;

            IFilteredStream stream = new FilteredStream();
            stream.AddTrack(randomWord1);
            stream.AddTrack(randomWord2);

            Func<ITweet, List<string>, bool> listen = delegate(ITweet tweet, List<string> matches)
            {
                if (tweet != null)
                {
                    // ACT
                    bool result = matches.Count == stream.TracksCount;

                    if (result)
                    {
                        Debug.WriteLine(tweet.Text);
                        ++success;
                    }
                    else
                    {
                        ++failure;
                    }
                }

                // End the stream
                return true;
            };

            Thread t = ThreadTestHelper.StartLifetimedThread(
                () => stream.StartStream(TokenSingleton.Token, listen),
                stream.StopStream,
                STREAM_DELAY, STREAM_START_DELAY);

            // Act
            ITweet newTweet1 = new Tweet(expectedMessage1, TokenTestSingleton.Instance);
            newTweet1.Publish();

            ITweet newTweet2 = new Tweet(expectedMessage2, TokenTestSingleton.Instance);
            newTweet2.Publish();

            ITweet newTweet3 = new Tweet(expectedMessage3, TokenTestSingleton.Instance);
            newTweet3.Publish();

            t.Join();

            // Cleanup
            newTweet1.Destroy();
            newTweet2.Destroy();
            newTweet3.Destroy();

            // Assert
            Assert.AreEqual(success, 1);
            Assert.AreEqual(failure, 2);
        }

        #endregion
    }

    [TestClass]
    public class FilteredStreamMoqTests
    {
        private MockRepository _mockery;
        private Mock<IToken> _mockToken;
        private const string BASE_URL = "https://stream.twitter.com/1.1/statuses/filter.json?";

        [TestInitialize]
        public void TestInitialize()
        {
            _mockery = new MockRepository(MockBehavior.Default)
            {
                DefaultValue = DefaultValue.Mock
            };

            _mockToken = _mockery.Create<IToken>();
        }

        #region QueryConstructor

        [TestMethod]
        public void StartStream_NoTrack_CreateProperWebRequest()
        {
            // Arrange
            var streamFilter = CreateStreamFilter();

            // Act
            try
            {
                streamFilter.StartStream(_mockToken.Object, (Action<ITweet>)null);
            }
            catch (InvalidCastException) { }

            // Assert
            _mockToken.Verify(x => x.GetQueryWebRequest(BASE_URL, HttpMethod.POST, null));
        }

        [TestMethod]
        public void StartStream_UniqueTrack_CreateProperWebRequest()
        {
            // Arrange
            var streamFilter = CreateStreamFilter();
            string track1 = "Track1 is good";
            streamFilter.AddTrack(track1);

            // Act
            try
            {
                streamFilter.StartStream(_mockToken.Object, (Action<ITweet>)null);
            }
            catch (InvalidCastException) { }

            // Assert
            string expectedURL = String.Format("{0}track={1}", BASE_URL, Uri.EscapeDataString(track1.ToLower()));
            _mockToken.Verify(x => x.GetQueryWebRequest(expectedURL, HttpMethod.POST, null));
        }

        [TestMethod]
        public void StartStream_MultipleTracks_CreateProperWebRequest()
        {
            // Arrange
            var streamFilter = CreateStreamFilter();

            const string track1 = "Track1 is good";
            const string track2 = "Track2 is too";
            streamFilter.AddTrack(track1);
            streamFilter.AddTrack(track2);

            // Act
            try
            {
                streamFilter.StartStream(_mockToken.Object, (Action<ITweet>)null);
            }
            catch (InvalidCastException) { }

            // Assert
            string expectedURL = String.Format("{0}track={1}%2C{2}", BASE_URL,
                                               Uri.EscapeDataString(track1.ToLower()),
                                               Uri.EscapeDataString(track2.ToLower()));
            _mockToken.Verify(x => x.GetQueryWebRequest(expectedURL, HttpMethod.POST, null));
        }

        #endregion

        public IFilteredStream CreateStreamFilter()
        {
            return new FilteredStream();
        }
    }
}
