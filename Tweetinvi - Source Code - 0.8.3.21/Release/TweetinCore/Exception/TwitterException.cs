using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using TweetinCore.Extensions;
using TweetinCore.Properties;

namespace TweetinCore.Exception
{
    public class TwitterException : WebException
    {
        // Twitter throws a WebException with a HTTP error status code
        // To provide more information, Twitter also send additional information
        // describing why you've received the Exception.
        // This class stores both information

        public DateTime CreatedAt { get; private set; }
        public int StatusCode { get; private set; }

        public string TwitterWebExceptionErrorDescription { get; private set; }

        private string _twitterErrorFirstMessage;
        public string TwitterErrorFirstMessage
        {
            get
            {
                return _twitterErrorFirstMessage;
            }
        }

        private readonly List<ITwitterExceptionInfo> _twitterExceptionsInfo;
        public IEnumerable<ITwitterExceptionInfo> TwitterExceptions
        {
            get { return _twitterExceptionsInfo; }
        }

        public void GetErrorsInfo(WebException wex)
        {
            var wexResponse = wex.Response as HttpWebResponse;

            if (wexResponse == null)
            {
                return;
            }

            try
            {
                using (var stream = wexResponse.GetResponseStream())
                {
                    if (stream == null)
                    {
                        return;
                    }

                    using (var reader = new StreamReader(stream))
                    {
                        var twitterExceptionInfo = reader.ReadToEnd();

                        try
                        {
                            var exceptionInfo = TweetinCoreGlobal.JsSerializer.Deserialize<Dictionary<string, object>>(twitterExceptionInfo);

                            ArrayList errors = exceptionInfo.GetPropAs<ArrayList>("errors");
                            if (errors != null)
                            {
                                foreach (var error in errors)
                                {
                                    _twitterExceptionsInfo.Add(new TwitterExceptionInfo(error as Dictionary<string, object>));
                                }
                            }

                            _twitterErrorFirstMessage = _twitterExceptionsInfo != null && 
                                                        _twitterExceptionsInfo.Any() ? _twitterExceptionsInfo[0].Message : "";
                        }
                        catch (ArgumentException)
                        {
                            _twitterErrorFirstMessage = twitterExceptionInfo;
                        }
                        
                    }
                }
            }
            catch (WebException) { }
        }

        public TwitterException(WebException wex)
            : base(wex.Message, wex.InnerException, wex.Status, wex.Response)
        {
            CreatedAt = DateTime.Now;
            _twitterExceptionsInfo = new List<ITwitterExceptionInfo>();

            StatusCode = ExceptionExtension.GetWebExceptionStatusNumber(wex);

            if (StatusCode != -1)
            {
                GetErrorsInfo(wex);
            }

            TwitterWebExceptionErrorDescription = Resources.ResourceManager.GetString(String.Format("ExceptionDescription_{0}", StatusCode));
        }
    }
}
