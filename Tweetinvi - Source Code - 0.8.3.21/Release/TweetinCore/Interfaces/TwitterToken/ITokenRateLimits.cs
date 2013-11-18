namespace TweetinCore.Interfaces.TwitterToken
{
    /// <summary>
    /// Lists of Rate Limits provided by Twitter API 1.1
    /// https://dev.twitter.com/docs/rate-limiting/1.1/limits
    /// </summary>
    public interface ITokenRateLimits
    {
        string AccessToken { get; }

        IRateLimit AccountSettingsLimit { get; }
        IRateLimit AccountVerifyCredentialsLimit { get; }

        IRateLimit ApplicationRateLimitStatusLimit { get; }

        IRateLimit BlocksIdsLimit { get; }
        IRateLimit BlocksListLimit { get; }

        IRateLimit DirectMessagesLimit { get; }
        IRateLimit DirectMessagesSentLimit { get; }
        IRateLimit DirectMessagesSentAndReceivedLimit { get; }
        IRateLimit DirectMessagesShowLimit { get; }

        IRateLimit FavoritesListLimit { get; }

        IRateLimit FollowersIdsLimit { get; }
        IRateLimit FollowersListLimit { get; }

        IRateLimit FriendsIdsLimit { get; }
        IRateLimit FriendsListLimit { get; }

        IRateLimit FriendshipsIncomingLimit { get; }
        IRateLimit FriendshipsLookupLimit { get; }
        IRateLimit FriendshipsNoRetweetsIdsLimit { get; }
        IRateLimit FriendshipsOutgoingLimit { get; }
        IRateLimit FriendshipsShowLimit { get; }

        IRateLimit GeoIdPlaceIdLimit { get; }
        IRateLimit GeoReverseGeoCodeLimit { get; }
        IRateLimit GeoSearchLimit { get; }
        IRateLimit GeoSimilarPlacesLimit { get; }

        IRateLimit HelpConfigurationLimit { get; }
        IRateLimit HelpLanguagesLimit { get; }
        IRateLimit HelpPrivacyLimit { get; }
        IRateLimit HelpTosLimit { get; }

        IRateLimit ListsListLimit { get; }
        IRateLimit ListsMembersLimit { get; }
        IRateLimit ListsMembersShowLimit { get; }
        IRateLimit ListsMembershipsLimit { get; }
        IRateLimit ListsOwnershipsLimit { get; }
        IRateLimit ListsShowLimit { get; }
        IRateLimit ListsStatusesLimit { get; }
        IRateLimit ListsSubscribersLimit { get; }
        IRateLimit ListsSubscribersShowLimit { get; }
        IRateLimit ListsSubscriptionsLimit { get; }

        IRateLimit SavedSearchesListLimit { get; }
        IRateLimit SavedSearchesShowIdLimit { get; }

        IRateLimit SearchTweetsLimit { get; }

        IRateLimit StatusesHomeTimelineLimit { get; }
        IRateLimit StatusesMentionsTimelineLimit { get; }
        IRateLimit StatusesOembedLimit { get; }
        IRateLimit StatusesRetweetersIdsLimit { get; }
        IRateLimit StatusesRetweetsIdLimit { get; }
        IRateLimit StatusesRetweetsOfMeLimit { get; }
        IRateLimit StatusesShowIdLimit { get; }
        IRateLimit StatusesUserTimelineLimit { get; }

        IRateLimit TrendsAvailableLimit { get; }
        IRateLimit TrendsClosestLimit { get; }
        IRateLimit TrendsPlaceLimit { get; }

        IRateLimit UsersContributeesLimit { get; }
        IRateLimit UsersContributorsLimit { get; }
        IRateLimit UsersLookupLimit { get; }
        IRateLimit UsersProfileBannerLimit { get; }
        IRateLimit UsersSearchLimit { get; }
        IRateLimit UsersShowIdLimit { get; }
        IRateLimit UsersSuggestionsLimit { get; }
        IRateLimit UsersSuggestionsSlugLimit { get; }
        IRateLimit UsersSuggestionsSlugMembersLimit { get; }
    }
}

