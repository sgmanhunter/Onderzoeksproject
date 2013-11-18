using System;
using System.Net;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Streaminvi.Helpers;
using Testinvi.Helpers;
using TweetinCore.Enum;
using TweetinCore.Interfaces.TwitterToken;

namespace Testinvi.Streaminvi
{
    [TestClass]
    public class StreamResultGeneratorTests
    {
        // This is the delay between performing an action
        // and detecting the action in the UserStream
        private const int STREAM_DELAY = 10000;

        // Change this const based on your connection speed
        // This is the time required for your connection to be 
        // ready to listen to the stream
        private const int STREAM_START_DELAY = 2000;

        private IToken _token;

        [TestInitialize]
        public void Initialize()
        {
            _token = TokenTestSingleton.Instance;
        }

        [TestMethod]
        public void EventsCalledProperly()
        {
            // Arrange
            StreamResultGenerator generator = new StreamResultGenerator();

            EventTestHelper<EventArgs> started = new EventTestHelper<EventArgs>();
            generator.StreamStarted += started.EventAction;

            EventTestHelper<EventArgs> resumed = new EventTestHelper<EventArgs>();
            generator.StreamResumed += resumed.EventAction;

            EventTestHelper<EventArgs> paused = new EventTestHelper<EventArgs>();
            generator.StreamPaused += paused.EventAction;

            EventTestHelper<EventArgs> stopped = new EventTestHelper<EventArgs>();
            generator.StreamStopped += stopped.EventAction;

            int nbRetrieved = 0;
            Func<string, bool> objectReceived = s =>
            {
                ++nbRetrieved;
                return true;
            };

            // Act
            ThreadTestHelper.StartLifetimedThread(
                () => generator.StartStream(objectReceived,
                    () => _token.GetQueryWebRequest("https://stream.twitter.com/1.1/statuses/sample.json", HttpMethod.GET)),
                generator.StopStream,
                STREAM_DELAY, STREAM_START_DELAY);

            // Assert
            started.VerifyNumberOfCalls(1);
            resumed.VerifyNumberOfCalls(0);
            paused.VerifyNumberOfCalls(0);
            stopped.VerifyNumberOfCalls(0);

            // Act

            // Leave the time to the object to receive Tweets
            Thread.Sleep(200);
            int beforePauseNbElements = nbRetrieved;
            generator.PauseStream();
            // Leave the time to the stream to get new objects
            Thread.Sleep(200);

            // Assert
            Assert.AreEqual(beforePauseNbElements, nbRetrieved);
            started.VerifyNumberOfCalls(1);
            resumed.VerifyNumberOfCalls(0);
            paused.VerifyNumberOfCalls(1);
            stopped.VerifyNumberOfCalls(0);

            // Act
            Thread.Sleep(200);
            generator.ResumeStream();
            // The stream can be resumed with a maximum delay of 1 second by default (STREAM_RESUME_DELAY)
            Thread.Sleep(1100);

            // Assert
            Assert.IsTrue(beforePauseNbElements < nbRetrieved);
            started.VerifyNumberOfCalls(1);
            resumed.VerifyNumberOfCalls(1);
            paused.VerifyNumberOfCalls(1);
            stopped.VerifyNumberOfCalls(0);

            // Act
            Thread.Sleep(200);
            beforePauseNbElements = nbRetrieved;
            generator.StopStream();
            Thread.Sleep(200);

            // Assert
            Assert.AreEqual(beforePauseNbElements, nbRetrieved);
            started.VerifyNumberOfCalls(1);
            resumed.VerifyNumberOfCalls(1);
            paused.VerifyNumberOfCalls(1);
            stopped.VerifyNumberOfCalls(1);
        }

        [TestMethod]
        public void ErrorHappened()
        {
            // Arrange
            StreamResultGenerator generator = new StreamResultGenerator();
            const int delayBeforeRestart = 1000;

            int nbRetrieved = 0;
            Func<string, bool> objectReceived = s =>
            {
                ++nbRetrieved;
                return true;
            };

            HttpWebRequest generatedWebRequest = _token.GetQueryWebRequest("https://stream.twitter.com/1.1/statuses/sample.json", HttpMethod.GET);

            EventTestHelper<EventArgs> started = new EventTestHelper<EventArgs>();
            generator.StreamStarted += started.EventAction;

            Thread t = null;
            EventTestHelper<EventArgs> stopped = new EventTestHelper<EventArgs>();
            generator.StreamStopped += stopped.EventAction;
            generator.StreamStopped += (sender, args) =>
            {
                if (args != null)
                {
                    t = ThreadTestHelper.StartLifetimedThread(
                            () => generator.StartStream(objectReceived,
                                () => _token.GetQueryWebRequest("https://stream.twitter.com/1.1/statuses/sample.json", HttpMethod.GET)),
                            generator.StopStream,
                        STREAM_DELAY, STREAM_START_DELAY, delayBeforeRestart);
                }
            };

            // Act
            ThreadTestHelper.StartDelayedAction(() => generatedWebRequest.GetResponse().Close(), 5000);
            generator.StartStream(objectReceived, () => generatedWebRequest);

            // Assert
            started.VerifyNumberOfCalls(1);
            stopped.VerifyNumberOfCalls(1);

            // Act
            int previousNbReceived = nbRetrieved;
            if (t != null)
            {
                // Waiting that the backup thread (t) has started 
                Thread.Sleep(delayBeforeRestart + STREAM_START_DELAY + 100);
            }

            // Assert
            started.VerifyNumberOfCalls(2);
            stopped.VerifyNumberOfCalls(1);

            // Act
            if (t != null)
            {
                t.Join();
            }

            // Assert
            Assert.IsTrue(nbRetrieved > previousNbReceived);
            started.VerifyNumberOfCalls(2);
            stopped.VerifyNumberOfCalls(2);
        }
    }
}
