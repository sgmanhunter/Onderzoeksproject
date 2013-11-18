using System;
using System.Collections.Generic;
using TweetinCore.Extensions;
using TweetinCore.Interfaces.TwitterToken;

namespace TwitterToken
{
    public class RateLimit : IRateLimit
    {
        public string ResourceName { get; private set; }
        public int Remaining { get; private set; }
        public int Limit { get; private set; }
        public double ResetTimeInSecond { get; private set; }
        public DateTime ResetDateTime { get; private set; }

        public RateLimit(string resourceName, Dictionary<string, object> rateInfos)
        {
            ResourceName = resourceName;
            
            Remaining = rateInfos.GetProp<int>("remaining");
            Limit = rateInfos.GetProp<int>("limit");

            long reset = rateInfos.GetProp<int>("reset");

            ResetDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            ResetDateTime = ResetDateTime.AddSeconds(reset).ToLocalTime();
            ResetTimeInSecond = (ResetDateTime - DateTime.Now).TotalSeconds;
        }
    }
}
