using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Threading;
using oAuthConnection;
using Streaminvi;
using TweetinCore.Enum;
using TweetinCore.Events;
using TweetinCore.Interfaces;
using TweetinCore.Interfaces.oAuth;
using TweetinCore.Interfaces.StreamInvi;
using TweetinCore.Interfaces.TwitterToken;
using Tweetinvi;
using Tweetinvi.Model;
using TwitterToken;
using UILibrary;
using System.Windows;
using Timer = System.Timers.Timer;

namespace Examplinvi
{
    class Program
    {
        // ReSharper disable LocalizableElement
        // ReSharper disable UnusedMember.Local

        #region Stream

        private static readonly List<ITweet> _streamList = new List<ITweet>();
        private static void ProcessTweet(ITweet tweet)
        {
            if (tweet == null)
            {
                return;
            }

            if (_streamList.Count % 125 != 124)
            {
                Console.WriteLine("{0} : \"{1}\"", tweet.Creator.Name, tweet.Text);
                _streamList.Add(tweet);
            }
            else
            {
                Console.WriteLine("Processing data");
                _streamList.Clear();
            }
        }

        // Base Stream
        public static void InfiniteStreamUsingStreamStopped_Recommended(IToken token)
        {
            // Creating the stream and specifying the delegate
            SimpleStream stream = new SimpleStream("https://stream.twitter.com/1.1/statuses/sample.json");

            stream.StreamStopped += (sender, args) =>
            {
                if (args.Value != null)
                {
                    Console.WriteLine("An exception occured... Well just restart it!");
                    Thread.Sleep(1000);
                    stream.StartStream(token, x => ProcessTweet(x));
                }
            };

            // Starting the stream by specifying credentials thanks to the Token
            stream.StartStream(token, x => ProcessTweet(x));
        }

        public static void InfiniteStreamUsingTwitterContext(IToken token)
        {
            // Creating the stream and specifying the delegate
            SimpleStream stream = new SimpleStream("https://stream.twitter.com/1.1/statuses/sample.json");
            int i = 0;
            stream.StreamStarted += (sender, args) => i = 0;

            TwitterContext context = new TwitterContext();
            while (true)
            {
                if (!context.TryInvokeAction(() => stream.StartStream(token, x => ProcessTweet(x))))
                {
                    Console.WriteLine("An exception occured... Well just restart it!");
                    Thread.Sleep(1000);
                    ++i;
                }

                // If the stream fails 50 times in a row the stream is operation is cancelled
                if (i >= 50)
                {
                    break;
                }
            }
        }

        private static void GetStreamStateInformation(IToken token)
        {
            SimpleStream stream = new SimpleStream("https://stream.twitter.com/1.1/statuses/sample.json");

            stream.StreamStarted += (sender, args) =>
            {
                Console.WriteLine("Stream started at {0}", DateTime.Now);
            };

            stream.StreamResumed += (sender, args) =>
            {
                Console.WriteLine("Stream resumed at {0}", DateTime.Now);
            };

            stream.StreamPaused += (sender, args) =>
            {
                Console.WriteLine("Stream paused at {0}", DateTime.Now);
            };

            stream.StreamStopped += (sender, args) =>
            {
                Console.WriteLine("Stream stopped at {0}", DateTime.Now);
            };

            int i = 0;
            stream.StartStream(token, tweet =>
            {
                Console.WriteLine(tweet.Text);
                if (i == 100)
                {
                    stream.PauseStream();
                    Thread t = new Thread(() =>
                    {
                        Thread.Sleep(10000);
                        stream.ResumeStream();
                    });

                    t.Start();
                }

                if (i == 200)
                {
                    stream.StopStream();
                }

                ++i;
            });
        }

        private static void StartTimeredStream(IToken token, int timerDuration)
        {
            IStream<ITweet> stream = new SimpleStream("https://stream.twitter.com/1.1/statuses/sample.json");

            EventHandler pauseStream = (sender, args) =>
            {
                Timer timer = new Timer(timerDuration);
                timer.Elapsed += (o, eventArgs) =>
                {
                    timer.Stop();
                    timer.Dispose();
                    stream.PauseStream();
                    Console.WriteLine("Stream paused for {0}ms...", timerDuration);
                };

                timer.Start();
            };

            EventHandler resumeStream = (sender, args) =>
            {
                Timer timer = new Timer(timerDuration);
                timer.Elapsed += (o, eventArgs) =>
                {
                    timer.Stop();
                    timer.Dispose();
                    stream.ResumeStream();
                    Console.WriteLine("Stream resumed for {0}ms...", timerDuration);
                };

                timer.Start();
            };

            stream.StreamStarted += pauseStream;
            stream.StreamResumed += pauseStream;
            stream.StreamPaused += resumeStream;

            stream.StartStream(token, tweet =>
            {
                // Write the tweet in the file!
                Console.WriteLine(tweet.Text);
            });
        }

        #region Simple Stream


        // Simple Stream ("sample.json")
        private static void SimpleStreamExample(IToken token)
        {
            // Creating the stream and specifying the delegate
            SimpleStream stream = new SimpleStream("https://stream.twitter.com/1.1/statuses/sample.json");
            stream.StreamStarted += (sender, args) => Console.WriteLine("Stream has started!");
            // Starting the stream by specifying credentials thanks to the Token
            stream.StartStream(token, x => ProcessTweet(x));
        }

        private static void SimpleStreamWithExceptionManagerExample(IToken token)
        {
            // Creating the stream and specifying the delegate
            SimpleStream stream = new SimpleStream("https://stream.twitter.com/1.1/statuses/sample.json");
            stream.StreamStarted += (sender, args) => Console.WriteLine("Stream has started!");
            // Starting the stream by specifying credentials thanks to the Token
            TwitterContext context = new TwitterContext();
            var success = context.TryInvokeAction(() => stream.StartStream(token, x => ProcessTweet(x)));

            if (!success)
            {
                Console.WriteLine("The error '{0}' occured!", context.LastActionException.Message);
            }
        }

        private static void SimpleStreamWithFilterExample(IToken token)
        {
            // Creating the stream and specifying the delegate
            SimpleStream stream = new SimpleStream("https://stream.twitter.com/1.1/statuses/sample.json");
            stream.StreamStarted += (sender, args) => Console.WriteLine("Stream has started!");
            // Starting the stream by specifying credentials thanks to the Token
            stream.AddTrack("syria");
            stream.AddTrack("usa");
            stream.AddTrack("france");
            stream.StartStream(token, x => ProcessTweet(x));
        }

        private static void SimpleStreamWithMatchingKeywordsExample(IToken token)
        {
            // Creating the stream and specifying the delegate
            SimpleStream stream = new SimpleStream("https://stream.twitter.com/1.1/statuses/sample.json");
            stream.StreamStarted += (sender, args) => Console.WriteLine("Stream has started!");
            // Starting the stream by specifying credentials thanks to the Token
            stream.AddTrack("syria");
            stream.AddTrack("usa", tweet =>
            {
                Console.WriteLine("They talk about USA in : {0}", tweet.Text);
            });
            stream.AddTrack("france", tweet =>
            {
                Console.WriteLine("They talk about france in : {0}", tweet.Text);
            });
            stream.StartStream(token, (tweet, tracks) => ProcessFilteredTweet(tweet, tracks));
        }
        #endregion

        #region Filtered Stream
        private static int _processedFilteredTweetCount;
        private static bool ProcessFilteredTweet(ITweet tweet, List<string> list)
        {
            // IMPORTANT : You receive both the Tweet coming from the filtered streamd (tweet)
            // AND the collection of keywords you tracked that allowed you to get this Tweet

            Console.WriteLine(tweet.Text);
            Console.WriteLine("Matched {0} tracks", list.Count);
            for (int i = 0; i < list.Count; ++i)
            {
                Console.WriteLine("\t- {0}", list[i]);
            }

            ++_processedFilteredTweetCount;

            // Stop the stream after 500 tweets
            return _processedFilteredTweetCount < 500;
        }

        // Track Keywords
        private static void StreamFilterBasicTrackExample(IToken token)
        {
            IFilteredStream stream = new FilteredStream();

            stream.StreamStarted += (sender, args) => Console.WriteLine("Stream has started!");
            stream.AddTrack("linvi");
            stream.AddTrack("tweetinvi");
            stream.AddTrack("usa");

            stream.LimitReached += (sender, args) =>
            {
                Console.WriteLine("You have missed {0} tweets because you were retrieving more than 1% of tweets", args.Value);
            };

            TwitterContext context = new TwitterContext();
            if (!context.TryInvokeAction(() => stream.StartStream(token, tweet => Console.WriteLine(tweet))))
            {
                Console.WriteLine("An Exception occured : '{0}'", context.LastActionTwitterException.TwitterWebExceptionErrorDescription);
            }
        }

        private static void StreamFilterWithManualTrackingExample(IToken token)
        {
            IFilteredStream stream = new FilteredStream();

            stream.StreamStarted += (sender, args) => Console.WriteLine("Stream has started!");

            // IMPORTANT : Perform an action when a specific Track is received :
            stream.AddTrack("Tweetinvi Linvi", tweet =>
            {
                Console.WriteLine("You received both Tweetinvi and Linvi in your tweet!");
            });

            stream.AddTrack("Linvi", tweet =>
            {
                // This is a specific action to perform when you retrieve Linvi!
                Console.WriteLine("linvi has been found in {0}!", tweet.Text);
            });

            // Classic add to the filter
            stream.AddTrack("obama");
            stream.AddTrack("syria");
            stream.AddTrack("france");
            stream.AddTrack("england");

            stream.LimitReached += (sender, args) =>
            {
                Console.WriteLine("You have missed {0} tweets because you were retrieving more than 1% of tweets", args.Value);
            };

            // The twitter context manages exceptions
            TwitterContext context = new TwitterContext();
            if (!context.TryInvokeAction(() => stream.StartStream(token, (tweet, matchingTracks) => ProcessFilteredTweet(tweet, matchingTracks))))
            {
                Console.WriteLine("A problem occured, check context.LastException!");
            }
        }

        // Track User (The stream follows only specific users)
        private static void StreamFilterBasicFollowExample(IToken token)
        {
            ITokenUser currentUser = new TokenUser(token);
            IFilteredStream stream = new FilteredStream();

            stream.StreamStarted += (sender, args) => Console.WriteLine("Stream has started!");
            stream.AddFollow(currentUser);
            stream.StartStream(token, tweet =>
            {
                string who;
                if (tweet.Creator.Id == currentUser.Id)
                {
                    who = "You've";
                }
                else
                {
                    who = "Someone you know has";
                }

                Console.WriteLine("{0} just published : {1}", who, tweet.Text);

                // Close the stream when you publish a tweet saying : 
                // 'Close my stream from twitter!'
                return tweet.Text != "Close my stream from twitter!";
            });
        }

        private static void StreamFilterBasicFollowWithEventRaisedForSpecificUser(IToken token)
        {
            ITokenUser currentUser = new TokenUser(token);
            IFilteredStream stream = new FilteredStream();

            stream.StreamStarted += (sender, args) => Console.WriteLine("Stream has started!");

            // User has a special action when he is detected
            stream.AddFollow(currentUser, tweet =>
            {
                Console.WriteLine("{0} published : '{1}'", currentUser.ScreenName, tweet.Text);
            });

            stream.StartStream(token, tweet =>
            {
                string who;
                if (tweet.Creator.Id == currentUser.Id)
                {
                    who = "You've";
                }
                else
                {
                    who = "Someone you know has";
                }

                Console.WriteLine("{0} just published : {1}", who, tweet.Text);

                // Close the stream when you publish a tweet saying : 
                // 'Close my stream from twitter!'
                return tweet.Text != "Close my stream from twitter!";
            });
        }

        // Track Location
        private static void StreamFilterLocationExample(IToken token)
        {
            //var topLeft = new Coordinates(-122.75, 36.8);
            //var bottomRight = new Coordinates(-121.75, 37.8);

            //var topLeft = new Coordinates(-89.673157, 21.056948);
            //var bottomRight = new Coordinates(-89.695645, 20.899185);

            var topLeft = new Coordinates(-180, -90);
            var bottomRight = new Coordinates(180, 90);

            var stream = new FilteredStream();
            //stream.AddTrack("tweetinvi");
            stream.AddLocation(topLeft, bottomRight);
            //stream.AddFollow(new TokenUser(token));

            Action<ITweet> tweetReceived = t =>
            {
                Console.WriteLine(t.Text);
            };

            stream.StartStream(token, tweetReceived);
        }

        private static void StreamFilterTrackSpecificLocation(IToken token)
        {

            var topLeft = new Coordinates(-122.75, 36.8);
            var bottomRight = new Coordinates(-121.75, 37.8);

            var stream = new FilteredStream();
            stream.AddTrack("tweetinvi");
            stream.AddLocation(new Location(new Coordinates(1.0, 1.0), new Coordinates(2.0, 2.0)));
            stream.AddLocation(topLeft, bottomRight, tweet =>
            {
                Console.WriteLine("This location has been detected in the tweet : '{0}'", tweet.Text);
            });
            stream.AddFollow(new TokenUser(token));

            Action<ITweet> tweetReceived = t =>
            {
                Console.WriteLine(t.Text);
            };

            stream.StartStream(token, tweetReceived);
        }

        // Track Keywords OR User
        private static void StreamFilterTrackORFollowExample(IToken token)
        {
            ITokenUser currentUser = new TokenUser(token);
            IFilteredStream stream = new FilteredStream();

            stream.StreamStarted += (sender, args) => Console.WriteLine("Stream has started!");

            stream.AddTrack("tweetinvi");
            stream.AddFollow(currentUser);
            stream.StartStream(token, (tweet, matchedTracks) =>
            {
                string who = tweet.Creator.Id == currentUser.Id ? "You've" : "Someone you know has";
                Console.WriteLine("{0} just published something about '{1}'. Look this is what has been published : {2}",
                    who, matchedTracks[0], tweet.Text);

                // Close the stream when you publish a tweet saying : 
                // 'Close my stream from twitter!'
                return tweet.Text != "Close my stream from twitter!";
            });
        }

        // Track Keywords OR User OR Location
        private static void StreamFilterTrackORFollowORLocationExample(IToken token)
        {
            var topLeft = new Coordinates(-122.75, 36.8);
            var bottomRight = new Coordinates(-121.75, 37.8);

            var stream = new FilteredStream();
            stream.AddTrack("tweetinvi");
            stream.AddFollow(new TokenUser(token));
            stream.AddLocation(topLeft, bottomRight);


            Action<ITweet> tweetReceived = t =>
            {
                Console.WriteLine(t.Text);
            };

            stream.StartStream(token, tweetReceived);
        }

        // Powered Filtered Tracks
        private static void TrackMessagesSentByUserFromSpecificLocation(IToken token)
        {
            var location = new Location(-124.75, 36.8, -126.89, 32.75);
            var user = new User("TweetinviApi");

            var stream = new FilteredStream();
            stream.StreamStarted += (sender, args) => { Console.WriteLine("Stream started!"); };
            stream.AddLocation(location);
            stream.AddFollow(user);

            Func<ITweet, List<string>, List<ILocation>, bool> tweetReceived =
                (tweet, matchingKeywords, matchingLocation) =>
                {
                    Console.WriteLine("A tweet matching all the requirements has been detected!");
                    Console.WriteLine("{0} keywords have been found", matchingKeywords.Count);
                    Console.WriteLine("{0} locations have been found", matchingLocation.Count);

                    // Close the stream when a tweet matches the requirements
                    return false;
                };

            stream.StartStreamMatchingAllConditions(token, tweetReceived);
        }

        private static void TrackKeywordsWrittenFromASpecificLocation(IToken token)
        {
            var location = new Location(-124.75, 36.8, -126.89, 32.75);

            var stream = new FilteredStream();
            stream.StreamStarted += (sender, args) => { Console.WriteLine("Stream started!"); };
            stream.AddLocation(location);
            stream.AddTrack("tweetinvi");
            stream.AddTrack("linvi");

            Func<ITweet, List<string>, List<ILocation>, bool> tweetReceived =
                (tweet, matchingKeywords, matchingLocation) =>
                {
                    Console.WriteLine("A tweet matching all the requirements has been detected!");
                    Console.WriteLine("{0} keywords have been found", matchingKeywords.Count);
                    Console.WriteLine("{0} locations have been found", matchingLocation.Count);

                    // Close the stream when a tweet matches the requirements
                    return false;
                };

            stream.StartStreamMatchingAllConditions(token, tweetReceived);
        }

        private static void TrackKeywordsWrittenFromASpecificLocationAndPerformSpecificActionForKeyword(IToken token)
        {
            var location = new Location(-124.75, 36.8, -126.89, 32.75);

            var stream = new FilteredStream();
            stream.StreamStarted += (sender, args) => { Console.WriteLine("Stream started!"); };
            stream.AddLocation(location);

            // Specify the action you want to be performed
            stream.AddTrack("tweetinvi", tweet =>
            {
                Console.WriteLine("Someone tweeted about Tweetinvi at this nice location!");
            });

            stream.AddTrack("linvi");

            Func<ITweet, List<string>, List<ILocation>, bool> tweetReceived =
                (tweet, matchingKeywords, matchingLocation) =>
                {
                    Console.WriteLine("A tweet matching all the requirements has been detected!");
                    Console.WriteLine("{0} keywords have been found", matchingKeywords.Count);
                    Console.WriteLine("{0} locations have been found", matchingLocation.Count);

                    // Close the stream when a tweet matches the requirements
                    return false;
                };

            stream.StartStreamMatchingAllConditions(token, tweetReceived);
        }

        private static void TracksMessagesSentByUserFromSpecificLocationAndContainingSpecificKeywords(IToken token)
        {
            var location = new Location(-124.75, 36.8, -126.89, 32.75);

            var stream = new FilteredStream();
            stream.StreamStarted += (sender, args) => { Console.WriteLine("Stream started!"); };
            stream.AddLocation(location);
            stream.AddTrack("tweetinvi");
            stream.AddTrack("linvi");
            stream.AddFollow(new TokenUser(token));

            Func<ITweet, List<string>, List<ILocation>, bool> tweetReceived =
                (tweet, matchingKeywords, matchingLocation) =>
                {
                    Console.WriteLine("A tweet matching all the requirements has been detected!");
                    Console.WriteLine("{0} keywords have been found", matchingKeywords.Count);
                    Console.WriteLine("{0} locations have been found", matchingLocation.Count);

                    // Close the stream when a tweet matches the requirements
                    return false;
                };

            stream.StartStreamMatchingAllConditions(token, tweetReceived);

            // Send a tweet that matches : 
            // ITweet t = new Tweet("tweetinvi is developped by linvi!", token);
            // t.PublishWithGeo(location.Coordinate1.Longitude, location.Coordinate1.Lattitude);
        }

        #endregion

        #region User Stream
        // User Stream
        private static void UserStreamWithAllEventsRegisteredExample(IToken token)
        {
            IUserStream us = new UserStream();

            // Tweet
            us.TweetCreatedByAnyone += (sender, args) =>
            {
                Console.WriteLine("Tweet '{0}' created!", args.Value.Text);
            };

            us.TweetCreatedByMe += (sender, args) =>
            {
                Console.WriteLine("Tweet '{0}' created by me!", args.Value.Text);
            };

            us.TweetCreatedByAnyoneButMe += (sender, args) =>
            {
                Console.WriteLine("Tweet '{0}' created by {1}!", args.Value.Text, args.Value.Creator.Id);
            };

            // Tracked
            us.TrackedTweetCreatedByAnyone += (sender, args) =>
            {
                Console.WriteLine("Tracked Tweet '{0}' created!", args.Value.Text);
            };

            us.TrackedTweetCreatedByMe += (sender, args) =>
            {
                Console.WriteLine("Tracked Tweet '{0}' created by me!", args.Value.Text);
            };

            us.TrackedTweetCreatedByAnyoneButMe += (sender, args) =>
            {
                Console.WriteLine("Tracked Tweet '{0}' created by {1}!", args.Value.Text, args.Value.Creator.Id);
            };

            // Message
            us.MessageSentOrReceived += (sender, args) =>
            {
                Console.WriteLine("Message '{0}' sent or received!", args.Value.Text);
            };

            us.MessageSentByMeToX += (sender, args) =>
            {
                Console.WriteLine("Message '{0}' sent by me to {1}!", args.Value.Text, args.Value3.Id);
            };

            us.MessageReceivedFromX += (sender, args) =>
            {
                Console.WriteLine("Message '{0}' received from {1}!", args.Value.Text, args.Value3.Id);
            };

            // Follow
            us.FollowUser += (sender, args) =>
            {
                Console.WriteLine("You are following : {0}", args.Value2.ScreenName);
            };

            us.UnFollowUser += (sender, args) =>
            {
                Console.WriteLine("You are not following {0} anymore", args.Value2.ScreenName);
            };

            us.FollowedByUser += (sender, args) =>
            {
                Console.WriteLine("{0} is following you", args.Value2.ScreenName);
            };

            us.WarningReceived += (sender, args) =>
            {
                Console.WriteLine("Warning : {0}", args.Value2);
                Console.WriteLine("Remaining : {0}", args.Value3);
            };

            us.StartStream(token);
        }

        private static void UserStreamWithTrackedTweetsExample(IToken token)
        {
            IUserStream us = new UserStream();

            us.TrackedTweetCreatedByAnyone += (sender, args) =>
            {
                Console.WriteLine("This tweet has matched the tracks!");
            };

            // Register the keywords
            us.AddTrack("linvi", tweetMessage =>
            {
                Console.WriteLine("Wow someone talk about linvi in '{0}'", tweetMessage);
            });

            us.AddTrack("tweetinvi");

            us.StartStream(token);
        }
        #endregion

        #endregion

        #region User

        #region CreateUser
        public static void CreateUser(IToken token, long id = 700562792)
        {
            IUser user = new User(id, token);
            Console.WriteLine(user.ScreenName);
        }

        public static void CreateUser(IToken token, string screenName = null)
        {
            IUser user = new User(screenName, token);
            Console.WriteLine(user.Id);
        }

        public static void CreateUserV2(IToken token, long id = 700562792)
        {
            IUser user = new User(id);
            // Here we need to specify the token to retrieve the information
            // otherwise the information won't be filled
            user.PopulateUser(token);
        }
        #endregion

        #region Get Friends

        public static void GetFriends(IToken token)
        {
            ITokenUser u = new TokenUser(token);
            u.PopulateFriendsFromFriendIds(true);

            foreach (var friend in u.Friends)
            {
                Console.WriteLine(friend.Name);
            }
        }

        private static void GetFriendIds(IToken token, long id = 700562792)
        {
            IUser user = new User(id, token);

            Console.WriteLine("List of friends from " + id);

            foreach (long friendId in user.FriendIds)
            {
                Console.WriteLine(friendId);
            }
        }

        private static void GetFriendIdsUsingUsername(IToken token, string username)
        {
            IUser user = new User(username, token);
            Console.WriteLine("List of friends from " + username);

            foreach (long friendId in user.FriendIds)
            {
                Console.WriteLine(friendId);
            }
        }
        #endregion

        #region Get Followers

        private static void GetFollowerIds(IToken token, long? id = 700562792)
        {
            IUser user = new User(id, token);
            Console.WriteLine("List of followers from " + id);

            foreach (long friendId in user.FollowerIds)
            {
                Console.WriteLine(friendId);
            }
        }

        private static void GetFollowerIdsUsingUsername(IToken token, string username)
        {
            IUser user = new User(username, token);
            Console.WriteLine("List of followers from " + username);

            foreach (long friendId in user.FollowerIds)
            {
                Console.WriteLine(friendId);
            }
        }

        public static void GetFollowers(IToken token)
        {
            ITokenUser u = new TokenUser(token);

            foreach (var follower in u.Followers)
            {
                Console.WriteLine(follower.Name);
            }
        }

        #endregion

        #region Get Profile Image

        static void GetProfileImage(IToken token)
        {
            User ladygaga = new User("ladygaga", token);
            string filePath = ladygaga.DownloadProfileImage(ImageSize.original);

            System.Diagnostics.Process.Start(filePath);
        }

        #endregion

        #region Get Contributors
        static void GetContributors(IToken token, long? id = 700562792, string screen_name = null, bool createContributorList = false)
        {
            IUser user;

            if (id != null)
            {
                user = new User(id, token);
            }
            else
            {
                user = new User(screen_name, token);
            }

            IList<IUser> contributors = user.GetContributors(createContributorList);
            IList<IUser> contributorsAttribute = user.Contributors;
            if (createContributorList && contributors != null)
            {
                if (contributorsAttribute == null ||
                    !contributors.Equals(contributorsAttribute))
                {
                    Console.WriteLine("The object attribute should be identical to the method result");
                }
            }
            if (contributors != null)
            {
                foreach (User c in contributors)
                {
                    Console.WriteLine("contributor id = " + c.Id + " - screen_name = " + c.ScreenName);
                }
            }
        }
        #endregion

        #region Get Contributees

        static void GetContributees(IToken token, long? id = 700562792, string screen_name = null, bool createContributeeList = false)
        {
            IUser user;
            if (id != null)
            {
                user = new User(id, token);
            }
            else
            {
                user = new User(screen_name, token);
            }
            IList<IUser> contributees = user.GetContributees(createContributeeList);
            IList<IUser> contributeesAttribute = user.Contributees;
            if (createContributeeList)
            {
                if ((contributees == null && contributeesAttribute != null) ||
                    (contributees != null && contributeesAttribute == null) ||
                    (contributees != null && !contributees.Equals(contributeesAttribute)))
                {
                    Console.WriteLine("The object attribute should be identical to the method result");
                }
            }
            if (contributees != null)
            {
                foreach (User c in contributees)
                {
                    Console.WriteLine("contributee id = " + c.Id + " - screen_name = " + c.ScreenName);
                }
            }
        }
        #endregion

        #region Get Direct Messages Sent
        static void GetDirectMessagesSent(IToken token)
        {
            ITokenUser user = new TokenUser(token);
            IList<IMessage> dmSent = user.GetLatestDirectMessagesSent();
            IList<IMessage> dmSentAttribute = user.LatestDirectMessagesSent;


            if ((dmSent == null && dmSentAttribute != null) ||
                (dmSent != null && dmSentAttribute == null) ||
                (dmSent != null && !dmSent.Equals(dmSentAttribute)))
            {
                Console.WriteLine("The object's attribute should be identical to the method result");
            }

            if (dmSent != null)
            {
                foreach (Message m in dmSent)
                {
                    Console.WriteLine("message id = " + m.Id + " - text = " + m.Text);
                }
            }
        }
        #endregion

        #region Get Direct Received
        static void GetDirectMessagesReceived(IToken token)
        {
            ITokenUser user = new TokenUser(token);
            IList<IMessage> dmReceived = user.GetLatestDirectMessagesReceived();
            IList<IMessage> dmReceivedAtrribute = user.LatestDirectMessagesReceived;

            if ((dmReceived == null && dmReceivedAtrribute != null) ||
                (dmReceived != null && dmReceivedAtrribute == null) ||
                (dmReceived != null && !dmReceived.Equals(dmReceivedAtrribute)))
            {
                Console.WriteLine("The object's attribute should be identical to the method result");
            }

            if (dmReceived != null)
            {
                foreach (Message m in dmReceived)
                {
                    Console.WriteLine("message id = " + m.Id + " - text = " + m.Text);
                }
            }
        }
        #endregion

        #region GetHomeTimeline

        static void GetHomeTimeline(IToken token)
        {
            ITokenUser u = new TokenUser(token);
            IList<ITweet> homeTimeline = u.GetHomeTimeline(20, true, true);

            Console.WriteLine(u.LatestHomeTimeline.Count);

            foreach (ITweet tweet in homeTimeline)
            {
                Console.WriteLine("\n\n{0}", tweet.Text);
            }
        }

        #endregion

        #region Get Timeline
        static void GetTimeline(IToken token, long id = 700562792, bool createTimeline = false)
        {
            IUser user = new User(id, token);
            IList<ITweet> timeline = user.GetUserTimeline(createTimeline);
            IList<ITweet> timelineAttribute = user.Timeline;
            if (createTimeline)
            {
                if ((timeline == null && timelineAttribute != null) ||
                    (timeline != null && timelineAttribute == null) ||
                    (timeline != null && !timeline.Equals(timelineAttribute)))
                {
                    Console.WriteLine("The object's attribute should be identical to the method result");
                }
            }
            if (timeline != null)
            {
                foreach (Tweet t in timeline)
                {
                    Console.WriteLine("tweet id = " + t.Id + " - text = " + t.Text + " - is retweet = " + t.Retweeted);
                }
            }
        }
        #endregion

        #region Get Mentions
        static void GetMentions(IToken token)
        {
            ITokenUser user = new TokenUser(token);
            IList<IMention> mentions = user.GetLatestMentionsTimeline();
            IList<IMention> mentionsAttribute = user.LatestMentionsTimeline;


            if ((mentions == null && mentionsAttribute != null) ||
                (mentions != null && mentionsAttribute == null) ||
                (mentions != null && !mentions.Equals(mentionsAttribute)))
            {
                Console.WriteLine("The object's attribute should be identical to the method result");
            }

            if (mentions != null)
            {
                foreach (Mention m in mentions)
                {
                    Console.WriteLine("tweet id = " + m.Id + " - text = " + m.Text + " - annotations = " + m.Annotations);
                }
            }
        }

        #endregion

        #region Get Blocked users
        static void GetBlockedUsers(IToken token, bool createBlockedUsers = true, bool createdBlockedUsersIds = true)
        {
            TokenUser user = new TokenUser(token);

            IList<IUser> blockedUsers = user.GetBlockedUsers(createBlockedUsers, createdBlockedUsersIds);
            if (blockedUsers == null)
            {
                return;
            }
            if (createBlockedUsers)
            {
                if (blockedUsers != user.BlockedUsers)
                {
                    Console.WriteLine("The object's attribute should be identical to the method result");
                }
            }

            foreach (IUser bu in blockedUsers)
            {
                Console.WriteLine("user id = " + bu.Id + " - user screen name = " + bu.ScreenName);
            }
        }

        static void GetBlockedUsersIds(IToken token, bool createdBlockedUsersIds = true)
        {
            TokenUser user = new TokenUser(token);

            IList<long> ids = user.GetBlockedUsersIds(createdBlockedUsersIds);
            if ((createdBlockedUsersIds) && (ids != user.BlockedUsersIds))
            {
                Console.WriteLine("The object's attribute should be identical to the method result");
            }

            foreach (long id in ids)
            {
                Console.WriteLine("user id = " + id);
            }
        }
        #endregion

        #region Get Suggested User (list and members)
        static void GetSuggestedUserList(IToken token, bool createSuggestedUserList = true)
        {
            ITokenUser user = new TokenUser(token);

            IList<ISuggestedUserList> suggUserList = user.GetSuggestedUserList(createSuggestedUserList);
            if ((createSuggestedUserList) && (!suggUserList.Equals(user.SuggestedUserList)))
            {
                Console.WriteLine("The object's attribute should be identical to the method result");
            }

            foreach (ISuggestedUserList sul in suggUserList)
            {
                Console.WriteLine("name = " + sul.Name + " ; slug = " + sul.Slug + " ; size = " + sul.Size);
            }

        }

        static void GetSuggestedUserListDetails(IToken token, string slug)
        {
            SuggestedUserList sul = new SuggestedUserList("fake", slug, 0);
            sul.RefreshAll(token);

            Console.WriteLine("name = " + sul.Name + " ; slug = " + sul.Slug + " ; size = " + sul.Size);
            foreach (User su in sul.Members)
            {
                Console.WriteLine("Suggested user: id = " + su.Id + " ; screen name = " + su.ScreenName);
            }
        }

        static void GetSuggestedUserListMembers(IToken token, string slug)
        {
            SuggestedUserList sul = new SuggestedUserList("fake", slug, 0);
            sul.RefreshMembers(token);

            foreach (User su in sul.Members)
            {
                Console.WriteLine("Suggested user: id = " + su.Id + " ; screen name = " + su.ScreenName);
            }
        }
        #endregion

        #endregion

        #region Tweet

        #region Publish Tweet

        public static void PublishTweet(IToken token)
        {
            ITweet t = new Tweet("Hello Tweetinvi2!");
            // token.Integrated_Exception_Handler = true;
            Console.WriteLine("Tweet has{0}been published", t.Publish(token) ? " " : " not ");
        }

        public static void PublishTweetWithGeo(IToken token)
        {
            // Create Tweet locally
            ITweet tweet = new Tweet(String.Format("Hello Tweetinvi With Geo {0}", DateTime.Now));

            double latitude = 37.7821120598956;
            double longitude = -122.400612831116;

            // Send the Tweet
            Console.WriteLine("Tweet has{0}been published",
                tweet.PublishWithGeo(latitude, longitude, true, token) ? " " : " not ");
        }

        public static void PublishInReplyTo(IToken token)
        {
            // Create Tweet locally
            ITweet tweet = new Tweet(String.Format("Hello Tweetinvi {0}", DateTime.Now), token);

            // Send the Tweet
            bool result = tweet.Publish();

            if (result)
            {
                ITweet reply = new Tweet(String.Format("Nice speech Tweetinvi {0}", DateTime.Now), token);

                result &= reply.PublishInReplyTo(tweet);
            }

            Console.WriteLine(result);
        }

        #endregion

        #region Retrieve an existing Tweet
        private static void GetTweetById(IToken token)
        {
            // This tweet has classic entities
            Tweet tweet1 = new Tweet(127512260116623360, token);
            Console.WriteLine(tweet1.Text);

            // This tweet has media entity
            try
            {
                Tweet tweet2 = new Tweet(112652479837110270, token);
                Console.WriteLine(tweet2.Text);
            }
            catch (WebException)
            {
                Console.WriteLine("Tweet has not been created!");
            }
        }
        #endregion

        #region Publish Retweet

        private static void PublishAndDestroyRetweet(IToken token)
        {
            IUser tweetinviApi = new User("tweetinviApi", token);
            List<ITweet> tweets = tweetinviApi.GetUserTimeline();

            // Retweeting the last tweet of TweetinviApi
            ITweet t = tweets[0];

            // Retweet is the tweet posted on the TokenUser timeline
            ITweet retweet = t.PublishRetweet();

            // Destroying the retweet
            retweet.Destroy();
        }

        #endregion

        #region Get Retweets of Tweet

        private static void Get_retweet_of_tweet(IToken token, long id)
        {
            ITweet tweet1 = new Tweet(id, token);
            IList<ITweet> retweets = tweet1.GetRetweets();
            foreach (Tweet r in retweets)
            {
                Console.WriteLine("tweet id  = " + r.Id + " - text = " + r.Text);
            }
        }
        #endregion

        #region Favourites

        private static void CreateFavouriteTweet(IToken token)
        {
            ITweet newTweet = new Tweet(String.Format("Favouriting tweet {0}", DateTime.Now), token);
            newTweet.Publish();
            newTweet.Favourited = true;
        }

        private static void GetFavouriteTweet(IToken token)
        {
            IUser user = new User("ladygaga", token);
            List<ITweet> tweets = user.GetFavourites();

            foreach (var tweet in tweets)
            {
                Console.WriteLine(tweet);
            }
        }

        private static void GetFavouriteSince(IToken token)
        {
            string text = String.Format("Favouriting tweet {0}", DateTime.Now);

            // Create and favourite a first tweet
            ITweet tweet1 = new Tweet(text, token);
            tweet1.Publish();
            tweet1.Favourited = true;

            ITweet tweet2 = new Tweet(text + " - 2", token);
            tweet2.Publish();
            tweet2.Favourited = true;

            ITweet tweet3 = new Tweet(text + " - 3", token);
            tweet3.Publish();
            tweet3.Favourited = true;

            IUser creator = tweet1.Creator;

            List<ITweet> favouritesSinceId = creator.GetFavouritesSinceId(tweet1.Id);

            // Should return the last 2 tweets

            foreach (var tweet in favouritesSinceId)
            {
                Console.WriteLine(tweet.ToString());
            }
        }

        #endregion

        #endregion

        #region Direct Message

        #region Message creation

        // Create a message and retrieve it from Twitter
        private static void Get_message(IToken token, long messageId)
        {
            IMessage m = new Message(messageId, token);

            Console.WriteLine("message text = " + m.Text + " ; receiver = " + m.Receiver.ScreenName + " ; sender = " + m.Sender.ScreenName);
        }

        // Create a new message
        private static IMessage createNewMessage()
        {
            IUser receiver = new User(543118219);
            IMessage msg = new Message(
                String.Format("Hello from Tweetinvi! ({0})", DateTime.Now.ToShortTimeString()),
                receiver);

            return msg;
        }

        #endregion

        #region Send Message

        private static void SendMessage(IToken token)
        {
            IMessage msg = createNewMessage();
            msg.Publish(token);
        }

        #endregion

        #endregion

        #region Search

        #region User

        private static void SearchUser(IToken token)
        {
            string searchQuery = "tweetinvi";

            IUserSearchEngine searchEngine = new UserSearchEngine(token);
            List<IUser> searchResult = searchEngine.Search(searchQuery);

            foreach (var user in searchResult)
            {
                Console.Write(user.ScreenName);
            }
        }

        #endregion

        #endregion

        #region Powered Users

        #region OAuthToken

        private static void SendTweetWithOAuth(IOAuthCredentials credentials)
        {
            OAuthToken t = new OAuthToken(credentials);
            t.ExecuteQuery("https://api.twitter.com/1.1/statuses/update.json?status=Hello Twitter From OAuth", HttpMethod.POST, null);
        }

        #endregion

        #region Token

        private static void SendTweetWithToken(IToken token)
        {
            token.ExecutePOSTQuery("https://api.twitter.com/1.1/statuses/update.json?status=helloTwitter");
        }

        /// <summary>
        /// Simple function that uses ExecuteQuery to retrieve information from the Twitter API
        /// </summary>
        /// <param name="token"></param>
        static void ExecuteQuery(IToken token)
        {
            // Retrieving information from Twitter API through Token method ExecuteRequest
            Dictionary<string, object>[] timeline = token.ExecuteGETQueryReturningCollectionOfObjects("https://api.twitter.com/1.1/statuses/home_timeline.json");

            // Working on each different object sent as a response to the Twitter API query
            for (int i = 0; i < timeline.Length; ++i)
            {
                Dictionary<String, object> post = timeline[i];
                Console.WriteLine("{0} : {1}\n", i, post["text"]);
            }
        }

        /// <summary>
        /// Function that execute cursor query and send information for each query executed
        /// </summary>
        /// <param name="token"></param>
        static void ExecuteCursorQuery(IToken token)
        {
            // The delegate is a function that will be called for each cursor
            DynamicResponseDelegate del = delegate(Dictionary<string, object> jsonResponse, long previousCursor, long nextCursor)
            {
                Console.WriteLine(previousCursor + " -> " + nextCursor + " : " + jsonResponse.Count);

                return jsonResponse.Count;
            };

            token.ExecuteCursorQuery("https://api.twitter.com/1.1/friends/ids.json?user_id=700562792", del);
        }

        #region ErrorHandling

        /// <summary>
        /// Testing the 3 ways to handle errors
        /// </summary>
        /// <param name="token"></param>
        static void TestErrorFunctions(IToken token)
        {
            integrated_error_handler(token);
            token_integrated_error_handler(token);
            execute_query_error_handler(token);
        }

        /// <summary>
        /// Initiating auto error handler
        /// You will not receive error information if handled by default error handler
        /// </summary>
        /// <param name="token"></param>
        static void integrated_error_handler(IToken token)
        {
            token.IntegratedExceptionHandler = true;

            // Error is not automatically handled

            try
            {
                // Calling a method that does not exist
                token.ExecuteGETQuery("https://api.twitter.com/1.1/users/contributors.json?user_id=700562792");
            }
            catch (WebException wex)
            {
                Console.WriteLine("An error occured!");
                Console.WriteLine(wex);
            }
        }

        /// <summary>
        /// When assigning an error_handler to a Token think that it will be kept alive 
        /// until you specify it does not exist anymore by specifying :
        /// 
        /// token.Integrated_Exception_Handler = false;
        /// 
        /// You can assign null value if you do not want anything to be performed for you
        /// </summary>
        /// <param name="token"></param>
        static void token_integrated_error_handler(IToken token)
        {
            token.ExceptionHandler = delegate(WebException wex)
            {
                Console.WriteLine("You received a Token generated error!");
                Console.WriteLine(wex.Message);
            };

            // Calling a method that does not exist
            token.ExecuteGETQuery("https://api.twitter.com/1.1/users/contributors.json?user_id=700562792");

            // Reset to basic Handler
            token.IntegratedExceptionHandler = false;
            // OR
            token.ResetExceptionHandler();
        }

        /// <summary>
        /// Uses the handler for only one query / work also for cursor queries
        /// </summary>
        /// <param name="token"></param>
        static void execute_query_error_handler(IToken token)
        {
            WebExceptionHandlingDelegate del = delegate(WebException wex)
            {
                Console.WriteLine("You received an execute_query_error!");
                Console.WriteLine(wex.Message);
            };

            token.ExecuteGETQuery("https://api.twitter.com/1.1/users/contributors.json?user_id=700562792", null, del);
        }

        #endregion

        #region Rate-Limit

        /// <summary>
        /// Enable you to Get all information from Token and how many query you can execute
        /// Each time a query is executed the XRateLimitRemaining is updated.
        /// To improve efficiency, the other values are NOT.
        /// If you need these please call the function GetRateLimit()
        /// </summary>
        static void GetRateLimit(IToken token)
        {
            ITokenRateLimits tokenLimits = token.GetRateLimit();
            Console.WriteLine("Remaning Requests for GetRate : {0}", tokenLimits.ApplicationRateLimitStatusLimit.Remaining);
            Console.WriteLine("Total Requests Allowed for GetRate : {0}", tokenLimits.ApplicationRateLimitStatusLimit.Limit);
            Console.WriteLine("GetRate limits will reset at : {0} local time", tokenLimits.ApplicationRateLimitStatusLimit.ResetDateTime.ToLongTimeString());
        }

        #endregion

        #endregion

        #endregion

        #region Token Generator

        public static int GetCaptchaFromConsole(string validationUrl)
        {
            Console.WriteLine("Please visit :");
            Console.WriteLine("{0}", validationUrl);
            Console.WriteLine("\nEnter validation key : ");
            string validationKey = Console.ReadLine();

            int result;
            if (Int32.TryParse(validationKey, out result))
            {
                return result;
            }

            return -1;
        }

        public static int GetCaptchaFromWPFInterfaceInConsole(string validationUrl)
        {
            int result = -1;

            Thread enterCaptchaThread = new Thread(() =>
            {
                Application app = new Application();
                result = app.Run(new ValidateApplicationCaptchaWindow(validationUrl, true));
            });

            enterCaptchaThread.SetApartmentState(ApartmentState.STA);
            enterCaptchaThread.Start();
            enterCaptchaThread.Join();

            return result;
        }

        public static int GetCaptchaFromWPF(string validationUrl)
        {
            int result = -1;

            Thread enterCaptchaThread = new Thread(() =>
            {
                Application app = new Application();
                ValidateApplicationCaptchaWindow window = new ValidateApplicationCaptchaWindow(validationUrl, true);
                window.Closed += (sender, args) =>
                {
                    result = window.VerifierKey;
                };

                app.Run(window);
            });

            enterCaptchaThread.SetApartmentState(ApartmentState.STA);
            enterCaptchaThread.Start();
            enterCaptchaThread.Join();

            return result;
        }

        public static IToken GenerateToken(IToken consumerToken, RetrieveCaptchaDelegate getCaptchaDelegate)
        {
            Console.WriteLine("Starting Token Generation...");
            ITokenCreator creator = new TokenCreator(consumerToken.ConsumerKey,
                                                     consumerToken.ConsumerSecret);

            Console.WriteLine("Please enter the verifier key...");
            IToken newToken = creator.CreateToken(getCaptchaDelegate);

            if (newToken != null)
            {
                Console.WriteLine("Token generated!");
                Console.WriteLine("Token Information : ");

                Console.WriteLine("Consumer Key : {0}", newToken.ConsumerKey);
                Console.WriteLine("Consumer Secret : {0}", newToken.ConsumerSecret);
                Console.WriteLine("Access Token : {0}", newToken.AccessToken);
                Console.WriteLine("Access Token Secret : {0}", newToken.AccessTokenSecret);

                ITokenUser loggedUser = new TokenUser(newToken);
                Console.WriteLine("Your name is {0}!", loggedUser.ScreenName);

                return newToken;
            }

            Console.WriteLine("Token could not be generated. Please login and specify your verifier key!");
            return null;
        }

        public static IToken GenerateTokenFromConsole(IToken consumerToken)
        {
            return GenerateToken(consumerToken, GetCaptchaFromConsole);
        }

        public static IToken GenerateTokenFromWPF(IToken consumerToken)
        {
            return GenerateToken(consumerToken, GetCaptchaFromWPF);
        }

        #endregion

        #region Exception Management

        public static void ManageMyActions(IToken token)
        {
            ITwitterContext context = new TwitterContext();

            ITokenUser u;
            if (!context.TryInvokeAction(() => new TokenUser(new Token("", "", "", "")), out u))
            {
                Console.WriteLine(context.LastActionException.Message);
                // At this point you must handle the exception!
            }

            if (u == null && !context.TryInvokeAction(() => new TokenUser(token), out u))
            {
                Console.WriteLine(context.LastActionException.Message);
                // We handled the exception improperly -- in the example it should not happen!
                return;
            }

            // The TokenUser now exist and we can use it!
            List<IUser> followers;
            if (!context.TryInvokeAction(() => u.Followers, out followers))
            {
                Console.WriteLine(context.LastActionTwitterException.TwitterWebExceptionErrorDescription);
                // return -- At this point you should not go further (we do it to show how the exception works)
            }

            // If the 2 previous operation succeeded then you can iterate on your followers!
            Console.WriteLine("Here are your followers!");
            foreach (var follower in followers)
            {
                Console.WriteLine(follower.Name);
            }
        }

        private static void ManageStreamException(IToken token)
        {
            // Creating the stream and specifying the delegate
            SimpleStream stream = new SimpleStream("https://stream.twitter.com/1.1/statuses/sample.json");
            // Starting the stream by specifying credentials thanks to the Token

            TwitterContext context = new TwitterContext();
            var success = context.TryInvokeAction(() => stream.StartStream(token, x => ProcessTweet(x)));

            if (!success)
            {
                if (context.LastActionTwitterException.StatusCode == -1)
                {
                    Console.WriteLine("You have a connection problem!");
                }
                else
                {
                    Console.WriteLine(context.LastActionTwitterException.TwitterWebExceptionErrorDescription);
                }
            }
        }

        #endregion

        /// <summary>
        /// Run a basic application to provide a code example
        /// </summary>
        static void Main()
        {
            // Initializing a Token with Twitter Credentials contained in the App.config
            IToken token = new Token(
                ConfigurationManager.AppSettings["token_AccessToken"],
                ConfigurationManager.AppSettings["token_AccessTokenSecret"],
                ConfigurationManager.AppSettings["token_ConsumerKey"],
                ConfigurationManager.AppSettings["token_ConsumerSecret"]);

            TokenSingleton.Token = token;

            #region Generate a Token with Access Token for the User
            // var t = GenerateTokenFromWPF(token);
            // var t2 = GenerateTokenFromConsole(token);
            //if (t != null)
            //{
            //    token = t;
            //}
            #endregion

            GetRateLimit(token);

            #region User Examples

            // User
            //createUser(token, "StevensDev");

            // Friends

            //User ladygaga = new User("ladygaga", token);
            //ladygaga.RefreshFriendIds(null, 500);

            //Console.WriteLine(ladygaga.Friends.Count);

            //GetFriends(token);
            //GetFriendIds(token, 579529593);
            //GetFriendIdsUsingUsername(token, "StevensDev");

            // Followers
            // UserGetFollowers(token);
            // GetFollowerIds(token, 579529593);
            // GetFollowerIdsUsingUsername(token, "StevensDev");

            // Contributors
            //GetContributors(token, 30973, null, true);
            //GetContributors(token, null, "twitterapi", true);
            //GetContributees(token, 15483731, null, true);
            //GetContributees(token, null, "LeeAdams", true);

            // TimeLines
            //GetTimeline(token, 579529593, true);
            //GetMentions(token);

            // Tweets
            //Get_retweet_of_tweet(token, 173198765052792833);
            //GetFavouriteSince(token);

            // List
            //GetSuggestedUserListDetails(token, "us-election-2012");
            //GetSuggestedUserListMembers(token, "us-election-2012");

            // Images
            //GetProfileImage(token);

            #endregion

            #region TokenUser Examples

            //GetHomeTimeline(token);
            //GetDirectMessagesReceived(token);
            //GetDirectMessagesSent(token);
            //GetBlockedUsers(token, true, true);
            //GetBlockedUsersIds(token, true);
            //GetSuggestedUserList(token, true);

            #endregion

            #region Tweet Examples

            //GetTweetById(token);
            //PublishTweet(token);
            //PublishTweetWithGeo(token);
            //PublishInReplyTo(token);
            //PublishAndDestroyRetweet(token);
            //CreateFavouriteTweet(token);
            //GetFavouriteTweet(token);

            #endregion

            #region Message Examples

            //Get_message(token, 347015339323842560);
            //SendMessage(token);
            //GetDirectMessagesSent(token);
            //GetDirectMessagesReceived(token);

            #endregion

            #region Streaming Examples

            // Powered Filtered Stream
            //TrackKeywordsWrittenFromASpecificLocation(token);
            //TrackKeywordsWrittenFromASpecificLocationAndPerformSpecificActionForKeyword(token);
            //TrackMessagesSentByUserFromSpecificLocation(token);
            //TracksMessagesSentByUserFromSpecificLocationAndContainingSpecificKeywords(token);

            //// Global stream Examples
            //GetStreamStateInformation(token);
            //ManageStreamException(token);
            //StartTimeredStream(token, 3000);

            //InfiniteStreamUsingStreamStopped_Recommended(token);
            //InfiniteStreamUsingTwitterContext(token);

            //SimpleStreamExample(token);
            //SimpleStreamWithExceptionManagerExample(token);
            //SimpleStreamWithFilterExample(token);
            //SimpleStreamWithMatchingKeywordsExample(token);

            //// Filtered stream examples
            //StreamFilterBasicTrackExample(token);
            //StreamFilterWithManualTrackingExample(token);
            //StreamFilterBasicFollowExample(token);

            //StreamFilterBasicFollowWithEventRaisedForSpecificUser(token);
            //StreamFilterLocationExample(token);
            //StreamFilterTrackSpecificLocation(token);

            //StreamFilterTrackORFollowExample(token);
            //StreamFilterTrackORFollowORLocationExample(token);

            // User stream examples
            // UserStreamExample(token);
            // UserStreamWithTrackedTweetsExample(token);

            #endregion

            #region SearchUser Examples

            // SearchUser(token);

            #endregion

            #region Exception Management

            // ManageMyActions(token);
            // ManageStreamException(token);

            #endregion

            #region Powered Users

            // SendTweetWithOAuth(token.TwitterCredentials);
            // SendTweetWithToken(token);
            // ExecuteQuery(token);
            // ExecuteCursorQuery(token);

            #endregion

            Console.WriteLine("End");
            Console.ReadKey();
        }
    }
}