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
        private string zoekwoord;
        public Tweet(ITweet tweet, string zoekwoord)
        {
            this.text = tweet.Text;
            this.id = tweet.IdStr;
            this.createdAt = tweet.CreatedAt;
            this.creatorUserName = tweet.Creator.ScreenName;
            this.tagString = convertListToString(tweet.Hashtags);
            this.zoekwoord = zoekwoord;
        }

        private string convertListToString(List<IHashTagEntity> hashtags)
        {
            string to_return = "";
            foreach (IHashTagEntity ihte in hashtags)
            {
                to_return += ihte.Text + ", ";
            }

            if (to_return.Length > 2)
            {
                //remove last comma
                to_return = to_return.Remove(to_return.Length - 2, 1);
            }

            
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
                SqlConnection sqlConnection = new SqlConnection(@"Data Source=BRU-SQL1.hogeschool-wvl.be;Initial Catalog=TwitterDB;Persist Security Info=True;User ID=TwitterDB;Password=pangilya;");
                if (TryConnect(sqlConnection))
                {
                    sqlConnection.Open();
                    SqlCommand sc = new SqlCommand("INSERT INTO dbo.tweets VALUES (@id, @text, @username, @createdAt, @tags, @zoekwoord)", sqlConnection);
                    sc.Parameters.AddWithValue("@id", this.id);
                    sc.Parameters.AddWithValue("@text", this.text);
                    sc.Parameters.AddWithValue("@username", this.creatorUserName);
                    sc.Parameters.AddWithValue("@createdAt", this.createdAt);
                    sc.Parameters.AddWithValue("@tags", this.tagString);
                    sc.Parameters.AddWithValue("@zoekwoord", this.zoekwoord);
                    sc.ExecuteNonQuery();
                    sqlConnection.Close();
                }
            }
            catch (Exception e)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\DA1\Desktop\Debug\log.txt", true))
                {
                    file.WriteLine("Error: database " + e.Message + " " + e.StackTrace);
                }
                //throw new Exception("Mistake in the database " + e.Message + " " + e.StackTrace);
            }
        }

        //new function outside of your main function
        private bool TryConnect(SqlConnection sqlConnection)
        {
            try 
            {
                sqlConnection.Open();
                DataTable tblDatabases = sqlConnection.GetSchema("Databases");
                sqlConnection.Close();
            }
            catch (SqlException e)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\DA1\Desktop\Debug\log.txt", true))
                {
                    file.WriteLine("Error: database connection " + e.Message + " " + e.StackTrace);
                }

                return false;
            }

            return true;    
        }






    }
}
