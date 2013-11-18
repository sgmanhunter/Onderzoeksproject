﻿namespace Tweetinvi.Helpers.Visitors
{
    public abstract class TwitterObjectVisitor<T> : IVisitor<T> where T : class 
    {
        /// <summary>
        /// Processes a concrete instance of TwitterObject.
        /// </summary>
        /// <param name="o">Instance of TwitterObject</param>
        public abstract void Visit(T o);
    }
}
