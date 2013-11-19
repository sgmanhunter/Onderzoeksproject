using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using oAuthConnection;
using Streaminvi;
using TweetinCore.Enum;
using TweetinCore.Events;
using TweetinCore.Interfaces;
using TweetinCore.Interfaces.oAuth;
using TweetinCore.Interfaces.StreamInvi;
using TweetinCore.Interfaces.TwitterToken;
using Tweetinvi;
using Tweetinvi.Model;
using TwitterToken;
using System.Configuration;


namespace TwitCrunchStream
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

       

        static void Main()
        {

            IToken token = new Token(
    ConfigurationManager.AppSettings["token_AccessToken"],
    ConfigurationManager.AppSettings["token_AccessTokenSecret"],
    ConfigurationManager.AppSettings["token_ConsumerKey"],
    ConfigurationManager.AppSettings["token_ConsumerSecret"]);

            TokenSingleton.Token = token;

            Woorden woord = new Woorden();
            woord.Zoekwoord = "pang";
            StreamManagement stream = new StreamManagement(woord.Zoekwoord);
           
           
           
            
            Console.WriteLine("test");
            
        }

        
        
    }
}
