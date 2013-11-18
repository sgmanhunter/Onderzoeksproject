using System.Web.Script.Serialization;

namespace TweetinCore
{
    public static class TweetinCoreGlobal
    {
        public static JavaScriptSerializer JsSerializer;

        static TweetinCoreGlobal()
        {
            JsSerializer = new JavaScriptSerializer();
        }
    }
}
