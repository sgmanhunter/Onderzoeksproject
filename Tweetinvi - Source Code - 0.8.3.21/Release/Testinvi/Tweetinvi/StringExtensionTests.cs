using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TweetinCore.Extensions;

namespace Testinvi.Tweetinvi
{
    [TestClass]
    public class StringExtensionTests
    {
        [TestMethod]
        public void TestLengthWith2Urls()
        {
            string test = "Hello http://tweetinvi.codeplex.com/salutLescopains 3615 Gerard www.linviIsMe.com piloupe";

            int twitterLength = StringExtension.TweetLenght(test);

            Assert.AreEqual(twitterLength, 71);
        }

        [TestMethod]
        public void TestLengthWith2UrlsAndHttps()
        {
            string test = "Hello https://tweetinvi.codeplex.com/salutLescopains 3615 Gerard www.linviIsMe.com piloupe";

            int twitterLength = StringExtension.TweetLenght(test);

            Assert.AreEqual(twitterLength, 72);
        }


        [TestMethod]
        public void TestLengthWithURLFollowedByDotAndSingleChar()
        {
            string test = "Hello https://tweetinvi.codeplex.com.a 3615 Gerard www.linviIsMe.com piloupe";

            int twitterLength = StringExtension.TweetLenght(test);

            Assert.AreEqual(twitterLength, 74);
        }

        [TestMethod]
        public void TestLengthWithURLFollowedByDotAndTwoChars()
        {
            string test = "Hello https://tweetinvi.codeplex.com.au 3615 Gerard www.linviIsMe.com piloupe";

            int twitterLength = StringExtension.TweetLenght(test);

            Assert.AreEqual(twitterLength, 72);
        }

        [TestMethod]
        public void TestLengthWithURLFollowedByArgsAndDot()
        {
            string test = "Hello https://tweetinvi.codeplex.com/salutLescopains.a 3615 Gerard www.linviIsMe.com piloupe";

            int twitterLength = StringExtension.TweetLenght(test);

            Assert.AreEqual(twitterLength, 72);
        }

        [TestMethod]
        public void TestLengthWithSmallUrl()
        {
            string test = "www.co.co";

            int twitterLength = StringExtension.TweetLenght(test);

            Assert.AreEqual(twitterLength, 22);
        }

        private void TestURLWithMultiplePrefix(string url, int expectedLength)
        {
            var basicTweetURL = String.Format("Hello there http:// {0} bye!", url);
            Assert.AreEqual(basicTweetURL.TweetLenght(), expectedLength);

            var wwwTweetURL = String.Format("Hello there http:// www.{0} bye!", url);
            Assert.AreEqual(wwwTweetURL.TweetLenght(), expectedLength);

            var httpTweetURL = String.Format("Hello there http:// http://{0} bye!", url);
            Assert.AreEqual(httpTweetURL.TweetLenght(), expectedLength);

            var httpsTweetURL = String.Format("Hello there http:// https://{0} bye!", url);
            Assert.AreEqual(httpsTweetURL.TweetLenght(), expectedLength + 1);

            var httpwwwTweetURL = String.Format("Hello there http:// http://{0} bye!", url);
            Assert.AreEqual(httpwwwTweetURL.TweetLenght(), expectedLength);

            var httpswwwTweetURL = String.Format("Hello there http:// https://{0} bye!", url);
            Assert.AreEqual(httpswwwTweetURL.TweetLenght(), expectedLength + 1);
        }

        [TestMethod]
        public void MultipleURLOfVariousFormat()
        {
            // Simple URL
            TestURLWithMultiplePrefix("co.com", 47);
            TestURLWithMultiplePrefix("tweetinvi.codeplex.com", 47);
            TestURLWithMultiplePrefix("tweetinvi.codeplex.com.a", 49);
            TestURLWithMultiplePrefix("tweetinvi.codeplex.com.au", 47);

            // Url with '/'
            TestURLWithMultiplePrefix("tweetinvi.codeplex.com/", 47);
            TestURLWithMultiplePrefix("tweetinvi.codeplex.com/a", 47);
            TestURLWithMultiplePrefix("tweetinvi.codeplex.com/salut", 47);
            TestURLWithMultiplePrefix("tweetinvi.codeplex.com/salut.a", 47);
            TestURLWithMultiplePrefix("tweetinvi.codeplex.com/salut.linvi", 47);
            TestURLWithMultiplePrefix("tweetinvi.codeplex.com/salut/linvi.a", 47);
            TestURLWithMultiplePrefix("tweetinvi.codeplex.com/salut/linvi.plop", 47);

            // Url finishing with '.'
            TestURLWithMultiplePrefix("tweetinvi.codeplex.com/.", 48);
            TestURLWithMultiplePrefix("tweetinvi.codeplex.com.", 48);
            TestURLWithMultiplePrefix("tweetinvi.codeplex.com/a.", 48);
            TestURLWithMultiplePrefix("tweetinvi.codeplex.com/salut.", 48);
            TestURLWithMultiplePrefix("tweetinvi.codeplex.com/salut.a.", 48);
            TestURLWithMultiplePrefix("tweetinvi.codeplex.com/salut.linvi.", 48);
            TestURLWithMultiplePrefix("tweetinvi.codeplex.com/salut/linvi.a.", 48);
            TestURLWithMultiplePrefix("tweetinvi.codeplex.com/salut/linvi.plop.", 48);

            // Url finishing with a special character
            TestURLWithMultiplePrefix("tweetinvi.codeplex.com/a!", 48);
            TestURLWithMultiplePrefix("tweetinvi.codeplex.com/salut!", 48);
            TestURLWithMultiplePrefix("tweetinvi.codeplex.com/salut.a!", 48);
            TestURLWithMultiplePrefix("tweetinvi.codeplex.com/salut.linvi!", 48);
            TestURLWithMultiplePrefix("tweetinvi.codeplex.com/salut/linvi.a!", 48);
            TestURLWithMultiplePrefix("tweetinvi.codeplex.com/salut/linvi.plop!", 48);
        }

        [TestMethod]
        public void URLWithOnly2CharsAtTheEnd()
        {
            string url = "salut.co";

            var basicTweetURL = String.Format("Hello there http:// {0} bye!", url);
            Assert.AreEqual(basicTweetURL.TweetLenght(), 33);

            int expectedLength = 47;
            var wwwTweetURL = String.Format("Hello there http:// www.{0} bye!", url);
            Assert.AreEqual(wwwTweetURL.TweetLenght(), expectedLength);

            var httpTweetURL = String.Format("Hello there http:// http://{0} bye!", url);
            Assert.AreEqual(httpTweetURL.TweetLenght(), expectedLength);

            var httpsTweetURL = String.Format("Hello there http:// https://{0} bye!", url);
            Assert.AreEqual(httpsTweetURL.TweetLenght(), expectedLength + 1);

            var httpwwwTweetURL = String.Format("Hello there http:// http://{0} bye!", url);
            Assert.AreEqual(httpwwwTweetURL.TweetLenght(), expectedLength);

            var httpswwwTweetURL = String.Format("Hello there http:// https://{0} bye!", url);
            Assert.AreEqual(httpswwwTweetURL.TweetLenght(), expectedLength + 1);
        }
    }
}
