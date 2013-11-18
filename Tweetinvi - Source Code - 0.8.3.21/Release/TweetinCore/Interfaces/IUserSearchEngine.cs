﻿using System.Collections.Generic;
using TweetinCore.Interfaces.TwitterToken;

namespace TweetinCore.Interfaces
{
    /// <summary>
    /// Provide methods related with search of User on Twitter
    /// </summary>
    public interface IUserSearchEngine : ISearchEngine<IUser>
    {
        /// <summary>
        /// Perform a basic search on Twitter API and returns the T type specified
        /// </summary>
        /// <param name="searchQuery">Search to be sent</param>
        /// <param name="includeEntities">Include entities to the results</param>
        /// <param name="token">Token used to perform the search</param>
        /// <returns>Collection of results</returns>
        List<IUser> Search(string searchQuery, bool includeEntities, IToken token = null);

        /// <summary>
        /// Perform a basic search on Twitter API and returns the T type specified
        /// </summary>
        /// <param name="searchQuery">Search to be sent</param>
        /// <param name="resultPerPage">Number of result per page</param>
        /// <param name="pageNumber">Page expected</param>
        /// <param name="token">Token used to perform the search</param>
        /// <param name="includeEntities">Include entities to the results</param>
        /// <returns>Collection of results</returns>
        List<IUser> Search(string searchQuery, int resultPerPage, int pageNumber, 
            IToken token = null, bool includeEntities = false);
    }
}
