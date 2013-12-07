using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetinCore.Interfaces;
using System.Data;
using System.Data.SqlClient;

namespace TwitCrunchStream
{
    class Tweet
    {
        private string text;
        private string id;
        private string creatorUserName;
        private DateTime createdAt;
        private string tagString;
        public Tweet(ITweet tweet)
        {
            this.text = tweet.Text;
            this.id = tweet.IdStr;
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

            try
            {
                System.Data.SqlClient.SqlConnection sqlConnection = new System.Data.SqlClient.SqlConnection(@"Server=Admin-PC\SQLEXPRESS;Database=twitcrunch;Trusted_Connection=True;");
                sqlConnection.Open();
                SqlCommand sc = new SqlCommand("INSERT INTO dbo.tweets VALUES (@id, @text, @username, @createdAt, @tags)", sqlConnection);
                sc.Parameters.AddWithValue("@id", this.id);
                sc.Parameters.AddWithValue("@text", this.text);
                sc.Parameters.AddWithValue("@username", this.creatorUserName);
                sc.Parameters.AddWithValue("@createdAt", this.createdAt);
                sc.Parameters.AddWithValue("@tags", this.tagString);
                sc.ExecuteNonQuery();
                sqlConnection.Close();
            }
            catch (Exception e)
            {
                
            }
        }




    }
}
