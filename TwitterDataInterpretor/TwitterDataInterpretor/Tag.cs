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
        private Dictionary<DateTime, float> forecastedMessagesByDate = new Dictionary<DateTime, float>();
        private Dictionary<DateTime?, int?> realMessagesByDateForForcastedPeriod;
        private Dictionary<DateTime, float?> forecastedClosedPercentage = new Dictionary<DateTime, float?>();
        private Dictionary<string , int?> topTen =  new Dictionary<string,int?>();
        private ArrayList tags;
        private string report;
        private string sTopTen;

        public Tag(DateTime from, DateTime until, string name)
        {
            this.name = name;
            this.from = from;
            this.until = until;
            CollectMessages();
            generateForecast(until, 5);
            CollectMessagesToCompareWithForecast(until, 5);
            MakeTopTen();
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

        private void CollectMessagesToCompareWithForecast(DateTime from, int daysToForecast)
        {
            // hier moeten we nog de voorbije forecast score vergelijken met reall data .
            // - hier heb ik nu eerst gwn de toekomstige forecast vergeleken met de current reall data 
            // dus we moeten gwn nog de voorbije forecastopslaan en vergelijken wss
            DateTime testDate1 = from;
            
       
                foreach (KeyValuePair<DateTime, float> forecastItem in forecastedMessagesByDate)
                {

                    foreach (KeyValuePair<DateTime?, int?> realItem in messagesByDate)
                    {
                        testDate1 = testDate1.AddDays(1);
                        float? result = (realItem.Value / forecastItem.Value) * 100;
                        if (result <= 100)
                        {
                            forecastedClosedPercentage.Add(testDate1, result);
                        }
                        else
                        {
                            result = 100;
                            forecastedClosedPercentage.Add(testDate1, result);
                        }
                    }
                }
            
        }

        private void MakeTopTen()
        {
            int?[] messagesTotal = messagesByDate.Values.ToArray();
            int sumMessages = 0;
            
            foreach (int? messageCount in messagesTotal)
            {
                sumMessages += (int)messageCount;
            }

            foreach (string tag in dbInstance.GetAllTagsFromDatabase())
            {
             
                    topTen.Add(tag, sumMessages);     
            }
               
            
            // dit hier toont 34 de zelfde waarde daarna verder :o omdat we 34 woorden heefd .... die toont dus 34 maal  27 kn 34 maal 4 k enzo :p  dus de resultaat maar 34 maar :p dus ja :p... moet nog opgelost worden
            var list = topTen.Values.ToList();
            list.Sort();

          
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
            report += String.Format("{0,-12}{1,8}{2, 15}\n", "Date", "Messages","Percentage");

            DateTime[] forecastedDates = forecastedMessagesByDate.Keys.ToArray();
            float[] forecastedMessages = forecastedMessagesByDate.Values.ToArray();
           
            float?[] forecastedPercentage = forecastedClosedPercentage.Values.ToArray(); 
            string[] topTenNames = topTen.Keys.ToArray();

            for (int i = 0; i < forecastedDates.Length; i++)
            {
                report += String.Format("{0,-12}{1,-8}{2, 14:P1}\n", forecastedDates[i].ToShortDateString(), forecastedMessages[i].ToString(), forecastedPercentage[i].ToString() + "%");
            }

            // dit mag niet herhalen en moet gwn onderaan de formulier getoont worden gesorteerd op top 10
            for (int i = 0; i < 9; i++)
            {
                report += String.Format("{0}", topTenNames[i].ToString()) + "\n";
            }

            return report + "\n-----\n";
        }
    }
}
