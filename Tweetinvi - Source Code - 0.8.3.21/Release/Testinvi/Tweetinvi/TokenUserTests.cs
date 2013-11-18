using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TweetinCore.Interfaces;
using TweetinCore.Interfaces.TwitterToken;
using Tweetinvi;
using TwitterToken;

namespace Testinvi.Tweetinvi
{
    [TestClass]
    public class TokenUserTests
    {
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

        #region Constructor

        [TestMethod]
        [TestCategory("Constructor"), TestCategory("TokenUser")]
        public void TokenUserConstructor()
        {
            TokenUser me = new TokenUser(TokenTestSingleton.Instance);
            IUser userMe = new User(TokenTestSingleton.ScreenName, TokenTestSingleton.Instance);

            Assert.AreEqual(TokenTestSingleton.ScreenName, me.ScreenName);
            Assert.AreEqual(userMe.Equals(me), true);
        }

        #endregion

        #region Publish Message

        [TestMethod]
        public void SendMessageToMultipleUsers()
        {
            // Arrange
            ITokenUser u = new TokenUser(TokenTestSingleton.Instance);
            List<IUser> followers = u.Followers;

            // Act
            var result = u.PublishMessage(String.Format("Hello {0}", DateTime.Now), followers, u.ObjectToken);

            // Assert
            Assert.IsTrue(result.Values.Where(x => x != null).Count() == followers.Count);

            // Cleanup
            foreach (var message in result.Values.Where(x => x != null))
            {
                message.Destroy(u.ObjectToken);
            }
        }

        #endregion

        #region GetDirectMessagesReceived

        [TestMethod]
        [TestCategory("GetDirectMessagesReceived"), TestCategory("User")]
        public void UserGetDirectMessagesReceived()
        {
            // This method is not right as the GetDirectMessage retrieve information for the TokenUser
            ITokenUser u = new TokenUser(TokenTestSingleton.Instance);
            List<IMessage> messages = u.GetLatestDirectMessagesReceived();

            Assert.AreNotEqual(messages, null);
            Assert.AreNotEqual(messages.Count, 0);
        }

        #endregion

        #region GetDirectMessagesSent

        [TestMethod]
        [TestCategory("GetDirectMessagesSent"), TestCategory("User")]
        public void UserGetDirectMessagesSent()
        {
            // This method is not right as the GetDirectMessage retrieve information for the TokenUser
            ITokenUser u = new TokenUser(TokenTestSingleton.Instance);
            string testValue = string.Format("Hello Test ({0})!", DateTime.Now);
            IMessage msg = new Message(testValue, u);
            msg.Publish(TokenTestSingleton.Instance);

            Thread.Sleep(2000);
            List<IMessage> messages = u.GetLatestDirectMessagesSent();

            Assert.AreNotEqual(messages, null);
            Assert.AreEqual(messages[0].Text == testValue, true);
            Assert.AreEqual(messages[0].Sender.Equals(u), true);
        }

        #endregion

        #region HomeTimeline

        [TestMethod]
        [TestCategory("GetHomeTimeline"), TestCategory("User")]
        public void UserGetLatestHomeTimeline()
        {
            ITokenUser u = new TokenUser(TokenTestSingleton.Instance);
            List<ITweet> tweets = u.GetLatestHomeTimeline();

            Assert.AreNotEqual(tweets, null);
            Assert.AreNotEqual(tweets.Count, 0);
        }

        [TestMethod]
        [TestCategory("GetHomeTimeline"), TestCategory("User")]
        public void UserGetHomeTimeline()
        {
            ITokenUser u = new TokenUser(TokenTestSingleton.Instance);
            List<ITweet> tweets = u.GetHomeTimeline(20, true, true);

            Assert.AreNotEqual(tweets, null);
            Assert.AreNotEqual(tweets.Count, 0);
        }

        #endregion

        #region MentionsTimeline

        [TestMethod]
        [TestCategory("GetMentions"), TestCategory("User")]
        public void UserGetMentions()
        {
            ITokenUser u = new TokenUser(TokenTestSingleton.Instance);
            List<IMention> mentions = u.GetLatestMentionsTimeline();

            Assert.AreNotEqual(mentions, null);
            Assert.AreNotEqual(mentions.Count, 0);
        }

        #endregion

        #region Friends - Followers

        [TestMethod]
        public void Follow()
        {
            // Arrange
            ITokenUser u = new TokenUser(_token1);
            ITokenUser u2 = new TokenUser(_token2);

            // Act
            var result = u.Follow(u2, true);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Unfollow()
        {
            // Arrange
            ITokenUser u = new TokenUser(_token1);
            ITokenUser u2 = new TokenUser(_token2);

            // Act
            var result = u.Unfollow(u2);

            // Assert
            Assert.IsTrue(result);
        }

        // This test should start with the 2 TokenUser not knowing each other
        [TestMethod]
        public void FriendshipTest()
        {
            // Arrange
            ITokenUser u = new TokenUser(_token1);
            ITokenUser u2 = new TokenUser(_token2);

            // Act
            IRelationship relationship = u.GetRelationship(u2);

            // Assert
            Assert.IsFalse(relationship.Following);
            Assert.IsFalse(relationship.FollowedBy);

            // Act
            u.Follow(u2, true);
            relationship = u.GetRelationship(u2);

            // Assert
            Assert.IsTrue(relationship.Following);
            Assert.IsFalse(relationship.FollowedBy);

            // Act
            u2.Follow(u, true);
            relationship = u2.GetRelationship(u);

            // Assert
            Assert.IsTrue(relationship.Following);
            Assert.IsTrue(relationship.FollowedBy);

            // Act
            u.Unfollow(u2);
            relationship = u.GetRelationship(u2);

            // Assert
            Assert.IsFalse(relationship.Following);
            Assert.IsTrue(relationship.FollowedBy);

            // Act
            u2.Unfollow(u);
            relationship = u2.GetRelationship(u);

            // Assert
            Assert.IsFalse(relationship.Following);
            Assert.IsFalse(relationship.FollowedBy);
        }

        #endregion

        #region GetBlockedUsers

        [TestMethod]
        [TestCategory("GetBlockedUsers"), TestCategory("User")]
        public void UserGetBlockedUsers()
        {
            ITokenUser u = new TokenUser(TokenTestSingleton.Instance);
            List<IUser> users = u.GetBlockedUsers();

            Assert.AreNotEqual(users, null);
            Assert.AreNotEqual(users.Count, 0);
        }

        #endregion

        #region Suggested Users

        [TestMethod]
        [TestCategory("Suggested Users"), TestCategory("TokenUser")]
        public void UserSuggestedUsers()
        {
            ITokenUser u = new TokenUser(TokenTestSingleton.Instance);
            List<ISuggestedUserList> userList = u.GetSuggestedUserList();

            Assert.AreNotEqual(userList, null);
            Assert.AreNotEqual(userList.Count, 0);
        }

        #endregion
    }
}
