using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetinCore.Interfaces;

namespace TwitCrunchStream
{
    class Tweet
    {
        private string text;
        private int id;
        private string creatorUserName;
        private DateTime createdAt;
        private string tagString;
        public Tweet(ITweet tweet)
        {
            this.text = tweet.Text;
            this.id = int.Parse(tweet.IdStr);
            this.createdAt = tweet.CreatedAt;
            this.creatorUserName = tweet.Creator.ScreenName;
            this.tagString = convertListToString(tweet.Hashtags);
        }

        private string convertListToString(List<IHashTagEntity> hashtags)
        {
            string to_return = "";
            foreach (IHashTagEntity ihte in hashtags)
            {
                to_return += ihte.Text + ", ";
            }

            //remove last comma
            to_return = to_return.Remove(to_return.Length - 2, 1);

            
            return to_return;
        }

        public void WriteToDatabase()
        {
            Console.WriteLine("Gebruiker: " + this.creatorUserName);
            Console.WriteLine("Text: " + this.text);
            Console.WriteLine("createdAt: " + this.createdAt);
            Console.WriteLine("ID: " + this.id);
            Console.WriteLine("Hashtags: " + this.tagString);


        }




    }
}
