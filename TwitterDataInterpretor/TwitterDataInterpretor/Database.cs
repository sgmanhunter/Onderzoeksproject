using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Collections;

namespace TwitterDataInterpretor
{
    public class Database
    {
        private static Database instance = new Database();
        private SqlConnection connection = new SqlConnection("Data Source=BRU-SQL1.hogeschool-wvl.be;Initial Catalog=TwitterDB;Persist Security Info=True;User ID=TwitterDB;Password=pangilya;");

        private Database()
        {

        }

        public static Database GetInstance()
        {
            return instance;
        }

        public string[] GetAllTagsFromDatabase()
        {
            ArrayList tags = new ArrayList();

            connection.Open();
            SqlCommand command = new SqlCommand("spGetAllTags", connection);
            command.CommandType = CommandType.StoredProcedure;

            SqlDataReader dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                tags.Add(dataReader["collectedByTag"] as String);
            }
            dataReader.Close();
            connection.Close();

            return (string[])tags.ToArray(typeof(string));
        }

        public Dictionary<DateTime?, int?> GetMessagesByDateForTag(string tag, DateTime from, DateTime until)
        {
            Dictionary<DateTime?, int?> messagesByDate = new Dictionary<DateTime?, int?>();
            connection.Open();
            SqlCommand command = new SqlCommand("spGetTweetsFrom", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@keyword", tag);
            command.Parameters.AddWithValue("@datefrom", from);
            command.Parameters.AddWithValue("@dateuntil", until);

            SqlDataReader dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                messagesByDate.Add(dataReader["Date"] as DateTime?, dataReader["Messages"] as int?);
            }
            dataReader.Close();
            connection.Close();

            return messagesByDate;
        }
        
    }
}
