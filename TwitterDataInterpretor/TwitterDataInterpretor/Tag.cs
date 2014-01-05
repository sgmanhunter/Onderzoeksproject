using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterDataInterpretor
{
    class Tag
    {
        private string name;
        private DateTime from;
        private DateTime until;
        private Database dbInstance = Database.GetInstance();
        private Dictionary<DateTime?, int?> messagesByDate;
        private Dictionary<DateTime?, int?> forecastedMessagesByDate;
        private Dictionary<DateTime?, int?> realMessagesByDateForForcastedPeriod;
        private string report;

        public Tag(DateTime from, DateTime until, string name)
        {
            this.name = name;
            this.from = from;
            this.until = until;
            CollectMessages();
        }

        private void CollectMessages()
        {
            messagesByDate = dbInstance.GetMessagesByDateForTag(this.name, this.from, this.until);
        }

        private void generateForecast(DateTime from, DateTime until)
        {
           // this.from.AddDays(7);
           // this.until.AddDays(14);

            int resultOfCollectedMessages = messagesByDate.Count;

            // gedeeld door het aantal (records / aantaldagen) +  aantalrecords mss werkt da zo wel
            //int resultOfTheForecastMessages = resultOfCollectedMessages /

          
            
        }

        private void CollectMessagesToCompareWithForecast(DateTime from, DateTime until)
        {

        }



        public override string ToString()
        {
            DateTime?[] dates = messagesByDate.Keys.ToArray();
            int?[] messages = messagesByDate.Values.ToArray();
           

            report += this.name + "\n";
            report += "data collected:\n";

            report += String.Format("{0,-12}{1,8}\n", "Date", "Messages");

            for (int i = 0; i < dates.Length; i++)
            {
                report += String.Format("{0,-12}{1,-8}\n", dates[i].Value.ToShortDateString(), messages[i].Value.ToString());
            }

            return report + "\n-----\n";
        }
    }
}
