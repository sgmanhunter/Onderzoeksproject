using System;
using System.Configuration;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Streaminvi;
using Testinvi.Helpers;
using TweetinCore.Events;
using TweetinCore.Interfaces;
using TweetinCore.Interfaces.StreamInvi;
using TweetinCore.Interfaces.TwitterToken;
using Tweetinvi;
using TwitterToken;

namespace Testinvi.Streaminvi
{
    [TestClass]
    public class UserStreamTests
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

        #region Publish Tweets
        // Tweets Created by Token1
        [TestMethod]
        public void StartStream_TokenUser1PublishATweet_TweetCreatedByMeRaised()
        {
            // Arrange
            IUserStream userStream = new UserStream(_token1);
            string expectedMessage = String.Format("Tweetinvi UserStream is good! ({0})", Guid.NewGuid());
            ITweet tweet = new Tweet(expectedMessage);

            bool eventRaised = false;
            userStream.TweetCreatedByMe += (sender, args) =>
            {
                eventRaised = expectedMessage == args.Value.Text;
            };

            Thread t = ThreadTestHelper.StartLifetimedThread(
                userStream.StartStream,
                userStream.StopStream,
                STREAM_DELAY, STREAM_START_DELAY);

            // Act
            userStream.TokenUser.PublishTweet(tweet);
            t.Join();

            // Cleanup
            tweet.Destroy(_token1);

            // Assert
            Assert.IsTrue(eventRaised);
        }

        [TestMethod]
        public void StartStream_TokenUser1PublishATweet_TweetCreatedByAnyRaised()
        {
            // Arrange
            IUserStream userStream = new UserStream(_token1);
            string expectedMessage = String.Format("Tweetinvi UserStream is good! ({0})", Guid.NewGuid());
            ITweet tweet = new Tweet(expectedMessage);

            bool eventRaised = false;
            userStream.TweetCreatedByAnyone += (sender, args) =>
            {
                eventRaised = expectedMessage == args.Value.Text;
            };

            Thread t = ThreadTestHelper.StartLifetimedThread(
                userStream.StartStream,
                userStream.StopStream,
                STREAM_DELAY, STREAM_START_DELAY);

            // Act
            userStream.TokenUser.PublishTweet(tweet);
            t.Join();

            // Cleanup
            tweet.Destroy(_token1);

            // Assert
            Assert.IsTrue(eventRaised);
        }

        [TestMethod]
        public void StartStream_TokenUser1PublishATweet_TweetCreatedByAnyoneButMeNOTRaised()
        {
            // Arrange
            IUserStream userStream = new UserStream(_token1);
            string expectedMessage = String.Format("Tweetinvi UserStream is good! ({0})", Guid.NewGuid());
            ITweet tweet = new Tweet(expectedMessage);

            bool eventRaised = false;
            userStream.TweetCreatedByAnyoneButMe += (sender, args) =>
            {
                eventRaised = expectedMessage == args.Value.Text;
            };

            Thread t = ThreadTestHelper.StartLifetimedThread(
                userStream.StartStream,
                userStream.StopStream,
                STREAM_DELAY, STREAM_START_DELAY);

            // Act
            userStream.TokenUser.PublishTweet(tweet);
            t.Join();

            // Cleanup
            tweet.Destroy(_token1);

            // Assert
            Assert.IsFalse(eventRaised);
        }

        // Tweets Created by Token2

        // This test currently requires that your TokenUser1 follows TokenUser2
        [TestMethod]
        public void StartStream_TokenUser2PublishATweet_TweetCreatedByMeNOTRaised()
        {
            // Arrange
            IUserStream userStream = new UserStream(_token1);
            string expectedMessage = String.Format("Tweetinvi UserStream is good! ({0})", Guid.NewGuid());
            ITweet tweet = new Tweet(expectedMessage, _token2);

            bool eventRaised = false;
            userStream.TweetCreatedByMe += (sender, args) =>
            {
                eventRaised = expectedMessage == args.Value.Text;
            };

            Thread t = ThreadTestHelper.StartLifetimedThread(
                userStream.StartStream,
                userStream.StopStream,
                STREAM_DELAY, STREAM_START_DELAY);

            // Act
            tweet.Publish();
            t.Join();

            // Cleanup
            tweet.Destroy();

            // Assert
            Assert.IsFalse(eventRaised);
        }

        // This test currently requires that your TokenUser1 follows TokenUser2
        [TestMethod]
        public void StartStream_TokenUser2PublishATweet_TweetCreatedAnyoneButMeRaised()
        {
            // Arrange
            IUserStream userStream = new UserStream(_token1);
            string expectedMessage = String.Format("Tweetinvi UserStream is good! ({0})", Guid.NewGuid());
            ITweet tweet = new Tweet(expectedMessage, _token2);

            bool eventRaised = false;
            userStream.TweetCreatedByAnyoneButMe += (sender, args) =>
            {
                eventRaised = expectedMessage == args.Value.Text;
            };

            Thread t = ThreadTestHelper.StartLifetimedThread(
                userStream.StartStream,
                userStream.StopStream,
                STREAM_DELAY, STREAM_START_DELAY);

            // Act
            tweet.Publish();
            t.Join();

            // Cleanup
            tweet.Destroy();

            // Assert
            Assert.IsTrue(eventRaised);
        }

        // This test currently requires that your TokenUser1 follows TokenUser2
        [TestMethod]
        public void StartStream_TokenUser2PublishATweet_TweetCreatedByAnyRaised()
        {
            // Arrange
            IUserStream userStream = new UserStream(_token1);
            string expectedMessage = String.Format("Tweetinvi UserStream is good! ({0})", Guid.NewGuid());
            ITweet tweet = new Tweet(expectedMessage, _token2);

            bool eventRaised = false;
            userStream.TweetCreatedByAnyone += (sender, args) =>
            {
                eventRaised = expectedMessage == args.Value.Text;
            };

            Thread t = ThreadTestHelper.StartLifetimedThread(
                userStream.StartStream,
                userStream.StopStream,
                STREAM_DELAY, STREAM_START_DELAY);

            // Act
            tweet.Publish();
            t.Join();

            // Cleanup
            tweet.Destroy();

            // Assert
            Assert.IsTrue(eventRaised);
        }
        #endregion

        #region Publish Tracked Tweets
        // Tweets Created by Token1
        [TestMethod]
        public void StartStream_TokenUser1PublishATweet_TrackedTweetCreatedByMeRaised()
        {
            // Arrange
            IUserStream userStream = new UserStream(_token1);
            string expectedMessage = String.Format("Tweetinvi Track {0}", Guid.NewGuid());
            string notExpectedMessage = String.Format("Tweetinvi NotTrack {0}", Guid.NewGuid());

            ITweet expectedtweet = new Tweet(expectedMessage);
            ITweet notExpectedTweet = new Tweet(notExpectedMessage);

            int nbRaised = 0;
            bool eventRaised = false;

            userStream.AddTrack(expectedMessage);
            userStream.TrackedTweetCreatedByMe += (sender, args) =>
            {
                ++nbRaised;
                eventRaised = expectedMessage == args.Value.Text;
            };

            Thread t = ThreadTestHelper.StartLifetimedThread(
                userStream.StartStream,
                userStream.StopStream,
                STREAM_DELAY, STREAM_START_DELAY);

            // Act
            userStream.TokenUser.PublishTweet(expectedtweet);
            userStream.TokenUser.PublishTweet(notExpectedTweet);
            t.Join();

            // Cleanup
            expectedtweet.Destroy(_token1);
            notExpectedTweet.Destroy(_token1);

            // Assert
            Assert.AreEqual(nbRaised, 1);
            Assert.IsTrue(eventRaised);
        }

        [TestMethod]
        public void StartStream_TokenUser1PublishATweet_TrackedTweetCreatedByAnyRaised()
        {
            // Arrange
            IUserStream userStream = new UserStream(_token1);
            string expectedMessage = String.Format("Tweetinvi Track {0}", Guid.NewGuid());
            string notExpectedMessage = String.Format("Tweetinvi NotTrack {0}", Guid.NewGuid());

            ITweet expectedtweet = new Tweet(expectedMessage);
            ITweet notExpectedTweet = new Tweet(notExpectedMessage);

            int nbRaised = 0;
            bool eventRaised = false;

            userStream.AddTrack(expectedMessage);
            userStream.TrackedTweetCreatedByMe += (sender, args) =>
            {
                ++nbRaised;
                eventRaised = expectedMessage == args.Value.Text;
            };

            Thread t = ThreadTestHelper.StartLifetimedThread(
                userStream.StartStream,
                userStream.StopStream,
                STREAM_DELAY, STREAM_START_DELAY);

            // Act
            userStream.TokenUser.PublishTweet(expectedtweet);
            t.Join();

            // Cleanup
            expectedtweet.Destroy(_token1);
            notExpectedTweet.Destroy(_token1);

            // Assert
            Assert.AreEqual(nbRaised, 1);
            Assert.IsTrue(eventRaised);
        }

        [TestMethod]
        public void StartStream_TokenUser1PublishATweet_TrackedTweetCreatedByAnyoneButMeNOTRaised()
        {
            // Arrange
            IUserStream userStream = new UserStream(_token1);
            string expectedMessage = String.Format("Tweetinvi Track {0}", Guid.NewGuid());
            string notExpectedMessage = String.Format("Tweetinvi NotTrack {0}", Guid.NewGuid());

            ITweet expectedtweet = new Tweet(expectedMessage);
            ITweet notExpectedTweet = new Tweet(notExpectedMessage);

            int nbRaised = 0;
            bool eventRaised = false;

            userStream.AddTrack(expectedMessage);
            userStream.TrackedTweetCreatedByAnyoneButMe += (sender, args) =>
            {
                ++nbRaised;
                eventRaised = expectedMessage == args.Value.Text;
            };

            Thread t = ThreadTestHelper.StartLifetimedThread(
                userStream.StartStream,
                userStream.StopStream,
                STREAM_DELAY, STREAM_START_DELAY);

            // Act
            userStream.TokenUser.PublishTweet(expectedtweet);
            userStream.TokenUser.PublishTweet(notExpectedTweet);
            t.Join();

            // Cleanup
            expectedtweet.Destroy(_token1);
            notExpectedTweet.Destroy(_token1);

            // Assert
            Assert.AreEqual(nbRaised, 0);
            Assert.IsFalse(eventRaised);
        }

        // Tweets Created by Token2

        // This test currently requires that your TokenUser1 follows TokenUser2
        [TestMethod]
        public void StartStream_TokenUser2PublishATweet_TrackedTweetCreatedByMeNOTRaised()
        {
            // Arrange
            IUserStream userStream = new UserStream(_token1);
            string expectedMessage = String.Format("Tweetinvi Track {0}", Guid.NewGuid());
            string notExpectedMessage = String.Format("Tweetinvi NotTrack {0}", Guid.NewGuid());

            ITweet expectedtweet = new Tweet(expectedMessage, _token2);
            ITweet notExpectedTweet = new Tweet(notExpectedMessage, _token2);

            int nbRaised = 0;
            bool eventRaised = false;

            userStream.AddTrack(expectedMessage);
            userStream.TrackedTweetCreatedByMe += (sender, args) =>
            {
                ++nbRaised;
                eventRaised = expectedMessage == args.Value.Text;
            };

            Thread t = ThreadTestHelper.StartLifetimedThread(
                userStream.StartStream,
                userStream.StopStream,
                STREAM_DELAY, STREAM_START_DELAY);

            // Act
            expectedtweet.Publish();
            notExpectedTweet.Publish();
            t.Join();

            // Cleanup
            expectedtweet.Destroy();
            notExpectedTweet.Destroy();

            // Assert
            Assert.AreEqual(nbRaised, 0);
            Assert.IsFalse(eventRaised);
        }

        // This test currently requires that your TokenUser1 follows TokenUser2
        [TestMethod]
        public void StartStream_TokenUser2PublishATweet_TrackedTweetCreatedAnyoneButMeRaised()
        {
            // Arrange
            IUserStream userStream = new UserStream(_token1);
            string expectedMessage = String.Format("Tweetinvi Track {0}", Guid.NewGuid());
            string notExpectedMessage = String.Format("Tweetinvi NotTrack {0}", Guid.NewGuid());

            ITweet expectedtweet = new Tweet(expectedMessage, _token2);
            ITweet notExpectedTweet = new Tweet(notExpectedMessage, _token2);

            int nbRaised = 0;
            bool eventRaised = false;

            userStream.AddTrack(expectedMessage);
            userStream.TrackedTweetCreatedByAnyoneButMe += (sender, args) =>
            {
                ++nbRaised;
                eventRaised = expectedMessage == args.Value.Text;
            };

            Thread t = ThreadTestHelper.StartLifetimedThread(
                userStream.StartStream,
                userStream.StopStream,
                STREAM_DELAY, STREAM_START_DELAY);

            // Act
            expectedtweet.Publish();
            notExpectedTweet.Publish();
            t.Join();

            // Cleanup
            expectedtweet.Destroy();
            notExpectedTweet.Destroy();

            // Assert
            Assert.AreEqual(nbRaised, 1);
            Assert.IsTrue(eventRaised);
        }

        // This test currently requires that your TokenUser1 follows TokenUser2
        [TestMethod]
        public void StartStream_TokenUser2PublishATweet_TrackedTweetCreatedByAnyRaised()
        {
            // Arrange
            IUserStream userStream = new UserStream(_token1);
            string expectedMessage = String.Format("Tweetinvi Track {0}", Guid.NewGuid());
            string notExpectedMessage = String.Format("Tweetinvi NotTrack {0}", Guid.NewGuid());

            ITweet expectedtweet = new Tweet(expectedMessage, _token2);
            ITweet notExpectedTweet = new Tweet(notExpectedMessage, _token2);

            int nbRaised = 0;
            bool eventRaised = false;

            userStream.AddTrack(expectedMessage);
            userStream.TrackedTweetCreatedByAnyone += (sender, args) =>
            {
                ++nbRaised;
                eventRaised = expectedMessage == args.Value.Text;
            };

            Thread t = ThreadTestHelper.StartLifetimedThread(
                userStream.StartStream,
                userStream.StopStream,
                STREAM_DELAY, STREAM_START_DELAY);

            // Act
            expectedtweet.Publish();
            notExpectedTweet.Publish();
            t.Join();

            // Cleanup
            expectedtweet.Destroy();
            notExpectedTweet.Destroy();

            // Assert
            Assert.AreEqual(nbRaised, 1);
            Assert.IsTrue(eventRaised);
        }
        #endregion

        #region Delete Tweets

        [TestMethod]
        public void StartStream_TokenUser1DeleteTweet_TweetDeletedByMeAndAnyoneRaised()
        {
            // Arrange
            IUserStream userStream = new UserStream(_token1);
            string expectedMessage = String.Format("Tweetinvi UserStream is good! ({0})", Guid.NewGuid());
            ITweet tweet = new Tweet(expectedMessage);

            bool tweetHasBeenDeletedByMe = false;
            userStream.TweetDeletedByMe += (sender, args) =>
            {
                tweetHasBeenDeletedByMe = tweet.Id == args.Value;
            };

            bool tweetHasBeenDeletedByAnyone = false;
            userStream.TweetDeletedByAnyone += (sender, args) =>
            {
                tweetHasBeenDeletedByAnyone = tweet.Id == args.Value;
            };

            bool tweetHasBeenDeletedByAnyoneButMe = false;
            userStream.TweetDeletedByAnyoneButMe += (sender, args) =>
            {
                tweetHasBeenDeletedByAnyoneButMe = true;
            };

            Thread t = ThreadTestHelper.StartLifetimedThread(
                userStream.StartStream,
                userStream.StopStream,
                STREAM_DELAY, STREAM_START_DELAY);

            userStream.TokenUser.PublishTweet(tweet);

            // Act
            tweet.Destroy(_token1);
            t.Join();

            // Assert
            Assert.IsTrue(tweetHasBeenDeletedByMe);
            Assert.IsTrue(tweetHasBeenDeletedByAnyone);
            Assert.IsFalse(tweetHasBeenDeletedByAnyoneButMe);
        }

        [TestMethod]
        public void StartStream_TokenUser2DeleteTweet_TweetDeletedByAnyoneButMeRaised()
        {
            // Arrange
            IUserStream userStream = new UserStream(_token1);
            string expectedMessage = String.Format("Tweetinvi UserStream is good! ({0})", Guid.NewGuid());
            ITweet tweet = new Tweet(expectedMessage, _token2);

            bool tweetHasBeenDeletedByMe = false;
            userStream.TweetDeletedByMe += (sender, args) =>
            {
                tweetHasBeenDeletedByMe = true;
            };

            bool tweetHasBeenDeletedByAnyone = false;
            userStream.TweetDeletedByAnyone += (sender, args) =>
            {
                tweetHasBeenDeletedByAnyone = tweet.Id == args.Value;
            };

            bool tweetHasBeenDeletedByAnyoneButMe = false;
            userStream.TweetDeletedByAnyoneButMe += (sender, args) =>
            {
                tweetHasBeenDeletedByAnyoneButMe = tweet.Id == args.Value;
            };

            Thread t = ThreadTestHelper.StartLifetimedThread(
                userStream.StartStream,
                userStream.StopStream,
                STREAM_DELAY, STREAM_START_DELAY);

            tweet.Publish();

            // Act
            tweet.Destroy();
            t.Join();

            // Assert
            Assert.IsFalse(tweetHasBeenDeletedByMe);
            Assert.IsTrue(tweetHasBeenDeletedByAnyone);
            Assert.IsTrue(tweetHasBeenDeletedByAnyoneButMe);
        }

        #endregion

        #region Publish Messages

        [TestMethod]
        public void StartStream_TokenUser1PublishAMessage_RaiseAppropriateEvents()
        {
            // Arrange
            IUserStream userStream = new UserStream(_token1);
            string expectedMessage = String.Format("Tweetinvi UserStream is good! ({0})", Guid.NewGuid());

            TokenUser user2 = new TokenUser(_token2);
            IMessage sentMessage = new Message(expectedMessage, user2, _token1);

            bool eventRaisedByMe = false;
            userStream.MessageSentByMeToX += (sender, args) =>
            {
                eventRaisedByMe = expectedMessage == args.Value.Text;
            };

            bool eventRaisedByAnyone = false;
            userStream.MessageSentOrReceived += (sender, args) =>
            {
                eventRaisedByAnyone = expectedMessage == args.Value.Text;
            };

            bool eventRaisedByAnyoneButMe = false;
            userStream.MessageReceivedFromX += (sender, args) =>
            {
                eventRaisedByAnyoneButMe = true;
            };

            Thread t = ThreadTestHelper.StartLifetimedThread(
                userStream.StartStream,
                userStream.StopStream,
                STREAM_DELAY, STREAM_START_DELAY);

            // Act
            sentMessage.Publish();
            // IMessage receivedMessage = new Message(sentMessage.Id, _token2);
            t.Join();

            // Cleanup
            sentMessage.Destroy(_token1);
            // There is no need to delete the message twice oO
            // receivedMessage.Destroy();

            // Assert
            Assert.IsTrue(eventRaisedByMe);
            Assert.IsTrue(eventRaisedByAnyone);
            Assert.IsFalse(eventRaisedByAnyoneButMe);
        }

        [TestMethod]
        public void StartStream_TokenUser2PublishAMessageToUser1_RaiseAppropriateEvents()
        {
            // Arrange
            IUserStream userStream = new UserStream(_token1);
            string expectedMessage = String.Format("Tweetinvi UserStream is good! ({0})", Guid.NewGuid());

            IMessage sentMessage = new Message(expectedMessage, userStream.TokenUser, _token2);

            bool eventRaisedByMe = false;
            userStream.MessageSentByMeToX += (sender, args) =>
            {
                eventRaisedByMe = true;
            };

            bool eventRaisedByAnyone = false;
            userStream.MessageSentOrReceived += (sender, args) =>
            {
                eventRaisedByAnyone = expectedMessage == args.Value.Text;
            };

            bool eventRaisedByAnyoneButMe = false;
            userStream.MessageReceivedFromX += (sender, args) =>
            {
                eventRaisedByAnyoneButMe = expectedMessage == args.Value.Text;
            };

            Thread t = ThreadTestHelper.StartLifetimedThread(
                userStream.StartStream,
                userStream.StopStream,
                STREAM_DELAY, STREAM_START_DELAY);

            // Act
            sentMessage.Publish();
            t.Join();

            // Cleanup
            sentMessage.Destroy();

            // Assert
            Assert.IsFalse(eventRaisedByMe);
            Assert.IsTrue(eventRaisedByAnyone);
            Assert.IsTrue(eventRaisedByAnyoneButMe);
        }

        #endregion

        #region Follower - Friends

        [TestMethod]
        public void FollowerAndFriends_MultipleChecks()
        {
            // Arrange - Global
            ITokenUser user1 = new TokenUser(_token1);
            ITokenUser user2 = new TokenUser(_token2);

            user1.Unfollow(user2);
            user2.Unfollow(user1);

            IUserStream user1Stream = new UserStream(user1);
            IUserStream user2Stream = new UserStream(user2);

            Thread t;
            Thread t2;

            #region Event Tester Initialization
            IEventTestHelper<GenericEventArgs<ITokenUser, IUser>> eventUser1FollowMoq =
                    new EventTestHelper<GenericEventArgs<ITokenUser, IUser>>();

            IEventTestHelper<GenericEventArgs<ITokenUser, IUser>> eventUser1FollowedByMoq =
                new EventTestHelper<GenericEventArgs<ITokenUser, IUser>>();

            IEventTestHelper<GenericEventArgs<ITokenUser, IUser>> eventUser1UnFollowMoq =
                new EventTestHelper<GenericEventArgs<ITokenUser, IUser>>();

            IEventTestHelper<GenericEventArgs<ITokenUser, IUser>> eventUser2FollowMoq =
                new EventTestHelper<GenericEventArgs<ITokenUser, IUser>>();

            IEventTestHelper<GenericEventArgs<ITokenUser, IUser>> eventUser2FollowedByMoq =
               new EventTestHelper<GenericEventArgs<ITokenUser, IUser>>();

            IEventTestHelper<GenericEventArgs<ITokenUser, IUser>> eventUser2UnFollowMoq =
                new EventTestHelper<GenericEventArgs<ITokenUser, IUser>>();

            user1Stream.FollowUser += eventUser1FollowMoq.EventAction;
            user1Stream.FollowedByUser += eventUser1FollowedByMoq.EventAction;
            user1Stream.UnFollowUser += eventUser1UnFollowMoq.EventAction;

            user2Stream.FollowUser += eventUser2FollowMoq.EventAction;
            user2Stream.FollowedByUser += eventUser2FollowedByMoq.EventAction;
            user2Stream.UnFollowUser += eventUser2UnFollowMoq.EventAction;

            // Assert
            IRelationship rel = user1.GetRelationship(user2);
            Assert.IsFalse(rel.Following);
            Assert.IsFalse(rel.FollowedBy);

            #endregion

            #region User1 follows User2
            // Arrange
            t = ThreadTestHelper.StartLifetimedThread(
                user1Stream.StartStream,
                user1Stream.StopStream,
                STREAM_DELAY + STREAM_START_DELAY);

            t2 = ThreadTestHelper.StartLifetimedThread(
                user2Stream.StartStream,
                user2Stream.StopStream,
                STREAM_DELAY, STREAM_START_DELAY);

            // Act
            user1.Follow(user2, true);
            t.Join();
            t2.Join();

            // Assert
            eventUser1FollowMoq.VerifyAtWhere(0, args => args.Value.Id == user1.Id && args.Value2.Id == user2.Id);
            eventUser1FollowedByMoq.VerifyNumberOfCalls(0);
            eventUser1UnFollowMoq.VerifyNumberOfCalls(0);

            eventUser2FollowMoq.VerifyNumberOfCalls(0);
            eventUser2FollowedByMoq.VerifyAtWhere(0, args => args.Value.Id == user2.Id && args.Value2.Id == user1.Id);
            eventUser2UnFollowMoq.VerifyNumberOfCalls(0);

            // ReSharper disable PossibleInvalidOperationException
            Assert.IsTrue(user1.FriendIds.Contains((long)user2.Id));

            Assert.IsFalse(user1.FollowerIds.Contains((long)user2.Id));
            Assert.IsFalse(user2.FriendIds.Contains((long)user1.Id));
            Assert.IsTrue(user2.FollowerIds.Contains((long)user1.Id));

            rel = user1.GetRelationship(user2);
            Assert.IsTrue(rel.Following);
            Assert.IsFalse(rel.FollowedBy);

            // Cleanup
            eventUser1FollowMoq.ResetMetadata();
            eventUser2FollowedByMoq.ResetMetadata();
            #endregion

            #region User2 follows User1
            // Arrange
            t = ThreadTestHelper.StartLifetimedThread(
                user1Stream.StartStream,
                user1Stream.StopStream,
                STREAM_DELAY + STREAM_START_DELAY);

            t2 = ThreadTestHelper.StartLifetimedThread(
                user2Stream.StartStream,
                user2Stream.StopStream,
                STREAM_DELAY, STREAM_START_DELAY);

            // Act
            user2.Follow(user1, true);
            t.Join();
            t2.Join();

            // Assert
            eventUser1FollowMoq.VerifyNumberOfCalls(0);
            eventUser1FollowedByMoq.VerifyAtWhere(0, args => args.Value.Id == user1.Id && args.Value2.Id == user2.Id);
            eventUser1UnFollowMoq.VerifyNumberOfCalls(0);

            eventUser2FollowMoq.VerifyAtWhere(0, args => args.Value.Id == user2.Id && args.Value2.Id == user1.Id);
            eventUser2FollowedByMoq.VerifyNumberOfCalls(0);
            eventUser2UnFollowMoq.VerifyNumberOfCalls(0);

            Assert.IsTrue(user1.FriendIds.Contains((long)user2.Id));
            Assert.IsTrue(user1.FollowerIds.Contains((long)user2.Id));
            Assert.IsTrue(user2.FriendIds.Contains((long)user1.Id));
            Assert.IsTrue(user2.FollowerIds.Contains((long)user1.Id));

            rel = user1.GetRelationship(user2);
            Assert.IsTrue(rel.Following);
            Assert.IsTrue(rel.FollowedBy);

            // Cleanup
            eventUser1FollowedByMoq.ResetMetadata();
            eventUser2FollowMoq.ResetMetadata();

            #endregion

            #region User1 unfollow User2
            // Arrange
            t = ThreadTestHelper.StartLifetimedThread(
                user1Stream.StartStream,
                user1Stream.StopStream,
                STREAM_DELAY + STREAM_START_DELAY);

            t2 = ThreadTestHelper.StartLifetimedThread(
                user2Stream.StartStream,
                user2Stream.StopStream,
                STREAM_DELAY, STREAM_START_DELAY);

            // Act
            user1.Unfollow(user2);
            t.Join();
            t2.Join();

            // Assert
            eventUser1FollowMoq.VerifyNumberOfCalls(0);
            eventUser1FollowedByMoq.VerifyNumberOfCalls(0);
            eventUser1UnFollowMoq.VerifyAtWhere(0, args => args.Value.Id == user1.Id && args.Value2.Id == user2.Id);

            eventUser2FollowMoq.VerifyNumberOfCalls(0);
            eventUser2FollowedByMoq.VerifyNumberOfCalls(0);
            eventUser2UnFollowMoq.VerifyNumberOfCalls(0);

            Assert.IsFalse(user1.FriendIds.Contains((long)user2.Id));
            Assert.IsTrue(user1.FollowerIds.Contains((long)user2.Id));
            Assert.IsTrue(user2.FriendIds.Contains((long)user1.Id));
            // The api does not inform that another user has stopped following
            Assert.IsTrue(user2.FollowerIds.Contains((long)user1.Id));

            rel = user1.GetRelationship(user2);
            Assert.IsFalse(rel.Following);
            Assert.IsTrue(rel.FollowedBy);

            // Cleanup
            eventUser1UnFollowMoq.ResetMetadata();

            #endregion

            #region User2 unfollow User1
            // Arrange
            t = ThreadTestHelper.StartLifetimedThread(
                user1Stream.StartStream,
                user1Stream.StopStream,
                STREAM_DELAY + STREAM_START_DELAY);

            t2 = ThreadTestHelper.StartLifetimedThread(
                user2Stream.StartStream,
                user2Stream.StopStream,
                STREAM_DELAY, STREAM_START_DELAY);

            // Act
            user2.Unfollow(user1);
            t.Join();
            t2.Join();

            // Assert
            eventUser1FollowMoq.VerifyNumberOfCalls(0);
            eventUser1FollowedByMoq.VerifyNumberOfCalls(0);
            eventUser1UnFollowMoq.VerifyNumberOfCalls(0);

            eventUser2FollowMoq.VerifyNumberOfCalls(0);
            eventUser2FollowedByMoq.VerifyNumberOfCalls(0);
            eventUser2UnFollowMoq.VerifyAtWhere(0, args => args.Value.Id == user2.Id && args.Value2.Id == user1.Id);

            Assert.IsFalse(user1.FriendIds.Contains((long)user2.Id));
            // The api does not inform that another user has stopped following
            Assert.IsTrue(user1.FollowerIds.Contains((long)user2.Id));
            Assert.IsFalse(user2.FriendIds.Contains((long)user1.Id));
            Assert.IsTrue(user2.FollowerIds.Contains((long)user1.Id));

            rel = user1.GetRelationship(user2);
            Assert.IsFalse(rel.Following);
            Assert.IsFalse(rel.FollowedBy);

            // Cleanup
            eventUser2UnFollowMoq.ResetMetadata();
            // ReSharper restore PossibleInvalidOperationException
            #endregion
        }

        #endregion
    }
}
