﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace TwitCrunch.SQL
{
    public class TwitCrunchStatsContext
    {

        public string ConnectionString { get; set; }

        public TwitCrunchStatsContext(string conString)
        {
            if (!string.IsNullOrEmpty(conString))
            {
                ConnectionString = conString;
            }
            else throw new Exception("ConnectionString must be set");            
        }


        public Dictionary<DateTime?, int?> GetDayStatsFromKeyWord(string keyWord)
        {
            Dictionary<DateTime?, int?> stats = new Dictionary<DateTime?, int?>();
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();

                //SqlCommand(stored procedure name, connection string)
                SqlCommand com = new SqlCommand("spGetTweetsFrom", con);
                com.CommandType = CommandType.StoredProcedure;

                //Add paramters to connection string
                com.Parameters.AddWithValue("@keyword",keyWord);
                com.Parameters.AddWithValue("@datefrom", new DateTime(2013, 12, 22));
                com.Parameters.AddWithValue("@dateuntil", new DateTime(2013, 12, 25));

                SqlDataReader dr = com.ExecuteReader();

                while (dr.Read())
                {
                    stats.Add(dr["Date"] as DateTime?, dr["Messages"] as int?);               
                }

                dr.Close();
            }
            return stats;
        }


    }
}
