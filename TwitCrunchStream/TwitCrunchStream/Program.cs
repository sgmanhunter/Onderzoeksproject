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
            //gsm merken
            zoekwoorden[0] = "Nokia";
            zoekwoorden[1] = "Samsung";
            zoekwoorden[2] = "Apple";
            zoekwoorden[3] = "LG";
            zoekwoorden[4] = "huawei";
            zoekwoorden[5] = "HTC";
            //films
            zoekwoorden[6] = "hobbit";
            zoekwoorden[7] = "thor";
            zoekwoorden[8] = "dhoom3";
            zoekwoorden[9] = "CaptainPhillips";
            zoekwoorden[10] = "TurboFast";
            zoekwoorden[11] = "EscapePlan";
            zoekwoorden[12] = "WalkingWithDinosaurs";
            zoekwoorden[13] = "CatchingFire";
            zoekwoorden[14] = "47Ronin";
            zoekwoorden[15] = "GrudgeMatch";
            zoekwoorden[16] = "gravity";
            zoekwoorden[17] = "waltermitty";
            zoekwoorden[18] = "TheWolfOfWallStreet";
            zoekwoorden[19] = "DevilsDue";
            zoekwoorden[20] = "RideAlong";
            zoekwoorden[21] = "SavingMrBanks";

            // games
            zoekwoorden[22] = "dayz";
            zoekwoorden[23] = "starbound";
            zoekwoorden[24] = "7DaysToDie";
            zoekwoorden[25] = "Minecraft";
            zoekwoorden[26] = "Godus";
            zoekwoorden[27] = "LeagueOfLegends";
            zoekwoorden[28] = "WoW";
            zoekwoorden[29] = "Diablo3";
            zoekwoorden[30] = "HEARTHSTONE";
            zoekwoorden[31] = "WoW";
            zoekwoorden[32] = "Diablo3";
            zoekwoorden[33] = "HEARTHSTONE";

            sm.Init(zoekwoorden);
        }

        
        
    }
}
