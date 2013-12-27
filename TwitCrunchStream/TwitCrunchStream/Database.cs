using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitCrunchStream
{
    class Database
    {
        public static void RegisterApplicationStopped()
        {
            try
            {
                //register app stopped
                System.Data.SqlClient.SqlConnection sqlConnection = new System.Data.SqlClient.SqlConnection(@"Data Source=BRU-SQL1.hogeschool-wvl.be;Initial Catalog=TwitterDB;Persist Security Info=True;User ID=TwitterDB;Password=pangilya;");
                sqlConnection.Open();
                SqlCommand sc = new SqlCommand("insert into apprunningtimes values (GETDATE(), 'STOP')", sqlConnection);
                sc.ExecuteNonQuery();
                sqlConnection.Close();
            }
            catch
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\DA1\Desktop\Debug\log.txt", true))
                {
                    file.WriteLine("Error: SQL");
                }
            }
        }

        public static void RegisterApplicationStarted()
        {
            try
            {
                //register app started
                System.Data.SqlClient.SqlConnection sqlConnection = new System.Data.SqlClient.SqlConnection(@"Data Source=BRU-SQL1.hogeschool-wvl.be;Initial Catalog=TwitterDB;Persist Security Info=True;User ID=TwitterDB;Password=pangilya;");
                sqlConnection.Open();
                SqlCommand sc = new SqlCommand("insert into apprunningtimes values (GETDATE(), 'START')", sqlConnection);
                sc.ExecuteNonQuery();
                sqlConnection.Close();
            }
            catch
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\DA1\Desktop\Debug\log.txt", true))
                {
                    file.WriteLine("Error: SQL");
                }
            }
        }
    }
}
