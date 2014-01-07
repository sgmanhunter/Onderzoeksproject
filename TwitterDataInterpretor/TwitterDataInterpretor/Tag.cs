using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;


namespace TwitterDataInterpretor
{
    class Tag
    {
        private string name;
        private DateTime from;
        private DateTime until;

        private Database dbInstance = Database.GetInstance();

        private Dictionary<DateTime?, int?> messagesByDate;
        private Dictionary<DateTime, double> forecastedMessagesByDate;
        private Dictionary<DateTime?, int?> realMessagesByDateForForcastedPeriod;

        public Tag(DateTime from, DateTime until, string name)
        {
            this.name = name;
            this.from = from;
            this.until = until;

            this.messagesByDate = dbInstance.GetMessagesByDateForTag(this.name, this.from, this.until);
            this.forecastedMessagesByDate = new Dictionary<DateTime, double>();

            DateTime dateFromForecast = until.AddDays(1);
            DateTime dateUntilForecast = until.AddDays(4);
            this.realMessagesByDateForForcastedPeriod = dbInstance.GetMessagesByDateForTag(this.name, dateFromForecast, dateUntilForecast);

            generateForecast(until, 4);
        }

        //Holt-model
        private void generateForecast(DateTime from, int daysToForecast)
        {
            double w = 0.5; //exponentiële-effeningsconstatnte  
            double v = 0.7; //trendeffeningsconstante

            //E[0] == E1 and T[0] == T1 are not defined
            double[] E = new double[messagesByDate.Values.Count()]; //level equation
            double[] T = new double[messagesByDate.Values.Count()]; //trend equation
            int?[] messages = messagesByDate.Values.ToArray();

            //E2 = Y2 
            E[1] = (double) messages[1];
            //T2 = Y2 - Y1
            T[1] = (double)messages[1] - (double)messages[0];

            for (int i = 2; i < messages.Length; i++)
            {
                E[i] = (w * (double)messages[i]) + ((1 - w) * (E[i - 1] + T[i - 1]));
                T[i] = (v * (E[i] - E[i - 1])) + ((1 - v) * T[i - 1]);
            }

            DateTime nextDay = from;
            for (int i = 1; i <= daysToForecast; i++)
            {
                nextDay = nextDay.AddDays(1);
                double forecastedValue = E[E.Length - 1] + (i * T[T.Length - 1]);
                forecastedMessagesByDate.Add(nextDay, forecastedValue);
            }
        }

        private string ShowCollectedData()
        {
            string gatheredData = "";
            //vars
            DateTime?[] dates = messagesByDate.Keys.ToArray();
            int?[] messages = messagesByDate.Values.ToArray();
            gatheredData += "data collected:\n";
            gatheredData += String.Format("{0,-12}{1,-8}\n", "Date", "Messages");
            for (int i = 0; i < dates.Length; i++)
            {  
                gatheredData += String.Format("{0,-12}{1,-8}\n", dates[i].Value.ToShortDateString(), messages[i].Value.ToString());
            }

            return gatheredData;
        }

        private string ShowForecastedData()
        {
            string forecast = "";
            DateTime[] forecastedDates = forecastedMessagesByDate.Keys.ToArray();
            double[] forecastedMessages = forecastedMessagesByDate.Values.ToArray();
            int?[] realMessages = realMessagesByDateForForcastedPeriod.Values.ToArray();

            forecast += "forecasts:\n";
            for (int i = 0; i < forecastedDates.Length; i++)
            {
                forecast += String.Format("{0,-12}{1,-8}\n", "Date:", forecastedDates[i].ToShortDateString());
                forecast += String.Format("{0,-12}{1,-8}\n", "Messages forecast:", Math.Round(forecastedMessages[i], 0));
                
                if (i < realMessages.Length)
                {
                    forecast += String.Format("{0,-12}{1,-8}", "Real amount of messages:", realMessages[i]);
                }

                forecast += "\n\n";
            }

            return forecast;
        }

        public override string ToString()
        {

            string report = "";
            report += this.name + "\n";
            report += ShowCollectedData();
            report += ShowForecastedData();
            return report + "-----\n\n";
        }
    }
}
