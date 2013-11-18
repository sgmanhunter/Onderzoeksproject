using System;
using System.Net;
using TweetinCore.Enum;
using TweetinCore.Interfaces.TwitterToken;

namespace Streaminvi
{
    public class ObjectStream : BaseStream<string>
    {
        private readonly string _streamURL;

        public ObjectStream(string streamURL)
        {
            _streamURL = streamURL;
        }

        public override void StartStream(IToken token, Func<string, bool> processTweetDelegate)
        {
            Func<HttpWebRequest> generateWebRequest = delegate
            {
                return token.GetQueryWebRequest(_streamURL, HttpMethod.GET);
            };

            _streamResultGenerator.StartStream(processTweetDelegate, generateWebRequest);
        }
    }
}
