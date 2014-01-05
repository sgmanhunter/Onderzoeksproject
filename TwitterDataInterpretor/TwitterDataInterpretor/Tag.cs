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
        private Dictionary<DateTime, float> forecastedMessagesByDate = new Dictionary<DateTime, float>();
        private Dictionary<DateTime?, int?> realMessagesByDateForForcastedPeriod;
        private string report;

        public Tag(DateTime from, DateTime until, string name)
        {
            this.name = name;
            this.from = from;
            this.until = until;
            CollectMessages();
            generateForecast(until, 5);
        }

        private void CollectMessages()
        {
            messagesByDate = dbInstance.GetMessagesByDateForTag(this.name, this.from, this.until);
        }

        private void generateForecast(DateTime from, int daysToForecast)
        {
            //linear regression
            float sumMessages = 0;
            float squareSumMessages = 0;
            float sumDays = 0;
            float squareSumDays = 0;
            float sumDaysMultipliedByMessages = 0;
            int?[] messages = messagesByDate.Values.ToArray();
            int[] days = new int[messages.Length];

            int counter = 0;
            foreach (int? messageCount in messages)
            {
                sumMessages += (float)messageCount;
                squareSumMessages += (float)Math.Pow(Convert.ToDouble(messageCount), 2);
                days[counter] = counter + 1;
                sumDays += days[counter];
                sumDaysMultipliedByMessages += (float)days[counter] * ((float)messageCount);
                squareSumDays += (int)Math.Pow(Convert.ToDouble(sumDays), 2);
                counter++;
            }

            float n = messages.Length;
            float a = ((sumMessages * squareSumDays) - (sumDays * sumDaysMultipliedByMessages)) /
                (n * (squareSumDays) - (int)Math.Pow(Convert.ToDouble(sumDays), 2));
            float b = (n * (sumDaysMultipliedByMessages) - (sumDays * sumMessages)) /
                (n * (squareSumDays) - (int)Math.Pow(Convert.ToDouble(sumDays), 2));

            DateTime testDate = from;
            for (int i = 0; i < daysToForecast; i++)
            {
                testDate = testDate.AddDays(1);
                float calculatedExtraDay = a+(b * (n + i));
                forecastedMessagesByDate.Add(testDate, calculatedExtraDay);
            }


            

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

            report += "forecasts:\n";
            report += String.Format("{0,-12}{1,8}\n", "Date", "Messages");

            DateTime[] forecastedDates = forecastedMessagesByDate.Keys.ToArray();
            float[] forecastedMessages = forecastedMessagesByDate.Values.ToArray();

            for (int i = 0; i < forecastedDates.Length; i++)
            {
                report += String.Format("{0,-12}{1,-8}\n", forecastedDates[i].ToShortDateString(), forecastedMessages[i].ToString());
            }


            return report + "\n-----\n";
        }
    }
}
