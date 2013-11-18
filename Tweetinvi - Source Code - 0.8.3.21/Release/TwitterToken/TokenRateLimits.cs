using System;
using System.Collections.Generic;
using TweetinCore.Extensions;
using TweetinCore.Interfaces.TwitterToken;
using TwitterToken.Properties;

namespace TwitterToken
{
    public class TokenRateLimits : ITokenRateLimits
    {
        public string AccessToken { get; private set; }

        public IRateLimit AccountSettingsLimit { get; private set; }
        public IRateLimit AccountVerifyCredentialsLimit { get; private set; }

        public IRateLimit ApplicationRateLimitStatusLimit { get; private set; }
        
        public IRateLimit BlocksIdsLimit { get; private set; }
        public IRateLimit BlocksListLimit { get; private set; }

        public IRateLimit DirectMessagesLimit { get; private set; }
        public IRateLimit DirectMessagesSentLimit { get; private set; }
        public IRateLimit DirectMessagesSentAndReceivedLimit { get; private set; }
        public IRateLimit DirectMessagesShowLimit { get; private set; }

        public IRateLimit FavoritesListLimit { get; private set; }

        public IRateLimit FollowersIdsLimit { get; private set; }
        public IRateLimit FollowersListLimit { get; private set; }
        
        public IRateLimit FriendsIdsLimit { get; private set; }
        public IRateLimit FriendsListLimit { get; private set; }
        
        public IRateLimit FriendshipsIncomingLimit { get; private set; }
        public IRateLimit FriendshipsLookupLimit { get; private set; }
        public IRateLimit FriendshipsNoRetweetsIdsLimit { get; private set; }
        public IRateLimit FriendshipsOutgoingLimit { get; private set; }
        public IRateLimit FriendshipsShowLimit { get; private set; }

        public IRateLimit GeoIdPlaceIdLimit { get; private set; }
        public IRateLimit GeoReverseGeoCodeLimit { get; private set; }
        public IRateLimit GeoSearchLimit { get; private set; }
        public IRateLimit GeoSimilarPlacesLimit { get; private set; }

        public IRateLimit HelpConfigurationLimit { get; private set; }
        public IRateLimit HelpLanguagesLimit { get; private set; }
        public IRateLimit HelpPrivacyLimit { get; private set; }
        public IRateLimit HelpTosLimit { get; private set; }

        public IRateLimit ListsListLimit { get; private set; }
        public IRateLimit ListsMembersLimit { get; private set; }
        public IRateLimit ListsMembersShowLimit { get; private set; }
        public IRateLimit ListsMembershipsLimit { get; private set; }
        public IRateLimit ListsOwnershipsLimit { get; private set; }
        public IRateLimit ListsShowLimit { get; private set; }
        public IRateLimit ListsStatusesLimit { get; private set; }
        public IRateLimit ListsSubscribersLimit { get; private set; }
        public IRateLimit ListsSubscribersShowLimit { get; private set; }
        public IRateLimit ListsSubscriptionsLimit { get; private set; }
        
        public IRateLimit SavedSearchesListLimit { get; private set; }
        public IRateLimit SavedSearchesShowIdLimit { get; private set; }
        
        public IRateLimit SearchTweetsLimit { get; private set; }
        
        public IRateLimit StatusesHomeTimelineLimit { get; private set; }
        public IRateLimit StatusesMentionsTimelineLimit { get; private set; }
        public IRateLimit StatusesOembedLimit { get; private set; }
        public IRateLimit StatusesRetweetersIdsLimit { get; private set; }
        public IRateLimit StatusesRetweetsIdLimit { get; private set; }
        public IRateLimit StatusesRetweetsOfMeLimit { get; private set; }
        public IRateLimit StatusesShowIdLimit { get; private set; }
        public IRateLimit StatusesUserTimelineLimit { get; private set; }
        
        public IRateLimit TrendsAvailableLimit { get; private set; }
        public IRateLimit TrendsClosestLimit { get; private set; }
        public IRateLimit TrendsPlaceLimit { get; private set; }

        public IRateLimit UsersContributeesLimit { get; private set; }
        public IRateLimit UsersContributorsLimit { get; private set; }
        
        public IRateLimit UsersLookupLimit { get; private set; }
        public IRateLimit UsersProfileBannerLimit { get; private set; }
        public IRateLimit UsersSearchLimit { get; private set; }
        public IRateLimit UsersShowIdLimit { get; private set; }
        public IRateLimit UsersSuggestionsLimit { get; private set; }
        public IRateLimit UsersSuggestionsSlugLimit { get; private set; }
        public IRateLimit UsersSuggestionsSlugMembersLimit { get; private set; }

        public TokenRateLimits(IToken token)
        {
            Dictionary<String, object> rateInfos = token.ExecuteGETQuery(Resources.QueryRateLimit);
            
            SetupAccessTokenRateLimit(rateInfos);
            SetupResourcesLimits(rateInfos);
        }

        private void SetupAccessTokenRateLimit(Dictionary<String, object> rateInfos)
        {
            var limitContext = rateInfos.GetProp<Dictionary<string, object>>("rate_limit_context");
            AccessToken = limitContext.GetProp<string>("access_token");
        }

        private void SetupResourcesLimits(Dictionary<String, object> rateInfos)
        {
            var resources = rateInfos.GetProp<Dictionary<string, object>>("resources");

            SetupAccountLimits(resources);
            SetupApplicationLimits(resources);
            SetupBlocksLimits(resources);
            SetupDirectMessagesLimits(resources);
            SetupFavouritesLimits(resources);
            SetupFollowersLimits(resources);
            SetupFriendsLimits(resources);
            SetupFriendshipsLimits(resources);
            SetupGeoLimits(resources);
            SetupHelpLimits(resources);
            SetupListsLimits(resources);
            SetupSavedSearchesLimits(resources);
            SetupSearchLimits(resources);
            SetupStatusesLimits(resources);
            SetupTrendsLimits(resources);
            SetupUsersLimits(resources);
        }

        private void SetupAccountLimits(Dictionary<string, object> resources)
        {
            const string resourceFamilyName = "account";

            var resourceFamilyDictionary = resources.GetProp<Dictionary<string, object>>(resourceFamilyName);

            AccountSettingsLimit = GenerateRateLimit(resourceFamilyName, "settings", resourceFamilyDictionary);
            AccountVerifyCredentialsLimit = GenerateRateLimit(resourceFamilyName, "verify_credentials", resourceFamilyDictionary);
        }

        private void SetupApplicationLimits(Dictionary<string, object> resources)
        {
            const string resourceFamilyName = "application";

            var resourceFamilyDictionary = resources.GetProp<Dictionary<string, object>>(resourceFamilyName);

            ApplicationRateLimitStatusLimit = GenerateRateLimit(resourceFamilyName, "rate_limit_status", resourceFamilyDictionary);
        }

        private void SetupBlocksLimits(Dictionary<string, object> resources)
        {
            const string resourceFamilyName = "blocks";

            var resourceFamilyDictionary = resources.GetProp<Dictionary<string, object>>(resourceFamilyName);

            BlocksIdsLimit = GenerateRateLimit(resourceFamilyName, "ids", resourceFamilyDictionary);
            BlocksListLimit = GenerateRateLimit(resourceFamilyName, "list", resourceFamilyDictionary);
        }

        private void SetupDirectMessagesLimits(Dictionary<string, object> resources)
        {
            const string resourceFamilyName = "direct_messages";

            var resourceFamilyDictionary = resources.GetProp<Dictionary<string, object>>(resourceFamilyName);

            DirectMessagesLimit = GenerateRateLimit(resourceFamilyName, String.Empty, resourceFamilyDictionary);
            DirectMessagesSentLimit = GenerateRateLimit(resourceFamilyName, "sent", resourceFamilyDictionary);
            DirectMessagesSentAndReceivedLimit = GenerateRateLimit(resourceFamilyName, "sent_and_received", resourceFamilyDictionary);
            DirectMessagesShowLimit = GenerateRateLimit(resourceFamilyName, "show", resourceFamilyDictionary);
        }

        private void SetupFavouritesLimits(Dictionary<string, object> resources)
        {
            const string resourceFamilyName = "favorites";

            var resourceFamilyDictionary = resources.GetProp<Dictionary<string, object>>(resourceFamilyName);

            FavoritesListLimit = GenerateRateLimit(resourceFamilyName, "list", resourceFamilyDictionary);
        }

        private void SetupFollowersLimits(Dictionary<string, object> resources)
        {
            const string resourceFamilyName = "followers";

            var resourceFamilyDictionary = resources.GetProp<Dictionary<string, object>>(resourceFamilyName);

            FollowersIdsLimit = GenerateRateLimit(resourceFamilyName, "ids", resourceFamilyDictionary);
            FollowersListLimit = GenerateRateLimit(resourceFamilyName, "list", resourceFamilyDictionary);
        }

        private void SetupFriendsLimits(Dictionary<string, object> resources)
        {
            const string resourceFamilyName = "friends";

            var resourceFamilyDictionary = resources.GetProp<Dictionary<string, object>>(resourceFamilyName);

            FriendsIdsLimit = GenerateRateLimit(resourceFamilyName, "ids", resourceFamilyDictionary);
            FriendsListLimit = GenerateRateLimit(resourceFamilyName, "list", resourceFamilyDictionary);
        }

        private void SetupFriendshipsLimits(Dictionary<string, object> resources)
        {
            const string resourceFamilyName = "friendships";

            var resourceFamilyDictionary = resources.GetProp<Dictionary<string, object>>(resourceFamilyName);

            FriendshipsIncomingLimit = GenerateRateLimit(resourceFamilyName, "incoming", resourceFamilyDictionary);
            FriendshipsLookupLimit = GenerateRateLimit(resourceFamilyName, "lookup", resourceFamilyDictionary);
            FriendshipsNoRetweetsIdsLimit = GenerateRateLimit(resourceFamilyName, "no_retweets/ids", resourceFamilyDictionary);
            FriendshipsOutgoingLimit = GenerateRateLimit(resourceFamilyName, "outgoing", resourceFamilyDictionary);
            FriendshipsShowLimit = GenerateRateLimit(resourceFamilyName, "show", resourceFamilyDictionary);
        }

        private void SetupGeoLimits(Dictionary<string, object> resources)
        {
            const string resourceFamilyName = "geo";

            var resourceFamilyDictionary = resources.GetProp<Dictionary<string, object>>(resourceFamilyName);

            GeoIdPlaceIdLimit = GenerateRateLimit(resourceFamilyName, "id/:place_id", resourceFamilyDictionary);
            GeoReverseGeoCodeLimit = GenerateRateLimit(resourceFamilyName, "reverse_geocode", resourceFamilyDictionary);
            GeoSearchLimit = GenerateRateLimit(resourceFamilyName, "search", resourceFamilyDictionary);
            GeoSimilarPlacesLimit = GenerateRateLimit(resourceFamilyName, "similar_places", resourceFamilyDictionary);
        }

        private void SetupHelpLimits(Dictionary<string, object> resources)
        {
            const string resourceFamilyName = "help";

            var resourceFamilyDictionary = resources.GetProp<Dictionary<string, object>>(resourceFamilyName);

            HelpConfigurationLimit = GenerateRateLimit(resourceFamilyName, "configuration", resourceFamilyDictionary);
            HelpLanguagesLimit = GenerateRateLimit(resourceFamilyName, "languages", resourceFamilyDictionary);
            HelpPrivacyLimit = GenerateRateLimit(resourceFamilyName, "privacy", resourceFamilyDictionary);
            HelpTosLimit = GenerateRateLimit(resourceFamilyName, "tos", resourceFamilyDictionary);
        }

        private void SetupListsLimits(Dictionary<string, object> resources)
        {
            const string resourceFamilyName = "lists";

            var resourceFamilyDictionary = resources.GetProp<Dictionary<string, object>>(resourceFamilyName);

            ListsListLimit = GenerateRateLimit(resourceFamilyName, "list", resourceFamilyDictionary);
            ListsMembersLimit = GenerateRateLimit(resourceFamilyName, "members", resourceFamilyDictionary);
            ListsMembersShowLimit = GenerateRateLimit(resourceFamilyName, "members/show", resourceFamilyDictionary);
            ListsMembershipsLimit = GenerateRateLimit(resourceFamilyName, "memberships", resourceFamilyDictionary);
            ListsOwnershipsLimit = GenerateRateLimit(resourceFamilyName, "ownerships", resourceFamilyDictionary);
            ListsShowLimit = GenerateRateLimit(resourceFamilyName, "show", resourceFamilyDictionary);
            ListsStatusesLimit = GenerateRateLimit(resourceFamilyName, "statuses", resourceFamilyDictionary);
            ListsSubscribersLimit = GenerateRateLimit(resourceFamilyName, "subscribers", resourceFamilyDictionary);
            ListsSubscribersShowLimit = GenerateRateLimit(resourceFamilyName, "subscribers/show", resourceFamilyDictionary);
            ListsSubscriptionsLimit = GenerateRateLimit(resourceFamilyName, "subscriptions", resourceFamilyDictionary);
        }

        private void SetupSavedSearchesLimits(Dictionary<string, object> resources)
        {
            const string resourceFamilyName = "saved_searches";

            var resourceFamilyDictionary = resources.GetProp<Dictionary<string, object>>(resourceFamilyName);

            SavedSearchesListLimit = GenerateRateLimit(resourceFamilyName, "list", resourceFamilyDictionary);
            SavedSearchesShowIdLimit = GenerateRateLimit(resourceFamilyName, "show/:id", resourceFamilyDictionary);
        }

        private void SetupSearchLimits(Dictionary<string, object> resources)
        {
            const string resourceFamilyName = "search";

            var resourceFamilyDictionary = resources.GetProp<Dictionary<string, object>>(resourceFamilyName);

            SearchTweetsLimit = GenerateRateLimit(resourceFamilyName, "tweets", resourceFamilyDictionary);
        }

        private void SetupStatusesLimits(Dictionary<string, object> resources)
        {
            const string resourceFamilyName = "statuses";

            var resourceFamilyDictionary = resources.GetProp<Dictionary<string, object>>(resourceFamilyName);

            StatusesHomeTimelineLimit = GenerateRateLimit(resourceFamilyName, "home_timeline", resourceFamilyDictionary);
            StatusesMentionsTimelineLimit = GenerateRateLimit(resourceFamilyName, "mentions_timeline", resourceFamilyDictionary);
            StatusesOembedLimit = GenerateRateLimit(resourceFamilyName, "oembed", resourceFamilyDictionary);
            StatusesRetweetersIdsLimit = GenerateRateLimit(resourceFamilyName, "retweeters/ids", resourceFamilyDictionary);
            StatusesRetweetsIdLimit = GenerateRateLimit(resourceFamilyName, "retweets/:id", resourceFamilyDictionary);
            StatusesRetweetsOfMeLimit = GenerateRateLimit(resourceFamilyName, "retweets_of_me", resourceFamilyDictionary);
            StatusesShowIdLimit = GenerateRateLimit(resourceFamilyName, "show/:id", resourceFamilyDictionary);
            StatusesUserTimelineLimit = GenerateRateLimit(resourceFamilyName, "user_timeline", resourceFamilyDictionary);
        }

        private void SetupTrendsLimits(Dictionary<string, object> resources)
        {
            const string resourceFamilyName = "trends";

            var resourceFamilyDictionary = resources.GetProp<Dictionary<string, object>>(resourceFamilyName);

            TrendsAvailableLimit = GenerateRateLimit(resourceFamilyName, "available", resourceFamilyDictionary);
            TrendsClosestLimit = GenerateRateLimit(resourceFamilyName, "closest", resourceFamilyDictionary);
            TrendsPlaceLimit = GenerateRateLimit(resourceFamilyName, "place", resourceFamilyDictionary);
        }

        private void SetupUsersLimits(Dictionary<string, object> resources)
        {
            const string resourceFamilyName = "users";

            var resourceFamilyDictionary = resources.GetProp<Dictionary<string, object>>(resourceFamilyName);

            UsersContributeesLimit = GenerateRateLimit(resourceFamilyName, "contributees", resourceFamilyDictionary);
            UsersContributorsLimit = GenerateRateLimit(resourceFamilyName, "contributors", resourceFamilyDictionary);
            UsersLookupLimit = GenerateRateLimit(resourceFamilyName, "lookup", resourceFamilyDictionary);
            UsersProfileBannerLimit = GenerateRateLimit(resourceFamilyName, "profile_banner", resourceFamilyDictionary);
            UsersSearchLimit = GenerateRateLimit(resourceFamilyName, "search", resourceFamilyDictionary);
            UsersShowIdLimit = GenerateRateLimit(resourceFamilyName, "show/:id", resourceFamilyDictionary);
            UsersSuggestionsLimit = GenerateRateLimit(resourceFamilyName, "suggestions", resourceFamilyDictionary);
            UsersSuggestionsSlugLimit = GenerateRateLimit(resourceFamilyName, "suggestions/:slug", resourceFamilyDictionary);
            UsersSuggestionsSlugMembersLimit = GenerateRateLimit(resourceFamilyName, "suggestions/:slug/members", resourceFamilyDictionary);
        }

        private IRateLimit GenerateRateLimit(
            string resourceFamilyName,
            string resourceName,
            Dictionary<string, object> resourceFamilyDictionary)
        {
            var resourcePath = String.Format("/{0}/{1}", resourceFamilyName, resourceName);

            if (String.IsNullOrEmpty(resourceName))
            {
                resourcePath = String.Format("/{0}", resourceFamilyName);
            }

            return new RateLimit(resourcePath, resourceFamilyDictionary.GetProp<Dictionary<string, object>>(resourcePath));
        }
    }
}
