using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TweetinCore.Interfaces;
using TweetinCore.Interfaces.TwitterToken;
using Tweetinvi;
using TwitterToken;

namespace Testinvi.Tweetinvi
{
    [TestClass]
    public class UserTests
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
        [TestCategory("Constructor"), TestCategory("User")]
        public void User_Constructor1()
        {
            long id = -1;
            IUser user = new User(id);

            Assert.AreEqual(user.Name, null);
        }

        [TestMethod]
        [TestCategory("Constructor"), TestCategory("User")]
        public void User_Constructor2()
        {
            // Expect the Ladygaga to be retrieved from Twitter
            IUser user = new User("ladygaga", TokenTestSingleton.Instance);

            Assert.AreEqual(user.Id, 14230524);
        }

        [TestMethod]
        [TestCategory("Constructor"), TestCategory("User")]
        public void User_Constructor3()
        {
            // Expect the Ladygaga to be retrieved from Twitter
            IUser user = new User(14230524, TokenTestSingleton.Instance);

            Assert.AreEqual(user.ScreenName, "ladygaga");
        } 
        #endregion

        #region GetFriends

        [TestMethod]
        [TestCategory("GetFriends"), TestCategory("User")]
        public void UserGetFriendIds()
        {
            IUser u = new User("tweetinviapi", TokenTestSingleton.Instance);
            u.PopulateFriendsFromFriendIds(true);

            Assert.AreNotEqual(u.FriendIds.Count, 0);
            Assert.AreEqual(u.FriendIds.Count, u.Friends.Count);
        }

        [TestMethod]
        [TestCategory("GetFriends"), TestCategory("User")]
        public void UserGetFriendIds1()
        {
            IUser u = new User("tweetinviapi", TokenTestSingleton.Instance);

            Assert.AreNotEqual(u.FriendIds.Count, 0);
            Assert.AreNotEqual(u.Friends.Count, 0);
        }

        #endregion

        #region GetFollowers

        [TestMethod]
        [TestCategory("GetFollowers"), TestCategory("User")]
        public void UserGetFollowers()
        {
            IUser u = new User("tweetinviapi", TokenTestSingleton.Instance);

            Assert.AreNotEqual(u.FollowerIds.Count, 0);
            Assert.AreEqual(u.FollowerIds.Count, u.Followers.Count);
        }

        #endregion

        #region Relationship

        [TestMethod]
        public void GetRelationship()
        {
            // Arrange
            ITokenUser u1 = new TokenUser(_token1);
            ITokenUser u2 = new TokenUser(_token2);

            // Act
            var relationship = u1.GetRelationship(u2);
        }

        #endregion

        #region DownloadProfileImage

        [TestMethod]
        [TestCategory("Image"), TestCategory("User")]
        public void UserDownloadProfileImage()
        {
            Debug.WriteLine(Directory.GetCurrentDirectory());
            
            string userName = "ladygaga";

            if (File.Exists(string.Format("{0}_normal.jpg", userName)))
            {
                File.Delete(string.Format("{0}_normal.jpg", userName));
            }

            IUser u = new User(userName, TokenTestSingleton.Instance);
            string filepath = u.DownloadProfileImage();

            Assert.AreEqual(filepath, string.Format("{0}_normal.jpg", userName));

            Assert.AreNotEqual(filepath, string.Empty);

            bool fileExist = File.Exists(filepath);
            Assert.AreEqual(fileExist, true);
        }

        #endregion

        #region GetContributors

        [TestMethod]
        [TestCategory("GetContributors"), TestCategory("User")]
        public void UserGetContributors()
        {
            IUser u = new User("ladygaga", TokenTestSingleton.Instance);
            List<IUser> contributors = u.GetContributors(true);

            Assert.AreNotEqual(contributors, null);
            Assert.AreEqual(contributors.Count, 0);
        }

        #endregion

        #region GetContributees
        
        [TestMethod]
        [TestCategory("GetContributees"), TestCategory("User")]
        public void UserGetContributees()
        {
            IUser u = new User("ladygaga", TokenTestSingleton.Instance);
            List<IUser> contributees = u.GetContributees(true);

            Assert.AreNotEqual(contributees, null);
            Assert.AreEqual(contributees.Count, 0);
        }

        #endregion

        #region UserTimeline

        [TestMethod]
        [TestCategory("User Timeline"), TestCategory("User")]
        public void UserGetTimeline()
        {
            IUser u = new User("ladygaga", TokenTestSingleton.Instance);
            List<ITweet> tweets = u.GetUserTimeline();

            Assert.AreNotEqual(tweets, null);
            Assert.AreNotEqual(tweets.Count, 0);
        }

        #endregion
    }
}
