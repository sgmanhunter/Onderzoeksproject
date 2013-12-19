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
using System.Collections;


namespace TwitCrunchStream
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

       

        static void Main()
        {
            StreamManagement sm = new StreamManagement();

            string[] zoekwoorden = new string[3];
            zoekwoorden[0] = "christmas";
            zoekwoorden[1] = "london";

            sm.Init(zoekwoorden);
        }

        
        
    }
}
