using System;
using System.Collections.Generic;
using System.Linq;
using TweetinCore.Events;
using TweetinCore.Interfaces;
using TweetinCore.Interfaces.TwitterToken;
using Tweetinvi.Properties;

namespace Tweetinvi.Helpers
{
    /// <summary>
    /// Access to methods related with user that cannot find
    /// their place in User or TokenUser
    /// </summary>
    public static class UserUtils
    {
        #region Private methods

        /// <summary>
        /// Update the current query to create the expected query
        /// </summary>
        private static string EnrichLookupQuery(string query, string extension)
        {
            if (extension.EndsWith("%2C"))
            {
                return query + "&" + extension.Remove(extension.Length - 3);
            }
            return query;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Return a list of users corresponding to the list of user ids and screen names given in parameter.
        /// Throw an exception if the token given in parameter is null or if both lists given in parameters are null.
        /// </summary>
        /// <param name="userIds">
        ///     List of user screen names. This parameter can be null
        ///     List of user ids. This parameter can be null
        /// </param>
        /// <param name="screenNames"></param>
        /// <param name="token">Token used to request the users' information from the Twitter API</param>
        /// <returns>The list of users retrieved from the Twitter API</returns>
        public static List<IUser> Lookup(List<long> userIds, List<string> screenNames, IToken token)
        {
            if (token == null)
            {
                throw new ArgumentException("Token must not be null");
            }

            if (userIds == null && screenNames == null)
            {
                throw new ArgumentException("User ids or screen names must be specified");
            }

            // Maximum number of users that can be requested from the Twitter API (in 1 single request)
            const int listMaxSize = 100;

            List<IUser> users = new List<IUser>();

            if (userIds == null)
            {
                userIds = new List<long>();
            }
            if (screenNames == null)
            {
                screenNames = new List<string>();
            }

            int userIndex = 0;
            int screenNameIndex = 0;
            while ((userIndex < userIds.Count) || (screenNameIndex < screenNames.Count))
            {
                // Keep track of the number of users that we are going to request from the Twitter API
                int indicesSumBeforeLoop = userIndex + screenNameIndex;
                string userIdsStrList = "user_id=";
                string screenNamesStrList = "screen_name=";

                // Take request parameters from both names list and id list
                // userIndex + screenNameIndex - indicesSumBeforeLoop) < listMaxSize ==> Check that the number of parameters given to the Twitter API request does not exceed the limit
                while (((userIndex + screenNameIndex - indicesSumBeforeLoop) < listMaxSize)
                    && (userIndex < userIds.Count)
                    && (screenNameIndex < screenNames.Count))
                {
                    screenNamesStrList += screenNames.ElementAt(screenNameIndex++) + "%2C";
                    userIdsStrList += userIds.ElementAt(userIndex++) + "%2C";
                }
                // Take request from id list
                while (((userIndex + screenNameIndex - indicesSumBeforeLoop) < listMaxSize)
                    && (userIndex < userIds.Count))
                {
                    userIdsStrList += userIds.ElementAt(userIndex++) + "%2C";
                }

                // Take name from name list
                while (((userIndex + screenNameIndex - indicesSumBeforeLoop) < listMaxSize)
                    && (screenNameIndex < screenNames.Count))
                {
                    screenNamesStrList += screenNames.ElementAt(screenNameIndex++) + "%2C";
                }

                String query = Resources.UserUtils_Lookup;
                // Add new parameters to the query and format it
                query = EnrichLookupQuery(query, screenNamesStrList);
                query = EnrichLookupQuery(query, userIdsStrList);

                ObjectResponseDelegate objectDelegate = delegate(Dictionary<string, object> responseObject)
                    {
                        User u = User.Create(responseObject);
                        users.Add(u);
                    };

                token.ExecuteGETQuery(query, objectDelegate);
            }

            return users;
        }

        public static bool AddUserInformationInQuery(
            ref string query, 
            IUser user, 
            string userIdParameterTitle = "user_id",
            string userNameParameterTitle = "screen_name")
        {
            if (user == null || user.Id == null && user.ScreenName == null)
            {
                return false;
            }

            string initialCharacter = "";
            if (!query.EndsWith("?") && !query.EndsWith("&") && query.Contains("?"))
            {
                initialCharacter = "&";
            }

            if (user.Id != null)
            {
                query += String.Format("{0}{1}={2}", initialCharacter, userIdParameterTitle, user.Id);
            }
            else
            {
                query += String.Format("{0}{1}={2}", initialCharacter, userNameParameterTitle, user.ScreenName);
            }

            return true;
        }

        public static void GetFollowers(
            IToken token,
            IUser user,
            int maxFriends = Int32.MaxValue,
            Action<IUser> followerRetrievedAction = null,
            Action<long> followerIdRetrievedAction = null,
            long cursor = 0)
        {
            Func<IUser, bool> friendRetrieved = null;
            Func<long, bool> friendIdRetrieved = null;

            if (followerRetrievedAction != null)
            {
                friendRetrieved = u =>
                {
                    followerRetrievedAction(u);
                    return true;
                };
            }

            if (followerIdRetrievedAction != null)
            {
                friendIdRetrieved = userId =>
                {
                    followerIdRetrievedAction(userId);
                    return true;
                };
            }

            GetFollowers(token, user, maxFriends, friendRetrieved, friendIdRetrieved, cursor);
        }

        public static void GetFollowers(
            IToken token,
            IUser user,
            int maxFriends = Int32.MaxValue,
            Func<IUser, bool> friendRetrieved = null,
            Func<long, bool> friendIdRetrieved = null,
            long cursor = 0)
        {
            string query = Resources.User_GetFollowers;
            GetFriendsOrFollowers(token, user, query, maxFriends, friendRetrieved, friendIdRetrieved, cursor);
        }

        public static void GetFriends(
            IToken token,
            IUser user,
            int maxFriends = Int32.MaxValue,
            Action<IUser> friendRetrievedAction = null,
            Action<long> friendIdRetrievedAction = null,
            long cursor = 0)
        {
            Func<IUser, bool> friendRetrieved = null;
            Func<long, bool> friendIdRetrieved = null;

            if (friendRetrievedAction != null)
            {
                friendRetrieved = u =>
                {
                    friendRetrievedAction(u);
                    return true;
                };
            }

            if (friendIdRetrievedAction != null)
            {
                friendIdRetrieved = userId =>
                {
                    friendIdRetrievedAction(userId);
                    return true;
                };
            }

            GetFriends(token, user, maxFriends, friendRetrieved, friendIdRetrieved, cursor);
        }

        public static void GetFriends(
            IToken token,
            IUser user,
            int maxFriends = Int32.MaxValue,
            Func<IUser, bool> friendRetrieved = null,
            Func<long, bool> friendIdRetrieved = null,
            long cursor = 0)
        {
            string query = Resources.User_GetFriends;
            GetFriendsOrFollowers(token, user, query, maxFriends, friendRetrieved, friendIdRetrieved, cursor);
        }

        private static void GetFriendsOrFollowers(
            IToken token,
            IUser user,
            string query,
            int maxFriends = Int32.MaxValue,
            Func<IUser, bool> friendRetrieved = null,
            Func<long, bool> friendIdRetrieved = null,
            long cursor = 0)
        {
            if (token == null)
            {
                return;
            }

            int currentNumberOfFriends = 0;
            DynamicResponseDelegate del = delegate(Dictionary<string, object> responseObject, long previousCursor, long nextCursor)
            {
                var userFriendIds = (responseObject["ids"] as IEnumerable<object>) != null ? (responseObject["ids"] as IEnumerable<object>).ToList() : null;

                if (userFriendIds != null)
                {
                    if (currentNumberOfFriends + userFriendIds.Count < maxFriends)
                    {
                        currentNumberOfFriends += userFriendIds.Count;

                        for (int i = 0; i < userFriendIds.Count; ++i)
                        {
                            long friendId = Int64.Parse(userFriendIds[i].ToString());

                            if (friendIdRetrieved != null)
                            {
                                friendIdRetrieved(friendId);
                            }

                            if (friendRetrieved != null)
                            {
                                friendRetrieved(new User(friendId));
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < userFriendIds.Count && currentNumberOfFriends < maxFriends; ++i)
                        {
                            ++currentNumberOfFriends;
                            long friendId = Int64.Parse(userFriendIds[i].ToString());

                            if (friendIdRetrieved != null)
                            {
                                friendIdRetrieved(friendId);
                            }

                            if (friendRetrieved != null)
                            {
                                friendRetrieved(new User(friendId));
                            }
                        }
                    }

                    return userFriendIds.Count();
                }

                return 0;
            };

            if (AddUserInformationInQuery(ref query, user))
            {
                token.ExecuteCursorQuery(query, 0, maxFriends, del);
            }
        }

        #endregion
    }
}
